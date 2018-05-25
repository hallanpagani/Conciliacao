using ConciliacaoModelo.model.adm;
using ConciliacaoPersistencia.banco;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ConciliacaoAPI.Controllers
{
    public class UsuarioController : ApiController
    {
        [HttpPost]
        public Usuario GetByEmailSenha(string email,string senha)
        {
            return DAL.GetObjeto<Usuario>(String.Format(@"ds_login='{0}' and ds_senha='{1}'",email,senha)   );
        }

        public List<Usuario> Get()
        {
            return DAL.ListarObjetos<Usuario>();
        }



    }
}
