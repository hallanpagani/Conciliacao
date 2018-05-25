using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Conciliacao.App_Helpers.Componentes;
using Conciliacao.Helper.Rest;
using Conciliacao.Models;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoPersistencia.banco;
using Conciliacao.Controllers.Generico;

namespace Conciliacao.Controllers.Conciliador
{
    [Authorize]
    public class ExtratoTEFController : AppController
    {
        static readonly TEFRestClient _restTEF = new TEFRestClient();

        // GET: ExtratoTEF
        public ActionResult Index()
        {

            var model = new ArquivoDeCartoesTEF
            {
                DataInicio = null,
                DataFinal = null,
                tp_arquivo = null,
                arquivo = null,
                ds_arquivo = null,
                ArquivosTEf = new List<TransacaoTEFListar>()
            };

            ViewBag.BotaoProcessar = "Buscar";

            return View(model);
        }

        public ActionResult IP()
        {
            DateTime today = DateTime.Now;
            DateTime answer = today.AddDays(-1);

            var model = new ArquivoDeCartoesTEF
            {
                DataInicio = answer,
                DataFinal = answer,
                tp_arquivo = null,
                arquivo = null,
                ds_arquivo = null,
                tp_data = 0, /// emissao
                ArquivosTEf = new List<TransacaoTEFListar>(),
                filtro_valor = 0,
                filtro_resumo = ""
            };
            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            ViewBag.tp_administradora = Models.AdministradoraTypes.getAdministradoras("");
            return View(model);
        }

        [HttpPost]
        public ActionResult IP(ArquivoDeCartoesTEF obj)
        {
            var model = new ArquivoDeCartoesTEF
            {
                DataInicio = obj.DataInicio,
                DataFinal = obj.DataFinal,
                tp_arquivo = null,
                arquivo = null,
                ds_arquivo = null,
                tp_data = 0, /// emissao
                ArquivosTEf = _restTEF.TransacaoTEFListar(obj.filtro_nm_rede, obj.tp_situacao, 0, obj.DataInicio ?? DateTime.MinValue, obj.DataFinal ?? DateTime.MaxValue, obj.filtro_valor, obj.filtro_nm_estabelecimento, obj.tp_administradora, obj.filtro_resumo) ?? new List<TransacaoTEFListar>(),
                filtro_nm_rede = obj.filtro_nm_rede,
                filtro_rede = obj.filtro_rede,
                filtro_valor = obj.filtro_valor,
                filtro_estabelecimento = obj.filtro_estabelecimento,
                filtro_nm_estabelecimento = obj.filtro_nm_estabelecimento
            };
            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            ViewBag.tp_administradora = Models.AdministradoraTypes.getAdministradoras("");
            return View(model);
        }


        [HttpPost]
        public JsonResult ProcessarArquivo()
        {
            var model = new ArquivoDeCartoesTEF();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];


                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        model.ds_arquivo = file.FileName;

                        var arquivo = new ConciliacaoArquivoManipular(new StreamReader(file.InputStream));
                        String first_line = arquivo.LerLinha(true);

                       // var a = first_line.Split(';');
                       // model.dt_inicio = Convert.ToDateTime(a[1].Substring(0, 2) + "/" + a[1].Substring(2, 2) + "/" + a[1].Substring(4, 4));
                       // model.dt_final = Convert.ToDateTime(a[2].Substring(0, 2) + "/" + a[2].Substring(2, 2) + "/" + a[2].Substring(4, 4));
                      //  model.tp_arquivo = a[3];

                        var bancario = new ConciliacaoTEFDesmontar(arquivo, first_line);
                        model.TEF = bancario.GetListTransacaoTEF();

                        DAL.GravarList(model.TEF);

                        ViewBag.BotaoProcessar = "Gravar";

                    }
                    catch (Exception ex)
                    {
                        this.AddNotification(ex.Message, NotificationType.Erro);
                    }

                }

            }
            var result = Json(model, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }


        public bool TEFInicializar()
        {
            TransacoesTEF TransacaoTEF = ConciliacaoPersistencia.model.TEFDAL.TransacaoIncluir( Convert.ToInt64(UsuarioLogado.IdConta), new DateTime(2017, 1, 1 ).Ticks);

            return true;
        }

        public bool ProcessarTEFDiaAnterior()
        {
            TransacoesTEF TransacaoTEF = ConciliacaoPersistencia.model.TEFDAL.TransacaoIncluir(Convert.ToInt64(UsuarioLogado.IdConta), DateTime.Now.AddDays(-1).Ticks);

            return true;
        }

        public bool ProcessarTEFBetween()
        {
            TransacoesTEF TransacaoTEF = ConciliacaoPersistencia.model.TEFDAL.TransacaoIncluir(Convert.ToInt64(UsuarioLogado.IdConta), DateTime.Now.AddDays(-2).Ticks);

            return true;
        }

        public bool ProcessarTEFTres()
        {
            TransacoesTEF TransacaoTEF = ConciliacaoPersistencia.model.TEFDAL.TransacaoIncluir(Convert.ToInt64(UsuarioLogado.IdConta), DateTime.Now.AddDays(-3).Ticks);

            return true;
        }

        public bool ProcessarTEFQuatro()
        {
            TransacoesTEF TransacaoTEF = ConciliacaoPersistencia.model.TEFDAL.TransacaoIncluir(Convert.ToInt64(UsuarioLogado.IdConta), DateTime.Now.AddDays(-4).Ticks);

            return true;
        }

        public bool ProcessarTEFCinco()
        {
            TransacoesTEF TransacaoTEF = ConciliacaoPersistencia.model.TEFDAL.TransacaoIncluir(Convert.ToInt64(UsuarioLogado.IdConta), DateTime.Now.AddDays(-5).Ticks);

            return true;
        }

        public bool ProcessarTEFSeis()
        {
            TransacoesTEF TransacaoTEF = ConciliacaoPersistencia.model.TEFDAL.TransacaoIncluir(Convert.ToInt64(UsuarioLogado.IdConta), DateTime.Now.AddDays(-6).Ticks);

            return true;
        }

        public bool ProcessarTEFSete()
        {
            TransacoesTEF TransacaoTEF = ConciliacaoPersistencia.model.TEFDAL.TransacaoIncluir(Convert.ToInt64(UsuarioLogado.IdConta), DateTime.Now.AddDays(-7).Ticks);

            return true;
        }

        public bool ProcessarTEFOnze()
        {
            TransacoesTEF TransacaoTEF = ConciliacaoPersistencia.model.TEFDAL.TransacaoIncluir(Convert.ToInt64(UsuarioLogado.IdConta), DateTime.Now.AddDays(-11).Ticks);

            return true;
        }

        public bool ProcessarTEFTrinta()
        {
            TransacoesTEF TransacaoTEF = ConciliacaoPersistencia.model.TEFDAL.TransacaoIncluir(Convert.ToInt64(UsuarioLogado.IdConta), DateTime.Now.AddDays(-30).Ticks);

            return true;
        }

        public bool ProcessarTEFTrintaUm()
        {
            TransacoesTEF TransacaoTEF = ConciliacaoPersistencia.model.TEFDAL.TransacaoIncluir(Convert.ToInt64(UsuarioLogado.IdConta), DateTime.Now.AddDays(-31).Ticks);

            return true;
        }

    }
}