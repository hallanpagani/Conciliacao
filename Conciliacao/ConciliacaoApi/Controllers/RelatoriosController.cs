using ConciliacaoModelo.model.conciliador;
using ConciliacaoModelo.model.conciliador.UseRede;
using ConciliacaoModelo.model.relatorio;
using ConciliacaoPersistencia.banco;
using ConciliacaoPersistencia.model;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Xml.Serialization;

namespace ConciliacaoAPI.Controllers
{
    public class RelatoriosController : ApiController
    {
        // GET: Relatorios
        [HttpGet]
        [Route("api/relatorios/TransacaoRedeVsTEFListar/{idconta:long}/{idrede:long}/{tpsituacao:long}/{tpdata:long}/{datainicio:long}/{datafinal:long}/{resumo}")]
        public List<TransacaoRedeVsTEFListar> TransacaoRedeVsTEFListar(long idconta, long idrede, long tpsituacao,
            long tpdata, long datainicio, long datafinal,string resumo = "0")
        {

            string filtro = String.Format("id_conta={0}", idconta);
            if (idrede != 0)
            {
                filtro = filtro +
                         String.Format(
                             " and ds_rede=(select ds_rede from cadastro_rede where id_rede={0} and id_conta={1})",
                             idrede, idconta);
            }

            if (tpsituacao == 0)
            {
                filtro = filtro +
                         " and coalesce((select 1 from conciliador_tef t where t.ds_rede = a.ds_rede and t.dt_transacao = a.dt_transacao and t.nsu_rede = a.nsu_rede),0) = 1";
            }
            else if (tpsituacao == 1)
            {
                filtro = filtro +
                         " and coalesce((select 1 from conciliador_tef t where t.ds_rede = a.ds_rede and t.dt_transacao = a.dt_transacao and t.nsu_rede = a.nsu_rede),0) = 0";
            }

            filtro = filtro +
                     String.Format(" and {0} between '{1}' and '{2}' ",
                         tpdata.Equals(0) ? "dt_transacao" : "dt_transacao",
                         new DateTime(datainicio).ToString("yyyy-MM-dd"),
                         new DateTime(datafinal).ToString("yyyy-MM-dd"));

            return DAL.ListarObjetos<TransacaoRedeVsTEFListar>(filtro,
                String.Format(" 6 {0} desc ", tpdata.Equals(0) ? ",dt_transacao" : ",dt_transacao"));
        }

        [HttpGet]
        [Route("api/relatorios/TransacaoRedeVsExtratoBancarioListar/{tpsituacao:long}/{tpdata:long}/{datainicio:long}/{datafinal:long}")]
        public List<TransacaoRedeVsExtratoBancarioListar> TransacaoRedeVsExtratoBancarioListar(long idconta, long tpsituacao, long tpdata, long datainicio, long datafinal)
        {

            string filtro = String.Format("id_conta={0}", idconta);
            /* if (idrede != 0)
             {
                 filtro = filtro + String.Format(" and ds_rede=(select ds_rede from cadastro_rede where id_rede={0} and id_conta={1})", idrede, idconta);
             }*/

            if (tpsituacao == 0)
            {
                filtro = filtro +
                         " and coalesce((select 1 from conciliador_rede r where r.vl_liquido = a.vl_mvto and r.dt_prev_pagto = a.dt_mvto and r.id_conta = a.id_conta ), 0) = 1";
            }
            else if (tpsituacao == 1)
            {
                filtro = filtro +
                         " and coalesce((select 1 from conciliador_rede r where r.vl_liquido = a.vl_mvto and r.dt_prev_pagto = a.dt_mvto and r.id_conta = a.id_conta ), 0) = 0";
            }

            filtro = filtro +
                     String.Format(" and {0} between '{1}' and '{2}' ", tpdata.Equals(0) ? "dt_mvto" : "dt_mvto",
                         new DateTime(datainicio).ToString("yyyy-MM-dd"),
                         new DateTime(datafinal).ToString("yyyy-MM-dd"));

            return DAL.ListarObjetos<TransacaoRedeVsExtratoBancarioListar>(filtro,
                String.Format(" 6 {0} desc ", tpdata.Equals(0) ? ",dt_mvto" : ",dt_mvto"));
        }

