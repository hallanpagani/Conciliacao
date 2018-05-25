using Conciliacao.App_Helpers.Componentes;
using Conciliacao.Models;
using Conciliacao.Models.UseRede;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoPersistencia.banco;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using ConciliacaoPersistencia.model;
using ConciliacaoModelo.model.conciliador.UseRede;
using Conciliacao.Controllers.Generico;
using ConciliacaoModelo.model.generico;
using System.Web;

namespace Conciliacao.Controllers.Conciliador
{
    [Authorize]
    public class ExtratoUserRedeController : AppController
    {
        // GET: ExtratoUserRede
        public ActionResult Index()
        {
            var model = new ArquivoDeCartoesUserRede
            {
                datainicial = null,
                dataarquivo = null,
                datafinal = null,
                tp_arquivo = null,
                arquivo = null,
                ds_arquivo = null,
                ArquivosDeCartoesEEVDUserRede = new List<ConciliacaoUseRedeEEVDResumoOperacaoStruct>(),
                ArquivosDeCartoesEEVCUserRede = new List<ConciliacaoUseRedeEEVCResumoOperacaoStruct>(),
                ArquivosDeCartoesEEFIUserRede = new List<ConciliacaoUseRedeEEFIResumoOperacaoStruct>()
            };

            ViewBag.BotaoProcessar = "Buscar";

            return View(model);
        }

        [HttpPost]
        public JsonResult ProcessarArquivo()
        {
            var httpRequest = System.Web.HttpContext.Current.Request;
            HttpFileCollection uploadFiles = httpRequest.Files;
            var docfiles = new List<string>();


            var model = new ArquivoDeCartoesUserRede();
            if (httpRequest.Files.Count > 0)
            {
                foreach (string fileName in httpRequest.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];

                    if (file != null && file.ContentLength > 0)
                    {
                        try
                        {
                            model.ds_arquivo = file.FileName;

                            var arquivo = new ConciliacaoArquivoManipular(new StreamReader(file.InputStream));
                            String first_line = arquivo.LerLinha(true);
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
                            ArquivoMD5 md5 = DAL.GetObjeto<ArquivoMD5>(string.Format("arquivo_md5='{0}' and id_conta={1}", lsretornomd5, conta.IdConta)) ?? new ArquivoMD5();
                            if (md5.arquivo_md5.Equals(""))
                            {
                                md5.arquivo_md5 = lsretornomd5;
                            }
                            else
                            {
                                md5_encontrado = true;
                            }

                            if (first_line.ToUpper().Contains("EEVD"))
                            {
                                var userrede = new ConciliacaoUseRedeEEVDDesmontar(arquivo, first_line);
                                model.ArquivosDeCartoesEEVDUserRede = userrede.GetResumoOperacao();


                                if (!md5_encontrado)
                                {
                                    DAL.GravarList(model.ArquivosDeCartoesEEVDUserRede);
                                    DAL.GravarList(userrede.GetComprovanteVenda());
                                    DAL.GravarList(userrede.GetPontoVenda());
                                    DAL.GravarList(userrede.GetTotalArquivo());
                                    DAL.GravarList(userrede.GetTotalMatriz());

                                    DAL.GravarList(userrede.TotalizadorBancoGet());
                                    DAL.GravarList(userrede.TotalizadorProdutoGet());
                                    DAL.GravarList(userrede.TotalizadorEstabelecimentoGet());

                                    DAL.Gravar(md5);
                                }

                            }
                            else if (first_line.ToUpper().Contains("EEVC"))
                            {
                                var userrede = new ConciliacaoUseRedeEEVCDesmontar(arquivo, first_line);
                                model.ArquivosDeCartoesEEVCUserRede = userrede.GetResumoArquivo();
                                if (!md5_encontrado)
                                {
                                    DAL.GravarList(userrede.TotalizadorBancoGet());
                                    DAL.GravarList(userrede.TotalizadorProdutoGet());
                                    DAL.GravarList(userrede.TotalizadorEstabelecimentoGet());

                                    
                                    DAL.GravarList(userrede.GetComprovanteVenda());
                                    DAL.GravarList(model.ArquivosDeCartoesEEVCUserRede);
                                    DAL.GravarList(userrede.GetParcelas());

                                    DAL.Gravar(md5);
                                }
                            }
                            else if (first_line.ToUpper().Contains("EEFI"))
                            {
                                var userrede = new ConciliacaoUseRedeEEFIDesmontar(arquivo, first_line);
                                model.ArquivosDeCartoesEEFIUserRede = userrede.GetResumoArquivo();
                                if (!md5_encontrado)
                                {
                                    DAL.GravarList(userrede.GetCreditos());
                                    DAL.GravarList(userrede.GetAjustesAntecipacao());
                                    DAL.GravarList(userrede.GetAjustesDesagendamento());
                                    DAL.GravarList(userrede.GetDesagendamentoParcelas());
                                    DAL.GravarList(userrede.GetTotalMatriz());
                                    DAL.GravarList(userrede.GetTotalCredito());

                                    DAL.Gravar(md5);
                                }
                            }


                            //  DAL.GravarList(model.ArquivosDeCartoesUserRede);

                            ViewBag.BotaoProcessar = "Gravar";

                        }
                        catch (Exception ex)
                        {
                            this.AddNotification(ex.Message + " " + ex.StackTrace, NotificationType.Erro);
                        }

                    }
                }

            }
            var result = Json(model, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        [HttpPost]
        public JsonResult ProcessarDetalhesEEVD(string numeroresumovenda)
        {
            var model = DAL.ListarObjetos<ConciliacaoUseRedeEEVDComprovanteVendaStructListar>(string.Format("numero_resumo_venda='{0}'", numeroresumovenda));
            var result = Json(model, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

    }
}