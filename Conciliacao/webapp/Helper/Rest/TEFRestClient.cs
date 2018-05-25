using ConciliacaoModelo.model.conciliador;
using ConciliacaoModelo.model.generico;
using ConciliacaoModelo.model.relatorio;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace Conciliacao.Helper.Rest
{
    public class TEFRestClient
    {
        private readonly RestClient _client;
        private readonly string _url = System.Configuration.ConfigurationManager.AppSettings["webapibaseurl"];

        public TEFRestClient()
        {
            _client = new RestClient(_url);
        }

        public List<TransacaoTEFTpTransacao> TransacaoTipoTransacoes()
        {
            var request = new RestRequest("api/tef/GetTipoTransacoes/{idconta}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.Timeout = 600000;
            var a = _client.Execute<List<TransacaoTEFTpTransacao>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<TransacaoTEFTpTransacao>();
            return a.Data;
        }

        public List<TransacaoTEFLojas> TransacaoTEFLojas()
        {
            var request = new RestRequest("api/tef/GetLojas/{idconta}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.Timeout = 600000;
            var a = _client.Execute<List<TransacaoTEFLojas>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<TransacaoTEFLojas>();
            return a.Data;
        }

        public List<TransacaoTEFListar> TransacaoTEFListar(string filtro_rede, int? filtro_situacao, int? filtro_tp_data, DateTime datainicio, DateTime datafinal, decimal valor, string nm_estabelecimento, string nm_administrador, string resumo = "", string nm_loja = "", string tp_transacao = "", string tp_operacao = "")
        {
            var request = new RestRequest("api/tef/TransacaoTEFListar/{idconta}/{rede}/{tpsituacao}/{tpdata}/{datainicio}/{datafinal}/{valor}/{estabelecimento}/{administrador}/{resumo}/{loja}/{tp_transacao}/{tp_operacao}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("rede", (filtro_rede ?? "000").PadLeft(10,' '), ParameterType.UrlSegment);
            request.AddParameter("tpsituacao", filtro_situacao ?? 3, ParameterType.UrlSegment);
            request.AddParameter("tpdata", filtro_tp_data ?? 3, ParameterType.UrlSegment);  // 1 - Emissão / 2 - Vencimento / 3 - Pagamento
            request.AddParameter("datainicio", datainicio.Ticks, ParameterType.UrlSegment);
            request.AddParameter("datafinal", datafinal.Ticks, ParameterType.UrlSegment);
            request.AddParameter("valor", valor, ParameterType.UrlSegment);
            request.AddParameter("estabelecimento", nm_estabelecimento ?? "-111", ParameterType.UrlSegment);
            request.AddParameter("administrador", nm_administrador ?? "-111", ParameterType.UrlSegment);
            request.AddParameter("resumo", resumo ?? "0", ParameterType.UrlSegment);
            request.AddParameter("loja", nm_loja ?? "0", ParameterType.UrlSegment);
            request.AddParameter("tp_transacao", tp_transacao ?? "0", ParameterType.UrlSegment);
            request.AddParameter("tp_operacao", tp_operacao ?? "0", ParameterType.UrlSegment);

            request.Timeout = 600000;
            var a = _client.Execute<List<TransacaoTEFListar>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<TransacaoTEFListar>();
            return a.Data;
        }

        public List<TransacaoRedeVsExtratoBancarioListar> TransacaoRedeVsExtratoBancarioListar(int? filtro_rede, int? filtro_situacao, int? filtro_tp_data, DateTime datainicio, DateTime datafinal)
        {
            var request = new RestRequest("api/relatorios/TransacaoRedeVsExtratoBancarioListar/{idconta}/{tpsituacao}/{tpdata}/{datainicio}/{datafinal}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
           // request.AddParameter("idrede", filtro_rede ?? 0, ParameterType.UrlSegment);
            request.AddParameter("tpsituacao", filtro_situacao ?? 3, ParameterType.UrlSegment);
            request.AddParameter("tpdata", filtro_tp_data ?? 3, ParameterType.UrlSegment);  // 1 - Emissão / 2 - Vencimento / 3 - Pagamento
            request.AddParameter("datainicio", datainicio.Ticks, ParameterType.UrlSegment);
            request.AddParameter("datafinal", datafinal.Ticks, ParameterType.UrlSegment);
            var a = _client.Execute<List<TransacaoRedeVsExtratoBancarioListar>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<TransacaoRedeVsExtratoBancarioListar>();
            return a.Data;
        }

        public List<ConciliacaoTransacaoRede> TransacaoRedeListar(int? filtro_rede, int? filtro_tp_cartao, DateTime datainicio, DateTime datafinal)
        {
            var request = new RestRequest("api/relatorios/TransacaoRedeListar/{idconta}/{tpcartao}/{datainicio}/{datafinal}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
           // request.AddParameter("idrede",  0, ParameterType.UrlSegment);
           // request.AddParameter("tpsituacao", filtro_situacao ?? 3, ParameterType.UrlSegment);
            request.AddParameter("tpcartao", filtro_tp_cartao ?? 3, ParameterType.UrlSegment);  // 0 - credito 1 -  debit0
            request.AddParameter("datainicio", datainicio.Ticks, ParameterType.UrlSegment);
            request.AddParameter("datafinal", datafinal.Ticks, ParameterType.UrlSegment);
            var a = _client.Execute<List<ConciliacaoTransacaoRede>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<ConciliacaoTransacaoRede>();
            return a.Data;
        }


    }
}