using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.banco;
using Funcoes.classes;
using System;
using System.Text;

namespace ConciliacaoPersistencia.model
{
    public class PagarDAL
    {
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
                    sb.Append("delete from financeiro_pagar ");
                    sb.Append("where ");
                    sb.Append(String.Format(" id_conta ={0} and ", idconta));
                    sb.Append(String.Format(" id ={0} ", id));
                    using (Comando comando = new Comando(transacao, sb.ToString()))
                    {
                        comando.Execute();
                       
                    }
                    transacao.Commit();
                    response = new Respostas(true, "Pagar excluído!", 0);
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
