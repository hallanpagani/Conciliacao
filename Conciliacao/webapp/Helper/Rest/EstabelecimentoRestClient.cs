using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.generico;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace Conciliacao.Helper.Rest
{
    public class EstabelecimentoRestClient
    {
        private readonly RestClient _client;
        private readonly string _url = System.Configuration.ConfigurationManager.AppSettings["webapibaseurl"];

        public EstabelecimentoRestClient()
        {
            _client = new RestClient(_url);
        }

        public Respostas AddEstabelecimento(Estabelecimento van)
        {
            var request = new RestRequest("api/Estabelecimento/Estabelecimento", Method.POST);
            request.AddJsonBody(van);
            var a = _client.Execute<Respostas>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new Respostas();
            return a.Data;
        }

        public Respostas AddEstabelecimentoRede(EstabelecimentoRede estab_rede)
        {
            var request = new RestRequest("api/Estabelecimento/EstabelecimentoRede", Method.POST);
            request.AddJsonBody(estab_rede);
            var a = _client.Execute<Respostas>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new Respostas();
            return a.Data;
        }

        public IEnumerable<EstabelecimentoListar> GetEstabelecimentosAll(string term)
        {
            var request = new RestRequest("api/Estabelecimento/GetEstabelecimentoAll/{idconta}/{termo}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("termo", term, ParameterType.UrlSegment);
            var a = _client.Execute<List<EstabelecimentoListar>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<EstabelecimentoListar>();
            return a.Data;
        }

        public Estabelecimento GetEstabelecimentoPorId(long id)
        {
            var request = new RestRequest("api/Estabelecimento/GetEstabelecimentoPorId/{idconta}/{id}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var a = _client.Execute<Estabelecimento>(request);
            if (a.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception(a.Content);
            if (a.StatusCode == HttpStatusCode.NotFound)
                throw new Exception(a.Content);

            return a.Data;
        }

        public List<EstabelecimentoRedeListar> GetEstabelecimentoRedePorId(long id)
        {
            var request = new RestRequest("api/Estabelecimento/GetEstabelecimentoRedePorId/{idconta}/{id}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var a = _client.Execute<List<EstabelecimentoRedeListar>>(request);
            if (a.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception(a.Content);
            if (a.StatusCode == HttpStatusCode.NotFound)
                throw new Exception(a.Content);

            return a.Data;
        }



        public IEnumerable<EstabelecimentoRedeListar> GetEstabelecimentosRedeAll(string term)
        {
            var request = new RestRequest("api/Estabelecimento/GetEstabelecimentosRedeAll/{idconta}/{termo}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("termo", term, ParameterType.UrlSegment);
            var a = _client.Execute<List<EstabelecimentoRedeListar>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<EstabelecimentoRedeListar>();
            return a.Data;
        }

    }
}