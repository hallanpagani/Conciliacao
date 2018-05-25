using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Conciliacao.App_Helpers.Componentes;
using Conciliacao.Models;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoPersistencia.banco;
using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.model;
using Conciliacao.Controllers.Generico;

namespace Conciliacao.Controllers.Conciliador
{
    [Authorize]
    public class ExtratoEstabelecimentoController : AppController
    {
        // GET: ExtatoEstabelecimento
        public ActionResult Index()
        {
            var model = new ArquivoDeCartoesEstabelecimento
            {
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
            var model = new ArquivosEstabelecimentos();
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

                        

                        bool md5_encontrado = false;
                        string lsArquivomd5 = "";
                        string is_linha_atual = "";
                        var i = 0;
                        while ((is_linha_atual = arquivo.LerLinha()) != null)
                        {
                            i++;
                            if (i > 500) {
                                break;
                            }
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

                        var bancario = new ConciliacaoEstabelecimentoDesmontar(arquivo, first_line);

                        model.ArquivoEstabelecimento = bancario.GetListTransacaoEstabelecimento();
                        if (!md5_encontrado)
                        {
                            DAL.GravarList(model.ArquivoEstabelecimento);
                            EstabelecimentoDAL.ExcluirDuplicados(Convert.ToInt32(UsuarioLogado.IdConta));
                            DAL.Gravar(md5);
                        }
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