using Conciliacao.Controllers.Generico;
using Conciliacao.Helper.Rest;
using ConciliacaoModelo.model.generico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Conciliacao.Controllers.Cadastros
{
    public class BancoController : AppController
    {
        static readonly BancosRestClient _restClient = new BancosRestClient();

        // GET: Banco
        [HttpGet]
        [OutputCache(Duration = 30)]
        public JsonResult GetBancos(string term)
        {
            List<Lista> list = _restClient.GetBancosAll().ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}