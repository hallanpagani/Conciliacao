using System;
using System.Web.Mvc;
using Conciliacao.App_Helpers.Componentes;
using Conciliacao.Controllers.Generico;
using ConciliacaoModelo.model.adm;
using ConciliacaoPersistencia.banco;
using ConciliacaoModelo.model.generico;

namespace Conciliacao.Controllers.Cadastros
{
    public class UsuarioController : AppController
    {
        [Authorize]
        public virtual ActionResult ConsultarUsuario()
        {
            if (!UsuarioLogado.Perfil.Equals("Administrador"))
            {
                return View("_SemPermissao");
            }
            var model = new BaseID();
            var lista = DAL.ListarObjetos<Usuario>(String.Format("id_conta={0}",model.IdConta),"ds_login");
            return View("~/views/usuario/listar.cshtml", lista);
        }

        [Authorize]
        [HttpGet]
        public virtual ActionResult CadastrarUsuario(int id = 0)
        {
            if (!UsuarioLogado.Perfil.Equals("Administrador"))
            {
                return View("_SemPermissao");
            }
            var model = new Usuario();
            if (id > 0)
            {
                model = DAL.GetObjetoById<Usuario>(id);
            }
            return View("~/views/usuario/incluir.cshtml", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult CadastrarUsuario(Usuario viewModel)
        {
            ViewBag.Notification = "";
            if (!ModelState.IsValid)
                return View("~/views/usuario/incluir.cshtml", viewModel);

            if (viewModel.Id == 0) {

                Usuario u = new Usuario();
                Filtros f = new Filtros(u);
                f.Add(() => u.Email, viewModel.Email, FiltroExpressao.Igual);
                u = DAL.GetObjeto<Usuario>(f);
                if (u != null)
                {
                    this.AddNotification("Informação! Usuário já cadastrado.", NotificationType.Alerta);
                    return View("~/views/usuario/incluir.cshtml", viewModel);
                }
            }

            try
            {
                DAL.Gravar(viewModel);
            }
            catch (Exception ex)
            {
                AddErrors(ex);
                return View("~/views/usuario/incluir.cshtml", viewModel);
            }
            if (viewModel.Id > 0)
            {
                this.AddNotification("Usuário alterado.", NotificationType.Sucesso);
            }
            else
            {
                this.AddNotification("Usuário incluído.", NotificationType.Sucesso);
            }
            

            var model = new Usuario();
            return View("~/views/usuario/incluir.cshtml", model);
        }

        private void AddErrors(Exception exc)
        {
            ModelState.AddModelError("", exc);
        }



    }
}