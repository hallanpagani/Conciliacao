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
                                                                        group by 1) as x))
                                                                        ", idconta));
        }

    }
}