using System.Net;
using RestSharp;
using ConciliacaoModelo.model.generico;
using System.Collections.Generic;
using System;
using ConciliacaoModelo.model.conciliador;

namespace Conciliacao.Helper.Rest
{
    public class DashboardRestClient
    {
        private readonly RestClient _client;
        private readonly string _url = System.Configuration.ConfigurationManager.AppSettings["webapibaseurl"];

        public DashboardRestClient()
        {
            _client = new RestClient(_url);
        }

        public TotaisDashboard GetTotalizadorDia(int dias)
        {
            var request = new RestRequest("api/dashboard/gettotalizadordia/{idconta}/{dias}", Method.GET);
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("dias", dias, ParameterType.UrlSegment);
            var a = _client.Execute<TotaisDashboard>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new TotaisDashboard();
            return a.Data;
        }

        public List<TotaisDashboardBanco> GetTotalizadorPorBanco(int dias)
        {
            var request = new RestRequest("api/dashboard/gettotalizadorbanco/{idconta}/{dias}", Method.GET);
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("dias", dias, ParameterType.UrlSegment);
            var a = _client.Execute<List<TotaisDashboardBanco>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<TotaisDashboardBanco>();
            return a.Data;
        }

        public List<TotaisDashboardProduto> GetTotalizadorPorProduto(int dias)
        {
            var request = new RestRequest("api/dashboard/gettotalizadorproduto/{idconta}/{dias}", Method.GET);
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("dias", dias, ParameterType.UrlSegment);
            var a = _client.Execute<List<TotaisDashboardProduto>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<TotaisDashboardProduto>();
            return a.Data;
        }

        public List<TotaisDashboardPorData> GetTotalizadorDia(DateTime dataInicial, DateTime dataFinal)
        {
            var request = new RestRequest("api/dashboard/gettotalizadordia/{idconta}/{dt_ini}/{dt_fim}", Method.GET);
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("dt_ini", dataInicial.Ticks, ParameterType.UrlSegment);
            request.AddParameter("dt_fim", dataFinal.Ticks, ParameterType.UrlSegment);
            var a = _client.Execute<List<TotaisDashboardPorData>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<TotaisDashboardPorData>();
            return a.Data;
        }

        public List<TotaisDashboardPorData> GetTotalizadorDiaDes(DateTime dataInicial, DateTime dataFinal)
        {
            var request = new RestRequest("api/dashboard/gettotalizadordiadesagendamentos/{idconta}/{dt_ini}/{dt_fim}", Method.GET);
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("dt_ini", dataInicial.Ticks, ParameterType.UrlSegment);
            request.AddParameter("dt_fim", dataFinal.Ticks, ParameterType.UrlSegment);
            var a = _client.Execute<List<TotaisDashboardPorData>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<TotaisDashboardPorData>();
            return a.Data;
        }

    }
}