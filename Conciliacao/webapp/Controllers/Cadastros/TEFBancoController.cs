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
    public class BancoTEFController : AppController
    {
        static readonly BancoTEFRestClient _restClient = new BancoTEFRestClient();

        public virtual ActionResult ConsultarBancoTEF()
        {
            var lista = _restClient.GetBancoTEFAll("");
            return View(lista);
        }

        [HttpGet]
        public virtual ActionResult CadastrarBancoTEF(int id = 0)
        {
            var lista = _restClient.GetBancoTEFAll("");
            var model = new BancoTEF();
            if (id > 0)
            {
                model = DAL.GetObjetoById<BancoTEF>(id);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult CadastrarBancoTEF(BancoTEF van)
        {
            ViewBag.Notification = "";
            if (!ModelState.IsValid)
                return View(van);

           /* if (viewModel.Id == 0)
            {

                BancoTEF u = new BancoTEF();
                Filtros f = new Filtros(u);
                f.Add(() => u.Email, viewModel.Email, FiltroExpressao.Igual);
                u = DAL.GetObjeto<BancoTEF>(f);
                if (u != null)
                {
                    this.AddNotification("Informação! Usuário já cadastrado.", NotificationType.Alerta);
                    return View("~/views/usuario/incluir.cshtml", viewModel);
                }
            }*/

            try
            {
                var resp = _restClient.AddBancoTEF(van);
            }
            catch (Exception ex)
            {
                AddErrors(ex);
                return View(van);
            }
            if (van.id_tef > 0)
            {
                this.AddNotification("Banco do TEF alterado.", NotificationType.Sucesso);
            }
            else
            {
                this.AddNotification("Banco do TEF incluído.", NotificationType.Sucesso);
            }


            var model = new BancoTEF();
            return View(model);
        }

        [HttpGet]
        [OutputCache(Duration = 30)]
        public JsonResult GetBancoTEF(string term)
        {
            List<Lista> list = _restClient.GetBancoTEFAll(term ?? "").Select(i => new Lista { id = i.id_tef, text = i.identificacao_tef.ToUpper() }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private void AddErrors(Exception exc)
        {
            ModelState.AddModelError("", exc);
        }

    }
}