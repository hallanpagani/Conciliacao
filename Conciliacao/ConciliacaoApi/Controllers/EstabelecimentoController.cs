using System.Collections.Generic;
using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.banco;
using System;
using System.Web.Http;
using ConciliacaoPersistencia.model;

namespace ConciliacaoAPI.Controllers
{
    public class EstabelecimentoController : ApiController
    {
        // GET: Cadastro
        [HttpPost]
        [Route("api/Estabelecimento/Estabelecimento")]
        public Respostas Estabelecimento([FromBody] Estabelecimento model)
        {
            Respostas response;
            try
            {
                var idBase = DAL.Gravar(model);
                response = new Respostas(true, "Estabelecimento incluído!", idBase);
            }
            catch (Exception ex)
            {
                response = new Respostas(true, ex.Message, 0);
            }
            return response;
        }

        [HttpPost]
        [Route("api/Estabelecimento/Estabelecimentorede")]
        public Respostas EstabelecimentoRede([FromBody] EstabelecimentoRede model)
        {
            Respostas response;
            try
            {
                var idBase = DAL.Gravar(model);
                response = new Respostas(true, "Estabelecimento rede incluído!", idBase);
            }
            catch (Exception ex)
            {
                response = new Respostas(true, ex.Message, 0);
            }
            return response;
        }

        [Route("api/Estabelecimento/DelEstabelecimentoPorId/{idconta:long}/{id:int}")]
        public Respostas DelEstabelecimentoPorId(long idconta, int id)
        {
            Respostas response;
            try
            {
                DAL.Excluir(DAL.GetObjetoById<Estabelecimento>(id));
                response = new Respostas(true, "Estabelecimento deletado!", 0);
            }
            catch (Exception ex)
            {
                response = new Respostas(true, ex.Message, 0);
            }
            return response;
        }

        [Route("api/Estabelecimento/GetEstabelecimentoPorId/{idconta:long}/{id:int}")]
        public Estabelecimento GetEstabelecimentoPorId(long idconta, int id)
        {
            return DAL.GetObjetoById<Estabelecimento>(id);
        }

        [Route("api/Estabelecimento/GetEstabelecimentoAll/{idconta:long}")]
        public IEnumerable<EstabelecimentoListar> GetEstabelecimentoAll(long idconta)
        {
            return GetEstabelecimentoAll(idconta, "");
        }

        [Route("api/Estabelecimento/GetEstabelecimentoAll/{idconta:long}/{termo}")]
        public IEnumerable<EstabelecimentoListar> GetEstabelecimentoAll(long idconta, string termo)
        {
            var u = new EstabelecimentoListar();
            var f = new Filtros(u);
            string filtro = "";
            if (!string.IsNullOrEmpty(termo))
            {
                f.AddLike(() => u.nome, termo, "");
                filtro = " and " +f;
            }
            return DAL.ListarObjetos<EstabelecimentoListar>(string.Format("id_conta={0} {1}", idconta, filtro), "nm_estabelecimento");
        }


        [Route("api/Estabelecimento/GetEstabelecimentoRedePorId/{idconta:long}/{id:int}")]
        public List<EstabelecimentoRedeListar> GetEstabelecimentoRedePorId(long idconta, int id)
        {
            return DAL.ListarObjetos<EstabelecimentoRedeListar>(string.Format("id_estabelecimento ={0} and id_conta={1}",id,idconta) );
        }

    }
}