using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.banco;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ConciliacaoAPI.Controllers
{
    public class BancosController : ApiController
    {
        [Route("api/Bancos/GetAll/{idconta:long}")]
        public IEnumerable<Lista> GetAll(long idconta)
        {
            return DAL.ListarObjetosFromSql<Lista>(String.Format(@"select 
                                                              `id` as id,
                                                              `banco` as text
                                                            FROM 
                                                              `cadastro_banco` b
                                                            where
                                                             b.`id` in  ((select banco from(

                                                                        select
                                                                          banco
                                                                        from 
                                                                          `conciliador_userede_eefi_credito` e
                                                                        where
                                                                          e.`id_conta` = {0}
                                                                        group by 1
                                                                        union all
                                                                        select
                                                                          CAST(banco AS UNSIGNED)
                                                                        from 
                                                                          `conciliador_userede_eevc_resumooperacao` r
                                                                        where
                                                                          r.`id_conta` = {0}
                                                                        group by 1
                                                                        union all
                                                                        select
                                                                          CAST(banco AS UNSIGNED)
                                                                        from 
                                                                          `conciliador_userede_eevd_resumooperacao` r
                                                                        where
                                                                          r.`id_conta` = {0}
                                                                        group by 1) as x))
                                                                        ", idconta));
        }

        [Route("api/Bancos/GetContasAll/{idconta:long}")]
        public IEnumerable<Lista> GetContasAll(long idconta)
        {
            return DAL.ListarObjetosFromSql<Lista>(String.Format(@"select distinct
                                                              conta as id,
                                                              cast(conta as char) as text
                                                            FROM (select conta from(
                                                                        select
                                                                          CAST(e.conta_corrente AS UNSIGNED) as conta
                                                                        from 
                                                                          `conciliador_userede_eefi_credito` e
                                                                        where
                                                                          e.`id_conta` = {0}
                                                                        group by 1
                                                                        union all
                                                                        select
                                                                          CAST(r.conta_corrente AS UNSIGNED)
                                                                        from 
                                                                          `conciliador_userede_eevc_resumooperacao` r
                                                                        where
                                                                          r.`id_conta` = {0}
                                                                        group by 1
                                                                        union all
                                                                        select
                                                                          CAST(r.conta_corrente AS UNSIGNED)
                                                                        from 
                                                                          `conciliador_userede_eevd_resumooperacao` r
                                                                        where
                                                                          r.`id_conta` = {0}
                                                                        group by 1) as x) as z ", idconta));
        }

        [Route("api/Bancos/GetAgenciasAll/{idconta:long}")]
        public IEnumerable<Lista> GetAgenciasAll(long idconta)
        {
            return DAL.ListarObjetosFromSql<Lista>(String.Format(@"select distinct
                                                              agencia as id,
                                                              cast(agencia as char) as text
                                                            FROM (select agencia from(
                                                                        select
                                                                          CAST(agencia AS UNSIGNED) as agencia
                                                                        from 
                                                                          `conciliador_userede_eefi_credito` e
                                                                        where
                                                                          e.`id_conta` = {0}
                                                                        group by 1
                                                                        union all
                                                                        select
                                                                          CAST(agencia AS UNSIGNED)
                                                                        from 
                                                                          `conciliador_userede_eevc_resumooperacao` r
                                                                        where
                                                                          r.`id_conta` = {0}
                                                                        group by 1
                                                                        union all
                                                                        select
                                                                          CAST(agencia AS UNSIGNED)
                                                                        from 
                                                                          `conciliador_userede_eevd_resumooperacao` r
                                                                        where
                                                                          r.`id_conta` = {0}
                                                                        group by 1) as x) as z", idconta));
        }

    }
}