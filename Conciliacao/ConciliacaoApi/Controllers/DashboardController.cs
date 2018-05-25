using ConciliacaoModelo.model.conciliador;
using ConciliacaoPersistencia.model;
using System.Collections.Generic;
using System.Web.Http;

namespace ConciliacaoAPI.Controllers
{
    public class DashboardController : ApiController
    {
        
        [HttpGet]
        [Route("api/dashboard/gettotalizadordia/{idconta:long}/{dias:long}")]
        public TotaisDashboard GetTotalizadorDia(long idconta, long dias)
        {
            var a = DashboardDAL.GetTotalizadorDia(idconta, dias);
            return a;
        }

        [HttpGet]
        [Route("api/dashboard/gettotalizadorbanco/{idconta:long}/{dias:long}")]
        public List<TotaisDashboardBanco> GetTotalizadorBanco(long idconta, long dias)
        {
            var a = DashboardDAL.GetTotaisDashboardBanco(idconta, dias);
            return a;
        }

        [HttpGet]
        [Route("api/dashboard/gettotalizadorproduto/{idconta:long}/{dias:long}")]
        public List<TotaisDashboardProduto> GetTotalizadorProduto(long idconta, long dias)
        {
            var a = DashboardDAL.GetTotaisDashboardProduto(idconta, dias);
            return a;
        }

        [HttpGet]
        [Route("api/dashboard/gettotalizadordia/{idconta:long}/{dt_ini:long}/{dt_fim:long}")]
        public List<TotaisDashboardPorData> GetTotalizadorDia(long idconta, long dt_ini, long dt_fim)
        {
            var a = DashboardDAL.GetTotaisDashboardDia(idconta, dt_ini, dt_fim);
            return a;
        }

        [HttpGet]
        [Route("api/dashboard/gettotalizadordiadesagendamentos/{idconta:long}/{dt_ini:long}/{dt_fim:long}")]
        public List<TotaisDashboardPorData> GetTotalizadorDiaDesagendamentos(long idconta, long dt_ini, long dt_fim)
        {
            var a = DashboardDAL.GetTotaisDashboardDiaDesagendamentos(idconta, dt_ini, dt_fim);
            return a;
        }

    }
}