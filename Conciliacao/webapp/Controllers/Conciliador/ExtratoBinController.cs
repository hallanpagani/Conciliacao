using Conciliacao.App_Helpers.Componentes;
using Conciliacao.Controllers.Generico;
using Conciliacao.Models;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.banco;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Conciliacao.Controllers.Conciliador
{
    [Authorize]
    public class ExtratoBinController : AppController
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


                            if ((first_line.ToUpper().Contains("FINANCEIRA")) && (first_line.ToUpper().Contains("FD DO BRASIL PROCESSAMENTO DE DADOS LTDA."))) //&& (arquivo.ProcessarArquivobanese(first_line))) //Opção de extrato - saldo em aberto
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

                                var bin = new ConciliacaoBinDesmontar(arquivo, first_line);

                                if (md5.arquivo_md5.Equals(""))
                                {
                                    md5.arquivo_md5 = lsretornomd5;

                                }
                                else
                                {
                                    md5_encontrado = true;
                                }

                                model.ArquivoResumo = bin.ROGet();
                                model.ArquivoTransacao = bin.CVArrayGet();
                                model.ArquivoDetalhado = bin.DetalhadoGet();
                                model.ArquivoCompleto = bin.CompletoGet();
                                model.ArquivoAntecipado = bin.ARGet();
                                if (!md5_encontrado)
                                {
                                    DAL.GravarList(bin.GetCreditos());

                                    // DAL.GravarList(banese.GetResumoArquivo());
                                    // DAL.GravarList(banese.GetAjustesAntecipacao());
                                    // DAL.GravarList(banese.GetAjustesDesagendamento());
                                    // DAL.GravarList(banese.GetDesagendamentoParcelas());

                                    DAL.GravarList(bin.GetResumoEEVDOperacao());
                                    DAL.GravarList(bin.GetComprovanteEEVDVenda());

                                    DAL.GravarList(bin.GetResumoEEVCArquivo());

                                    DAL.GravarList(bin.GetComprovanteEEVCCVenda());
                                    DAL.GravarList(bin.GetParcelas());

                                    DAL.GravarList(bin.TotalizadorProdutoGet());
                                    DAL.GravarList(bin.TotalizadorBancoGet());
                                    DAL.GravarList(bin.TotalizadorEstabelecimentoGet());

                                    //  DAL.GravarList(banese.RedeGet());

                                    DAL.Gravar(md5);

                                }
                                ViewBag.BotaoProcessar = "Buscar";
                            }
                            else if ((first_line.ToUpper().Contains("VENDAS")) && (first_line.ToUpper().Contains("FD DO BRASIL PROCESSAMENTO DE DADOS LTDA.")))
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

                                var bin = new ConciliacaoBinDesmontar(arquivo, first_line);

                                if (md5.arquivo_md5.Equals(""))
                                {
                                    md5.arquivo_md5 = lsretornomd5;

                                }
                                else
                                {
                                    md5_encontrado = true;
                                }

                                model.ArquivoResumo = bin.ROGet();
                                model.ArquivoTransacao = bin.CVArrayGet();
                                model.ArquivoDetalhado = bin.DetalhadoGet();
                                model.ArquivoCompleto = bin.CompletoGet();
                                model.ArquivoAntecipado = bin.ARGet();
                                if (!md5_encontrado)
                                {
                                    DAL.GravarList(bin.GetCreditos());

                                    // DAL.GravarList(banese.GetResumoArquivo());
                                    // DAL.GravarList(banese.GetAjustesAntecipacao());
                                    // DAL.GravarList(banese.GetAjustesDesagendamento());
                                    // DAL.GravarList(banese.GetDesagendamentoParcelas());

                                    DAL.GravarList(bin.GetResumoEEVDOperacao());
                                    DAL.GravarList(bin.GetComprovanteEEVDVenda());

                                    DAL.GravarList(bin.GetResumoEEVCArquivo());

                                    DAL.GravarList(bin.GetComprovanteEEVCCVenda());
                                    DAL.GravarList(bin.GetParcelas());

                                    DAL.GravarList(bin.TotalizadorProdutoGet());
                                    DAL.GravarList(bin.TotalizadorBancoGet());
                                    DAL.GravarList(bin.TotalizadorEstabelecimentoGet());

                                    //  DAL.GravarList(banese.RedeGet());

                                    DAL.Gravar(md5);
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                this.AddNotification(e.Message, "Alerta");
               // ModelState.AddModelError(e.Message, e.Message);
                return View(model);
                
            }

            return View(model);
        }
    }
}