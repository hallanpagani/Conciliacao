using ConciliacaoModelo.model.adm;
using ConciliacaoPersistencia.banco;
using System.Collections.Generic;
using System.Web.Http;

namespace ConciliacaoAPI.Controllers
{
    public class ContaController : ApiController
    {
        [Route("api/Conta/GetContasAll/{termo}")]
        public IEnumerable<Conta> GetContasAll(string termo = "")
        {
            var u = new Conta();
            var f = new Filtros(u);
            string filtro = "";
            if (!string.IsNullOrEmpty(termo))
            {

                f.AddLike(() => u.DsLogin, termo, "");
                filtro = " " + f;
            }
            return DAL.ListarObjetos<Conta>(string.Format("{0}", filtro), "ds_login");
        }

        [Route("api/Conta/GetContasAll")]
        public IEnumerable<Conta> GetContasAll()
        {
            return DAL.ListarObjetos<Conta>("", "ds_login");
        }

    }
}
