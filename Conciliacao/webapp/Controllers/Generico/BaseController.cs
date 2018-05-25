using System.Web.Mvc;
using Conciliacao.App_Helpers;
using System.Security.Claims;

namespace Conciliacao.Controllers.Generico
{

    public abstract class BaseController<TModel> : Controller where TModel : new() 
    {
        public AppUsuario UsuarioLogado
        {
            get
            {
                return new AppUsuario(this.User as ClaimsPrincipal);
            }
        }

        /*

        public virtual void Validar(TModel model)
        {
            ///
        }

        public virtual void GetViewBags()
        {
            /// algum codigo aqui
        }

        public virtual void AntesDeGravar(TModel model)
        {
            /// algum codigo aqui
        }

        public virtual void AposGravar(long idNovo, TModel model)
        {
            /// algum codigo aqui
        }

        // GET: Crud/Details/5
        public virtual ActionResult Editar(int id)
        {
            /// algum codigo aqui
            return View();
        }

        public virtual ActionResult Incluir()
        {
            GetViewBags();
            var model = new TModel();
            return View(model);
        } 

        [HttpGet]
        public virtual ActionResult Redireciona(TModel model)
        {
            return View(model);
        }*/

    }
}
