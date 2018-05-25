using System;
using System.Security.Claims;
using System.Web;

namespace Conciliacao
{
    public class AppUsuario : ClaimsPrincipal
    {
        private string cod;
        private string chave;

        public AppUsuario(ClaimsPrincipal principal)
            : base(principal)
        {
        }

        public string Nome
        {
            get
            {
                return this.FindFirst(ClaimTypes.Name).Value ?? "Relogue-se";
            }
        }

        public string Perfil
        {
            get
            {
                return this.FindFirst(ClaimTypes.Role).Value ?? "Administrador";
            }
        }

        public string SUser
        {
            get
            {
                
                try
                {
                    cod = this.FindFirst(ClaimTypes.Sid).Value ?? "0";
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Response.Redirect("~/sistema/login");
                }
                return cod;
            }

        }

        public string ContaChave
        {
            get
            {

                try
                {
                    chave = this.FindFirst(ClaimTypes.Authentication).Value ?? "0";
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Response.Redirect("~/sistema/login");
                }
                return chave;
            }

        }

        public string IdConta
        {
            get
            {
                try
                {
                    chave = this.FindFirst(ClaimTypes.GroupSid).Value ?? "0";
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Response.Redirect("~/sistema/login");
                }
                return chave;
            }
           
        }

    }
}