using Conciliacao.App_Helpers.Componentes;
using Conciliacao.Models;
using Conciliacao.Models.UseRede;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoPersistencia.banco;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Conciliacao.Controllers.Generico;
using ConciliacaoModelo.model.generico;
using System.Web;

namespace Conciliacao.Controllers.Conciliador
{
    [Authorize]
    public class ExtratoBaneseController : AppController
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


                            if (first_line.ToUpper().Contains("BANESE")) //&& (arquivo.ProcessarArquivobanese(first_line))) //Opção de extrato - saldo em aberto
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

                                var banese = new ConciliacaoBaneseDesmontar(arquivo, first_line);

                                if (md5.arquivo_md5.Equals(""))
                                {
                                    md5.arquivo_md5 = lsretornomd5;

                                }
                                else
                                {
                                    md5_encontrado = true;
                                }

                                model.ArquivoResumo = banese.ROGet();
                                model.ArquivoTransacao = banese.CVArrayGet();
                                model.ArquivoDetalhado = banese.DetalhadoGet();
                                model.ArquivoCompleto = banese.CompletoGet();
                                model.ArquivoAntecipado = banese.ARGet();
                                if (!md5_encontrado)
                                {
                                    DAL.GravarList(banese.GetCreditos());

                                    // DAL.GravarList(banese.GetResumoArquivo());
                                    // DAL.GravarList(banese.GetAjustesAntecipacao());
                                    // DAL.GravarList(banese.GetAjustesDesagendamento());
                                    // DAL.GravarList(banese.GetDesagendamentoParcelas());

                                    DAL.GravarList(banese.GetResumoEEVDOperacao());
                                    DAL.GravarList(banese.GetComprovanteEEVDVenda());

                                    DAL.GravarList(banese.GetResumoEEVCArquivo());

                                    DAL.GravarList(banese.GetComprovanteEEVCCVenda());
                                    DAL.GravarList(banese.GetParcelas());

                                    DAL.GravarList(banese.TotalizadorProdutoGet());
                                    DAL.GravarList(banese.TotalizadorBancoGet());
                                    DAL.GravarList(banese.TotalizadorEstabelecimentoGet());

                                    //  DAL.GravarList(banese.RedeGet());

                                    DAL.Gravar(md5);

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
    }
}