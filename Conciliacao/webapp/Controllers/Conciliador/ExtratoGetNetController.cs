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
    public class ExtratoGetNetController : AppController
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


                            if (first_line.ToUpper().Contains("GETNET")) //&& (arquivo.ProcessarArquivobanese(first_line))) //Opção de extrato - saldo em aberto
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

                                var getnet = new ConciliacaoGetNetDesmontar(arquivo, first_line);

                                if (md5.arquivo_md5.Equals(""))
                                {
                                    md5.arquivo_md5 = lsretornomd5;

                                }
                                else
                                {
                                    md5_encontrado = true;
                                }

                                model.ArquivoResumo = getnet.ROGet();
                                model.ArquivoTransacao = getnet.CVArrayGet();
                                model.ArquivoDetalhado = getnet.DetalhadoGet();
                                model.ArquivoCompleto = getnet.CompletoGet();
                                model.ArquivoAntecipado = getnet.ARGet();
                                if (!md5_encontrado)
                                {
                                    DAL.GravarList(getnet.GetCreditos());

                                    DAL.GravarList(getnet.GetResumoEEVDOperacao());
                                    DAL.GravarList(getnet.GetComprovanteEEVDVenda());

                                    DAL.GravarList(getnet.GetResumoEEVCArquivo());

                                    DAL.GravarList(getnet.GetComprovanteEEVCCVenda());
                                    DAL.GravarList(getnet.GetParcelas());

                                    DAL.GravarList(getnet.TotalizadorProdutoGet());
                                    DAL.GravarList(getnet.TotalizadorBancoGet());
                                    DAL.GravarList(getnet.TotalizadorEstabelecimentoGet());

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