        [HttpGet]
        [Route("api/relatorios/TransacaoRedeListar/{idconta:long}/{tpcartao:long}/{tpdata:long}/{datainicio:long}/{datafinal:long}/{resumo}/{bandeira}/{banco}")]
        public List<ConciliacaoTransacaoRede> TransacaoRedeListar(long idconta, long tpcartao, long tpdata, long datainicio, long datafinal, string resumo = "0",string bandeira = "0", string banco="0" )
        {

            string filtro = String.Format("where a.id_conta={0}", idconta);
            string filtro1 = filtro + " coalesce(a.hora_transacao, '00:00:00') <> '00:00:00' ";

            if (!string.IsNullOrEmpty(resumo) && (!resumo.Equals("0")))
            {
                filtro = filtro + string.Format(" and a.numero_resumo_venda = {0} ", resumo.ToUpper());
            }


            /* if (idrede != 0)
             {
                 filtro = filtro + String.Format(" and ds_rede=(select ds_rede from cadastro_rede where id_rede={0} and id_conta={1})", idrede, idconta);
             }

             if (tpsituacao == 0)
             {
                 filtro = filtro + " and coalesce((select 1 from conciliador_tef t where t.ds_rede = a.ds_rede and t.dt_transacao = a.dt_transacao and t.nsu_rede = a.nsu_rede),0) = 1";
             }
             else if (tpsituacao == 1)
             {
                 filtro = filtro + " and coalesce((select 1 from conciliador_tef t where t.ds_rede = a.ds_rede and t.dt_transacao = a.dt_transacao and t.nsu_rede = a.nsu_rede),0) = 0";
             }*/

            filtro1 = filtro +
                 String.Format(" and {0} between '{1}' and '{2}' ",
                 tpdata == 0 ? "a.data_cv" : "a.data_credito",
                     new DateTime(datainicio).ToString("yyyy-MM-dd"),
                     new DateTime(datafinal).ToString("yyyy-MM-dd"));

            filtro = filtro +
                     String.Format(" and {0} between '{1}' and '{2}' ",
                     tpdata == 0 ? "a.data_resumo_venda" : "a.data_credito",
                         new DateTime(datainicio).ToString("yyyy-MM-dd"),
                         new DateTime(datafinal).ToString("yyyy-MM-dd"));


            if (!bandeira.Equals("0") && (!bandeira.Equals("\"\"")))
            {
                filtro = filtro + string.Format(" and UPPER(a.bandeira) = '{0}' ", bandeira.ToUpper());
                filtro1 = filtro1 + string.Format(" and UPPER(a.bandeira) = '{0}' ", bandeira.ToUpper());
            }

            if (!banco.Equals("0") && (!banco.Equals("\"\"")))
            {
                filtro = filtro + string.Format(" and CAST(banco AS UNSIGNED) = {0} ", banco.ToUpper());
                filtro1 = filtro1 + string.Format(" and CAST(banco AS UNSIGNED) = {0} ", banco.ToUpper());
            }


            /*  string sqldebitocielo = @"select 
                                      1 as ordem, 
                                      'RESUMO' as resumo,
                                      99 as tipo_registro,
                                      nm_estabelecimento as numero_filiacao_pv,
                                      data_credito,
                                      dt_transacao as data_resumo_venda,
                                      numero_resumo_venda,
                                      '' as nsu_rede,
                                      valor_bruto,
                                      valor_liquido,
                                      bandeira,
                                      c.id_conta,
                                      '' as status_transacao,
                                      (100-((valor_liquido*100)/valor_bruto)) as taxa_cobrada,
                                      'Débito' as produto,
                                      banco,
                                      agencia,
                                      conta_corrente,
                                      '' as numero_cartao,
                                      '' as tipo_captura
                              from conciliador_userede_eevd_resumooperacao c
                              union all
                              select 
                                      2 as ordem,
                                      'COMPROVANTE' as resumo,
                                      100 as tipo_registro,
                                      numero_filiacao_pv,
                                      data_credito,
                                      data_cv,
                                      numero_resumo_venda,
                                      a.numero_cv,
                                      valor_bruto,
                                      valor_liquido,
                                      bandeira,
                                      a.id_conta,
                                      a.status_transacao,
                                      a.taxa_cobrada,
                                      'Débito' as produto,
                                      '' as banco,
                                      '' as agencia,
                                      '' as conta_corrente,
                                      numero_cartao,
                                      tipo_captura
                              from `conciliador_userede_eevd_comprovantevenda` a";*/


            string sqldebito = @"select distinct
                                    1 as ordem, 
	                                'RESUMO' as resumo,
                                    tipo_registro,
                                    numero_filiacao_pv,
                                    data_credito,
                                    (select max(x.data_resumo_venda) 
                                       from conciliador_userede_eevd_resumooperacao x
                                       where 
                                          x.numero_resumo_venda = a.numero_resumo_venda and
                                          x.numero_filiacao_pv = a.numero_filiacao_pv and
                                          x.data_credito = a.data_credito and
                                          x.valor_bruto = a.valor_bruto
                                      ) as data_resumo_venda,
                                    cast(numero_resumo_venda as CHAR(20)) as numero_resumo_venda ,
                                    '' as nsu_rede,
                                    valor_bruto,
                                    valor_liquido,
                                    bandeira,
                                    a.id_conta,
                                    (100-((valor_liquido*100)/valor_bruto)) as taxa_cobrada,
                                    'Débito' as produto,
                                    banco,
                                    agencia,
                                    conta_corrente,
                                    '' as numero_cartao,
                                    '' as tipo_captura,
                                    er.codigo_estabelecimento,
                                    er.nome_estabelecimento 
                            from conciliador_userede_eevd_resumooperacao a
                            left join cadastro_estabelecimento_rede er on er.id_estabelecimento_rede = cast(a.numero_filiacao_pv as decimal(20,0)) and (er.id_rede = coalesce(a.rede,1))
                            " + filtro+
                            
                            @"union all
                            select distinct
                                    2 as ordem,
	                                'COMPROVANTE' as resumo,
                                    a.tipo_registro,
                                    a.numero_filiacao_pv,
                                    a.data_credito,
                                    a.data_cv,
                                    a.numero_resumo_venda,
                                    a.numero_cv,
                                    a.valor_bruto,
                                    a.valor_liquido,
                                    a.bandeira,
                                    a.id_conta,
                                    a.taxa_cobrada,
                                    'Débito' as produto,
                                    ar.banco as banco,
                                    ar.agencia as agencia,
                                    ar.conta_corrente as conta_corrente,
                                    a.numero_cartao,
                                    a.tipo_captura,
                                    er.codigo_estabelecimento,
                                    er.nome_estabelecimento 
                            from conciliador_userede_eevd_comprovantevenda a
                            left join conciliador_userede_eevd_resumooperacao ar on ar.numero_filiacao_pv = cast(a.numero_filiacao_pv as decimal(20,0)) and 
                            														ar.numero_resumo_venda = a.numero_resumo_venda and
                                                                                    ar.id_conta = a.id_conta and
                                                                                    ar.bandeira = a.bandeira
                            left join cadastro_estabelecimento_rede er on er.id_estabelecimento_rede = cast(a.numero_filiacao_pv as decimal(20,0)) and (er.id_rede = coalesce(a.rede,1))
                            " + filtro1;


            string sqlcredito = @"select distinct
                                    1 as ordem, 
	                                'RESUMO' as resumo,
                                    tipo_registro,
                                    numero_filiacao_pv,
                                    data_credito,
                                    data_resumo_venda,
                                    numero_resumo_venda,
                                    '' as nsu_rede,
                                    valor_bruto,
                                    valor_liquido,
                                    bandeira,
                                    a.id_conta,
                                    (100-((valor_liquido*100)/valor_bruto)) as taxa_cobrada,
                                    'Crédito' as produto,
                                    banco,
                                    agencia,
                                    conta_corrente,
                                    '' as numero_cartao,
                                    '' as tipo_captura,
                                    er.codigo_estabelecimento,
                                    er.nome_estabelecimento  
                            from conciliador_userede_eevc_resumooperacao a
                            left join cadastro_estabelecimento_rede er on er.id_estabelecimento_rede = cast(a.numero_filiacao_pv as decimal(20,0)) and (er.id_rede = coalesce(a.rede,1))
                            " + filtro + @"                            
                            union all
                            select distinct
                                    2 as ordem,
	                                'COMPROVANTE' as resumo,
                                    a.tipo_registro,
                                    a.numero_filiacao_pv,
                                    a.data_credito,
                                    data_cv,
                                    a.numero_resumo_venda,
                                    a.numero_cv,
                                    a.valor_bruto,
                                    a.valor_liquido,
                                    a.bandeira,
                                    a.id_conta,
                                    a.taxa_cobrada,
                                    'Crédito' as produto,
                                    ar.banco as banco,
                                    ar.agencia as agencia,
                                    ar.conta_corrente as conta_corrente,
                                    a.numero_cartao,
                                    a.tipo_captura,
                                    er.codigo_estabelecimento,
                                    er.nome_estabelecimento 
                            from conciliador_userede_eevc_comprovantevenda a
                            left join conciliador_userede_eevc_resumooperacao ar on ar.numero_filiacao_pv = cast(a.numero_filiacao_pv as decimal(20,0)) and 
                            														ar.numero_resumo_venda = a.numero_resumo_venda and
                                                                                    ar.id_conta = a.id_conta and
                                                                                    ar.bandeira = a.bandeira
                            left join cadastro_estabelecimento_rede er on er.id_estabelecimento_rede = cast(a.numero_filiacao_pv as decimal(20,0)) and (er.id_rede = coalesce(a.rede,1))
                            " + filtro1;


            if (tpcartao == 1)
            {
                var ListarObjetosFromSql = DAL.ListarObjetosFromSql<ConciliacaoTransacaoRede>(string.Format(@"select * from (" + sqldebito + @"
                                                                                ) as a
                                                                                {0} and
                                                                                DATEDIFF( a.data_credito,a.data_resumo_venda) < 20
                                                                                order by numero_resumo_venda, 1

                                                                                ", filtro));
                return ListarObjetosFromSql; 
            }
            else if (tpcartao == 0)
            {
                return DAL.ListarObjetosFromSql<ConciliacaoTransacaoRede>(string.Format(@"select * from ("+sqlcredito +@"
                                                                                ) as  a
                                                                                {0}                                                                                
                                                                                order by numero_resumo_venda, 1

                                                                                ", filtro));
            }
            else
            {
                return DAL.ListarObjetosFromSql<ConciliacaoTransacaoRede>(string.Format(@"select * from (
                                                                                " + sqldebito + @"  
                                                                        
                                                                                  union all "
                                                                                 + sqlcredito + @" ) as  a
                                                                                                                                                  
                                                                                order by numero_resumo_venda, 1

                                                                                ", filtro));
            }
        }

        [HttpGet]
        [Route("api/relatorios/TransacaoRedeListarXML/{chave}/{tpcartao:long}/{datainicio}/{datafinal}/{resumo}")]
        public List<ConciliacaoTransacaoRede> TransacaoRedeListarXML(string chave, long tpcartao, string datainicio, string datafinal, string resumo = "0")
        {

            if (chave.Equals(""))
            {
                return new List<ConciliacaoTransacaoRede>();
            }

            string idconta = DAL.GetString(string.Format("select id from sistema_conta where ds_chave='{0}' ", chave), "");
            if (idconta.Equals(""))
            {
                return new List<ConciliacaoTransacaoRede>();
            }



            string filtro = String.Format("where id_conta={0}", idconta);

            if (!string.IsNullOrEmpty(resumo) && (!resumo.Equals("0")))
            {
                filtro = filtro + string.Format(" and numero_resumo_venda = {0} ", resumo.ToUpper());
            }


            /* if (idrede != 0)
             {
                 filtro = filtro + String.Format(" and ds_rede=(select ds_rede from cadastro_rede where id_rede={0} and id_conta={1})", idrede, idconta);
             }

             if (tpsituacao == 0)
             {
                 filtro = filtro + " and coalesce((select 1 from conciliador_tef t where t.ds_rede = a.ds_rede and t.dt_transacao = a.dt_transacao and t.nsu_rede = a.nsu_rede),0) = 1";
             }
             else if (tpsituacao == 1)
             {
                 filtro = filtro + " and coalesce((select 1 from conciliador_tef t where t.ds_rede = a.ds_rede and t.dt_transacao = a.dt_transacao and t.nsu_rede = a.nsu_rede),0) = 0";
             }*/

            filtro = filtro +
                     String.Format(" and {0} between '{1}' and '{2}' ",
                          "data_resumo_venda",
                         DateTime.ParseExact(string.Format("{0}/{1}/{2}", datainicio.ToString().Substring(0, 2),
                                                                          datainicio.ToString().Substring(2, 2),
                                                                          datainicio.ToString().Substring(4, 4))
            , "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"),
                         DateTime.ParseExact(string.Format("{0}/{1}/{2}", datafinal.ToString().Substring(0, 2),
                                                                          datafinal.ToString().Substring(2, 2),
                                                                          datafinal.ToString().Substring(4, 4)), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));

            if (tpcartao == 1)
            {

                return DAL.ListarObjetosFromSql<ConciliacaoTransacaoRede>(string.Format(@"select * from (
                                                                                select 
                                                                                       1 as ordem, 
	                                                                                   'RESUMO' as resumo,
                                                                                       tipo_registro,
                                                                                       numero_filiacao_pv,
                                                                                       data_credito,
                                                                                       data_resumo_venda,
                                                                                       numero_resumo_venda,
                                                                                       '' as nsu_rede,
                                                                                       valor_bruto,
                                                                                       valor_liquido,
                                                                                       bandeira,
                                                                                       a.id_conta,
                                                                                       '' as status_transacao,
                                                                                       0 as taxa_cobrada 
                                                                                from conciliador_userede_eevd_resumooperacao c
                                                                                union all
                                                                                select 
                                                                                       2 as ordem,
	                                                                                   'COMPROVANTE' as resumo,
                                                                                       tipo_registro,
                                                                                       numero_filiacao_pv,
                                                                                       '' as data_credito,
                                                                                       data_cv,
                                                                                       numero_resumo_venda,
                                                                                       a.numero_cv,
                                                                                       valor_bruto,
                                                                                       valor_liquido,
                                                                                       bandeira,
                                                                                       a.id_conta,
                                                                                       a.status_transacao,
                                                                                       a.taxa_cobrada
                                                                                from `conciliador_userede_eevd_comprovantevenda` a ) as  x
                                                                                {0}                                                                                
                                                                                order by numero_resumo_venda, 1

                                                                                ", filtro));
            }
            else if (tpcartao == 0)
            {
                return DAL.ListarObjetosFromSql<ConciliacaoTransacaoRede>(string.Format(@"select * from (


                                                                                select 
                                                                                       1 as ordem, 
	                                                                                   'RESUMO' as resumo,
                                                                                       tipo_registro,
                                                                                       numero_filiacao_pv,
                                                                                       data_credito,
                                                                                       data_resumo_venda,
                                                                                       numero_resumo_venda,
                                                                                       '' as nsu_rede,
                                                                                       valor_bruto,
                                                                                       valor_liquido,
                                                                                       bandeira,
                                                                                       a.id_conta,
                                                                                       '' as status_transacao,
                                                                                       0 as taxa_cobrada 
                                                                                from conciliador_userede_eevc_resumooperacao c
                                                                                union all
                                                                                select 
                                                                                       2 as ordem,
	                                                                                   'COMPROVANTE' as resumo,
                                                                                       tipo_registro,
                                                                                       numero_filiacao_pv,
                                                                                       null as data_credito,
                                                                                       data_cv,
                                                                                       numero_resumo_venda,
                                                                                       a.numero_cv,
                                                                                       valor_bruto,
                                                                                       valor_liquido,
                                                                                       bandeira,
                                                                                       a.id_conta,
                                                                                       a.status_cv,
                                                                                       a.taxa_cobrada
                                                                                from `conciliador_userede_eevc_comprovantevenda` a ) as  x
                                                                                {0}                                                                                
                                                                                order by numero_resumo_venda, 1

                                                                                ", filtro));
            } else
            {
                return DAL.ListarObjetosFromSql<ConciliacaoTransacaoRede>(string.Format(@"select * from (
                                                                                select 
                                                                                       1 as ordem, 
	                                                                                   'RESUMO' as resumo,
                                                                                       tipo_registro,
                                                                                       numero_filiacao_pv,
                                                                                       data_credito,
                                                                                       data_resumo_venda,
                                                                                       numero_resumo_venda,
                                                                                       '' as nsu_rede,
                                                                                       valor_bruto,
                                                                                       valor_liquido,
                                                                                       bandeira,
                                                                                       a.id_conta,
                                                                                       '' as status_transacao,
                                                                                       0 as taxa_cobrada
                                                                                from conciliador_userede_eevd_resumooperacao c
                                                                                union all
                                                                                select 
                                                                                       2 as ordem,
	                                                                                   'COMPROVANTE' as resumo,
                                                                                       tipo_registro,
                                                                                       numero_filiacao_pv,
                                                                                       data_credito,
                                                                                       data_cv,
                                                                                       numero_resumo_venda,
                                                                                       a.numero_cv,
                                                                                       valor_bruto,
                                                                                       valor_liquido,
                                                                                       bandeira,
                                                                                       a.id_conta,
                                                                                       a.status_transacao,
                                                                                       a.taxa_cobrada
                                                                                from `conciliador_userede_eevd_comprovantevenda` a
                                                                                                                                                        
                                                                               
                                                                                union all
                                                                                select 
                                                                                       1 as ordem, 
	                                                                                   'RESUMO' as resumo,
                                                                                       tipo_registro,
                                                                                       numero_filiacao_pv,
                                                                                       data_credito,
                                                                                       data_resumo_venda,
                                                                                       numero_resumo_venda,
                                                                                       '' as nsu_rede,
                                                                                       valor_bruto,
                                                                                       valor_liquido,
                                                                                       bandeira,
                                                                                       a.id_conta,
                                                                                       '' as status_transacao,
                                                                                       0 as taxa_cobrada
                                                                                from conciliador_userede_eevc_resumooperacao c
                                                                                union all
                                                                                select 
                                                                                       2 as ordem,
	                                                                                   'COMPROVANTE' as resumo,
                                                                                       tipo_registro,
                                                                                       numero_filiacao_pv,
                                                                                       null as data_credito,
                                                                                       data_cv,
                                                                                       numero_resumo_venda,
                                                                                       a.numero_cv,
                                                                                       valor_bruto,
                                                                                       valor_liquido,
                                                                                       bandeira,
                                                                                       a.id_conta,
                                                                                       a.status_cv,
                                                                                       a.taxa_cobrada
                                                                                from `conciliador_userede_eevc_comprovantevenda` a ) as  x
                                                                                {0}                                                                                
                                                                                order by numero_resumo_venda, 1

                                                                                ", filtro));
            }
        }

        [HttpGet]
        [Route("api/relatorios/TransacaoTEFListarXML/{chave}/{rede}/{datainicio}/{datafinal}/{valor}/{estabelecimento}/{administrador}/{resumo}/{loja}/{tp_transacao}/{tp_operacao}")]
        public List<TransacaoTEFListar> TransacaoTEFListarXML(string chave, string rede, string datainicio, string datafinal, string valor, string estabelecimento, string administrador, string resumo, string loja, string tp_transacao, string tp_operacao)
        {
            if (chave.Equals(""))
            {
                return new List<TransacaoTEFListar>();
            }

            string idconta = DAL.GetString(string.Format("select id from sistema_conta where ds_chave='{0}' ", chave), "");
            if (idconta.Equals(""))
            {
                return new List<TransacaoTEFListar>();
            }

            var dtini = DateTime.ParseExact(string.Format("{0}/{1}/{2}", datainicio.ToString().Substring(0, 2),
                                                                          datainicio.ToString().Substring(2, 2),
                                                                          datainicio.ToString().Substring(4, 4)), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            var dtfim = DateTime.ParseExact(string.Format("{0}/{1}/{2}", datafinal.ToString().Substring(0, 2),
                                                              datafinal.ToString().Substring(2, 2),
                                                              datafinal.ToString().Substring(4, 4)), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);


            var tef = TEFDAL.TransacaoTEFListar(Convert.ToInt32(idconta), rede, 0, 0, dtini.Ticks, dtfim.Ticks, Convert.ToDecimal(valor), estabelecimento ?? "-111", administrador ?? "-111", resumo, loja, tp_transacao, tp_operacao);


            foreach (var item in tef)
            {
                var sql = string.Format(@"select distinct encontrou
                                           from(select 1 as encontrou
                                                from
                                                    conciliador_userede_eevd_comprovantevenda b
                                                where
                                                    (b.numero_cv = cast('{0}' as DECIMAL)) and (b.id_conta = {1} )
                                                union all
                                                select
                                                    1
                                                from
                                                    conciliador_userede_eevc_comprovantevenda c
                                                where
                                                    (c.numero_cv = cast('{0}' as DECIMAL)) and (c.id_conta = {1} )  ) as x  ",
                    item.nsu_rede, idconta);
                item.conciliado = (int)DAL.GetInt(sql, 0);
            }

            return tef;
        }

        [HttpGet]
        [Route("api/relatorios/RelatorioFinanceiroCreditosListarXML/{chave}/{datainicio}/{datafinal}/{bandeira}/{resumo}/{banco}")]
        public List<ConciliacaoUseRedeEEFICreditosStructListar> RelatorioFinanceiroCreditosListarXML(string chave, string datainicio, string datafinal, string bandeira, string resumo = "0", string banco = "0")
        {
            if (chave.Equals(""))
            {
                return new List<ConciliacaoUseRedeEEFICreditosStructListar>();
            }

            string idconta = DAL.GetString(string.Format("select id from sistema_conta where ds_chave='{0}' ", chave), "");
            if (idconta.Equals(""))
            {
                return new List<ConciliacaoUseRedeEEFICreditosStructListar>();
            }

            var dtini = string.Format("{2}-{1}-{0}", datainicio.ToString().Substring(0, 2),
                                                                          datainicio.ToString().Substring(2, 2),
                                                                          datainicio.ToString().Substring(4, 4));

            var dtfim = string.Format("{2}-{1}-{0}", datafinal.ToString().Substring(0, 2),
                                                              datafinal.ToString().Substring(2, 2),
                                                              datafinal.ToString().Substring(4, 4));

            var filtro = "";
            if (!bandeira.Equals("0") && (!bandeira.Equals("\"\"")))
            {
                filtro = filtro + string.Format(" and UPPER(bandeira) = '{0}' ", bandeira.ToUpper());

                if (resumo != "0") {

                    filtro = filtro + string.Format(" and numero_rv = {4} ", resumo);

                }
            }

            if (!banco.Equals("0") && (!banco.Equals("\"\"")))
            {
                filtro = filtro + string.Format(" and CAST(banco AS UNSIGNED) = {0} ", banco.ToUpper());
            }

            return DAL.ListarObjetosFromSql<ConciliacaoUseRedeEEFICreditosStructListar>(string.Format(@"select numero_pv_centralizador,
                                                                                               (
                                                                                                 select codigo_estabelecimento
                                                                                                 from cadastro_estabelecimento_rede er
                                                                                                 where er.id_estabelecimento_rede = cast(a.numero_pv_centralizador as
                                                                                                 decimal (20, 0)) and
                                                                                                       (er.id_rede = coalesce(a.rede, 1))
                                                                                               ) as codigo_estabelecimento,
                                                                                               (
                                                                                                 select nome_estabelecimento
                                                                                                 from cadastro_estabelecimento_rede er
                                                                                                 where er.id_estabelecimento_rede = cast(a.numero_pv_centralizador as
                                                                                                 decimal (20, 0)) and
                                                                                                       (er.id_rede = coalesce(a.rede, 1))
                                                                                               ) as nome_estabelecimento,
                                                                                               numero_documento,
                                                                                               data_lancamento,
                                                                                               valor_lancamento,
                                                                                               banco as banco,
                                                                                               TRIM(LEADING '0' FROM agencia) as agencia,
                                                                                               TRIM(LEADING '0' FROM conta_corrente) as conta_corrente,
                                                                                               data_movimento,
                                                                                               numero_rv,
                                                                                               data_rv,
                                                                                               bandeira,
                                                                                               tipo_transacao,
                                                                                               valor_bruto_rv,
                                                                                               valor_taxa_desconto,
                                                                                               numero_parcela,
                                                                                               situacao,
                                                                                               numero_pv_original,
                                                                                               rede
                                                                                        from conciliador_userede_eefi_credito a
                                                                                        where {0}
                                                                                        group by
                                                                                               1, 2, 3, 4, 5, 6,
                                                                                               7, 8, 9, 10, 11, 12, 13,
                                                                                               14, 15, 16, 17, 18, 19, 20", (string.Format("id_conta={0} and data_lancamento >= '{1}' and data_lancamento <= '{2}' {3} ", idconta, dtini, dtfim, filtro)) ));  
        }

        [HttpGet]
        [Route("api/relatorios/RelatorioFinanceiroAntecipacaoListarXML/{chave}/{datainicio}/{datafinal}")]
        public List<ConciliacaoUseRedeEEFIAntecipacaoStructListar> RelatorioFinanceiroAntecipacaoListarXML(string chave, string datainicio, string datafinal)
        {
            if (chave.Equals(""))
            {
                return new List<ConciliacaoUseRedeEEFIAntecipacaoStructListar>();
            }

            string idconta = DAL.GetString(string.Format("select id from sistema_conta where ds_chave='{0}' ", chave), "");
            if (idconta.Equals(""))
            {
                return new List<ConciliacaoUseRedeEEFIAntecipacaoStructListar>();
            }

            var dtini = string.Format("{2}-{1}-{0}", datainicio.ToString().Substring(0, 2),
                                                                          datainicio.ToString().Substring(2, 2),
                                                                          datainicio.ToString().Substring(4, 4));

            var dtfim = string.Format("{2}-{1}-{0}", datafinal.ToString().Substring(0, 2),
                                                              datafinal.ToString().Substring(2, 2),
                                                              datafinal.ToString().Substring(4, 4));

            return DAL.ListarObjetos<ConciliacaoUseRedeEEFIAntecipacaoStructListar>(string.Format("id_conta={0} and data_lancamento >= '{1}' and data_lancamento <= '{2}' ", idconta, dtini, dtfim));
        }

        [HttpGet]
        [Route("api/relatorios/RelatorioFinanceiroDesagendamentoParcelasListarXML/{chave}/{datainicio}/{datafinal}")]
        public List<ConciliacaoUseRedeEEFIDesagendamentoParcelasStructListar> RelatorioFinanceiroDesagendamentoParcelasListarXML(string chave, string datainicio, string datafinal)
        {
            if (chave.Equals(""))
            {
                return new List<ConciliacaoUseRedeEEFIDesagendamentoParcelasStructListar>();
            }

            string idconta = DAL.GetString(string.Format("select id from sistema_conta where ds_chave='{0}' ", chave), "");
            if (idconta.Equals(""))
            {
                return new List<ConciliacaoUseRedeEEFIDesagendamentoParcelasStructListar>();
            }

            var dtini = string.Format("{2}-{1}-{0}", datainicio.ToString().Substring(0, 2),
                                                                          datainicio.ToString().Substring(2, 2),
                                                                          datainicio.ToString().Substring(4, 4));

            var dtfim = string.Format("{2}-{1}-{0}", datafinal.ToString().Substring(0, 2),
                                                              datafinal.ToString().Substring(2, 2),
                                                              datafinal.ToString().Substring(4, 4));

            return DAL.ListarObjetos<ConciliacaoUseRedeEEFIDesagendamentoParcelasStructListar>(string.Format("id_conta={0} and data_credito >= '{1}' and data_credito <= '{2}' ", idconta, dtini, dtfim));
        }

        [HttpGet]
        [Route("api/relatorios/RelatorioFinanceiroAjustesDesagendamentoListarXML/{chave}/{datainicio}/{datafinal}")]
        public List<ConciliacaoUseRedeEEFIAjustesDesagendamentoStructListar> RelatorioFinanceiroAjustesDesagendamentoListarXML(string chave, string datainicio, string datafinal)
        {
            if (chave.Equals(""))
            {
                return new List<ConciliacaoUseRedeEEFIAjustesDesagendamentoStructListar>();
            }

            string idconta = DAL.GetString(string.Format("select id from sistema_conta where ds_chave='{0}' ", chave), "");
            if (idconta.Equals(""))
            {
                return new List<ConciliacaoUseRedeEEFIAjustesDesagendamentoStructListar>();
            }

            var dtini = string.Format("{2}-{1}-{0}", datainicio.ToString().Substring(0, 2),
                                                                          datainicio.ToString().Substring(2, 2),
                                                                          datainicio.ToString().Substring(4, 4));

            var dtfim = string.Format("{2}-{1}-{0}", datafinal.ToString().Substring(0, 2),
                                                              datafinal.ToString().Substring(2, 2),
                                                              datafinal.ToString().Substring(4, 4));

            return DAL.ListarObjetos<ConciliacaoUseRedeEEFIAjustesDesagendamentoStructListar>(string.Format("id_conta={0} and data_ajuste >= '{1}' and data_ajuste <= '{2}' ", idconta, dtini, dtfim));
        }
    }
}