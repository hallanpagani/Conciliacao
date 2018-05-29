using System.Globalization;
using Conciliacao.Controllers.Generico;
using Conciliacao.Helper.Rest;
using Conciliacao.Models.Relatorios;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoModelo.model.relatorio;
using System;
using System.Collections.Generic;
using System.Linq;
using ConciliacaoPersistencia.model;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Web.Mvc;
using ConciliacaoPersistencia.banco;
using ConciliacaoModelo.model.generico;
using ConciliacaoModelo.model.conciliador.UseRede;
using ConciliacaoModelo.model.cadastros;

namespace Conciliacao.Controllers
{
    [Authorize]
    public class RelatoriosController : AppController
    {
        static readonly RelatoriosRestClient _restRelatorio = new RelatoriosRestClient();
        static readonly TEFRestClient _restTEF = new TEFRestClient();

        // GET: Relatorios
        public ActionResult RelatoriosRedeVsTEF()
        {
            var listar = new RedeVsTEFViewModel
            {
                tp_situacao = 2,
                ListaRedeVsTEF = new List<TransacaoRedeVsTEFListar>(),
                DataInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                DataFinal = DateTime.Now.Date,
                tp_data = 0, /// emissao
            };
            ViewBag.tp_data = Models.DataTypes.getDataTypes();

            return View("RelatoriosRedeVsTEF", listar);
        }

