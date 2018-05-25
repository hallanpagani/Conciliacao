using Conciliacao.Models;
using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoModelo.model.conciliador.UseRede;
using ConciliacaoModelo.model.relatorio;
using ConciliacaoPersistencia.banco;
using ConciliacaoPersistencia.model;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Xml.Serialization;

namespace ConciliacaoAPI.Controllers
{
    public class TEFController : ApiController
    {
        // GET: Relatorios
        /* [HttpGet]
         [Route("api/tef/TransacaoTEFListar/{idconta:long}/{idrede:long}/{tpsituacao:long}/{tpdata:long}/{datainicio:long}/{datafinal:long}")]
         public List<TransacaoTEF> TransacaoTEFListar(long idconta, long idrede, long tpsituacao,
             long tpdata, long datainicio, long datafinal)
         {

             string filtro = String.Format("id_conta={0}", idconta);
             if (idrede != 0)
             {
                 filtro = filtro +
                          String.Format(
                              " and ds_rede=(select ds_rede from cadastro_rede where id_rede={0} and id_conta={1})",
                              idrede, idconta);
             }

             if (tpsituacao == 0)
             {
                 filtro = filtro +
                          " and coalesce((select 1 from conciliador_tef t where t.ds_rede = a.ds_rede and t.dt_transacao = a.dt_transacao and t.nsu_rede = a.nsu_rede),0) = 1";
             }
             else if (tpsituacao == 1)
             {
                 filtro = filtro +
                          " and coalesce((select 1 from conciliador_tef t where t.ds_rede = a.ds_rede and t.dt_transacao = a.dt_transacao and t.nsu_rede = a.nsu_rede),0) = 0";
             }

             filtro = filtro +
                      String.Format(" and {0} between '{1}' and '{2}' ",
                          tpdata.Equals(0) ? "dt_transacao" : "dt_transacao",
                          new DateTime(datainicio).ToString("yyyy-MM-dd"),
                          new DateTime(datafinal).ToString("yyyy-MM-dd"));

             return DAL.ListarObjetos<TransacaoTEF>(filtro,
                 String.Format(" 6 {0} desc ", tpdata.Equals(0) ? ",dt_transacao" : ",dt_transacao"));
         } */

        [HttpGet]
        [Route("api/tef/GetLojas/{idconta:long}")]
        public List<TransacaoTEFLojas> GetLojas(long idconta)
        {
            var tef = TEFDAL.GetLojas(idconta);
            return tef;
        }

        [HttpGet]
        [Route("api/tef/GetTipoTransacoes/{idconta:long}")]
        public List<TransacaoTEFTpTransacao> GetTipoTransacoes(long idconta)
        {
            var tef = TEFDAL.GetTipoTransacoes(idconta);
            return tef;
        }

        [HttpGet]
        [Route("api/tef/TransacaoTEFListar/{idconta:long}/{rede}/{tpsituacao:long}/{tpdata:long}/{datainicio:long}/{datafinal:long}/{valor}/{estabelecimento}/{administrador}/{resumo}/{loja}/{tp_transacao}/{tp_operacao}")]
        public List<TransacaoTEFListar> TransacaoTEFListar(long idconta, string rede, long tpsituacao,
            long tpdata, long datainicio, long datafinal, string valor, string estabelecimento, string administrador, string resumo="0", string loja = "0", string tp_transacao = "0", string tp_operacao = "0")
        {
            var tef = TEFDAL.TransacaoTEFListar(idconta, rede, 0, 0, datainicio, datafinal, Convert.ToDecimal(valor), estabelecimento ?? "-111", administrador ?? "-111", resumo, loja, tp_transacao, tp_operacao);
            return tef;

            /*

            string filtro = String.Format("id_conta={0}", idconta);
            if (idrede != 0)
            {
                filtro = filtro +
                         String.Format(
                             " and ds_rede=(select ds_rede from cadastro_rede where id_rede={0} and id_conta={1})",
                             idrede, idconta);
            }

            if (tpsituacao == 0)
            {
                filtro = filtro +
                         " and coalesce((select 1 from conciliador_tef t where t.ds_rede = a.ds_rede and t.dt_transacao = a.dt_transacao and t.nsu_rede = a.nsu_rede),0) = 1";
            }
            else if (tpsituacao == 1)
            {
                filtro = filtro +
                         " and coalesce((select 1 from conciliador_tef t where t.ds_rede = a.ds_rede and t.dt_transacao = a.dt_transacao and t.nsu_rede = a.nsu_rede),0) = 0";
            }

            filtro = filtro +
                     String.Format(" and {0} between '{1}' and '{2}' ",
                         tpdata.Equals(0) ? "dt_transacao" : "dt_transacao",
                         new DateTime(datainicio).ToString("yyyy-MM-dd"),
                         new DateTime(datafinal).ToString("yyyy-MM-dd"));

           DAL.ListarObjetos<TransacaoTEF>(filtro,
                String.Format(" 6 {0} desc ", tpdata.Equals(0) ? ",dt_transacao" : ",dt_transacao")); */
        }

