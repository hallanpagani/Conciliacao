using System;
using System.Data;
using System.Text;
using Funcoes.classes;
using MySql.Data.MySqlClient;
using ConciliacaoPersistencia.banco;
using ConciliacaoPersistencia.classes;


namespace ConciliacaoPersistencia.model
{
    public class PedidoDAL
    {
        
        #region get produtos
        private static string GetProdutosSQL(string filtro)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select id_pedido_produto, e.id_produto, ");
            sb.Append(" codigo, nome, e.qtd, p.vl_custo, e.vl_unit, e.qtd * e.vl_unit as total ");
            sb.Append(" from tb_pedido_produto e ");
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

        public static DataTable GetProdutosFromPedido(int IdPedido)
        {
            return GetProdutos("id_pedido = " + IdPedido.ToString());
        }

        public static Leitor GetProdutosFromPedidoLeitor(int IdPedido)
        {
            return DAL.GetLeitorFromSQL(GetProdutosSQL("id_pedido=" + IdPedido.ToString()));
        }

        public static DataTable GetProdutoFromPedido(int IdPedidoProduto)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select nome, e.qtd * e.vl_unit as total ");
            sb.Append(" from tb_pedido_produto e ");
            sb.Append(" join tb_produto p on p.id_produto = e.id_produto ");
            sb.Append(" where id_pedido_produto = " + IdPedidoProduto.ToString());
            return DAL.ListarFromSQL(sb.ToString());
        }
        #endregion

        public static decimal GetTotalPedido(int IdPedido)
        {
            return DAL.GetDecimal("select sum(vl_unit * qtd) as total from tb_pedido_produto where id_pedido = " + IdPedido.ToString());
        }

        public static void Excluir(int id)
        {
            using (Conexao conexao = Conexao.Get(DAL.GetStringConexao()))
            using (Transacao transacao = new Transacao(conexao))
                try
                {
                    string sql = "delete from tb_pedido_produto where id_pedido = " + id.ToString();
                    using (Comando comando = new Comando(transacao, sql))
                    {
                        comando.Execute();
                        Log.Sql(sql);
                    }

                    sql = "delete from tb_pedido where id_pedido = " + id.ToString();
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
            sb.Append("select p.id_pedido, p.data_inc, p.id_cliente, c.nome, ");
            sb.Append("sum(pp.vl_unit * pp.qtd) as vl_pedido, p.data_fecha, ");
            sb.Append("(select sum(valor) from tb_receber r where r.id_pedido = p.id_pedido and saldo = 0) as vl_recebido ");
            sb.Append("from tb_pedido p ");
            sb.Append("join tb_pedido_produto pp on pp.id_pedido = p.id_pedido ");
            sb.Append("join tb_cliente c on c.id_cliente = p.id_cliente ");
            sb.Append("where status='F' ");
            if (filtro != string.Empty)
            {
                sb.Append(" and ");
                sb.Append(filtro);
            }
            sb.Append(" group by p.id_pedido, p.data_inc, p.data_fecha, p.id_cliente, c.nome ");
            sb.Append("order by ");
            sb.Append(ordem);
            return DAL.ListarFromSQL(sb.ToString());
        }

        public static void AtualizarValorProduto(int IdPedidoProduto, decimal NovoValor)
        {
            using (Conexao conexao = Conexao.Get(DAL.GetStringConexao()))
            {
                string sql = String.Format("update tb_pedido_produto set vl_unit = {0}/qtd where id_pedido_produto = {1}", NovoValor.ToString().Replace(".", "").Replace(",", "."), IdPedidoProduto);
                using (Comando comando = new Comando(conexao, sql))
                {
                    comando.Execute();
                    Log.Sql(sql);
                }
            }
        }

        public static void Estornar(int id)
        {
            using (Conexao conexao = Conexao.Get(DAL.GetStringConexao()))
            using (Transacao transacao = new Transacao(conexao))
                try
                {

                    // primeiro carrega os produtos e quantidades para restaurar o saldo
                    DataTable tbProd = new DataTable();
                    string sql = "select id_produto, qtd from tb_pedido_produto where id_pedido =" + id.ToString();
                    using (Comando comando = new Comando(conexao, sql))
                    {
                        using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(comando.GetCommand()))
                            dataAdapter.Fill(tbProd);
                    }

                    // percorremos todos os produtos e update no saldo do estoque
                    foreach (DataRow linha in tbProd.Rows)
                    {
                        sql = string.Format("update tb_produto set qtd=qtd+{0} where id_produto={1}", linha["qtd"].ToString(), linha["id_produto"].ToString());
                        using (Comando comando = new Comando(conexao, sql))
                        {
                            comando.Execute();
                            Log.Sql(sql);
                        }
                    }
                    // pegarmos tambem as contas recebidas a vista ou já baixadas desta venda
                    DataTable tbRecebida = new DataTable();
                    sql = "select id_receber from tb_receber where id_pedido = " + id.ToString();
                    using (Comando comando = new Comando(conexao, sql))
                    {
                        using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(comando.GetCommand()))
                            dataAdapter.Fill(tbRecebida);
                    }

                    // percorremos todos as contas a receber e apagar o histórico do recebido
                    foreach (DataRow linha in tbRecebida.Rows)
                    {
                        sql = string.Format("delete from tb_recebido where id_receber = {0}", linha["id_receber"].ToString());
                        using (Comando comando = new Comando(conexao, sql))
                        {
                            comando.Execute();
                            Log.Sql(sql);
                        }
                    }

                    // apagar do contas receber
                    sql = "delete from tb_receber where id_pedido =" + id.ToString();
                    using (Comando comando = new Comando(transacao, sql))
                    {
                        comando.Execute();
                        Log.Sql(sql);
                    }

                    // apagar do histórico de produto
                    sql = "delete from tb_venda_produto where id_pedido =" + id.ToString();
                    using (Comando comando = new Comando(transacao, sql))
                    {
                        comando.Execute();
                        Log.Sql(sql);
                    }

                    // apaga os itens do pedido
                    sql = "delete from tb_pedido_produto where id_pedido = " + id.ToString();
                    using (Comando comando = new Comando(transacao, sql))
                    {
                        comando.Execute();
                        Log.Sql(sql);
                    }

                    // apaga o pedido
                    sql = "delete from tb_pedido where id_pedido = " + id.ToString();
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
    }
}