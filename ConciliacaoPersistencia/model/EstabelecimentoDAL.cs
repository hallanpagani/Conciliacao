using ConciliacaoPersistencia.banco;
using System;

namespace ConciliacaoPersistencia.model
{
    public class EstabelecimentoDAL
    {
        public static void ExcluirDuplicados(int id_conta )
        {
            using (Conexao conexao2 = Conexao.Get(DAL.GetStringConexao()))
            using (Transacao transacao = new Transacao(conexao2))
                try
                {
                    string sql2 = string.Format(@"delete from conciliador_estabelecimento where id_conciliador in ( select id from (select dt_transacao, vl_bruto, tot_parcela, nsu_rede, produto, min(id_conciliador) as id , count(*) from conciliador_estabelecimento where id_conta={0} group by 1,2,3,4,5 having count(*) > 1 order by dt_transacao) as x )", id_conta);
                    using (Comando comando = new Comando(transacao, sql2))
                    {
                        comando.Execute();
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
