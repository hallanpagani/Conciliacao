using System.Collections.Generic;
using System.Net;
using Conciliacao.Helper.Interfaces;
using System;
using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.generico;
using RestSharp;

namespace Conciliacao.Helper.Rest
{
    public class BancoTEFRestClient 
    {
        private readonly RestClient _client;
        private readonly string _url = System.Configuration.ConfigurationManager.AppSettings["webapibaseurl"];

        public BancoTEFRestClient()
        {
            _client = new RestClient(_url);
        }

        public Respostas AddBancoTEF(BancoTEF van)
        {
            var request = new RestRequest("api/BancoTEF/BancoTEF", Method.POST);
            request.AddJsonBody(van);
            var a = _client.Execute<Respostas>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new Respostas();
            return a.Data;
        }

        public IEnumerable<BancoTEFListar> GetBancoTEFAll(string term)
        {
            var request = new RestRequest("api/BancoTEF/GetBancoTEFAll/{idconta}/{termo}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("termo", term, ParameterType.UrlSegment);
            var a = _client.Execute<List<BancoTEFListar>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<BancoTEFListar>();
            return a.Data;
        }

        public BancoTEF GetBancoTEFPorId(long id)
        {
            var request = new RestRequest("api/BancoTEF/GetBancoTEFPorId/{idconta}/{id}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var a = _client.Execute<BancoTEF>(request);
            if (a.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception(a.Content);
            if (a.StatusCode == HttpStatusCode.NotFound)
                throw new Exception(a.Content);

            return a.Data;
        }
    }
}