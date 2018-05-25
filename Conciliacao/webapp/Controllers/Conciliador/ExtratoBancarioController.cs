using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Conciliacao.Models;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoPersistencia.banco;
using Conciliacao.Controllers.Generico;
using Conciliacao.App_Helpers.Componentes;
using System.Text;
using OFXSharp;
using System.Collections.Generic;
using ConciliacaoModelo.model.generico;

namespace Conciliacao.Controllers.Conciliador
{
    public class ExtratoBancarioController : AppController
    {
        // GET: ExtratoBancario
        public ActionResult Index()
        {
            var model = new ArquivosBancarios {arquivo = null, ds_arquivo = null};
            /*model.ArquivoResumo = new List<ArquivoResumo>();
            model.ArquivoCompleto = new List<ArquivoCompleto>();
            model.ArquivoDetalhado = new List<ArquivoDetalhado>();
            model.ArquivoTransacao = new List<ArquivoTransacao>();
            model.ArquivoAntecipado = new List<ArquivoAntecipado>();*/

            ViewBag.BotaoProcessar = "Buscar";

            return View(model);
        }

        [HttpPost]
        public JsonResult ProcessarArquivo()
        {
            var model = new ArquivosBancarios();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];


                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        model.ds_arquivo = file.FileName;

                        string lsArquivomd5 = file.FileName;
                        string lsretornomd5 = CriarMD5.RetornarMD5(lsArquivomd5);
                        var conta = new BaseID();
                        ArquivoMD5 md5 =
                            DAL.GetObjeto<ArquivoMD5>(string.Format("arquivo_md5='{0}' and id_conta={1}",
                                lsretornomd5, conta.IdConta)) ?? new ArquivoMD5();

                        if (Path.GetExtension(file.FileName).ToLower().Equals(".ofx"))
                        {
                            var parser = new OFXDocumentParser();
                            var ofxDocument = parser.Import(file.InputStream);

                            var list = ofxDocument.Transactions.Select(item => new TransacaoBancaria
                            {
                                conta = ofxDocument.Account.AccountID, 
                                ds_historico = item.Memo, 
                                nr_doc = item.TransactionID, 
                                vl_mvto = item.Amount, 
                                tp_mvto = item.TransType.ToString(), 
                                dt_mvto = item.Date
                            }
                            ).Where(c => c.tp_mvto.Substring(0,1) == "C").ToList();

                            model.ArquivoBancario = list;
                            model.datainicio = ofxDocument.StatementStart;
                            model.datafinal = ofxDocument.StatementEnd;
                           
                        } 
                        else
                        {

                            var arquivo = new ConciliacaoArquivoManipular(new StreamReader(file.InputStream));
                            String first_line = arquivo.LerLinha(true);

                            var bancario = new ConciliacaoBancariaDesmontar(arquivo, first_line);
                            model.ArquivoBancario = bancario.GetListTransacaoBancaria();
                        }

                      //  if (md5.arquivo_md5.Equals(""))
                      //  {
                            md5.arquivo_md5 = lsretornomd5;
                            DAL.GravarList(model.ArquivoBancario);
                          //  DAL.Gravar(md5);
                       // }


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
    }
}