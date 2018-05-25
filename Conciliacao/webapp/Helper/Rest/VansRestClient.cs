using System.Collections.Generic;
using System.Net;
using Conciliacao.Helper.Interfaces;
using System;
using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.generico;
using RestSharp;

namespace Conciliacao.Helper.Rest
{
    public class VansRestClient 
    {
        private readonly RestClient _client;
        private readonly string _url = System.Configuration.ConfigurationManager.AppSettings["webapibaseurl"];

        public VansRestClient()
        {
            _client = new RestClient(_url);
        }

        public Respostas AddVans(Vans van)
        {
            var request = new RestRequest("api/vans/van", Method.POST);
            request.AddJsonBody(van);
            var a = _client.Execute<Respostas>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new Respostas();
            return a.Data;
        }

        public IEnumerable<VansListar> GetVansAll(string term)
        {
            var request = new RestRequest("api/Vans/GetVansAll/{idconta}/{termo}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("termo", term, ParameterType.UrlSegment);
            var a = _client.Execute<List<VansListar>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<VansListar>();
            return a.Data;
        }

        public Vans GetVansPorId(long id)
        {
            var request = new RestRequest("api/Vans/GetVansPorId/{idconta}/{id}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var a = _client.Execute<Vans>(request);
            if (a.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception(a.Content);
            if (a.StatusCode == HttpStatusCode.NotFound)
                throw new Exception(a.Content);

            return a.Data;
        }
    }
}