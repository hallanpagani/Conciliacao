using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Conciliacao.App_Helpers.Componentes;
using Conciliacao.Controllers.Generico;
using Conciliacao.Helper.Rest;
using ConciliacaoModelo.model.adm;
using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.banco;
using System.Linq;

namespace Conciliacao.Controllers.Cadastros
{
    [Authorize]
    public class ContaController : AppController
    {
        //static readonly UsuariosRestClient _restClient = new UsuariosRestClient();
        static readonly ContasRestClient _restContaClient = new ContasRestClient();

        public virtual ActionResult ConsultarConta()
        {
            if (!UsuarioLogado.Perfil.Equals("Administrador"))
            {
                return View("_SemPermissao");
            }
            var model = new Conta();
            var lista = DAL.ListarObjetos<Conta>("","ds_login");
            return View("~/views/conta/listar.cshtml", lista);
        }

        [Authorize]
        [HttpGet]
        public virtual ActionResult CadastrarConta(int id = 0)
        {
            if (!UsuarioLogado.Perfil.Equals("Administrador"))
            {
                return View("_SemPermissao");
            }
            var model = new Conta();
            if (id > 0)
            {
                model = DAL.GetObjetoById<Conta>(id);
            }
            return View("~/views/conta/incluir.cshtml", model);
        }

        [HttpPost]
        public ActionResult CadastrarConta(Conta viewModel)
        {
            ViewBag.Notification = "";
            if (!ModelState.IsValid)
                return View("~/views/conta/incluir.cshtml", viewModel);

            viewModel.DsLogin = viewModel.DsLogin.ToUpper();

            if (viewModel.Id == 0) {

                Conta u = new Conta();
                var f = new Filtros(u);
                f.Add(() => u.DsLogin, viewModel.DsLogin, FiltroExpressao.Igual);
                u = DAL.GetObjeto<Conta>(f);
                if (u != null)
                {
                    this.AddNotification("Informação! Conta já cadastrado.", NotificationType.Alerta);
                    return View("~/views/conta/incluir.cshtml", viewModel);
                }
            }

            try
            {
                DAL.Gravar(viewModel);
            }
            catch (Exception ex)
            {
                AddErrors(ex);
                return View("~/views/conta/incluir.cshtml", viewModel);
            }
            if (viewModel.Id > 0)
            {
                this.AddNotification("Conta alterado.", NotificationType.Sucesso);
            }
            else
            {
                this.AddNotification("Conta incluído.", NotificationType.Sucesso);
            }
            

            var model = new Conta();
            return View("~/views/conta/incluir.cshtml", model);
        }

        private void AddErrors(Exception exc)
        {
            ModelState.AddModelError("", exc);
        }

        [HttpGet]
        [OutputCache(Duration = 30)]
        public JsonResult GetContas(string term)
        {
            List<Lista> list = _restContaClient.GetContasAll(term ?? "").Select(i => new Lista { id = i.Id, text = i.DsLogin.ToUpper() }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }


    }
}