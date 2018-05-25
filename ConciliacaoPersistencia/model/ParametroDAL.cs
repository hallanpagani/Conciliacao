using System;
using Funcoes.classes;
using ConciliacaoPersistencia.banco;
using ConciliacaoPersistencia.classes;

namespace ConciliacaoPersistencia.model
{
    public static class ParametroDAL
    {
        public static decimal GetTaxaJuro()
        {
            return DAL.GetDecimal("select taxa_juro from tb_parametro limit 1");
        }

        public static void GravarTaxaJuro(decimal valor)
        {
            using (Conexao conexao = Conexao.Get(DAL.GetStringConexao()))
            using (Comando comando = new Comando(conexao, "update tb_parametro set taxa_juro=@taxa_juro"))
            {
                comando.AddParam("@taxa_juro", valor);
                Log.Sql(SqlUtil.CommandAsSql(comando.GetCommand())); // !!!!!!!!!!!!!!!!!!!! LOGA
                comando.Execute();
            }
        }
    }
}