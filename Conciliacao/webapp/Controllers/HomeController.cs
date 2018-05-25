#region Using

using System;
using System.Web;
using Conciliacao.Controllers.Generico;
using Conciliacao.Helper.Interfaces;
using Conciliacao.Helper.Rest;
using Conciliacao.Models;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

#endregion

namespace Conciliacao.Controllers
{
    [Authorize]
    public class HomeController : AppController
    {
        // GET: home/index
        public ActionResult Index()
        {
            try
            {
                if ((UsuarioLogado.Perfil ?? "").Equals(""))
                {
                    return RedirectToAction("Login", "Sistema");
                }
                if ((UsuarioLogado.Perfil ?? "").Equals("Administrador"))
                {
                    return RedirectToAction("RelatorioTransacoes", "Financeiro");
                }
                if ((UsuarioLogado.Perfil ?? "").Equals("Parcial"))
                {
                    return RedirectToAction("RelatorioTransacoes", "Financeiro");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Sistema");
            }
           
            return RedirectToAction("RelatorioTransacoes", "Financeiro");
        }

        public ActionResult Widgets()
        {
            return View();
        }

    }
}