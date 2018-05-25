using ConciliacaoModelo.model.conciliador;
using ConciliacaoModelo.model.conciliador.UseRede;
using ConciliacaoModelo.model.generico;
using ConciliacaoModelo.model.relatorio;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace Conciliacao.Helper.Rest
{
    public class RelatoriosRestClient
    {
        private readonly RestClient _client;
        private readonly string _url = System.Configuration.ConfigurationManager.AppSettings["webapibaseurl"];

        public RelatoriosRestClient()
        {
            _client = new RestClient(_url);
        }

        public List<TransacaoRedeVsTEFListar> TransacaoRedeVsTEFListar(int? filtro_rede, int? filtro_situacao, int? filtro_tp_data, DateTime datainicio, DateTime datafinal, string resumo)
        {
            var request = new RestRequest("api/relatorios/TransacaoRedeVsTEFListar/{idconta}/{idrede}/{tpsituacao}/{tpdata}/{datainicio}/{datafinal}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            request.AddParameter("idrede", filtro_rede ?? 0, ParameterType.UrlSegment);
            request.AddParameter("tpsituacao", filtro_situacao ?? 3, ParameterType.UrlSegment);
            request.AddParameter("tpdata", filtro_tp_data ?? 3, ParameterType.UrlSegment);  // 1 - Emissão / 2 - Vencimento / 3 - Pagamento
            request.AddParameter("datainicio", datainicio.Ticks, ParameterType.UrlSegment);
            request.AddParameter("datafinal", datafinal.Ticks, ParameterType.UrlSegment);
            request.AddParameter("resumo", string.IsNullOrEmpty(resumo) ? "0" : resumo.Trim(), ParameterType.UrlSegment);
            var a = _client.Execute<List<TransacaoRedeVsTEFListar>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<TransacaoRedeVsTEFListar>();
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

        public List<ConciliacaoTransacaoRede> TransacaoRedeListar(int? filtro_rede, int? filtro_tp_cartao, int? filtro_tp_data, DateTime datainicio, DateTime datafinal, string resumo, string bandeira, string banco = "0")
        {
            var request = new RestRequest("api/relatorios/TransacaoRedeListar/{idconta}/{tpcartao}/{tpdata}/{datainicio}/{datafinal}/{resumo}/{bandeira}/{banco}", Method.GET) { RequestFormat = DataFormat.Json };
            var obj = new BaseID();
            request.AddParameter("idconta", obj.IdConta, ParameterType.UrlSegment);
            // request.AddParameter("idrede",  0, ParameterType.UrlSegment);
            // request.AddParameter("tpsituacao", filtro_situacao ?? 3, ParameterType.UrlSegment);
            request.AddParameter("tpcartao", filtro_tp_cartao ?? 3, ParameterType.UrlSegment);  // 0 - credito 1 -  debit0
            request.AddParameter("tpdata", filtro_tp_data, ParameterType.UrlSegment);
            request.AddParameter("datainicio", datainicio.Ticks, ParameterType.UrlSegment);
            request.AddParameter("datafinal", datafinal.Ticks, ParameterType.UrlSegment);
            request.AddParameter("resumo", string.IsNullOrEmpty(resumo) ? "0" : resumo.Trim(), ParameterType.UrlSegment);
            request.AddParameter("bandeira", string.IsNullOrEmpty(bandeira) ? "0" : bandeira, ParameterType.UrlSegment);
            request.AddParameter("banco", string.IsNullOrEmpty(banco) ? "0" : banco, ParameterType.UrlSegment);

            var a = _client.Execute<List<ConciliacaoTransacaoRede>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<ConciliacaoTransacaoRede>();
            return a.Data;
        }

        public List<ConciliacaoUseRedeEEFICreditosStructListar> RelatorioFinanceiroCreditosListar(string chave, string datainicio, string datafinal, string bandeira, string resumo, string banco)
        {
            var request = new RestRequest("api/relatorios/RelatorioFinanceiroCreditosListarXML/{chave}/{datainicio}/{datafinal}/{bandeira}/{resumo}/{banco}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("chave", chave, ParameterType.UrlSegment);
            request.AddParameter("datainicio", datainicio, ParameterType.UrlSegment);
            request.AddParameter("datafinal", datafinal, ParameterType.UrlSegment);
            request.AddParameter("bandeira", string.IsNullOrEmpty(bandeira) ? "0" : bandeira.Trim(), ParameterType.UrlSegment);
            request.AddParameter("resumo", string.IsNullOrEmpty(resumo) ? "0" : resumo.Trim(), ParameterType.UrlSegment);
            request.AddParameter("banco", string.IsNullOrEmpty(banco) ? "0" : banco.Trim(), ParameterType.UrlSegment);
            var a = _client.Execute<List<ConciliacaoUseRedeEEFICreditosStructListar>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<ConciliacaoUseRedeEEFICreditosStructListar>();
            return a.Data;
        }

        public List<ConciliacaoUseRedeEEFIAntecipacaoStructListar> RelatorioFinanceiroAntecipacaoListar(string chave, string datainicio, string datafinal)
        {
            var request = new RestRequest("api/relatorios/RelatorioFinanceiroAntecipacaoListarXML/{chave}/{datainicio}/{datafinal}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("chave", chave, ParameterType.UrlSegment);
            request.AddParameter("datainicio", datainicio, ParameterType.UrlSegment);
            request.AddParameter("datafinal", datafinal, ParameterType.UrlSegment);
            var a = _client.Execute<List<ConciliacaoUseRedeEEFIAntecipacaoStructListar>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<ConciliacaoUseRedeEEFIAntecipacaoStructListar>();
            return a.Data;
        }

        public List<ConciliacaoUseRedeEEFIAjustesDesagendamentoStructListar> RelatorioFinanceiroAjustesDesagendamentoListar(string chave, string datainicio, string datafinal)
        {
            var request = new RestRequest("api/relatorios/RelatorioFinanceiroAjustesDesagendamentoListarXML/{chave}/{datainicio}/{datafinal}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("chave", chave, ParameterType.UrlSegment);
            request.AddParameter("datainicio", datainicio, ParameterType.UrlSegment);
            request.AddParameter("datafinal", datafinal, ParameterType.UrlSegment);
            var a = _client.Execute<List<ConciliacaoUseRedeEEFIAjustesDesagendamentoStructListar>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<ConciliacaoUseRedeEEFIAjustesDesagendamentoStructListar>();
            return a.Data;
        }

        public List<ConciliacaoUseRedeEEFIDesagendamentoParcelasStructListar> RelatorioFinanceiroDesagendamentoParcelasListar(string chave, string datainicio, string datafinal)
        {
            var request = new RestRequest("api/relatorios/RelatorioFinanceiroDesagendamentoParcelasListarXML/{chave}/{datainicio}/{datafinal}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("chave", chave, ParameterType.UrlSegment);
            request.AddParameter("datainicio", datainicio, ParameterType.UrlSegment);
            request.AddParameter("datafinal", datafinal, ParameterType.UrlSegment);
            var a = _client.Execute<List<ConciliacaoUseRedeEEFIDesagendamentoParcelasStructListar>>(request);
            if (a.StatusCode != HttpStatusCode.OK)
                a.Data = new List<ConciliacaoUseRedeEEFIDesagendamentoParcelasStructListar>();
            return a.Data;
        }


    }
}