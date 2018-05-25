using Microsoft.AspNet.Identity.EntityFramework;

namespace Conciliacao.Models
{
    public class UsuarioApp: IdentityUser
    {
        public long IdConta;
        public string Perfil;
        public bool SPRUsuario;
        public string ChaveConta;
    }
}