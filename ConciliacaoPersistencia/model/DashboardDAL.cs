using ConciliacaoPersistencia.banco;
using System;
using System.Collections.Generic;
using System.Text;
using ConciliacaoModelo.model.conciliador;

namespace ConciliacaoPersistencia.model
{
    public static class DashboardDAL
    {
        public static TotaisDashboard GetTotalizadorDia(long idConta, long dias)
        {
            if (idConta == 0)
            {
                return new TotaisDashboard();
            }
            var sb = new StringBuilder();

            sb.Append("select ");
            sb.Append("  sum(coalesce(total_bruto)) as total_bruto, ");
            sb.Append("  sum(coalesce(total_liquido)) as total_liquido ");
            sb.Append("from ");
            sb.Append("  totalizador_previsao ");
            sb.AppendFormat(" where id_conta={0} and ", idConta);

            if (dias == 0)
            {
                sb.AppendFormat(" data_prevista = '{0}' ", DateTime.Now.ToString("yyyy-MM-dd"));
            }
            else if (dias == 7)
            {
                sb.AppendFormat(" data_prevista between '{0}' and '{1}' ", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(8).ToString("yyyy-MM-dd"));
            }
            else if (dias == 30)
            {
                sb.AppendFormat(" data_prevista between '{0}' and '{1}' ", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(31).ToString("yyyy-MM-dd"));
            }
            else if (dias == -1)
            {
                sb.AppendFormat(" month(data_prevista) = {0} and year(data_prevista) = {1}", DateTime.Now.Month, DateTime.Now.Year);
            }

            try
            {
                return DAL.ListarObjetoFromSql<TotaisDashboard>(sb.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public static List<TotaisDashboardBanco> GetTotaisDashboardBanco(long idConta, long dias)
        {
            if (idConta == 0)
            {
                return new List<TotaisDashboardBanco>();
            }
            var sb = new StringBuilder();
            sb.Append("select id_banco, ");
            sb.Append("sum(total_pendente) as total_pendente, ");
            sb.Append("sum(total_bruto) as total_bruto,  ");             
            sb.Append("sum(total_hoje) as total_hoje  ");
            sb.Append("from ( ");
            sb.Append("select ");
            sb.Append("  id_banco, ");
            sb.Append("   0 as total_pendente, ");
            sb.Append("  sum(coalesce(total,0)) as total_bruto, ");
            sb.Append("   0 as total_hoje ");
            sb.Append("from ");
            sb.Append("  totalizador_banco ");
            sb.AppendFormat(" where id_conta={0} and ", idConta);
            sb.AppendFormat(" data_prevista between '{0}' and '{1}' ", DateTime.Now.AddDays(-31).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
            sb.AppendFormat(" group by 1,2,4  ");
            sb.Append(" union ");
            sb.Append("select ");
            sb.Append("      id_banco,");
            sb.Append("      sum(coalesce(total,0)),");
            sb.Append("      0, ");
            sb.Append("      0  ");
            sb.Append("    from ");
            sb.Append("      totalizador_banco b ");
            sb.AppendFormat(" where id_conta={0} and ", idConta);
            sb.Append(" data_prevista >  current_date");
            sb.AppendFormat(" group by 1,3  ");
            sb.Append(" union ");
            sb.Append("select ");
            sb.Append("      id_banco,");
            sb.Append("      0,");
            sb.Append("      0, ");
            sb.Append("      sum(coalesce(total,0)) as total_hoje  ");
            sb.Append("    from ");
            sb.Append("      totalizador_banco b ");
            sb.AppendFormat(" where id_conta={0} and ", idConta);
            sb.Append(" data_prevista =  current_date");
            sb.AppendFormat(" group by 1,2,3  ");
            sb.Append("  ) as x ");
            sb.Append("group by 1");



            try
            {
                return DAL.ListarObjetosFromSql<TotaisDashboardBanco>(sb.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<TotaisDashboardProduto> GetTotaisDashboardProduto(long idConta, long dias)
        {
            if (idConta == 0)
            {
                return new List<TotaisDashboardProduto>();
            }
            var sb = new StringBuilder();
            sb.Append("select ");
            sb.Append("  ds_produto, ");
            sb.Append("  sum(coalesce(total_bruto)) as total_bruto, ");
            sb.Append("  sum(coalesce(total_liquido)) as total_liquido ");
            sb.Append("from ");
            sb.Append("  totalizador_previsao ");
            sb.AppendFormat(" where id_conta={0} and ", idConta);
            if (dias >= 0) {
                sb.AppendFormat(" data_prevista between '{0}' and '{1}' ", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(dias).ToString("yyyy-MM-dd"));
            } else
            {
                sb.AppendFormat(" data_prevista between '{0}' and '{1}' ", DateTime.Now.AddDays(dias).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"));
            }           
            sb.AppendFormat(" group by 1  ");
            sb.AppendFormat(" order by 3 desc  ");
            try
            {
                return DAL.ListarObjetosFromSql<TotaisDashboardProduto>(sb.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<TotaisDashboardPorData> GetTotaisDashboardDia(long idConta, long dt_ini, long dt_fim)
        {
            if (idConta == 0)
            {
                return new List<TotaisDashboardPorData>();
            }
            var sb = new StringBuilder();

            sb.Append("select ");
            sb.Append(" data_prevista,  ");
            sb.Append("  sum(coalesce(total_bruto)) as total_bruto, ");
            sb.Append("  sum(coalesce(total_liquido)) as total_liquido, ");


            sb.Append("  ds_rede as rede  "); 


         /*   sb.Append("    case  ");
            sb.Append("      when upper(ds_produto)like '%CRÉDITO%' then  ");
            sb.Append("        'Crédito'  ");
            sb.Append("      when upper(ds_produto) like '%DÉBITO%' then  ");
            sb.Append("        'Débito'  ");
            sb.Append("      else   ");  
            sb.Append("         'Crédito'  ");
            sb.Append("    end as tipo  "); */
            sb.Append("from ");
            sb.Append("  totalizador_previsao ");
            sb.AppendFormat(" where id_conta={0} and ", idConta);
            sb.AppendFormat(" data_prevista between '{0}' and '{1}' ", new DateTime(dt_ini).ToString("yyyy-MM-dd"), new DateTime(dt_fim).ToString("yyyy-MM-dd"));
            sb.Append("  group by 1,4 ");
          //  sb.Append("  group by 1 "); 
            sb.Append("  order by 1 ");
            try
            {
                return DAL.ListarObjetosFromSql<TotaisDashboardPorData>(sb.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<TotaisDashboardPorData> GetTotaisDashboardDiaDesagendamentos(long idConta, long dt_ini, long dt_fim)
        {
            if (idConta == 0)
            {
                return new List<TotaisDashboardPorData>();
            }
            var sb = new StringBuilder();

            sb.Append("select ");
            sb.Append(" data_ajuste as data_prevista,  ");
            sb.Append("  sum(coalesce(valor_transacao)) as total_bruto, ");
            sb.Append("  sum(coalesce(valor_cancelado)) as total_liquido ");
            sb.Append("from ");
            sb.Append("  conciliador_userede_eefi_ajustes_desagendamento ");
            sb.AppendFormat(" where id_conta={0} and ", idConta);
            sb.AppendFormat(" data_ajuste between '{0}' and '{1}' ", new DateTime(dt_ini).ToString("yyyy-MM-dd"), new DateTime(dt_fim).ToString("yyyy-MM-dd"));
            sb.Append("  group by 1 ");
            try
            {
                return DAL.ListarObjetosFromSql<TotaisDashboardPorData>(sb.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
        }



    }
}
