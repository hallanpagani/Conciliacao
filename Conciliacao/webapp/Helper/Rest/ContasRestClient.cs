using System;
using ConciliacaoModelo.model.adm;
using RestSharp;
using Conciliacao.Helper.Interfaces;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Net;

namespace Conciliacao.Helper.Rest
{
    public class ContasRestClient 
    {
        private readonly RestClient _client;
        private readonly string _url = System.Configuration.ConfigurationManager.AppSettings["webapibaseurl"];

        public ContasRestClient()
        {
            _client = new RestClient(_url);
        }

        public IEnumerable<Conta> GetContasAll(string term)
        {
            var request = new RestRequest("api/Conta/GetContasAll/{termo}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("termo", term, ParameterType.UrlSegment);
            var a = _client.Execute<List<Conta>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<Conta>();
            return a.Data;
        }
    }
}