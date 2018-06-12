using ConciliacaoModelo.model.generico;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Conciliacao.Helper.Rest
{
    public class BancosRestClient 
    {
        private readonly RestClient _client;
        private readonly string _url = System.Configuration.ConfigurationManager.AppSettings["webapibaseurl"];

        public BancosRestClient()
        {
            _client = new RestClient(_url);
        }

        public IEnumerable<Lista> GetBancosAll()
        {
            var request = new RestRequest("api/Bancos/GetAll/{idconta}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
//            request.AddParameter("termo", term ?? "", ParameterType.UrlSegment);
            var a = _client.Execute<List<Lista>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<Lista>();
            return a.Data;
        }

        public IEnumerable<Lista> GetContasAll()
        {
            var request = new RestRequest("api/Bancos/GetContasAll/{idconta}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            //            request.AddParameter("termo", term ?? "", ParameterType.UrlSegment);
            var a = _client.Execute<List<Lista>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<Lista>();
            return a.Data;
        }

        public IEnumerable<Lista> GetAgenciasAll()
        {
            var request = new RestRequest("api/Bancos/GetAgenciasAll/{idconta}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            //            request.AddParameter("termo", term ?? "", ParameterType.UrlSegment);
            var a = _client.Execute<List<Lista>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<Lista>();
            return a.Data;
        }

    }
}