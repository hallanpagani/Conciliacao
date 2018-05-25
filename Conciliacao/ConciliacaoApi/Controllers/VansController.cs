using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.banco;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ConciliacaoAPI.Controllers
{
    public class VansController : ApiController
    {
        // GET: Cadastro
        [HttpPost]
        [Route("api/vans/van")]
        public Respostas VAN([FromBody] Vans model)
        {
            Respostas response;
            try
            {
                var idBase = DAL.Gravar(model);
                response = new Respostas(true, "VAN incluída!", idBase);
            }
            catch (Exception ex)
            {
                response = new Respostas(true, ex.Message, 0);
            }
            return response;
        }

        [Route("api/vans/DelVanPorId/{idconta:long}/{id:int}")]
        public Respostas DelVanPorId(long idconta, int id)
        {
            Respostas response;
            try
            {
                DAL.Excluir(DAL.GetObjetoById<Vans>(id));
                response = new Respostas(true, "VANs deletada!", 0);
            }
            catch (Exception ex)
            {
                response = new Respostas(true, ex.Message, 0);
            }
            return response;
        }

        [Route("api/vans/GetVansPorId/{idconta:long}/{id:int}")]
        public Vans GetVansPorId(long idconta, int id)
        {
            return DAL.GetObjetoById<Vans>(id);
        }

        [Route("api/vans/GetVansAll/{idconta:long}")]
        public IEnumerable<VansListar> GetVansAll(long idconta)
        {
            return GetVansAll(idconta, "");
        }

        [Route("api/vans/GetVansAll/{idconta:long}/{termo}")]
        public IEnumerable<VansListar> GetVansAll(long idconta, string termo)
        {
            var u = new Vans();
            var f = new Filtros(u);
            string filtro = "";
            if (!string.IsNullOrEmpty(termo))
            {

                f.AddLike(() => u.identificacao_van, termo, "");
                filtro = " and " + f;
            }
            return DAL.ListarObjetos<VansListar>(string.Format("id_conta={0} {1}", idconta, filtro), "ds_van");
        }

    }
}