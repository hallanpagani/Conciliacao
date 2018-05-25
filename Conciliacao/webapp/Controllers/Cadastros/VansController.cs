using Conciliacao.App_Helpers.Componentes;
using Conciliacao.Controllers.Generico;
using Conciliacao.Helper.Rest;
using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.banco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Conciliacao.Controllers.Cadastros
{
    [Authorize]
    public class VansController : AppController
    {
        static readonly VansRestClient _restClient = new VansRestClient();

        public virtual ActionResult ConsultarVans()
        {
            var lista = _restClient.GetVansAll("");
            return View(lista);
        }

        [HttpGet]
        public virtual ActionResult CadastrarVans(int id = 0)
        {
            var lista = _restClient.GetVansAll("");
            var model = new Vans();
            if (id > 0)
            {
                model = DAL.GetObjetoById<Vans>(id);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult CadastrarVans(Vans van)
        {
            ViewBag.Notification = "";
            if (!ModelState.IsValid)
                return View(van);

           /* if (viewModel.Id == 0)
            {

                Vans u = new Vans();
                Filtros f = new Filtros(u);
                f.Add(() => u.Email, viewModel.Email, FiltroExpressao.Igual);
                u = DAL.GetObjeto<Vans>(f);
                if (u != null)
                {
                    this.AddNotification("Informação! Usuário já cadastrado.", NotificationType.Alerta);
                    return View("~/views/usuario/incluir.cshtml", viewModel);
                }
            }*/

            try
            {
                var resp = _restClient.AddVans(van);
            }
            catch (Exception ex)
            {
                AddErrors(ex);
                return View(van);
            }
            if (van.id_van > 0)
            {
                this.AddNotification("VAN alterada.", NotificationType.Sucesso);
            }
            else
            {
                this.AddNotification("VAN incluída.", NotificationType.Sucesso);
            }


            var model = new Vans();
            return View(model);
        }

        [HttpGet]
        [OutputCache(Duration = 30)]
        public JsonResult GetVans(string term)
        {
            List<Lista> list = _restClient.GetVansAll(term ?? "").Select(i => new Lista { id = i.id_van, text = i.identificacao_van.ToUpper() }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private void AddErrors(Exception exc)
        {
            ModelState.AddModelError("", exc);
        }

    }
}