using System;
using System.Data;
using System.Text;
using ConciliacaoPersistencia.banco;
using ConciliacaoModelo.model.generico;

namespace ConciliacaoPersistencia.model
{
    public class ReceberDAL
    {
        private static string GetSQLTitulosComJuros(string filtro, string ordem)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("select id_receber, data_inc, id_pedido, vcto, dias, valor, saldo, id_cliente, ");
            sb.Append("       descricao, nome, juro, saldo+juro as saldo_juro ");
            sb.Append("from ( ");
            // sub-select 1
            sb.Append("   select id_receber, descricao, id_pedido, data_inc, vcto, dias, valor, saldo, id_cliente, nome, ");
            sb.Append("          case when dias < 1 and taxa > 0 then ");
            sb.Append("             (abs(dias)/30)*taxa*saldo ");
            sb.Append("           else 0 end as juro ");
            sb.Append("   from ( ");
            // sub-select 2
            sb.Append("       select id_receber, descricao, id_pedido, data_inc, vcto, ");
            sb.Append("              datediff(vcto, current_date) as dias, valor, saldo, p.id_cliente, p.nome, ");
            sb.Append("              (select taxa_juro/100 from tb_parametro limit 1) as taxa ");
            sb.Append("       from tb_receber r ");
            sb.Append("       join tb_cliente p on p.id_cliente = r.id_cliente  ");
            sb.Append(" where status='A' and saldo > 0 ");

            if (filtro != string.Empty)
            {
                sb.Append(" and ");
                sb.Append(filtro);
            }

            sb.Append(") as x ) as z ");
            sb.Append(" order by ");
            sb.Append(ordem);

            return sb.ToString();
        }

        public static DataTable ListarAbertas(string filtro, string ordem)
        {
            return DAL.ListarFromSQL(GetSQLTitulosComJuros(filtro, ordem));
        }

        public static DataTable GetProdutosFromReceber(int idReceber)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select p.codigo, v.id_produto, p.nome, v.qtd, v.vl_venda, v.vl_custo, v.vl_venda * v.qtd as total ");
            sb.Append("from tb_venda_produto v ");
            sb.Append("join tb_produto p on p.id_produto = v.id_produto ");
            sb.Append("where v.id_pedido = (select id_pedido from tb_receber where id_receber =  ");
            sb.Append(idReceber.ToString());
            sb.Append(") order by p.nome ");

            return DAL.ListarFromSQL(sb.ToString());
        }

        public static DataTable GetVencidasEProximaSemana()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select id_receber, vcto, datediff(vcto, current_date) as dias, saldo, p.id_cliente, p.nome ");
            sb.Append("from tb_receber r ");
            sb.Append("join tb_cliente p on p.id_cliente = r.id_cliente ");
            sb.Append("where status = 'A' and vcto <= date_add(current_date, INTERVAL 7 DAY) ");
            sb.Append("order by vcto, saldo desc");
            return DAL.ListarFromSQL(sb.ToString());
        }

        public static DataTable GetTitulosMesmoClienteMesmaData(int idCliente, DateTime vcto)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("r.id_cliente = ");
            sb.Append(idCliente.ToString());
            sb.Append(" and vcto = '");
            sb.Append(vcto.ToString("yyyy-MM-dd"));
            sb.Append("' ");

            return DAL.ListarFromSQL(GetSQLTitulosComJuros(sb.ToString(), "saldo desc"));
        }

        public static DataTable ListarRecebidas(string filtro, string ordem)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select id_receber, data_baixa, vcto, valor - saldo as valor, p.id_cliente, p.nome, ");
            sb.Append("       case when saldo < valor and saldo > 0 then valor else null end as valor_original, ");
            sb.Append("       case when status = 'A' then 'Sim' else '' end as baixa_parcial, descricao, id_pedido, ");
            sb.Append("       (select count(id_receber) from tb_recebido b where b.id_receber = r.id_receber) as detalhe ");
            sb.Append("from tb_receber r ");
            sb.Append("join tb_cliente p on p.id_cliente = r.id_cliente ");
            sb.Append("where saldo < valor ");
            if (filtro != string.Empty)
            {
                sb.Append(" and ");
                sb.Append(filtro);
            }
            sb.Append(" order by ");
            sb.Append(ordem);
            return DAL.ListarFromSQL(sb.ToString());
        }

        public static DataTable GetDetalhesRecebido(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select id_receber, valor, data, hora from tb_recebido where id_receber = ");
            sb.Append(id.ToString());
            sb.Append(" order by data, hora");
            return DAL.ListarFromSQL(sb.ToString());
        }

        public static Respostas Excluir(long idconta, long id)
        {
            Respostas response;
            using (Conexao conexao = Conexao.Get(DAL.GetStringConexao()))
            using (Transacao transacao = new Transacao(conexao))
                try
                {
                    // apagar do histórico de recebimento
                    //string sql = "delete from tb_recebido where id_receber =" + id.ToString();
                    //using (Comando comando = new Comando(transacao, sql))
                    //{
                    //    comando.Execute();
                    //    Log.Sql(sql);
                    //}
                    // apagar do contas receber
                    StringBuilder sb = new StringBuilder();
                    sb.Append("delete from financeiro_receber ");
                    sb.Append("where ");
                    sb.Append(String.Format(" id_conta ={0} and ", idconta));
                    sb.Append(String.Format(" id ={0} ", id));
                    using (Comando comando = new Comando(transacao, sb.ToString()))
                    {
                        comando.Execute();
                    }
                    transacao.Commit();
                    response = new Respostas(true, "Receber excluído!",0);
                }
                catch (Exception ex)
                {
                    transacao.RollBack();
                    response = new Respostas(true, ex.Message, 0);
                    throw ex;
                }
            return response;
        }
    }
}
 