using Conciliacao.Helper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConciliacaoModelo.model.generico;
using RestSharp;
using System.Net;

namespace Conciliacao.Helper.Rest
{
    public class AgendaRestClient : IAgendaRestClient
    {
        private readonly RestClient _client;
        private readonly string _url = System.Configuration.ConfigurationManager.AppSettings["webapibaseurl"];

        public AgendaRestClient()
        {
            _client = new RestClient(_url);
        }

        public Respostas DelAgendaPorId(int id)
        {
            var request = new RestRequest("api/Agenda/DelAgendaPorId/{idconta}/{id}", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var a = _client.Execute<Respostas>(request);
            if (a.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception(a.Content);
            if (a.StatusCode == HttpStatusCode.NotFound)
                throw new Exception(a.Content);

            return a.Data;
        }

        public decimal GetSessaoAtualPorIdCliente(int id)
        {
            var request = new RestRequest("api/agenda/GetSessaoAtualPorIdCliente/{idconta}/{idcliente}", Method.GET);
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("idcliente", id, ParameterType.UrlSegment);
            var a = _client.Execute<Decimal>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new Decimal();
            return a.Data;
        }

        public decimal GetSessaoFaltantePorIdCliente(int id)
        {
            var request = new RestRequest("api/agenda/GetSessaoFaltantePorIdCliente/{idconta}/{idcliente}", Method.GET);
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("idcliente", id, ParameterType.UrlSegment);
            var a = _client.Execute<Decimal>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new Decimal();
            return a.Data;
        }

        public Respostas DelAgendaClientePorId(long idcliente)
        {
            var request = new RestRequest("api/Agenda/DelAgendaClientePorId/{idconta}/{idcliente}", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("idcliente", idcliente, ParameterType.UrlSegment);
            var a = _client.Execute<Respostas>(request);
            if (a.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception(a.Content);
            if (a.StatusCode == HttpStatusCode.NotFound)
                throw new Exception(a.Content);

            return a.Data;
        }

        public Respostas AgendaPacienteFinalizar(long idcliente, long idservico)
        {
            var request = new RestRequest("api/Agenda/AgendaPacienteFinalizar/{idconta}/{idcliente}/{idservico}", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("idcliente", idcliente, ParameterType.UrlSegment);
            request.AddParameter("idservico", idservico, ParameterType.UrlSegment);
            var a = _client.Execute<Respostas>(request);
            if (a.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception(a.Content);
            if (a.StatusCode == HttpStatusCode.NotFound)
                throw new Exception(a.Content);

            return a.Data;

        }
    }
}