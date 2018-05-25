using System.Web.Mvc;
using Conciliacao.Controllers.Generico;

namespace Conciliacao.Controllers
{
    [Authorize]
    public class FinanceiroController : AppController
    {
        public ActionResult RelatorioDespesas()
        {
            if (!UsuarioLogado.Perfil.Equals("Administrador"))
            {
                return View("_SemPermissao");
            }
            return View("RelatorioReceitasDespesas");
        }

        public ActionResult RelatorioTransacoes()
        {
            if (!UsuarioLogado.Perfil.Equals("Administrador"))
            {
                return View("_SemPermissao");
            }
            return View("RelatorioReceitasDespesas");
        }

    }
}