        [HttpGet]
        [Route("api/tef/GravarRegistrosTEF/")]
        public bool GravarRegistrosTEF()
        {

            List<BancoTEF> ConexaoTEF = DAL.ListarObjetos<BancoTEF>();

            foreach (var item in ConexaoTEF)
            {
                var data = DAL.GetDate(string.Format("select cast(coalesce(max(Transacao_Inicio)  ,'1900-01-01') as date) as maior_data from cadastro_transacoes_tef where id_conta = {0} and UPPER(estabelecimento) like UPPER('{1}%') ", item.IdConta, item.identificacao_tef));

                TransacoesTEF TransacaoTEF = TEFDAL.TransacaoIncluir(Convert.ToInt64(item.IdConta), data.AddDays(1).Ticks, item);
            }
            return true;
        }

        [HttpGet]
        [Route("api/tef/GravarRegistrosData/{idconta:long}/{data}")]
        public bool GravarRegistrosTEFData(int idconta, string data)
        {

            List<BancoTEF> ConexaoTEF = DAL.ListarObjetos<BancoTEF>();

            foreach (var item in ConexaoTEF)
            {
                var ddata = DateTime.Parse(data); //  DAL.GetDate(string.Format("select cast(coalesce(max(data_atual)  ,'1900-01-01') as date) as maior_data from cadastro_transacoes_tef where id_conta = {0} and UPPER(estabelecimento) like UPPER('{1}%') ", item.IdConta, item.identificacao_tef));

                TransacoesTEF TransacaoTEF = TEFDAL.TransacaoIncluir(Convert.ToInt64(idconta), ddata.Ticks, ddata.Ticks, item);
            }
            return true;
        }

        [HttpGet]
        [Route("api/tef/GravarRegistrosEletricaCapit/")]
        public bool GravarRegistrosEletricaCapit()
        {
            var data_ini = Convert.ToDateTime("2017-09-06");
            var data_fim = Convert.ToDateTime("2017-09-17");

            BancoTEF model = new BancoTEF();
            model.identificacao_tef = "EletricaCapit";
            model.usuario_banco = "vspague_suporte";
            model.ip_banco = "172.31.255.38";
            model.porta_banco = "1521";
            model.senha_banco = "@s0t3cht1";
            model.IdConta = 9;

            TransacoesTEF TransacaoTEF = TEFDAL.TransacaoIncluir(9, data_ini.Ticks, data_fim.Ticks, model);

            return true;
        }


        [HttpGet]
        [Route("api/tef/GravarRegistrosAbdias/")]
        public bool GravarRegistrosAbdias()
        {
            var data_ini = Convert.ToDateTime("2017-09-01");
            var data_fim = Convert.ToDateTime("2018-09-17");

            BancoTEF model = new BancoTEF();
            model.identificacao_tef = "VarejaoAbdias";
            model.usuario_banco = "vspague_suporte";
            model.ip_banco = "172.31.255.28";
            model.porta_banco = "1521";
            model.senha_banco = "@s0t3cht1";
            model.IdConta = 118;

            TransacoesTEF TransacaoTEF = TEFDAL.TransacaoIncluir(118, data_ini.Ticks, data_fim.Ticks, model);

            return true;
        }


    }
}