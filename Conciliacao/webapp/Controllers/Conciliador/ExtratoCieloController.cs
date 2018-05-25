using System.Globalization;
using System.IO;
using Conciliacao.Models;
using Conciliacao.Models.conciliacao;
using ConciliacaoModelo.model.conciliador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.banco;

namespace Conciliacao.Controllers
{

    public class ExtratoCieloController : Controller
    {
        // GET: Conciliador
        [Authorize]
        public ActionResult Processar()
        {
            var model = new ArquivoDeCartoes
            {
                ArquivoResumo = new List<ArquivoResumo>(),
                ArquivoCompleto = new List<ArquivoCompleto>(),
                ArquivoDetalhado = new List<ArquivoDetalhado>(),
                ArquivoTransacao = new List<ArquivoTransacao>(),
                ArquivoAntecipado = new List<ArquivoAntecipado>(),
                datainicial = null,
                dataarquivo = null,
                datafinal = null,
                tp_arquivo = null,
                arquivo = null,
                ds_arquivo = null
            };

            ViewBag.BotaoProcessar = "Buscar";

            return View(model);
        }


        [HttpPost]
        public ActionResult Processar(ArquivoDeCartoes model)
        {
            try
            {
                model.ArquivoResumo = new List<ArquivoResumo>();
                model.ArquivoCompleto = new List<ArquivoCompleto>();
                model.ArquivoDetalhado = new List<ArquivoDetalhado>();
                model.ArquivoTransacao = new List<ArquivoTransacao>();
                model.ArquivoAntecipado = new List<ArquivoAntecipado>();
                int i = 0;

                if (Request.Files.Count > 0)
                {
                    foreach (string fileName in Request.Files)
                    {


                        HttpPostedFileBase file = Request.Files[i];
                        i = i + 1;


                        if (file != null && file.ContentLength > 0)
                        {
                            model.ds_arquivo = file.FileName;

                            var arquivo = new ConciliacaoArquivoManipular(new StreamReader(file.InputStream));

                            String first_line = arquivo.LerLinha(true);

                            
                            if ((first_line.ToUpper().Contains("CIELO")) && (arquivo.ProcessarArquivoCielo(first_line))) //Opção de extrato - saldo em aberto
                            {
                        
                                bool md5_encontrado = false;
                                string lsArquivomd5 = "";
                                string is_linha_atual = "";
                                while ((is_linha_atual = arquivo.LerLinha()) != null)
                                {
                                    lsArquivomd5 = lsArquivomd5 + is_linha_atual;
                                }

                                arquivo.Seek(0);
                                string lsretornomd5 = CriarMD5.RetornarMD5(lsArquivomd5);
                                var conta = new BaseID();
                                ArquivoMD5 md5 =
                                    DAL.GetObjeto<ArquivoMD5>(string.Format("arquivo_md5='{0}' and id_conta={1}",
                                        lsretornomd5, conta.IdConta)) ?? new ArquivoMD5();

                                var cielo = new ConciliacaoCieloDesmontar(arquivo, first_line);

                                if (md5.arquivo_md5.Equals(""))
                                {
                                    md5.arquivo_md5 = lsretornomd5;
                                  
                                }
                                else
                                {
                                    md5_encontrado = true;
                                }

                                model.ArquivoResumo = cielo.ROGet();
                                model.ArquivoTransacao = cielo.CVArrayGet();
                                model.ArquivoDetalhado = cielo.DetalhadoGet();
                                model.ArquivoCompleto = cielo.CompletoGet();
                                model.ArquivoAntecipado = cielo.ARGet();
                                if (!md5_encontrado)
                                {
                                    DAL.GravarList(cielo.GetCreditos());
                                    /* DAL.GravarList(cielo.GetResumoArquivo());
                                 DAL.GravarList(cielo.GetAjustesAntecipacao());
                                 DAL.GravarList(cielo.GetAjustesDesagendamento());
                                 DAL.GravarList(cielo.GetDesagendamentoParcelas());*/

                                    DAL.GravarList(cielo.GetResumoEEVDOperacao());
                                    DAL.GravarList(cielo.GetComprovanteEEVDVenda());

                                    DAL.GravarList(cielo.GetResumoEEVCArquivo());

                                    DAL.GravarList(cielo.GetComprovanteEEVCCVenda());
                                    DAL.GravarList(cielo.GetParcelas());

                                    DAL.GravarList(cielo.TotalizadorProdutoGet());
                                    DAL.GravarList(cielo.TotalizadorBancoGet());
                                    DAL.GravarList(cielo.TotalizadorEstabelecimentoGet());

                                    //  DAL.GravarList(cielo.RedeGet());

                                    DAL.Gravar(md5);

                                } else
                                {
                                    DAL.GravarList(cielo.GetCreditos());
                                }
                                ViewBag.BotaoProcessar = "Buscar";
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return View(model);
        }


        // GET: Conciliador
        [Authorize]
        public ActionResult Processar2()
        {
            var model = new ArquivoDeCartoes
            {
                ArquivoResumo = new List<ArquivoResumo>(),
                ArquivoCompleto = new List<ArquivoCompleto>(),
                ArquivoDetalhado = new List<ArquivoDetalhado>(),
                ArquivoTransacao = new List<ArquivoTransacao>(),
                ArquivoAntecipado = new List<ArquivoAntecipado>(),
                datainicial = null,
                dataarquivo = null,
                datafinal = null,
                tp_arquivo = null,
                arquivo = null,
                ds_arquivo = null
            };

            ViewBag.BotaoProcessar = "Buscar";

            return View(model);
        }


        [HttpPost]
        public JsonResult ProcessarArquivo()
        {
            ArquivoDeCartoes model = new ArquivoDeCartoes();
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

                        if (first_line.ToUpper().Contains("CIELO"))
                        {
                            var cielo = new ConciliacaoCieloDesmontar(arquivo, first_line);

                            model.ArquivoResumo = cielo.ROGet();
                            model.ArquivoTransacao = cielo.CVArrayGet();
                            model.ArquivoDetalhado = cielo.DetalhadoGet();
                            model.ArquivoCompleto = cielo.CompletoGet();
                            model.ArquivoAntecipado = cielo.ARGet();

                            DAL.GravarList(cielo.RedeGet());

                            // DAL.GravarList(cielo.TotalizadorProdutoGet());
                            // DAL.GravarList(cielo.TotalizadorBancoGet());
                            // DAL.GravarList(cielo.TotalizadorEstabelecimentoGet());

                            ViewBag.BotaoProcessar = "Buscar";

                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }

            }
            var result = Json(model, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

    }
}