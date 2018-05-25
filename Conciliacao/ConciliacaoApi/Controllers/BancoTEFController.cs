using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.banco;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ConciliacaoAPI.Controllers
{
    public class BancoTEFController : ApiController
    {
        // GET: Cadastro
        [HttpPost]
        [Route("api/BancoTEF/BancoTEF")]
        public Respostas BancoTEF([FromBody] BancoTEF model)
        {
            Respostas response;
            try
            {
                var idBase = DAL.Gravar(model);
                response = new Respostas(true, "Banco TEF incluído!", idBase);
            }
            catch (Exception ex)
            {
                response = new Respostas(true, ex.Message, 0);
            }
            return response;
        }

        [Route("api/BancoTEF/DelBancoTEFPorId/{idconta:long}/{id:int}")]
        public Respostas DelBancoTEFPorId(long idconta, int id)
        {
            Respostas response;
            try
            {
                DAL.Excluir(DAL.GetObjetoById<BancoTEF>(id));
                response = new Respostas(true, "Banco TEF deletada!", 0);
            }
            catch (Exception ex)
            {
                response = new Respostas(true, ex.Message, 0);
            }
            return response;
        }

        [Route("api/BancoTEF/GetBancoTEFPorId/{idconta:long}/{id:int}")]
        public BancoTEF GetBancoTEFPorId(long idconta, int id)
        {
            return DAL.GetObjetoById<BancoTEF>(id);
        }

        [Route("api/BancoTEF/GetBancoTEFAll/{idconta:long}")]
        public IEnumerable<BancoTEFListar> GetBancoTEFAll(long idconta)
        {
            return GetBancoTEFAll(idconta, "");
        }

        [Route("api/BancoTEF/GetBancoTEFAll/{idconta:long}/{termo}")]
        public IEnumerable<BancoTEFListar> GetBancoTEFAll(long idconta, string termo)
        {
            var u = new BancoTEF();
            var f = new Filtros(u);
            string filtro = "";
            if (!string.IsNullOrEmpty(termo))
            {

                f.AddLike(() => u.identificacao_tef, termo, "");
                filtro = " and " + f;
            }
            return DAL.ListarObjetos<BancoTEFListar>(string.Format("id_conta={0} {1}", idconta, filtro), "ds_tef");
        }

    }
}