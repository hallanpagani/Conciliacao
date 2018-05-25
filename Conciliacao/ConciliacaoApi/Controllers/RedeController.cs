using System.Runtime.Remoting.Channels;
using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.banco;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ConciliacaoAPI.Controllers
{
    public class RedeController : ApiController
    {
        // GET: Cadastro
        [HttpPost]
        [Route("api/rede/Rede")]
        public Respostas Rede([FromBody] Rede model)
        {
            Respostas response;
            try
            {
                var idBase = DAL.Gravar(model);
                response = new Respostas(true, "Rede incluída!", idBase);
            }
            catch (Exception ex)
            {
                response = new Respostas(true, ex.Message, 0);
            }
            return response;
        }

        [Route("api/rede/DelRedePorId/{idconta:long}/{id:int}")]
        public Respostas DelRedePorId(long idconta, int id)
        {
            Respostas response;
            try
            {
                DAL.Excluir(DAL.GetObjetoById<Rede>(id));
                response = new Respostas(true, "Rede deletada!", 0);
            }
            catch (Exception ex)
            {
                response = new Respostas(true, ex.Message, 0);
            }
            return response;
        }

        [Route("api/rede/GetRedesPorId/{idconta:long}/{id:int}")]
        public Rede GetRedesPorId(long idconta, int id)
        {
            return DAL.GetObjetoById<Rede>(id);
        }

        [Route("api/rede/GetRedesAll/{idconta:long}")]
        public IEnumerable<RedeListar> GetRedesAll(long idconta)
        {
            return GetRedesAll(idconta, "");
        }

        [Route("api/rede/GetRedesAll/{idconta:long}/{termo}")]
        public IEnumerable<RedeListar> GetRedesAll(long idconta, string termo)
        {
            var u = new Rede();
            var f = new Filtros(u);
            string filtro = "";
            if (!string.IsNullOrEmpty(termo))
            {

                f.AddLike(() => u.Nome, termo, "");
                filtro = " and " + f;
            }

            return DAL.ListarObjetos<RedeListar>(string.Format("{0}", termo), "id_rede");
            //return DAL.ListarObjetos<RedeListar>(string.Format("id_conta={0} {1}", idconta, termo), "ds_Rede");
        }

    }
}