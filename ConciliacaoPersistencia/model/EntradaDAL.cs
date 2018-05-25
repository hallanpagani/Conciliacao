using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Funcoes.classes;
using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.entrada;
using ConciliacaoPersistencia.banco;
using ConciliacaoPersistencia.classes;

namespace ConciliacaoPersistencia.model
{
    public class EntradaDAL
    {
        public static Entrada GetPendente(int idUsuario)
        {
            Entrada retorno = new Entrada();

            StringBuilder sb = new StringBuilder();
            sb.Append("select id_entrada, data_inc ");
            sb.Append("from tb_entrada ");
            sb.Append("where status = 'A' and id_usuario = ");
            sb.Append(idUsuario.ToString());

            DataTable tb = DAL.ListarFromSQL(sb.ToString());

            if (tb.Rows.Count > 0)
            {
                DataRow dr = tb.Rows[0];
                retorno.Id = int.Parse(dr["id_entrada"].ToString());
                retorno.DataInc = DateTime.Parse(dr["data_inc"].ToString());
            }
            return retorno;
        }

        public static DataTable GetProdutosFromEntrada(int IdEntrada)
        {
            return GetProdutos("id_entrada = " + IdEntrada.ToString());
        }

        private static string GetProdutosSQL(string filtro)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select id_entrada_produto, e.id_produto, ");
            sb.Append(" codigo, nome, e.qtd, e.vl_custo, e.vl_venda, e.qtd * e.vl_custo as total ");
            sb.Append(" from tb_entrada_produto e ");
            sb.Append(" join tb_produto p on p.id_produto = e.id_produto ");
            if (filtro != string.Empty)
            {
                sb.Append(" where ");
                sb.Append(filtro);
            }
            sb.Append(" order by nome");
            return sb.ToString();
        }

        public static DataTable GetProdutos(string filtro)
        {
            return DAL.ListarFromSQL(GetProdutosSQL(filtro));
        }

        public static Leitor GetProdutosFromEntradaLeitor(int IdEntrada)
        {
            return DAL.GetLeitorFromSQL(GetProdutosSQL("id_entrada = " + IdEntrada.ToString()));
        }

        public static void Excluir(int id)
        {
            using (Conexao conexao = Conexao.Get(DAL.GetStringConexao()))
            using (Transacao transacao = new Transacao(conexao))
                try
                {
                    string sql = "delete from tb_entrada_produto where id_entrada = " + id.ToString();

                    using (Comando comando = new Comando(transacao, sql))
                    {
                        comando.Execute();
                        Log.Sql(sql);
                    }

                    sql = "delete from tb_entrada where id_entrada = " + id.ToString();

                    using (Comando comando = new Comando(transacao, sql))
                    {
                        comando.Execute();
                        Log.Sql(sql);
                    }
                    transacao.Commit();
                }
                catch (Exception ex)
                {
                    transacao.RollBack();
                    throw ex;
                }
        }

       

        public static DataTable GetLista(string filtro, string ordem)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("select e.id_entrada, e.data_fecha, u.nome as usuario, ");
            sb.Append("  sum(p.vl_custo * p.qtd) as total_entrada,  "); // valor da entrada
            sb.Append("  count(p.id_entrada_produto) as qtd "); // valor da entrada
            sb.Append("from tb_entrada e ");
            sb.Append("join tb_entrada_produto p on p.id_entrada = e.id_entrada ");
            sb.Append("join tb_usuario u on u.id_usuario = e.id_usuario ");
            sb.Append("where status = 'F' ");
            if (filtro != string.Empty)
            {
                sb.Append(" and ");
                sb.Append(filtro);
            }
            sb.Append("group by e.id_entrada, e.data_fecha ");
            sb.Append("order by ");
            sb.Append(ordem);
            return DAL.ListarFromSQL(sb.ToString());
        }

        public static DataTable ListarProdutosEntrada(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select e.id_produto, codigo, nome, e.qtd, e.vl_custo, e.vl_venda, e.qtd * e.vl_custo as total ");
            sb.Append("from tb_entrada_produto e ");
            sb.Append("join tb_produto p on p.id_produto = e.id_produto ");
            sb.Append("where id_entrada = ");
            sb.Append(id.ToString());
            sb.Append(" order by nome");
            return DAL.ListarFromSQL(sb.ToString());
        }
    }
}