        public ActionResult RelatorioFinanceiroCreditosPreview(FormCollection frm)
        {
            DateTime datainicio = DateTime.ParseExact(frm["DataInicio"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime datafinal = DateTime.ParseExact(frm["DataFinal"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var model = new TransacaoCreditosViewModel();
            model.ListCreditos = _restRelatorio.RelatorioFinanceiroCreditosListar(UsuarioLogado.ContaChave, frm["datainicio"].Replace("/", ""), frm["datafinal"].Replace("/", ""), frm["tp_administradora"].Trim(), frm["filtro_resumo"], frm["filtro_banco"]).OrderBy(s => s.data_lancamento).ToList();
            // model.ListDebitos = _restRelatorio.TransacaoRedeListar(0, 1, 1, datainicio, datafinal, frm["filtro_resumo"], frm["tp_administradora"].Trim(), frm["filtro_banco"]) ?? new List<ConciliacaoTransacaoRede>();

            ViewBag.DataInicio = frm["datainicio"];
            ViewBag.DataFinal = frm["datafinal"];

            ViewBag.QtdRegistros = model.ListCreditos.Count();
            var TotalLiqCreditos = model.ListCreditos.Sum(x => x.valor_lancamento);
            ViewBag.TotalLiqCreditos = TotalLiqCreditos.ToString("#,##0.00");

            var conta = new BaseID();

            ViewBag.tp_administradora = Models.AdministradoraTypes.getAdministradoras(frm["tp_administradora"].Trim()).Where(x => x.Selected == true).First().Text;
            ViewBag.filtro_resumo = frm["filtro_resumo"];

            if (frm["filtro_banco"] != "")
            {
                model.filtro_banco = Convert.ToInt32(frm["filtro_banco"]);
                model.filtro_nm_banco = frm["filtro_nm_banco"];
            }

            return View(model);
        }

        public ActionResult RelatorioFinanceiroDebitosPreview(FormCollection frm)
        {
            DateTime datainicio = DateTime.ParseExact(frm["DataInicio"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime datafinal = DateTime.ParseExact(frm["DataFinal"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var model = new TransacaoCreditosViewModel();
            // model.ListCreditos = _restRelatorio.RelatorioFinanceiroCreditosListar(UsuarioLogado.ContaChave, frm["datainicio"].Replace("/", ""), frm["datafinal"].Replace("/", ""), frm["tp_administradora"].Trim(), frm["filtro_resumo"], frm["filtro_banco"]).OrderBy(s => s.data_lancamento).ToList();  
            model.ListDebitos = (_restRelatorio.TransacaoRedeListar(0, 1, 1, datainicio, datafinal, frm["filtro_resumo"], frm["tp_administradora"].Trim(), frm["filtro_banco"]) ?? new List<ConciliacaoTransacaoRede>()).Where(x => x.is_tipo_registro.Equals("COMPROVANTE")).OrderBy(s => s.is_numero_filiacao_pv).ThenBy(s => s.is_numero_resumo_vendas).ThenByDescending(s=>s.is_tipo_registro).ThenByDescending(s => s.data_cv).ToList() ;

            ViewBag.DataInicio = frm["datainicio"];
            ViewBag.DataFinal = frm["datafinal"];

            ViewBag.QtdRegistrosDebitos = model.ListDebitos.Count(x => x.is_tipo_registro.Equals("COMPROVANTE"));
            var TotalLiqDebitos = model.ListDebitos.Where(x => x.is_tipo_registro.Equals("COMPROVANTE")).Sum(x => x.is_valor_liquido);
            ViewBag.TotalLiqDebitos = TotalLiqDebitos.ToString("#,##0.00");

            ViewBag.tp_administradora = Models.AdministradoraTypes.getAdministradoras(frm["tp_administradora"].Trim()).Where(x => x.Selected == true).First().Text;
            ViewBag.filtro_resumo = frm["filtro_resumo"];

            if (frm["filtro_banco"] != "")
            {
                model.filtro_banco = Convert.ToInt32(frm["filtro_banco"]);
                model.filtro_nm_banco = frm["filtro_nm_banco"];
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult RelatoriosRedeVsTEF(RedeVsTEFViewModel model)
        {

            if (model.filtro_rede.GetValueOrDefault(0) == 0)
            {
                model.filtro_nm_rede = "";
            }

            ViewBag.tp_data = Models.DataTypes.getDataTypes();

            var listar = new RedeVsTEFViewModel
            {
                filtro_rede = model.filtro_rede,
                filtro_nm_rede = model.filtro_nm_rede,
                tp_situacao = model.tp_situacao,
                ListaRedeVsTEF = _restRelatorio.TransacaoRedeVsTEFListar(model.filtro_rede, model.tp_situacao, model.tp_data, model.DataInicio ?? DateTime.MinValue, model.DataFinal ?? DateTime.MaxValue, model.resumo) ?? new List<TransacaoRedeVsTEFListar>(),
                DataInicio = model.DataInicio ?? DateTime.MinValue,
                DataFinal = model.DataFinal ?? DateTime.MaxValue,
                tp_data = model.tp_data
            };

            return View("RelatoriosRedeVsTEF", listar);
        }


        // GET: Relatorios
        public ActionResult RelatoriosRedeVsExtratoBancario()
        {
            var listar = new RedeVsExtratoBancarioViewModel
            {
                tp_situacao = 2,
                ListaRedeVsExtratoBancario = new List<TransacaoRedeVsExtratoBancarioListar>(),
                DataInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                DataFinal = DateTime.Now.Date,
                tp_data = 0, /// emissao
            };
            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            return View("RelatoriosRedeVsExtratoBancario", listar);
        }

        [Authorize]
        [HttpPost]
        public ActionResult RelatoriosRedeVsExtratoBancario(RedeVsExtratoBancarioViewModel model)
        {

            if (model.filtro_rede.GetValueOrDefault(0) == 0)
            {
                model.filtro_nm_rede = "";
            }

            ViewBag.tp_data = Models.DataTypes.getDataTypes();

            var listar = new RedeVsExtratoBancarioViewModel
            {
                filtro_rede = model.filtro_rede,
                filtro_nm_rede = model.filtro_nm_rede,
                tp_situacao = model.tp_situacao,
                ListaRedeVsExtratoBancario = _restRelatorio.TransacaoRedeVsExtratoBancarioListar(model.filtro_rede, model.tp_situacao, model.tp_data, model.DataInicio ?? DateTime.MinValue, model.DataFinal ?? DateTime.MaxValue) ?? new List<TransacaoRedeVsExtratoBancarioListar>(),
                DataInicio = model.DataInicio ?? DateTime.MinValue,
                DataFinal = model.DataFinal ?? DateTime.MaxValue,
                tp_data = model.tp_data
            };

            return View("RelatoriosRedeVsExtratoBancario", listar);
        }

        // GET: Relatorios
        public ActionResult RelatoriosTransacoesRede(string data, string debitocredito)
        {
            var datainicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var datafinal = DateTime.Now.Date;
            var tp_cartao = 3;
            var transacaoarquivos = new List<ConciliacaoTransacaoRede>();
            if (!String.IsNullOrEmpty(data))
            {
                datainicio = DateTime.ParseExact(data.Substring(4, 4) + "-" + data.Substring(2, 2) + "-" + data.Substring(0, 2), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                datafinal = datainicio;
            }

            if (!String.IsNullOrEmpty(debitocredito))
            {
                tp_cartao = 3;
                transacaoarquivos = _restRelatorio.TransacaoRedeListar(0, tp_cartao, 1, datainicio, datafinal, "0", "0") ??
                                    new List<ConciliacaoTransacaoRede>();
            }

            ViewBag.tp_cartao = Models.CartaoTypes.getCartaoTypes(tp_cartao);

            var listar = new TransacaoRedeViewModel
            {
                tp_situacao = 2,
                TransacaoArquivos = transacaoarquivos,
                DataInicio = datainicio,
                DataFinal = datafinal,
                tp_data = 0, /// emissao
                tp_cartao = tp_cartao,
                resumo = "0"
            };

            ViewBag.Disabled = "disabled";
            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            return View("RelatoriosTransacoesRede", listar);
        }

        [HttpPost]
        public ActionResult RelatoriosTransacoesRede(TransacaoRedeViewModel model)
        {

            if (model.filtro_rede.GetValueOrDefault(0) == 0)
            {
                model.filtro_nm_rede = "";
            }

            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            ViewBag.tp_cartao = Models.CartaoTypes.getCartaoTypes();

            var listar = new TransacaoRedeViewModel
            {
                filtro_rede = model.filtro_rede,
                filtro_nm_rede = model.filtro_nm_rede,
                tp_situacao = model.tp_situacao,
                TransacaoArquivos = _restRelatorio.TransacaoRedeListar(0, model.tp_cartao, 0, model.DataInicio ?? DateTime.MinValue, model.DataFinal ?? DateTime.MaxValue, model.resumo, "0") ?? new List<ConciliacaoTransacaoRede>(),
                DataInicio = model.DataInicio ?? DateTime.MinValue,
                DataFinal = model.DataFinal ?? DateTime.MaxValue,
                tp_data = model.tp_data,
                tp_cartao = model.tp_cartao,
                resumo = model.resumo
            };

            return View("RelatoriosTransacoesRede", listar);
        }

        public ActionResult BuscaTEF()
        {

            ViewBag.tp_cartao = Models.CartaoTypes.getCartaoTypes(3);

            var listar = new TransacaoRedeViewModel
            {
                tp_situacao = 2,
                TransacaoArquivos = new List<ConciliacaoTransacaoRede>(),
                DataInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                DataFinal = DateTime.Now.Date,
                tp_data = 0, /// emissao
                tp_cartao = 3
            };
            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            ViewBag.Disabled = "disabled";
            return View("RelatoriosTransacoesRede", listar);
        }

        [HttpPost]
        public ActionResult BuscaTEF(TransacaoRedeViewModel model)
        {
            if (model.filtro_rede.GetValueOrDefault(0) == 0)
            {
                model.filtro_nm_rede = "";
            }

            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            ViewBag.tp_cartao = Models.CartaoTypes.getCartaoTypes();

            var listar = new TransacaoRedeViewModel
            {
                filtro_rede = model.filtro_rede,
                filtro_nm_rede = model.filtro_nm_rede,
                tp_situacao = model.tp_situacao,
                TransacaoArquivos = _restRelatorio.TransacaoRedeListar(0, model.tp_cartao, 0, model.DataInicio ?? DateTime.MinValue, model.DataFinal ?? DateTime.MaxValue, model.resumo, "0") ?? new List<ConciliacaoTransacaoRede>(),
                DataInicio = model.DataInicio ?? DateTime.MinValue,
                DataFinal = model.DataFinal ?? DateTime.MaxValue,
                tp_data = model.tp_data,
                tp_cartao = model.tp_cartao
            };

            var conta = new BaseID();

            foreach (var item in listar.TransacaoArquivos)
            {
                var status =
                 TEFDAL.GetStatusTEF(conta.IdConta, item.is_numero_cv);
                item.is_codigo_tef = status.CodigoTEF;
                item.is_nsu_tef = status.NSUTEF;
                item.is_nsu_rede = status.NSUREDE;
            }

            return View("RelatoriosTransacoesRede", listar);


        }

        [HttpPost]
        public FileResult BuscaTEFXML(FormCollection frm)
        {
            var stream = new MemoryStream();
            try
            {
                DateTime datainicio = DateTime.ParseExact(frm["DataInicio"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DateTime datafinal = DateTime.ParseExact(frm["DataFinal"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                var listar = new TransacaoRedeViewModel
                {
                    TransacaoArquivos = _restRelatorio.TransacaoRedeListar(0, Convert.ToInt32(frm["tp_cartao"]), 0, datainicio, datafinal, frm["resumo"], "0") ?? new List<ConciliacaoTransacaoRede>(),
                    DataInicio = datainicio,
                    DataFinal = datafinal,
                    tp_data = Convert.ToInt32(frm["tp_data"]),
                    tp_cartao = Convert.ToInt32(frm["tp_cartao"]),
                    resumo = frm["resumo"]
                };

                var conta = new BaseID();
                foreach (var item in listar.TransacaoArquivos)
                {
                    var status =
                     TEFDAL.GetStatusTEF(conta.IdConta, item.is_numero_cv);
                    item.is_codigo_tef = status.CodigoTEF;
                    item.is_nsu_tef = status.NSUTEF;
                    item.is_nsu_rede = status.NSUREDE;
                }


                var objs = new List<ConciliacaoTransacaoRede>();
                var doc = new XmlDocument();
                var serializer = new XmlSerializer(objs.GetType());

                serializer.Serialize(stream, listar.TransacaoArquivos);
                stream.Position = 0;
                doc.Load(stream);

                string FileBankPhysicalFolder = Server.MapPath("~/XMLS/");
                string Name = ("RelatorioTrancacoes_" + DateTime.Now.Date.ToString().Substring(0, 10).Replace("/", "") + "_" + DateTime.Now.ToString().Substring(11, 8).Replace(":", "") + ".xml").Replace(" ", "_");
                string FileBankPath = FileBankPhysicalFolder + Name;
                if (!System.IO.File.Exists(FileBankPath))
                {
                    System.IO.File.Create(FileBankPath).Close();
                }
                string Content = doc.InnerXml;
                System.IO.File.WriteAllText(FileBankPath, Content);

                FileInfo file = new FileInfo(FileBankPath); //path: directory of file; fich: name of file with spaces
                Response.Clear();
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", file.Name));
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.Flush();
                Response.WriteFile(FileBankPath);
                Response.End();

                return File(FileBankPath, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(FileBankPath));

            }

            catch (Exception e)
            {
                DALErro.Gravar(e.Message + " / " + e.StackTrace);

                throw e;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }

        public ActionResult RelatoriosTransacoesTEF()
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
                tp_administradora = "",
                filtro_resumo = "",
                filtro_nm_loja = "",
                filtro_tp_transacao = "",
                filtro_tp_operacao = "",
                tp_situacao = 2
            };
            ViewBag.tp_administradora = Models.AdministradoraTypes.getAdministradoras("");
            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            ViewBag.filtro_nm_loja = _restTEF.TransacaoTEFLojas().Select(i => new SelectListItem { Text = i.loja, Value = i.loja.ToUpper() }).ToList();
            ViewBag.filtro_tp_transacao = _restTEF.TransacaoTipoTransacoes().Select(i => new SelectListItem { Text = i.tp_transacao, Value = i.tp_transacao.ToUpper() }).ToList();
            ViewBag.filtro_tp_operacao = Models.AdministradoraTypes.getTpOperacoes("");
            return View(model);
        }

        [HttpPost]
        public ActionResult RelatoriosTransacoesTEF(ArquivoDeCartoesTEF obj)
        {
            var arquivostef = _restTEF.TransacaoTEFListar(obj.filtro_rede == null ? null : obj.filtro_nm_rede, obj.tp_situacao, 0, obj.DataInicio ?? DateTime.MinValue, obj.DataFinal ?? DateTime.MaxValue, obj.filtro_valor, obj.filtro_nm_estabelecimento, obj.tp_administradora, obj.filtro_resumo, obj.filtro_nm_loja, obj.filtro_tp_transacao, obj.filtro_tp_operacao) ?? new List<TransacaoTEFListar>();

            var model = new ArquivoDeCartoesTEF
            {
                DataInicio = obj.DataInicio,
                DataFinal = obj.DataFinal,
                tp_arquivo = null,
                arquivo = null,
                ds_arquivo = null,
                tp_data = 0, /// emissao
                ArquivosTEf = arquivostef,

                filtro_tp_transacao = obj.filtro_tp_transacao == null ? null : obj.filtro_tp_transacao,
                filtro_tp_operacao = obj.filtro_tp_operacao == null ? null : obj.filtro_tp_operacao,

                filtro_nm_loja = obj.filtro_nm_loja == null ? null : obj.filtro_nm_loja,
                filtro_nm_rede = obj.filtro_rede == null ? null : obj.filtro_nm_rede,
                filtro_rede = obj.filtro_rede,
                filtro_valor = obj.filtro_valor,
                filtro_resumo = obj.filtro_resumo,
                filtro_estabelecimento = obj.filtro_estabelecimento,
                filtro_nm_estabelecimento = obj.filtro_estabelecimento == null ? null : obj.filtro_nm_estabelecimento
            };

            var conta = new BaseID();
            foreach (var item in model.ArquivosTEf)
            {
                var sql = string.Format(@"select distinct encontrou
                                           from(select 1 as encontrou
                                                from
                                                    conciliador_userede_eevd_comprovantevenda b
                                                where
                                                    ((b.numero_cv = cast('{0}' as DECIMAL)) or (b.numero_cv = cast('{2}' as DECIMAL))) and (b.id_conta = {1} )
                                                union all
                                                select
                                                    1
                                                from
                                                    conciliador_userede_eevc_comprovantevenda c
                                                where
                                                    ((c.numero_cv = cast('{0}' as DECIMAL)) or (c.numero_cv = cast('{2}' as DECIMAL))) and (c.id_conta = {1} )  ) as x  ",
                    item.nsu_rede, conta.IdConta, item.nsu_tef);
                item.conciliado = (int)DAL.GetInt(sql, 0);
            }

            if (obj.tp_situacao == 0)
            {
                model.ArquivosTEf = model.ArquivosTEf.Where(x => x.conciliado == 1).ToList();
            }
            if (obj.tp_situacao == 1)
            {
                model.ArquivosTEf = model.ArquivosTEf.Where(x => x.conciliado == 0).ToList();
            }

            ViewBag.filtro_nm_loja = _restTEF.TransacaoTEFLojas().Select(i => new SelectListItem { Text = i.loja, Value = i.loja.ToUpper() }).ToList();

            ViewBag.tp_data = Models.DataTypes.getDataTypes();

            ViewBag.tp_administradora = Models.AdministradoraTypes.getAdministradoras("");

            ViewBag.filtro_tp_operacao = Models.AdministradoraTypes.getTpOperacoes("");

            ViewBag.filtro_tp_transacao = _restTEF.TransacaoTipoTransacoes().Select(i => new SelectListItem { Text = i.tp_transacao, Value = i.tp_transacao.ToUpper() }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult RelatoriosTransacoesTEFTXT(FormCollection frm)
        {
            DateTime datainicio = DateTime.ParseExact(frm["DataInicio"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime datafinal = DateTime.ParseExact(frm["DataFinal"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            var model = new ArquivoDeCartoesTEF
            {
                DataInicio = datainicio,
                DataFinal = datafinal,
                tp_arquivo = null,
                arquivo = null,
                ds_arquivo = null,
                tp_data = 0, /// emissao
                ArquivosTEf = _restTEF.TransacaoTEFListar(frm["filtro_nm_rede"], Convert.ToInt32(frm["tp_situacao"]), 0, datainicio, datafinal, 0, frm["filtro_nm_estabelecimento"], frm["tp_administradora"], frm["filtro_resumo"]) ?? new List<TransacaoTEFListar>(),
                filtro_nm_rede = frm["filtro_nm_rede"],
                filtro_rede = Convert.ToInt32(frm["filtro_rede"]),
                filtro_valor = Convert.ToDecimal(frm["filtro_valor"]),
                filtro_resumo = frm["filtro_resumo"]
            };

            var conta = new BaseID();

            string fileName = DateTime.Now.ToString() + ".txt";


            foreach (var item in model.ArquivosTEf)
            {
                var sql = string.Format(@"select distinct encontrou
                                           from(select 1 as encontrou
                                                from
                                                    conciliador_userede_eevd_comprovantevenda b
                                                where
                                                    (b.numero_cv = cast('{0}' as DECIMAL)) and (b.id_conta = {1} )
                                                union all
                                                select
                                                    1
                                                from
                                                    conciliador_userede_eevc_comprovantevenda c
                                                where
                                                    (c.numero_cv = cast('{0}' as DECIMAL)) and (c.id_conta = {1} )  ) as x  ",
                                        item.nsu_rede, conta.IdConta);
                item.conciliado = (int)DAL.GetInt(sql, 0);

                string textToAdd = "";
                using (StreamWriter writer = new StreamWriter(fileName, true))
                {
                    writer.Write(textToAdd);
                }

            }

            ViewBag.tp_data = Models.DataTypes.getDataTypes();

            ViewBag.tp_administradora = Models.AdministradoraTypes.getAdministradoras("");

            return View(model);
        }

        [HttpGet]
        public ActionResult RelatorioExtratoEstabelecimento()
        {
            var list = new RelatorioEstabelecimentoViewModel();
            ViewBag.DataInicio = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.DataFinal = DateTime.Now.ToString("dd/MM/yyyy");

            list.filtro_resumo = "";


            ViewBag.QtdRegistros = "0";
            ViewBag.TotalBruto = "0,00";

            return View(list);
        }

        [HttpPost]
        public ActionResult RelatorioExtratoEstabelecimento(RelatorioEstabelecimentoViewModel model)
        {
            string filtro = "";
            if (!(model.filtro_resumo ?? String.Empty).Equals(""))
            {
                filtro = string.Format(" and cast(nsu_rede as unsigned)={0} ", model.filtro_resumo);
            }
            model.ListaEstabelecimento = DAL.ListarObjetos<TransacaoEstabelecimentoListar>(string.Format(" id_conta={0} and dt_transacao >= '{1}' and dt_transacao <= '{2}' " + filtro,
                    Convert.ToInt32(UsuarioLogado.IdConta), (model.DataInicio ?? DateTime.MinValue).ToString("yyyy-MM-dd"), (model.DataFinal ?? DateTime.MaxValue).ToString("yyyy-MM-dd")));
            ViewBag.DataInicio = model.DataInicio.ToString();
            ViewBag.DataFinal = model.DataFinal.ToString();

            ViewBag.QtdRegistros = "0";
            ViewBag.TotalBruto = "0,00";

            return View(model);
        }

        [HttpGet]
        public ActionResult RelatorioFinanceiroCredito()
        {
            var model = new TransacaoCreditosViewModel();
            ViewBag.DataInicio = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.DataFinal = DateTime.Now.ToString("dd/MM/yyyy");

            ViewBag.QtdRegistros = "0";
            ViewBag.QtdRegistrosDebitos = "0";
            ViewBag.TotalLiqCreditos = "0,00";
            ViewBag.TotalLiqDebitos = "0,00";
            ViewBag.TotalGeralLiq = "0,00";
            ViewBag.tp_administradora = Models.AdministradoraTypes.getAdministradoras("");
            ViewBag.filtro_resumo = "";

            var conta = new BaseID();


            ViewBag.Credito = "active";
            ViewBag.Debito = "";

            return View(model);
        }

        [HttpPost]
        public ActionResult RelatorioFinanceiroCredito(FormCollection frm)
        {
            DateTime datainicio = DateTime.ParseExact(frm["DataInicio"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime datafinal = DateTime.ParseExact(frm["DataFinal"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            ViewBag.Credito = "active";
            ViewBag.Debito = "";


            var model = new TransacaoCreditosViewModel();
            model.ListCreditos = _restRelatorio.RelatorioFinanceiroCreditosListar(UsuarioLogado.ContaChave, frm["datainicio"].Replace("/", ""), frm["datafinal"].Replace("/", ""), frm["tp_administradora"].Trim(), frm["filtro_resumo"], frm["filtro_banco"]);
            model.ListDebitos = _restRelatorio.TransacaoRedeListar(0, 1, 1, datainicio, datafinal, frm["filtro_resumo"], frm["tp_administradora"].Trim(), frm["filtro_banco"]) ?? new List<ConciliacaoTransacaoRede>();

            ViewBag.DataInicio = frm["datainicio"];
            ViewBag.DataFinal = frm["datafinal"];

            ViewBag.QtdRegistros = model.ListCreditos.Count();
            ViewBag.QtdRegistrosDebitos = model.ListDebitos.Count(x => x.is_tipo_registro.Equals("COMPROVANTE"));

            var TotalLiqDebitos = model.ListDebitos.Where(x => x.is_tipo_registro.Equals("COMPROVANTE")).Sum(x => x.is_valor_liquido);
            var TotalLiqCreditos = model.ListCreditos.Sum(x => x.valor_lancamento);

            ViewBag.TotalLiqCreditos = TotalLiqCreditos.ToString("#,##0.00");
            ViewBag.TotalLiqDebitos = TotalLiqDebitos.ToString("#,##0.00");
            ViewBag.TotalGeralLiq = (TotalLiqDebitos + TotalLiqCreditos).ToString("#,##0.00");

            var conta = new BaseID();
            

            ViewBag.tp_administradora = Models.AdministradoraTypes.getAdministradoras(frm["tp_administradora"].Trim());
            ViewBag.filtro_resumo = frm["filtro_resumo"];

            if (frm["filtro_banco"] != "") {
                model.filtro_banco = Convert.ToInt32(frm["filtro_banco"]);
                model.filtro_nm_banco = frm["filtro_nm_banco"];
            }

            model.ListDebitos = model.ListDebitos.Where(x => x.is_tipo_registro.Equals("RESUMO")).ToList();

            return View(model);
        }


        [HttpGet]
        public ActionResult RelatorioFinanceiroExtratoBancario(string data, string historico, decimal valor)
        {
            DateTime datainicio = DateTime.ParseExact(data, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime datafinal = DateTime.ParseExact(data, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            BancoExpressao bancoexpressao = DAL.GetObjeto<BancoExpressao>(string.Format("nm_expressao=trim('{0}')", historico.Trim()))?? new BancoExpressao();

            var model = new TransacaoCreditosViewModel();

            if (bancoexpressao.tp_credito_debito.ToUpper() == "C")
            {
                model.ListCreditos = _restRelatorio.RelatorioFinanceiroCreditosListar(UsuarioLogado.ContaChave, data.Replace("/", ""), data.Replace("/", ""), bancoexpressao.bandeira, "", "");
            } else
            {
                model.ListDebitos = _restRelatorio.TransacaoRedeListar(0, 1, 1, datainicio, datafinal, "", bancoexpressao.bandeira, "") ?? new List<ConciliacaoTransacaoRede>();

                //model.ListDebitos = model.ListDebitos.Where(x => x.is_valor_liquido >= valor-(decimal)0.01 && x.is_valor_liquido <= valor+(decimal)0.01).ToList();
            }

            ViewBag.Credito = bancoexpressao.tp_credito_debito.ToUpper() == "C" ? "active" : "";
            ViewBag.Debito  = bancoexpressao.tp_credito_debito.ToUpper() == "D" ? "active" : "";


            ViewBag.DataInicio = data;
            ViewBag.DataFinal = data;

            ViewBag.QtdRegistros = model.ListCreditos.Count();
            ViewBag.QtdRegistrosDebitos = model.ListDebitos.Count(x => x.is_tipo_registro.Equals("COMPROVANTE"));

            var TotalLiqDebitos = model.ListDebitos.Where(x => x.is_tipo_registro.Equals("COMPROVANTE")).Sum(x => x.is_valor_liquido);
            var TotalLiqCreditos = model.ListCreditos.Sum(x => x.valor_lancamento);

            ViewBag.TotalLiqCreditos = TotalLiqCreditos.ToString("#,##0.00");
            ViewBag.TotalLiqDebitos = TotalLiqDebitos.ToString("#,##0.00");
            ViewBag.TotalGeralLiq = (TotalLiqDebitos + TotalLiqCreditos).ToString("#,##0.00");

            var conta = new BaseID();


            ViewBag.tp_administradora = Models.AdministradoraTypes.getAdministradoras(bancoexpressao.bandeira);
            ViewBag.filtro_resumo = "";

           /* if (frm["filtro_banco"] != "")
            {
                model.filtro_banco = Convert.ToInt32(frm["filtro_banco"]);
                model.filtro_nm_banco = frm["filtro_nm_banco"];
            } */

            return View("RelatorioFinanceiroCredito", model);
        }


        [HttpGet]
        public ActionResult RelatorioFinanceiroAntecipacao()
        {
            var list = new List<ConciliacaoUseRedeEEFIAntecipacaoStructListar>();
            ViewBag.DataInicio = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.DataFinal = DateTime.Now.ToString("dd/MM/yyyy");

            ViewBag.QtdRegistros = "0";
            ViewBag.TotalBruto = "0,00";

            return View(list);
        }


        [HttpGet]
        public ActionResult RelatoriosExtratoBancarios()
        {
            var list = new ArquivosBancarios();

            list.datainicio = DateTime.Now;
            list.datafinal = DateTime.Now;

            list.ArquivoBancario = DAL.ListarObjetos<TransacaoBancaria>(string.Format("id_conta={0} and dt_mvto between '{1}' and '{2}' ", UsuarioLogado.IdConta, list.datainicio.ToString("yyyy-MM-dd"), list.datafinal.ToString("yyyy-MM-dd")));

            ViewBag.QtdRegistros = list.ArquivoBancario.Count();
            ViewBag.TotalBruto = list.ArquivoBancario.Sum(x => x.vl_mvto);

            return View(list);
        }

        [HttpPost]
        public ActionResult RelatoriosExtratoBancarios(FormCollection frm)
        {
            DateTime datainicio = DateTime.ParseExact(frm["datainicio"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime datafinal = DateTime.ParseExact(frm["datafinal"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            var list = new ArquivosBancarios();
            list.datainicio = datainicio;
            list.datafinal = datafinal;

            list.ArquivoBancario = DAL.ListarObjetos<TransacaoBancaria>(string.Format("id_conta={0} and dt_mvto between '{1}' and '{2}' ", UsuarioLogado.IdConta, list.datainicio.ToString("yyyy-MM-dd"), list.datafinal.ToString("yyyy-MM-dd")),"","1,2,3,4,5,6,7");

            ViewBag.QtdRegistros = list.ArquivoBancario.Count();
            ViewBag.TotalBruto = list.ArquivoBancario.Sum(x => x.vl_mvto);

            return View(list);
        }

        [HttpPost]
        public ActionResult RelatorioFinanceiroAntecipacao(FormCollection frm)
        {
            var list = _restRelatorio.RelatorioFinanceiroAntecipacaoListar(UsuarioLogado.ContaChave, frm["datainicio"].Replace("/", ""), frm["datafinal"].Replace("/", ""));

            ViewBag.DataInicio = frm["datainicio"];
            ViewBag.DataFinal = frm["datafinal"];

            ViewBag.QtdRegistros = list.Count();
            ViewBag.TotalBruto = list.Sum(x => x.valor_lancamento);

            return View(list);
        }

        [HttpGet]
        public ActionResult RelatorioFinanceiroAjustesDesagendamento()
        {
            var list = new List<ConciliacaoUseRedeEEFIAjustesDesagendamentoStructListar>();
            ViewBag.DataInicio = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.DataFinal = DateTime.Now.ToString("dd/MM/yyyy");

            ViewBag.QtdRegistros = "0";
            ViewBag.TotalBruto = "0,00";

            return View(list);
        }

        [HttpPost]
        public ActionResult RelatorioFinanceiroAjustesDesagendamento(FormCollection frm)
        {
            var list = _restRelatorio.RelatorioFinanceiroAjustesDesagendamentoListar(UsuarioLogado.ContaChave, frm["datainicio"].Replace("/", ""), frm["datafinal"].Replace("/", ""));

            ViewBag.DataInicio = frm["datainicio"];
            ViewBag.DataFinal = frm["datafinal"];

            ViewBag.QtdRegistros = list.Count();
            ViewBag.TotalBruto = list.Sum(x => x.valor_transacao);

            return View(list);
        }

        [HttpGet]
        public ActionResult RelatorioFinanceiroDesagendamentoParcelas()
        {
            var list = new List<ConciliacaoUseRedeEEFIDesagendamentoParcelasStructListar>();
            ViewBag.DataInicio = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.DataFinal = DateTime.Now.ToString("dd/MM/yyyy");

            ViewBag.QtdRegistros = "0";
            ViewBag.TotalBruto = "0,00";

            return View(list);
        }

        [HttpPost]
        public ActionResult RelatorioFinanceiroDesagendamentoParcelas(FormCollection frm)
        {
            var list = _restRelatorio.RelatorioFinanceiroDesagendamentoParcelasListar(UsuarioLogado.ContaChave, frm["datainicio"].Replace("/", ""), frm["datafinal"].Replace("/", ""));

            ViewBag.DataInicio = frm["datainicio"];
            ViewBag.DataFinal = frm["datafinal"];

            ViewBag.QtdRegistros = list.Count();
            ViewBag.TotalBruto = list.Sum(x => x.valor_ajuste);

            return View(list);
        }



    }
}