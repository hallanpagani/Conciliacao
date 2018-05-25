using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConciliacaoModelo.model.cadastros;
using Conciliacao.Helper.Rest;
using ConciliacaoModelo.model.generico;
using Conciliacao.App_Helpers.Componentes;
using Microsoft.Ajax.Utilities;

namespace Conciliacao.Controllers
{
    public class RedesController : Controller
    {
        static readonly RedeRestClient _restClient = new RedeRestClient();

        // GET: Redes
        public ActionResult CadastrarRedes(long id = 0)
        {
            var model = id > 0 ? _restClient.GetRedesPorId(id) : new Rede();
            return View(model);
        }

        [HttpPost]
        public ActionResult CadastrarRedes(Rede rede)
        {
            ViewBag.Notification = "";
            if (!ModelState.IsValid)
                return View(rede);

            rede.Nome = rede.Nome.ToUpper();
            try
            {
                var resp = _restClient.AddRede(rede);

                //criar as pastas 
                var path = string.Format("~/ARQUIVOS/{0}", rede.Nome);

                var map = Server.MapPath(path);
                Directory.CreateDirectory(map);
                Directory.CreateDirectory(map+"/PROCESSADO");
                Directory.CreateDirectory(map +"/PROBLEMAS");
                Directory.CreateDirectory(map +"/SEM_REGISTRO");

            }
            catch (Exception ex)
            {
                AddErrors(ex);
                return View(rede);
            }
            if (rede.id_rede > 0)
            {
                this.AddNotification("Rede alterada.", NotificationType.Sucesso);
            }
            else
            {
                this.AddNotification("Rede incluída.", NotificationType.Sucesso);
            }


            var model = new Rede();
            return View(model);
        }

        // GET: Estabelecimentos
        public ActionResult ConsultarRedes()
        {
            var model = _restClient.GetRedesAll("");
            return View(model);
        }

        // GET: Estabelecimentos
        public ActionResult DeletarRedes(long id)
        {
            var Respostas = _restClient.DelRedePorId(id);
            this.AddNotification(Respostas.Mensagem, NotificationType.Sucesso);
            return RedirectToAction("ConsultarRedes");
        }

        [HttpGet]
        [OutputCache(Duration = 30)]
        public JsonResult GetRedes(string term)
        {
            List<Lista> list = _restClient.GetRedesAll(term ?? "").Select(i => new Lista { id = i.id_rede, text = i.Nome.ToUpper() }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private void AddErrors(Exception exc)
        {
            ModelState.AddModelError("", exc);
        }
    }
}