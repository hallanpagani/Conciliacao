using ConciliacaoPersistencia.banco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConciliacaoPersistencia.model
{
    public class DALErro
    {
        public static void Gravar(string message)
        {

            using (Conexao conexao2 = Conexao.Get(DAL.GetStringConexao()))
            using (Transacao transacao = new Transacao(conexao2))
                try
                {
                    string sql2 = string.Format("insert into sistema_erro(mensagem) value ('{0}'); ", message) ;
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
