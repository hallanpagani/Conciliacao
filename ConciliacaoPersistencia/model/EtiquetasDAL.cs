using Funcoes.classes;
using ConciliacaoPersistencia.banco;
using ConciliacaoPersistencia.classes;

namespace ConciliacaoPersistencia.model
{
    public class EtiquetasDAL
    {
        public static void LimparTudo()
        {
            using (Conexao conexao = Conexao.Get(DAL.GetStringConexao()))
            {
                string sql = "delete from tb_etiquetas";

                using (Comando comando = new Comando(conexao, sql))
                {
                    comando.Execute();
                    Log.Sql(sql);
                }
            }
        }
    }
}