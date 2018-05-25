using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.generico;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace Conciliacao.Helper.Rest
{
    public class RedeRestClient
    {
        private readonly RestClient _client;
        private readonly string _url = System.Configuration.ConfigurationManager.AppSettings["webapibaseurl"];

        public RedeRestClient()
        {
            _client = new RestClient(_url);
        }

        public Respostas AddRede(Rede rede)
        {
            var request = new RestRequest("api/rede/rede", Method.POST);
            request.AddJsonBody(rede);
            var a = _client.Execute<Respostas>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new Respostas();
            return a.Data;
        }

        public IEnumerable<RedeListar> GetRedesAll(string term)
        {
            var request = new RestRequest("api/rede/GetRedesAll/{idconta}/{termo}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("termo", term, ParameterType.UrlSegment);
            var a = _client.Execute<List<RedeListar>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<RedeListar>();
            return a.Data;
        }

        public Rede GetRedesPorId(long id)
        {
            var request = new RestRequest("api/rede/GetRedesPorId/{idconta}/{id}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var a = _client.Execute<Rede>(request);
            if (a.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception(a.Content);
            if (a.StatusCode == HttpStatusCode.NotFound)
                throw new Exception(a.Content);

            return a.Data;
        }

        public Respostas DelRedePorId(long id)
        {
            var request = new RestRequest("api/rede/DelRedePorId/{idconta}/{idrede}", Method.POST);
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("idrede", id, ParameterType.UrlSegment);
            var response = _client.Execute<Respostas>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }

    }
}