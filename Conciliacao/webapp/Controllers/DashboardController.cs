using System.Collections.Generic;
using Conciliacao.Helper.Rest;
using System.Web.Mvc;
using System;
using System.Linq;
using ConciliacaoModelo.model.conciliador;

namespace Conciliacao.Controllers
{
    public class DashboardController : Controller
    {
        static readonly DashboardRestClient _restClient = new DashboardRestClient();

        [HttpGet]
        [OutputCache(Duration = 10)]
        public JsonResult GetTotalizadores()
        {
            var obj = new List<TotaisDashboard>
            {
                _restClient.GetTotalizadorDia(0),
                _restClient.GetTotalizadorDia(7),
                _restClient.GetTotalizadorDia(30),
                _restClient.GetTotalizadorDia(-1),
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 10)]
        public JsonResult GetTotalizadoresPorBanco()
        {
            var obj = _restClient.GetTotalizadorPorBanco(30);
            
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 10)]
        public JsonResult GetTotalizadoresPorProduto()
        {
            var obj = _restClient.GetTotalizadorPorProduto(-30);

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 10)]
        public JsonResult GetTotalizadoresPorProdutoProximos()
        {
            var obj = _restClient.GetTotalizadorPorProduto(30);

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTotalizadoresPorDia(string start, string end)
        {
            //código para trazer os eventos do mês
            DateTime dataInicial = Convert.ToDateTime(start);
            DateTime dataFinal = Convert.ToDateTime(end);
            if ((dataInicial - dataFinal).TotalDays > -2)
            {
                dataFinal = dataInicial;
            }

            List<TotaisDashboardPorData> eventosDb = _restClient.GetTotalizadorDia(dataInicial, dataFinal).OrderBy(o => o.rede).ToList();

            var eventos = from e in (eventosDb ?? new List<TotaisDashboardPorData>())
                orderby e.data_prevista, e.rede
                select new
                {
                    title = e.total_liquido.ToString("#,##0.00") + " " + e.rede,
                    start = e.rede.ToUpper().Equals("USEREDE") ? new DateTime(e.data_prevista.Year, e.data_prevista.Month, e.data_prevista.Day, 13,0,0) : e.rede.ToUpper().Equals("CIELO") ? new DateTime(e.data_prevista.Year, e.data_prevista.Month, e.data_prevista.Day, 12,0,0) : new DateTime(e.data_prevista.Year, e.data_prevista.Month, e.data_prevista.Day, 11, 0, 0),
                    end = e.rede.ToUpper().Equals("USEREDE") ? new DateTime(e.data_prevista.Year, e.data_prevista.Month, e.data_prevista.Day, 14, 0, 0) : e.rede.ToUpper().Equals("CIELO") ? new DateTime(e.data_prevista.Year, e.data_prevista.Month, e.data_prevista.Day, 13, 0, 0) : new DateTime(e.data_prevista.Year, e.data_prevista.Month, e.data_prevista.Day, 12, 0, 0),
                    allDay = false,
                    color = e.rede.ToUpper().Equals("USEREDE") ? "#c15d0b" : e.rede.ToUpper().Equals("CIELO") ? "#0b51c1" : e.rede.ToUpper().Equals("BANESE") ? "#3CB371" : e.rede.ToUpper().Equals("GETNET") ? "#b2443b" :  "#000000",
                    data = e.data_prevista.ToString("ddMMyyyy"),
                    debitocredito = "T" // e.tipo.Equals("Crédito") ? "C" : "D"
                };

            return Json(eventos.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTotalizadoresPorDiaDes(string start, string end)
        {
            //código para trazer os eventos do mês
            DateTime dataInicial = Convert.ToDateTime(start);
            DateTime dataFinal = Convert.ToDateTime(end);
            if ((dataInicial - dataFinal).TotalDays > -2)
            {
                dataFinal = dataInicial;
            }

            List<TotaisDashboardPorData> eventosDb = _restClient.GetTotalizadorDiaDes(dataInicial, dataFinal).ToList();

            var eventos = from e in eventosDb ?? new List<TotaisDashboardPorData>()
                          select new
                          {
                              title = e.total_liquido.ToString("#,##0.00"),
                              start = new DateTime(e.data_prevista.Year, e.data_prevista.Month, e.data_prevista.Day),
                              /* end = new DateTime(e.data_prevista.Year, e.data_prevista.Month, e.data_prevista.Day), */
                              allDay = true,
                              color = "#c10b2f"
                          };

            return Json(eventos.ToList(), JsonRequestBehavior.AllowGet);
        }

    }
}