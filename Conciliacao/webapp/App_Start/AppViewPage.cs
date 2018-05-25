using System.Security.Claims;
using System.Web.Mvc;

namespace Conciliacao
{
    public abstract class AppViewPage<TModel> : WebViewPage<TModel>
    {
        protected AppUsuario UsuarioLogado
        {
            get
            {
                return new AppUsuario(this.User as ClaimsPrincipal);
            }
        }
    }

    public abstract class AppViewPage : AppViewPage<dynamic>
    {
    }
}