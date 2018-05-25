using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Conciliacao.Controllers.Generico;
using Conciliacao.Helper.Rest;
using Conciliacao.Models.Relatorios;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoModelo.model.conciliador.UseRede;
using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.banco;
using ConciliacaoPersistencia.model;
using ConciliacaoModelo.model.adm;
using System.Web;
using Conciliacao.Models;

namespace Conciliacao.Controllers.Exportar
{
    public class ExportarController : AppController
    {

        public class rvValor
        {
            public int RV;
            public decimal valorbruto;
            public decimal valorliquido;
        }


        static readonly RelatoriosRestClient _restRelatorio = new RelatoriosRestClient();
        static readonly TEFRestClient _restTEF = new TEFRestClient();

        string _arquivo_erros = "";
        string _file_erros = "";

        public ActionResult MovimentacoesVenda(string data, string debitocredito)
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
                resumo = ""
            };

            ViewBag.Disabled = "disabled";
            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            return View(listar);
        }

        public ActionResult MovimentacoesVendaResumo(string data, string debitocredito)
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
                resumo = ""
            };

            ViewBag.Disabled = "disabled";
            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            return View(listar);
        }

        
        public ActionResult MovimentacoesVendaCriticas(TransacaoRedeViewModel model)
        {
            /* if (Cookie.Exists("arquivoerrovenda"))
             {
                 var arquivoerro = Cookie.Get("arquivoerrovenda");
                 var dir = Server.MapPath("~\\TXT");
                 Response.Clear();
                 Response.ContentType = "text/plain";
                 Response.AppendHeader("Content-Disposition", String.Format("attachment;filename={0}", arquivoerro));
                 Response.TransmitFile(Path.Combine(dir, arquivoerro));
                 Response.End();
             }*/

            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            ViewBag.tp_cartao = Models.CartaoTypes.getCartaoTypes(model.tp_cartao);
            ViewBag.tp_banco = Models.BancoTypes.getBancoTypes(model.tp_banco);
            ViewBag.tp_movimento = Models.MovimentacaoTypes.getMovimentacaoTypes(model.tp_movimento);

            model.ArquivosCriticas = DAL.ListarObjetos<ArquivoDeCriticas>(string.Format("ds_arquivo='{0}'",
                Cookie.Get("arquivoerrovenda"))) ?? new List<ArquivoDeCriticas>();

            /*  var dir = Server.MapPath("~\\TXT");
              Response.Clear();
              Response.ContentType = "text/plain";
              Response.AppendHeader("Content-Disposition", String.Format("attachment;filename={0}", "MOVIMENT_RET_" + DateTime.Now.ToString("yyyyMMdd") + "_PAG_CRITICA.txt"));
              Response.TransmitFile(Path.Combine(dir, "MOVIMENT_" + DateTime.Now.ToString("yyyyMMdd") + "_PAG_CRITICA.txt"));
              Response.End();*/

            return View("MovimentacoesVenda", model);

        }


        [HttpPost]
        public void MovimentacoesVenda(TransacaoRedeViewModel model)
        {
            if (model.filtro_rede.GetValueOrDefault(0) == 0)
            {
                model.filtro_nm_rede = "";
            }

            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            ViewBag.tp_cartao = Models.CartaoTypes.getCartaoTypes();

            var listarrede = new TransacaoRedeViewModel
            {
                filtro_rede = model.filtro_rede,
                filtro_nm_rede = model.filtro_nm_rede,
                tp_situacao = model.tp_situacao,
                TransacaoArquivos = _restRelatorio.TransacaoRedeListar(0, 3 /* model.tp_cartao*/ , 0, model.DataInicio ?? DateTime.MinValue, model.DataFinal ?? DateTime.MaxValue, model.resumo, "0") ?? new List<ConciliacaoTransacaoRede>(),
                DataInicio = model.DataInicio ?? DateTime.MinValue,
                DataFinal = model.DataFinal ?? DateTime.MaxValue,
                tp_data = model.tp_data,
                tp_cartao = model.tp_cartao,
                resumo = model.resumo
            };

            string filtro_cartao = "and produto" + (model.tp_cartao == 2 ? "='D'" : model.tp_cartao == 1 ? "='C'" : "<>'X'");
            string filtro_rede = model.filtro_nm_rede.Trim().Equals("") ? "" : string.Format("and upper(operadora_desc) = '{0}' ", model.filtro_nm_rede.Trim().ToUpper());
            string filtro_conta = string.Format("and id_conta = '{0}' ", UsuarioLogado.IdConta );

            var listar = DAL.ListarObjetos<TransacaoEstabelecimentoListar>(string.Format("dt_transacao between '{0}' and '{1}' {2} {3} {4}", (model.DataInicio ?? DateTime.MinValue).ToString("yyyy-MM-dd"), (model.DataFinal ?? DateTime.MaxValue).ToString("yyyy-MM-dd"), filtro_cartao, filtro_rede, filtro_conta));

            if (!listar.Any()) return;

            var dir = Server.MapPath("~\\TXT");
            var arquivo = "MOVIMENT_RET_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";

            var file = Path.Combine(dir, arquivo);
            //var fileerros = Path.Combine(dir, arquivoerro);

            Directory.CreateDirectory(dir);

            Cookie.Delete("arquivoerrovenda");
            Cookie.Set("arquivoerrovenda", arquivo);
            List<ArquivoDeCriticas> ListCriticas = new List<ArquivoDeCriticas>();

            int i = 0;
            StringBuilder l = new StringBuilder();
            StringBuilder l_erros = new StringBuilder();

            using (StreamWriter sw = System.IO.File.CreateText(file))
            {
                //  StreamWriter swerros = System.IO.File.CreateText(fileerros);

                //  swerros.WriteLine("Data da venda;Data de credito;NSU;Autorizacao;Plano;Rede;Bandeira;Produto;Valor venda;Taxa administracao;Numero do lote;Area cliente;Banco;Agencia;Conta;Codigo loja;Loja;Codigo do Estabelecimento;Numero cartao;Valor liquido;Valor Bruto;Modo Captura;Status;Reservado");
                sw.WriteLine("Data da venda;Data de credito;NSU;Autorizacao;Plano;Rede;Bandeira;Produto;Valor venda;Taxa administracao;Numero do lote;Area cliente;Banco;Agencia;Conta;Codigo loja;Loja;Codigo do Estabelecimento;Numero cartao;Valor liquido;Valor Bruto;Modo Captura;Status;Reservado");

               // var conta = new BaseID();
                foreach (var items in listar)
                {
                    //var a = DAL.ListarFromSQL(string.Format("select * from cadastro_estabelecimento_rede id_estabelecimento_rede ={0}",));
                    i++;
                    var redes = listarrede.TransacaoArquivos.FirstOrDefault(x => x.is_numero_cv_long == Convert.ToInt64(items.nsu_rede)) ?? new ConciliacaoTransacaoRede();
                    var redes_c =
                        listarrede.TransacaoArquivos.Where(
                            x => x.is_numero_resumo_vendas_long == Convert.ToInt64(redes.is_numero_resumo_vendas)) ??
                        new List<ConciliacaoTransacaoRede>();

                    var rtef = TEFDAL.GetStatusTEF(Convert.ToInt32(UsuarioLogado.IdConta), Convert.ToInt64(items.nsu_rede).ToString()) ?? new RetornoTEF();// TEFDAL.GetStatusTEF(conta.IdConta, Convert.ToInt64(items.nsu_rede).ToString());
                    var bandeira = !(items.bandeira_desc ?? string.Empty).Equals("") ? items.bandeira_desc.ToUpper().Trim() : !(rtef.Bandeira ?? string.Empty).ToUpper().Trim().Equals("") ? (rtef.Bandeira ?? string.Empty).ToUpper().Trim() : (redes.is_bandeira ?? string.Empty).ToUpper();

                    if (redes.is_data_credito.ToString("yyyyMMdd").Equals("00010101"))
                    {
                        var arq = new ArquivoDeCriticas();
                        arq.ds_motivo = "Registro não encontrado nos arquivos da Rede!";
                        arq.ds_arquivo = arquivo;
                        arq.dt_credito = redes.is_data_credito;
                        arq.dt_transacao = items.dt_transacao;
                        arq.numero_resumo_venda = redes.is_numero_resumo_vendas_long.ToString();
                        arq.nsu_rede = items.nsu_rede_long.ToString();
                        arq.vl_bruto = items.vl_bruto;
                        arq.vl_liquido = redes.is_valor_liquido;
                        arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                        ListCriticas.Add(arq);

                        /* l_erros.AppendFormat("{0};", items.dt_transacao.ToString("yyyyMMdd"));
                         l_erros.AppendFormat("{0};", redes.is_data_credito.ToString("yyyyMMdd"));
                         l_erros.AppendFormat("{0};", items.nsu_rede_long);
                         l_erros.AppendFormat("{0};", (!items.is_autorizacao.Equals("") ? items.is_autorizacao : (rtef.Autorizacao ?? string.Empty).Equals("") ? "" : (rtef.Autorizacao ?? string.Empty))); // Autorizacao
                         l_erros.AppendFormat("{0};", items.tot_parcela); // Plano
                         l_erros.AppendFormat("{0};", items.operadora_desc.ToUpper());
                         l_erros.AppendFormat("{0};", bandeira);
                         l_erros.AppendFormat("{0};", items.produto.Substring(0, 1).ToUpper());
                         l_erros.AppendFormat("{0};", items.vl_bruto.ToString().Replace(",", "").Replace(".", ""));
                         l_erros.AppendFormat("{0};", redes.taxa_cobrada.ToString().Replace(",", "").Replace(".", ""));
                         l_erros.AppendFormat("{0:D20};", redes.is_numero_resumo_vendas_long);
                         l_erros.AppendFormat("{0};", items.area_cliente);
                         l_erros.AppendFormat("{0};", "0");
                         l_erros.AppendFormat("{0};", "0");
                         l_erros.AppendFormat("{0};", "0");
                         l_erros.AppendFormat("{0};", items.cod_loja); // CODIGO DA LOJA
                         l_erros.AppendFormat("{0};", items.nm_estabelecimento.Trim()); // LOJA
                         l_erros.AppendFormat("{0};", (redes.is_numero_filiacao_pv ?? string.Empty).Trim()); // Estabelecimento
                         l_erros.AppendFormat("{0};", (redes.numero_cartao ?? string.Empty).Trim().Replace("x", "*").Replace("X", "*"));
                         l_erros.AppendFormat("{0};", redes.is_valor_liquido.ToString().Replace(",", "").Replace(".", ""));
                         l_erros.AppendFormat("{0};", items.vl_bruto.ToString().Replace(",", "").Replace(".", ""));
                         l_erros.AppendFormat("{0};", (redes.tipo_captura ?? string.Empty).Equals("PVD") || (redes.tipo_captura ?? string.Empty).Equals("PDV") ? "TEF/PDV" : redes.tipo_captura ?? string.Empty);
                         l_erros.AppendFormat("{0};", items.cod_loja); // CODIGO DA LOJA
                         l_erros.AppendFormat("{0};", (redes.is_status_transacao ?? string.Empty).ToUpper().Equals("APROVADO") ? "2" : (redes.is_status_transacao ?? string.Empty).ToUpper().Trim().Equals("CV/NSU OK") ? "02" : (redes.is_status_transacao ?? string.Empty));
                         //  l.AppendFormat("{0}", string.Empty); // Reservado
                         //  l.AppendFormat("{0};", Environment.NewLine); // Reservado
                         swerros.WriteLine(l_erros.ToString());
                         l_erros.Clear();*/
                      //  continue;
                    }
                    else if (bandeira.Trim().Equals(""))
                    {
                        var arq = new ArquivoDeCriticas();
                        arq.ds_motivo = "Bandeira do cartão não informada!";
                        arq.ds_arquivo = arquivo;
                        arq.dt_credito = redes.is_data_credito;
                        arq.dt_transacao = items.dt_transacao;
                        arq.numero_resumo_venda = redes.is_numero_resumo_vendas_long.ToString();
                        arq.nsu_rede = items.nsu_rede_long.ToString();
                        arq.vl_bruto = items.vl_bruto;
                        arq.vl_liquido = redes.is_valor_liquido;
                        arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                        ListCriticas.Add(arq);
                       // continue;
                    }
                    else if (
                        (!items.is_autorizacao.Equals("")
                            ? items.is_autorizacao
                            : (rtef.Autorizacao ?? string.Empty).Equals("")
                                ? ""
                                : (rtef.Autorizacao ?? string.Empty)).Equals(""))
                    {
                        var arq = new ArquivoDeCriticas();
                        arq.ds_motivo = "Campo autorização não informado!";
                        arq.ds_arquivo = arquivo;
                        arq.dt_credito = redes.is_data_credito;
                        arq.dt_transacao = items.dt_transacao;
                        arq.numero_resumo_venda = redes.is_numero_resumo_vendas_long.ToString();
                        arq.nsu_rede = items.nsu_rede_long.ToString();
                        arq.vl_bruto = items.vl_bruto;
                        arq.vl_liquido = redes.is_valor_liquido;
                        arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                        ListCriticas.Add(arq);
                       // continue;
                    }



                    l.AppendFormat("{0};", items.dt_transacao.ToString("yyyyMMdd"));
                    l.AppendFormat("{0};", redes.is_data_credito.ToString("yyyyMMdd"));
                    l.AppendFormat("{0};", items.nsu_rede_long);
                    l.AppendFormat("{0};", (!items.is_autorizacao.Equals("") ? items.is_autorizacao : (rtef.Autorizacao ?? string.Empty).Equals("") ? "" : (rtef.Autorizacao ?? string.Empty))); // Autorizacao
                    l.AppendFormat("{0};", items.tot_parcela); // Plano
                    l.AppendFormat("{0};", items.operadora_desc.ToUpper());
                    l.AppendFormat("{0};", bandeira);
                    l.AppendFormat("{0};", items.produto.Substring(0, 1).ToUpper());
                    l.AppendFormat("{0};", items.vl_bruto.ToString().Replace(",", "").Replace(".", ""));
                    l.AppendFormat("{0};", redes.taxa_cobrada.ToString().Replace(",", "").Replace(".", ""));
                    l.AppendFormat("{0:D20};", redes.is_numero_resumo_vendas_long);
                    l.AppendFormat("{0};", items.area_cliente);
                    l.AppendFormat("{0};", (redes_c.FirstOrDefault(x => x.is_tipo_registro == "RESUMO") ?? new ConciliacaoTransacaoRede() ).banco);
                    l.AppendFormat("{0};", (redes_c.FirstOrDefault(x => x.is_tipo_registro == "RESUMO") ?? new ConciliacaoTransacaoRede() ).agencia);
                    l.AppendFormat("{0};", (redes_c.FirstOrDefault(x => x.is_tipo_registro == "RESUMO") ?? new ConciliacaoTransacaoRede() ).conta_corrente);
                    l.AppendFormat("{0};", items.cod_loja); // CODIGO DA LOJA
                    l.AppendFormat("{0};", items.nm_estabelecimento.Trim()); // LOJA
                    l.AppendFormat("{0};", redes.is_numero_filiacao_pv); // Estabelecimento
                    l.AppendFormat("{0};", (redes.numero_cartao ?? string.Empty).Trim().Replace("x", "*").Replace("X", "*"));
                    l.AppendFormat("{0};", redes.is_valor_liquido.ToString().Replace(",", "").Replace(".", ""));
                    l.AppendFormat("{0};", items.vl_bruto.ToString().Replace(",", "").Replace(".", ""));
                    l.AppendFormat("{0};", (redes.tipo_captura ?? string.Empty).Equals("PVD") || (redes.tipo_captura ?? string.Empty).Equals("PDV") ? "TEF/PDV" : redes.tipo_captura ?? string.Empty);
                    l.AppendFormat("{0};", items.cod_loja); // CODIGO DA LOJA
                    l.AppendFormat("{0};", (redes.is_status_transacao ?? string.Empty).ToUpper().Equals("APROVADO") ? "2" : (redes.is_status_transacao ?? string.Empty).ToUpper().Trim().Equals("CV/NSU OK") ? "02" : (redes.is_status_transacao ?? string.Empty));
                    //  l.AppendFormat("{0}", string.Empty); // Reservado
                    //  l.AppendFormat("{0};", Environment.NewLine); // Reservado
                    sw.WriteLine(l.ToString());
                    l.Clear();
                }
                //  swerros.Dispose();
            }

            DAL.GravarList(ListCriticas);
            Response.Clear();
            Response.ContentType = "text/plain";
            Response.AppendHeader("Content-Disposition", String.Format("attachment;filename={0}", arquivo));
            Response.TransmitFile(file);
            Response.End();
        }

        [HttpPost]
        public ActionResult MovimentacoesVendaResumoCriticas(TransacaoRedeViewModel model)
        {
            /* if (Cookie.Exists("arquivoerrovendaresumo"))
             {

                 var arquivoerro = Cookie.Get("arquivoerrovendaresumo");
                 var dir = Server.MapPath("~\\TXT");
                 Response.Clear();
                 Response.ContentType = "text/plain";
                 Response.AppendHeader("Content-Disposition", String.Format("attachment;filename={0}", arquivoerro));
                 Response.TransmitFile(Path.Combine(dir, arquivoerro));
                 Response.End();
             }*/

            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            ViewBag.tp_cartao = Models.CartaoTypes.getCartaoTypes(model.tp_cartao);
            ViewBag.tp_banco = Models.BancoTypes.getBancoTypes(model.tp_banco);
            ViewBag.tp_movimento = Models.MovimentacaoTypes.getMovimentacaoTypes(model.tp_movimento);

            model.ArquivosCriticas = DAL.ListarObjetos<ArquivoDeCriticas>(string.Format("ds_arquivo='{0}'",
                Cookie.Get("arquivoerrovendaresumo"))) ?? new List<ArquivoDeCriticas>();

            /*  var dir = Server.MapPath("~\\TXT");
              Response.Clear();
              Response.ContentType = "text/plain";
              Response.AppendHeader("Content-Disposition", String.Format("attachment;filename={0}", "MOVIMENT_RET_" + DateTime.Now.ToString("yyyyMMdd") + "_PAG_CRITICA.txt"));
              Response.TransmitFile(Path.Combine(dir, "MOVIMENT_" + DateTime.Now.ToString("yyyyMMdd") + "_PAG_CRITICA.txt"));
              Response.End();*/

            return View("MovimentacoesVendaResumo", model);

        }

        [HttpPost]
        public void MovimentacoesVendaResumo(TransacaoRedeViewModel model)
        {
            if (model.filtro_rede.GetValueOrDefault(0) == 0)
            {
                model.filtro_nm_rede = "";
            }

            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            ViewBag.tp_cartao = Models.CartaoTypes.getCartaoTypes();

            var listarrede = new TransacaoRedeViewModel
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

            string filtro_cartao = "and produto" + (model.tp_cartao == 2 ? "='D'" : model.tp_cartao == 1 ? "='C'" : "<>'X'");
            string filtro_rede = model.filtro_nm_rede.Trim().Equals("") ? "" : string.Format("and upper(operadora_desc) = '{0}' ", model.filtro_nm_rede.Trim().ToUpper());
            var listar = DAL.ListarObjetos<TransacaoEstabelecimento>(string.Format("dt_transacao between '{0}' and '{1}' {2} {3}", (model.DataInicio ?? DateTime.MinValue).ToString("yyyy-MM-dd"), (model.DataFinal ?? DateTime.MaxValue).ToString("yyyy-MM-dd"), filtro_cartao, filtro_rede));

            if (!listar.Any()) return;

            var dir = Server.MapPath("~\\TXT");
            var arquivo = "MOVIMENT_RET_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_RESUMO.txt";
            //var arquivoerro = "MOVIMENT_RET_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_RESUMO_CRITICA.txt";

            var file = Path.Combine(dir, arquivo);

            Cookie.Delete("arquivoerrovendaresumo");
            Cookie.Set("arquivoerrovendaresumo", arquivo);
            List<ArquivoDeCriticas> ListCriticas = new List<ArquivoDeCriticas>();

            Directory.CreateDirectory(dir);

            StringBuilder l = new StringBuilder();
            StringBuilder l_erros = new StringBuilder();

            using (StreamWriter sw = System.IO.File.CreateText(file))
            {
                // StreamWriter swerros = System.IO.File.CreateText(Path.Combine(dir, arquivoerro));

                l.AppendFormat("{0}", 0);
                l.AppendFormat("{0:D3}", "005");
                l.AppendFormat("{0}", DateTime.Now.ToString("yyyyMMdd"));
                l.AppendFormat("{0}", "".PadLeft(16));
                l.AppendFormat("{0}", (model.DataInicio ?? DateTime.MinValue).ToString("yyyyMMdd")); // Autorizacao
                l.AppendFormat("{0}", (model.DataFinal ?? DateTime.MinValue).ToString("yyyyMMdd")); // Autorizacao
                l.AppendFormat("{0}", "ALLPAYMENTS".PadRight(13));
                l.AppendFormat("{0}", "LOGIN".PadRight(13));
                l.AppendFormat("{0:D6}", 1);
                l.AppendFormat("{0}", " ".PadRight(164));
                sw.WriteLine(l.ToString());
                //swerros.WriteLine(l.ToString());
                l.Clear();


                // swerros.WriteLine("Data da venda;Data de credito;NSU;Autorizacao;Plano;Rede;Bandeira;Produto;Valor venda;Taxa administracao;Numero do lote;Area cliente;Banco;Agencia;Conta;Codigo loja;Loja;Codigo do Estabelecimento;Numero cartao;Valor liquido;Valor Bruto;Modo Captura;Status;Reservado");
                // sw.WriteLine("Data da venda;Data de credito;NSU;Autorizacao;Plano;Rede;Bandeira;Produto;Valor venda;Taxa administracao;Numero do lote;Area cliente;Banco;Agencia;Conta;Codigo loja;Loja;Codigo do Estabelecimento;Numero cartao;Valor liquido;Valor Bruto;Modo Captura;Status;Reservado");
                var contador = 1;
                decimal totBruto = 0;
                decimal totLiquido = 0;
                var conta = new BaseID();
                foreach (var items in listar)
                {
                    //var a = DAL.ListarFromSQL(string.Format("select * from cadastro_estabelecimento_rede id_estabelecimento_rede ={0}",));

                    var redes = listarrede.TransacaoArquivos.FirstOrDefault(x => x.is_numero_cv_long == Convert.ToInt64(items.nsu_rede)) ?? new ConciliacaoTransacaoRede();
                    var redes_c =
                        listarrede.TransacaoArquivos.Where(
                            x => x.is_numero_resumo_vendas_long == Convert.ToInt64(redes.is_numero_resumo_vendas)) ??
                        new List<ConciliacaoTransacaoRede>();


                    var rtef = TEFDAL.GetStatusTEF(conta.IdConta, Convert.ToInt64(items.nsu_rede).ToString());//new RetornoTEF();// TEFDAL.GetStatusTEF(conta.IdConta, Convert.ToInt64(items.nsu_rede).ToString());

                    if (redes.is_data_credito.ToString("yyyyMMdd").Equals("00010101"))
                    {
                        var arq = new ArquivoDeCriticas();
                        arq.ds_motivo = "Registro não encontrado nos arquivos da Rede!";
                        arq.ds_arquivo = arquivo;
                        arq.dt_credito = redes.is_data_credito;
                        arq.dt_transacao = items.dt_transacao;
                        arq.numero_resumo_venda = redes.is_numero_resumo_vendas_long.ToString();
                        arq.nsu_rede = items.nsu_rede_long.ToString();
                        arq.vl_bruto = items.vl_bruto;
                        arq.vl_liquido = redes.is_valor_liquido;
                        arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                        ListCriticas.Add(arq);
                        continue;

                        /* l_erros.AppendFormat("{0}", 3);
                         l_erros.AppendFormat("{0}", items.dt_transacao.ToString("yyyyMMdd"));
                         l_erros.AppendFormat("{0}", redes.is_data_credito.ToString("yyyyMMdd"));
                         l_erros.AppendFormat("{0:D12}", items.nsu_rede_long);
                         l_erros.AppendFormat("{0}", (!items.is_autorizacao.Equals("") ? items.is_autorizacao : (rtef.Autorizacao ?? string.Empty).Equals("") ? "" : (rtef.Autorizacao ?? string.Empty)).PadRight(12)); // Autorizacao
                         l_erros.AppendFormat("{0:D2}", items.tot_parcela); // Plano
                         l_erros.AppendFormat("{0:D2}", Convert.ToInt64(Operadora(items.operadora_desc.ToUpper())));

                         l_erros.AppendFormat("{0:D2}", Convert.ToInt64(Bandeira((rtef.Bandeira ?? "00").ToUpper().Trim())) > 0 ?
                                                        Convert.ToInt64(Bandeira((rtef.Bandeira ?? "00").ToUpper().Trim())) :

                                                        Convert.ToInt64(Bandeira((items.bandeira_desc ?? "00").ToUpper().Trim())) > 0 ?
                                                        Convert.ToInt64(Bandeira((items.bandeira_desc ?? "00").ToUpper().Trim())) :

                                                        Convert.ToInt64(Bandeira((redes.is_bandeira ?? "00").ToUpper().Trim()))

                                                        );
                         l_erros.AppendFormat("{0}", items.produto.Substring(0, 1).ToUpper());
                         l_erros.AppendFormat("{0:D13}", Convert.ToInt64(items.vl_bruto.ToString().Replace(",", "").Replace(".", "")));
                         l_erros.AppendFormat("{0:D13}", Convert.ToInt64(redes.taxa_cobrada.ToString().Replace(",", "").Replace(".", "")));
                         l_erros.AppendFormat("{0:D20}", redes.is_numero_resumo_vendas_long);
                         l_erros.AppendFormat("{0}", items.area_cliente.PadRight(50));
                         l_erros.AppendFormat("{0:D4}", Convert.ToInt64((redes_c.FirstOrDefault(x => x.is_tipo_registro == "RESUMO") ?? new ConciliacaoTransacaoRede()).banco));
                         l_erros.AppendFormat("{0:D6}", Convert.ToInt64((redes_c.FirstOrDefault(x => x.is_tipo_registro == "RESUMO") ?? new ConciliacaoTransacaoRede()).agencia));
                         l_erros.AppendFormat("{0:D13}", Convert.ToInt64((redes_c.FirstOrDefault(x => x.is_tipo_registro == "RESUMO") ?? new ConciliacaoTransacaoRede()).conta_corrente));
                         l_erros.AppendFormat("{0:D6}", Convert.ToInt64(items.cod_loja)); // CODIGO DA LOJA
                         l_erros.AppendFormat("{0}", items.nm_estabelecimento.Trim().PadRight(20)); // LOJA
                         l_erros.AppendFormat("{0:D20}", Convert.ToInt64((redes.is_numero_filiacao_pv ?? "0").Trim())); // Estabelecimento
                         l_erros.AppendFormat("{0:D20}", (redes.numero_cartao ?? string.Empty).Trim().Replace("x", "*").Replace("X", "*").PadLeft(20, '0'));
                         l_erros.AppendFormat("{0:D13}", Convert.ToInt64(redes.is_valor_liquido.ToString().Replace(",", "").Replace(".", "")));
                         l_erros.AppendFormat("{0:D13}", Convert.ToInt64(items.vl_bruto.ToString().Replace(",", "").Replace(".", "")));
                         l_erros.AppendFormat("{0:D15}", Convert.ToInt64(items.cod_loja)); // CODIGO DA LOJA
                         l_erros.AppendFormat("{0:D2}", (redes.is_status_transacao ?? string.Empty).ToUpper().Trim().Equals("APROVADO") ? "02" : (redes.is_status_transacao ?? string.Empty).ToUpper().Trim().Equals("CV/NSU OK") ? "02" : (redes.is_status_transacao ?? string.Empty));
                         l_erros.AppendFormat("{0}", " ".PadLeft(14));
                         //  l.AppendFormat("{0}", string.Empty); // Reservado
                         //  l.AppendFormat("{0};", Environment.NewLine); // Reservado
                         swerros.WriteLine(l_erros.ToString());
                         l_erros.Clear();
                         continue;*/
                    }
                    else if ((Convert.ToInt64(Bandeira((rtef.Bandeira ?? "00").ToUpper().Trim())) > 0 ?
                                                 Convert.ToInt64(Bandeira((rtef.Bandeira ?? "00").ToUpper().Trim())) :

                                                Convert.ToInt64(Bandeira((items.bandeira_desc ?? "00").ToUpper().Trim())) > 0 ?
                                                       Convert.ToInt64(Bandeira((items.bandeira_desc ?? "00").ToUpper().Trim())) :

                                                       Convert.ToInt64(Bandeira((redes.is_bandeira ?? "00").ToUpper().Trim()))) == 0)
                    {
                        var arq = new ArquivoDeCriticas();
                        arq.ds_motivo = "Bandeira do cartão não informada!";
                        arq.ds_arquivo = arquivo;
                        arq.dt_credito = redes.is_data_credito;
                        arq.dt_transacao = items.dt_transacao;
                        arq.numero_resumo_venda = redes.is_numero_resumo_vendas_long.ToString();
                        arq.nsu_rede = items.nsu_rede_long.ToString();
                        arq.vl_bruto = items.vl_bruto;
                        arq.vl_liquido = redes.is_valor_liquido;
                        arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                        ListCriticas.Add(arq);
                        continue;
                    }
                    else if (
                        (!items.is_autorizacao.Equals("")
                            ? items.is_autorizacao
                            : (rtef.Autorizacao ?? string.Empty).Equals("")
                                ? ""
                                : (rtef.Autorizacao ?? string.Empty)).Equals(""))
                    {
                        var arq = new ArquivoDeCriticas();
                        arq.ds_motivo = "Campo autorização não informado!";
                        arq.ds_arquivo = arquivo;
                        arq.dt_credito = redes.is_data_credito;
                        arq.dt_transacao = items.dt_transacao;
                        arq.numero_resumo_venda = redes.is_numero_resumo_vendas_long.ToString();
                        arq.nsu_rede = items.nsu_rede_long.ToString();
                        arq.vl_bruto = items.vl_bruto;
                        arq.vl_liquido = redes.is_valor_liquido;
                        arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                        ListCriticas.Add(arq);
                        continue;
                    }

                    totBruto = totBruto + items.vl_bruto;
                    totLiquido = totLiquido + redes.is_valor_liquido;

                    try
                    {
                        contador++;
                        l.AppendFormat("{0}", 3);
                        l.AppendFormat("{0}", items.dt_transacao.ToString("yyyyMMdd"));
                        l.AppendFormat("{0}", redes.is_data_credito.ToString("yyyyMMdd"));
                        l.AppendFormat("{0:D12}", items.nsu_rede_long);
                        l.AppendFormat("{0}", (!items.is_autorizacao.Equals("") ? items.is_autorizacao : (rtef.Autorizacao ?? string.Empty).Equals("") ? "" : (rtef.Autorizacao ?? string.Empty)).PadRight(12)); // Autorizacao
                        l.AppendFormat("{0:D2}", items.tot_parcela); // Plano
                        l.AppendFormat("{0:D2}", Convert.ToInt64(Operadora(items.operadora_desc.ToUpper())));

                        l.AppendFormat("{0:D2}", Convert.ToInt64(Bandeira((rtef.Bandeira ?? "00").ToUpper().Trim())) > 0 ?
                                                 Convert.ToInt64(Bandeira((rtef.Bandeira ?? "00").ToUpper().Trim())) :

                                                Convert.ToInt64(Bandeira((items.bandeira_desc ?? "00").ToUpper().Trim())) > 0 ?
                                                       Convert.ToInt64(Bandeira((items.bandeira_desc ?? "00").ToUpper().Trim())) :

                                                       Convert.ToInt64(Bandeira((redes.is_bandeira ?? "00").ToUpper().Trim()))

                                                );


                        l.AppendFormat("{0}", items.produto.Substring(0, 1).ToUpper());
                        l.AppendFormat("{0:D13}", Convert.ToInt64(items.vl_bruto.ToString().Replace(",", "").Replace(".", "")));
                        l.AppendFormat("{0:D13}", Convert.ToInt64(redes.taxa_cobrada.ToString().Replace(",", "").Replace(".", "")));
                        l.AppendFormat("{0:D20}", redes.is_numero_resumo_vendas_long);
                        l.AppendFormat("{0}", items.area_cliente.PadRight(50));
                        l.AppendFormat("{0:D4}", Convert.ToInt64((redes_c.FirstOrDefault(x => x.is_tipo_registro == "RESUMO") ?? new ConciliacaoTransacaoRede()).banco));
                        l.AppendFormat("{0:D6}", Convert.ToInt64((redes_c.FirstOrDefault(x => x.is_tipo_registro == "RESUMO") ?? new ConciliacaoTransacaoRede()).agencia));
                        l.AppendFormat("{0:D13}", Convert.ToInt64((redes_c.FirstOrDefault(x => x.is_tipo_registro == "RESUMO") ?? new ConciliacaoTransacaoRede()).conta_corrente));
                        l.AppendFormat("{0:D6}", Convert.ToInt64(items.cod_loja)); // CODIGO DA LOJA
                        l.AppendFormat("{0}", items.nm_estabelecimento.Trim().PadRight(20)); // LOJA
                        l.AppendFormat("{0:D20}", redes.is_numero_filiacao_pv); // Estabelecimento
                        l.AppendFormat("{0:D20}", (redes.numero_cartao ?? string.Empty).Trim().Replace("x", "*").Replace("X", "*").PadLeft(20, '0'));
                        l.AppendFormat("{0:D13}", Convert.ToInt64(redes.is_valor_liquido.ToString().Replace(",", "").Replace(".", "")));
                        l.AppendFormat("{0:D13}", Convert.ToInt64(items.vl_bruto.ToString().Replace(",", "").Replace(".", "")));
                        l.AppendFormat("{0:D15}", Convert.ToInt64(items.cod_loja)); // CODIGO DA LOJA
                        l.AppendFormat("{0:D2}", (redes.is_status_transacao ?? string.Empty).ToUpper().Trim().Equals("APROVADO") ? "02" : (redes.is_status_transacao ?? string.Empty).ToUpper().Trim().Equals("CV/NSU OK") ? "02" : (redes.is_status_transacao ?? string.Empty));
                        l.AppendFormat("{0}", " ".PadLeft(14));
                        //  l.AppendFormat("{0}", string.Empty); // Reservado
                        //  l.AppendFormat("{0};", Environment.NewLine); // Reservado
                        sw.WriteLine(l.ToString());
                        l.Clear();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }


                l.AppendFormat("{0}", 9);
                l.AppendFormat("{0:D6}", contador);
                l.AppendFormat("{0:D13}", Convert.ToInt64(totBruto.ToString().Replace(",", "").Replace(".", "")));
                l.AppendFormat("{0:D13}", Convert.ToInt64((totBruto - totLiquido).ToString().Replace(",", "").Replace(".", "")));
                l.AppendFormat("{0:D13}", 0);
                l.AppendFormat("{0:D13}", 0);
                l.AppendFormat("{0:D13}", 0);
                l.AppendFormat("{0}", " ".PadLeft(168));
                sw.WriteLine(l.ToString());
                //swerros.WriteLine(l.ToString());
                l.Clear();



                //swerros.Dispose();
            }

            DAL.GravarList(ListCriticas);
            Response.Clear();
            Response.ContentType = "text/plain";
            Response.AppendHeader("Content-Disposition", String.Format("attachment;filename={0}", arquivo));
            Response.TransmitFile(file);
            Response.End();
        }

        /* [HttpPost]
         public ActionResult MovimentacoesPagamentoCriticas(TransacaoRedeViewModel model)
         {




             ViewBag.tp_data = Models.DataTypes.getDataTypes();
             ViewBag.tp_cartao = Models.CartaoTypes.getCartaoTypes(model.tp_cartao);
             ViewBag.tp_banco = Models.BancoTypes.getBancoTypes(model.tp_banco);
             ViewBag.tp_movimento = Models.MovimentacaoTypes.getMovimentacaoTypes(model.tp_movimento);

             model.ArquivosCriticas = DAL.ListarObjetos<ArquivoDeCriticas>(string.Format("ds_arquivo='{0}'",
                 Cookie.Get("arquivoerropagamento"))) ?? new List<ArquivoDeCriticas>();

            //  var dir = Server.MapPath("~\\TXT");
            //   Response.Clear();
            //   Response.ContentType = "text/plain";
            //   Response.AppendHeader("Content-Disposition", String.Format("attachment;filename={0}", "MOVIMENT_RET_" + DateTime.Now.ToString("yyyyMMdd") + "_PAG_CRITICA.txt"));
            //   Response.TransmitFile(Path.Combine(dir, "MOVIMENT_" + DateTime.Now.ToString("yyyyMMdd") + "_PAG_CRITICA.txt"));
            //   Response.End();

             return View("MovimentacoesPagamento", model);
         }*/

        [HttpPost]
        public ActionResult MovimentacoesPagamentoArquivo(TransacaoRedeViewModel model)
        {

            if (Cookie.Get("arquivopagamento").Equals("")) {
                ModelState.AddModelError("", "Nenhum arquivo para exportar. Primeiramente busque os registros!");
                RedirectToAction("MovimentacoesPagamento");
            };

            ViewBag.tp_data      = DataTypes.getDataTypes();
            ViewBag.tp_cartao    = CartaoTypes.getCartaoTypes(model.tp_cartao);
            ViewBag.tp_banco     = BancoTypes.getBancoTypes(model.tp_banco);
            ViewBag.tp_movimento = MovimentacaoTypes.getMovimentacaoTypes(model.tp_movimento);

            // System.IO.File.WriteAllLines(Path.Combine(dir, arquivo), model.ArquivoDePagamento);
            // model.ArquivosCriticas = DAL.ListarObjetos<ArquivoDeCriticas>(string.Format("ds_arquivo='{0}'",
            //     Cookie.Get("arquivoerropagamento"))) ?? new List<ArquivoDeCriticas>();

            var dir = Server.MapPath("~\\TXT");
            var arquivo = Cookie.Get("arquivopagamento");
            var file = Path.Combine(dir, arquivo);

            Response.Clear();
            Response.ContentType = "text/plain";
            Response.AppendHeader("Content-Disposition", String.Format("attachment;filename={0}", arquivo));
            Response.TransmitFile(file);
            Response.End();
            Cookie.Delete("arquivopagamento");
            return View("MovimentacoesPagamento");
        }

        public ActionResult MovimentacoesPagamento(string data, string debitocredito)
        {

            ViewBag.QtdPagamento = "0";
            ViewBag.TotalPagamentoBruto = "0,00";
            ViewBag.TotalPagamentoLiquido = "0,00";

            ViewBag.QtdCriticas = "0";
            ViewBag.TotalCriticasBruto = "0,00";
            ViewBag.TotalCriticasLiquido = "0,00";

            ViewBag.QtdGeral = "0";
            ViewBag.TotalGeralBruto = "0,00";
            ViewBag.TotalGeralLiquido = "0,00";

            var datainicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var datafinal = DateTime.Now.Date;
            var tp_cartao = 3;
            var tp_banco = 0;
            var tp_movimento = 0;
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

            ViewBag.tp_banco = BancoTypes.getBancoTypes(0);

            ViewBag.tp_cartao = CartaoTypes.getCartaoTypes(tp_cartao);

            ViewBag.tp_movimento = MovimentacaoTypes.getMovimentacaoTypes(1);

            var listar = new TransacaoRedeViewModel
            {
                tp_situacao = 2,
                TransacaoArquivos = transacaoarquivos,
                DataInicio = datainicio,
                DataFinal = datafinal,
                tp_data = 0, /// emissao
                tp_cartao = tp_cartao,
                tp_banco = tp_banco,
                tp_movimento = tp_movimento,
                resumo = ""
            };

            ViewBag.Disabled = "disabled";
            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            return View(listar);
        }

        [HttpPost]
        public ViewResult MovimentacoesPagamento(TransacaoRedeViewModel model)
        {
            try
            {
                /*  List<rvValor> RVVALOR = new List<rvValor>();*/
                ViewBag.tp_data = DataTypes.getDataTypes();
                ViewBag.tp_cartao = CartaoTypes.getCartaoTypes();
                ViewBag.tp_banco = BancoTypes.getBancoTypes(model.tp_banco);
                ViewBag.tp_movimento = MovimentacaoTypes.getMovimentacaoTypes(model.tp_movimento);

                var dir = Server.MapPath("~\\TXT");
                var arquivo = "MOVIMENT_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_PAG.txt";
                var file = Path.Combine(dir, arquivo);
                Directory.CreateDirectory(dir);

                Cookie.Delete("arquivopagamento");
                Cookie.Set("arquivopagamento", arquivo);
                List<ArquivoDeCriticas> ListCriticas = new List<ArquivoDeCriticas>();
                StringBuilder l = new StringBuilder();
                StringBuilder l_erros = new StringBuilder();


                var contador = 0;
                decimal totBruto = 0;
                decimal totLiquido = 0;
                decimal totAntecipado = 0;

                decimal totParcelas = 0;

                using (StreamWriter sw = System.IO.File.CreateText(file))
                {
                    // StreamWriter swerros = System.IO.File.CreateText(Path.Combine(dir, "MOVIMENT_" + DateTime.Now.ToString("yyyyMMdd") + "_PAG_CRITICA.txt"));

                    l.AppendFormat("{0}", 0);
                    l.AppendFormat("{0:D3}", "005");
                    l.AppendFormat("{0}", (model.DataInicio ?? DateTime.MinValue).ToString("yyyyMMdd"));
                    l.AppendFormat("{0}", "".PadLeft(16));
                    l.AppendFormat("{0}", (model.DataInicio ?? DateTime.MinValue).ToString("yyyyMMdd"));
                    l.AppendFormat("{0}", (model.DataFinal ?? DateTime.MinValue).ToString("yyyyMMdd"));
                    l.AppendFormat("{0}", "ALLPAYMENTS".PadRight(13));
                    l.AppendFormat("{0}", "LOGIN".PadRight(13));
                    l.AppendFormat("{0:D6}", 1);
                    l.AppendFormat("{0}", " ".PadRight(164));
                    sw.WriteLine(l.ToString());
                    // swerros.WriteLine(l.ToString());
                    l.Clear();

                    var conta = new BaseID();
                    decimal bruto = 0;

                    if ((model.tp_movimento == 1) || (model.tp_movimento == 0))
                    {


                        if ((model.tp_cartao == 0) || (model.tp_cartao == 3))
                        {

                            List<ConciliacaoUseRedeEEFICreditosStructListar> MovFin = DAL.ListarObjetos<ConciliacaoUseRedeEEFICreditosStructListar>(
                            string.Format("id_conta = {3} and data_lancamento between '{0}' and '{1}'  {2} ", (model.DataInicio ?? DateTime.MinValue).ToString("yyyy-MM-dd"),
                                (model.DataFinal ?? DateTime.MaxValue).ToString("yyyy-MM-dd"),

                                (model.tp_banco > 0 && model.tp_banco < 1000) ? "and banco =" + Convert.ToInt64(model.tp_banco.ToString().Substring(0, 3)) :
                                (model.tp_banco > 0 && model.tp_banco > 1000) ? "and banco =" + model.tp_banco.ToString().Substring(0, 3) + " and cast(conta_corrente as unsigned)=" + model.tp_banco.ToString().Substring(3, 6) :


                                "", UsuarioLogado.IdConta), "  valor_lancamento desc ");



                            foreach (var items in MovFin)
                            {

                                List<ConciliacaoUseRedeEEVCComprovanteVendaStructListar> CTransacaoRedeCredito =
                                    DAL.ListarObjetos<ConciliacaoUseRedeEEVCComprovanteVendaStructListar>(string.Format("numero_resumo_venda={0} and id_conta={1} ", items.numero_rv, UsuarioLogado.IdConta, Convert.ToInt32(items.numero_parcela.Substring(0, 2))));


                                if (CTransacaoRedeCredito.Count() == 0)
                                {

                                    var arq = new ArquivoDeCriticas();
                                    arq.ds_motivo = "Não encontrado Extrato REDE!";
                                    arq.ds_arquivo = arquivo;
                                    arq.dt_credito = items.data_lancamento;
                                    arq.dt_transacao = items.data_movimento;
                                    arq.numero_resumo_venda = items.numero_rv.ToString();
                                    arq.nsu_rede = "0";// itemsRede.is_numero_cv;
                                    arq.vl_bruto = items.valor_bruto_rv;
                                    arq.vl_liquido = items.valor_lancamento;
                                    arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                    ListCriticas.Add(arq);
                                    continue;
                                }

                                /*ConciliacaoUseRedeEEFIAntecipacaoStruct CTransacaoAntecipacao =
                                    DAL.GetObjeto<ConciliacaoUseRedeEEFIAntecipacaoStruct>(string.Format("numero_rv_correspondente={0} and numero_parcela='{1}' and id_conta={2}", items.numero_rv, items.numero_parcela, UsuarioLogado.IdConta)) ?? new ConciliacaoUseRedeEEFIAntecipacaoStruct(); */


                                foreach (var itemsRede in CTransacaoRedeCredito)
                                {

                                    TransacaoEstabelecimentoListar extrato_item;

                                    if (itemsRede.is_tipo_registro == 12)
                                    {

                                        if (!(itemsRede.is_numero_parcelas >= Convert.ToInt32(items.numero_parcela.Substring(0, 2))))
                                            continue;


                                        extrato_item = DAL.GetObjeto<TransacaoEstabelecimentoListar>(string.Format("nsu_rede={0} and id_conta={1}", itemsRede.is_numero_cv_long, UsuarioLogado.IdConta));


                                        // List<ConciliacaoUseRedeParcelasStructListar> CTransacaoRedeParcelas = DAL.ListarObjetos<ConciliacaoUseRedeParcelasStructListar>(string.Format("cast(numero_resumo_venda as unsigned)={0} and id_conta={1} and numero_parcela={2} and cast(numero_filiacao_pv as unsigned) ={3} ", items.numero_rv, UsuarioLogado.IdConta, Convert.ToInt32(items.numero_parcela.Substring(0, 2)), items.numero_pv_original ));
                                        var parcliq = Convert.ToInt64(items.numero_parcela.Substring(0, 2)) == 1 ? itemsRede.is_valor_liquido_primeira_parc : itemsRede.is_valor_liquido_demais_parc;
                                        var valordesconto = Math.Round((itemsRede.is_valor_desconto / itemsRede.is_numero_parcelas), 2);
                                        var parbrut = parcliq + valordesconto;

                                        bruto = bruto + valordesconto - (itemsRede.is_valor_desconto / itemsRede.is_numero_parcelas);


                                        if ((itemsRede.is_numero_autorizacao ?? string.Empty).Equals(""))
                                        {
                                            var arq = new ArquivoDeCriticas();
                                            arq.ds_motivo = "Sem código de Autorização - Crédito!";
                                            arq.ds_arquivo = arquivo;
                                            arq.dt_credito = items.data_lancamento;
                                            arq.dt_transacao = itemsRede.is_data_cv;
                                            arq.numero_resumo_venda = itemsRede.is_numero_resumo_vendas.ToString();
                                            arq.nsu_rede = itemsRede.is_numero_cv.ToString();
                                            arq.vl_bruto = parbrut;
                                            arq.vl_liquido = parcliq;
                                            arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                            ListCriticas.Add(arq);
                                        }
                                        else if (extrato_item != null)
                                        {
                                            contador++;

                                            model.ArquivosPagamento.Add(
                                                new ExtratoPagamento
                                                {
                                                    Tipo_registro = 5,
                                                    Data_credito = extrato_item.dt_transacao,
                                                    Data_venda = items.data_lancamento,
                                                    NSU = extrato_item.nsu_rede_long,
                                                    Autorizacao = itemsRede.is_numero_autorizacao,
                                                    Plano = itemsRede.is_numero_parcelas,
                                                    Parcela = Convert.ToInt32(items.numero_parcela.Substring(0, 2)),
                                                    Rede = Convert.ToInt32(Operadora(extrato_item.operadora_desc.ToUpper())),
                                                    RedeNome = extrato_item.operadora_desc.ToUpper(),
                                                    Bandeira = Convert.ToInt32(Bandeira((items.bandeira ?? string.Empty).ToUpper().Trim())),
                                                    BandeiraNome = (items.bandeira ?? string.Empty).ToUpper().Trim(),
                                                    Produto = "C",
                                                    Valor_bruto = parbrut,
                                                    Valor_administracao = valordesconto,
                                                    Valor_antecipacao = 0,
                                                    Numero_lote = itemsRede.is_numero_resumo_vendas_long,
                                                    Area_cliente = extrato_item.area_cliente,
                                                    Banco = Convert.ToInt32(items.banco),
                                                    Agencia = Convert.ToInt32(items.agencia),
                                                    Conta = Convert.ToInt32(items.conta_corrente),
                                                    Cod_loja = Convert.ToInt32(extrato_item.cod_loja),
                                                    Cod_Estabelecimento = extrato_item.nm_estabelecimento.Trim(),
                                                    Loja = itemsRede.is_numero_filiacao_pv.ToString(),
                                                    Status = "PAG",
                                                    Valor_liquido = parcliq,
                                                    Cod_loja_2 = extrato_item.cod_loja,
                                                    Reservado = ""
                                                }
                                            );


                                            l.AppendFormat("{0}", 5);
                                            l.AppendFormat("{0}", extrato_item.dt_transacao.ToString("yyyyMMdd"));
                                            l.AppendFormat("{0}", items.data_lancamento.ToString("yyyyMMdd"));
                                            l.AppendFormat("{0:D12}", extrato_item.nsu_rede_long);
                                            l.AppendFormat("{0}", (itemsRede.is_numero_autorizacao ?? string.Empty).PadRight(12)); // Autorizacao
                                            l.AppendFormat("{0:D2}", Convert.ToInt64(itemsRede.is_numero_parcelas)); // Plano
                                            l.AppendFormat("{0:D2}", Convert.ToInt32(items.numero_parcela.Substring(0, 2))); // Parcela
                                            l.AppendFormat("{0:D2}", Convert.ToInt64(Operadora(extrato_item.operadora_desc.ToUpper())));
                                            l.AppendFormat("{0:D2}", Convert.ToInt64(Bandeira((items.bandeira ?? string.Empty).ToUpper().Trim())));
                                            l.AppendFormat("{0}", "C");
                                            l.AppendFormat("{0:D13}", Convert.ToInt64(parbrut.ToString().Replace(",", "").Replace(".", "")));
                                            l.AppendFormat("{0:D13}", Convert.ToInt64(valordesconto.ToString().Replace(",", "").Replace(".", "")));
                                            l.AppendFormat("{0:D13}", 0);
                                            l.AppendFormat("{0:D20}", itemsRede.is_numero_resumo_vendas_long);
                                            l.AppendFormat("{0}", extrato_item.area_cliente.PadRight(50));
                                            l.AppendFormat("{0:D4}", Convert.ToInt64(items.banco));
                                            l.AppendFormat("{0:D6}", Convert.ToInt64(items.agencia));
                                            l.AppendFormat("{0:D13}", Convert.ToInt64(items.conta_corrente));
                                            l.AppendFormat("{0:D6}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                            l.AppendFormat("{0}", extrato_item.nm_estabelecimento.Trim().PadRight(20)); // LOJA
                                            l.AppendFormat("{0:D20}", itemsRede.is_numero_filiacao_pv); // Estabelecimento
                                            l.AppendFormat("{0}", "PAG");
                                            l.AppendFormat("{0:D13}", Convert.ToInt64(parcliq.ToString().Replace(",", "").Replace(".", "")));
                                            l.AppendFormat("{0:D15}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                            l.AppendFormat("{0}", string.Empty.PadLeft(11)); // Reservado
                                            sw.WriteLine(l.ToString());
                                            l.Clear();

                                            totBruto = totBruto + parbrut;
                                            totLiquido = totLiquido + parcliq;
                                            // totAntecipado = totAntecipado + CTransacaoAntecipacao.valor_lancamento;

                                            /*  var RVEncontrado = RVVALOR.FirstOrDefault(d => d.RV == items.numero_rv);
                                              if (RVEncontrado != null)
                                              {
                                                  RVEncontrado.valorbruto = RVEncontrado.valorbruto + parbrut;
                                                  RVEncontrado.valorliquido = RVEncontrado.valorliquido +
                                                                              parcliq;

                                              }
                                              else
                                              {

                                                  var item = new rvValor
                                                  {
                                                      RV = items.numero_rv,
                                                      valorbruto = parbrut,
                                                      valorliquido = parcliq

                                                  };
                                                  RVVALOR.Add(item);

                                              }
                                              RVVALOR.OrderByDescending(x => x.RV);*/


                                        }
                                        else
                                        {
                                            var arq = new ArquivoDeCriticas();
                                            arq.ds_motivo = "Não encontrado Extrato ERP!";
                                            arq.ds_arquivo = arquivo;
                                            arq.dt_credito = items.data_lancamento;
                                            arq.dt_transacao = itemsRede.is_data_cv;
                                            arq.numero_resumo_venda = itemsRede.is_numero_resumo_vendas.ToString();
                                            arq.nsu_rede = itemsRede.is_numero_cv.ToString();
                                            arq.vl_bruto = parbrut;
                                            arq.vl_liquido = parcliq;
                                            arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                            ListCriticas.Add(arq);
                                        }
                                    }
                                    else
                                    {
                                        extrato_item =
                                             DAL.GetObjeto<TransacaoEstabelecimentoListar>(string.Format("nsu_rede={0} and id_conta={1}",
                                                 itemsRede.is_numero_cv_long, UsuarioLogado.IdConta));


                                        if ((itemsRede.is_numero_autorizacao ?? string.Empty).Equals(""))
                                        {
                                            var arq = new ArquivoDeCriticas();
                                            arq.ds_motivo = "Sem código de Autorização - Crédito!";
                                            arq.ds_arquivo = arquivo;
                                            arq.dt_credito = items.data_lancamento;
                                            arq.dt_transacao = itemsRede.is_data_cv;
                                            arq.numero_resumo_venda = itemsRede.is_numero_resumo_vendas.ToString();
                                            arq.nsu_rede = itemsRede.is_numero_cv.ToString();
                                            arq.vl_bruto = itemsRede.is_valor_bruto;
                                            arq.vl_liquido = itemsRede.is_valor_liquido;
                                            arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                            ListCriticas.Add(arq);
                                        }
                                        if (extrato_item != null)
                                        {
                                            contador++;

                                            model.ArquivosPagamento.Add(
                                                new ExtratoPagamento
                                                {
                                                    Tipo_registro = 5,
                                                    Data_credito = extrato_item.dt_transacao,
                                                    Data_venda = items.data_lancamento,
                                                    NSU = extrato_item.nsu_rede_long,
                                                    Autorizacao = itemsRede.is_numero_autorizacao,
                                                    Plano = Convert.ToInt32(items.numero_parcela.Substring(3, 2)),
                                                    Parcela = Convert.ToInt32(items.numero_parcela.Substring(0, 2)),
                                                    Rede = Convert.ToInt32(Operadora(extrato_item.operadora_desc.ToUpper())),
                                                    RedeNome = extrato_item.operadora_desc.ToUpper(),
                                                    Bandeira = Convert.ToInt32(Bandeira((items.bandeira ?? string.Empty).ToUpper().Trim())),
                                                    BandeiraNome = (items.bandeira ?? string.Empty).ToUpper().Trim(),
                                                    Produto = "C",
                                                    Valor_bruto = itemsRede.is_valor_bruto,
                                                    Valor_administracao = itemsRede.is_valor_desconto,
                                                    Valor_antecipacao = 0,
                                                    Numero_lote = itemsRede.is_numero_resumo_vendas_long,
                                                    Area_cliente = extrato_item.area_cliente,
                                                    Banco = Convert.ToInt32(items.banco),
                                                    Agencia = Convert.ToInt32(items.agencia),
                                                    Conta = Convert.ToInt32(items.conta_corrente),
                                                    Cod_loja = Convert.ToInt32(extrato_item.cod_loja),
                                                    Cod_Estabelecimento = extrato_item.nm_estabelecimento.Trim(),
                                                    Loja = itemsRede.is_numero_filiacao_pv.ToString(),
                                                    Status = "PAG",
                                                    Valor_liquido = itemsRede.is_valor_liquido,
                                                    Cod_loja_2 = extrato_item.cod_loja,
                                                    Reservado = ""
                                                }
                                            );


                                            l.AppendFormat("{0}", 5);
                                            l.AppendFormat("{0}", extrato_item.dt_transacao.ToString("yyyyMMdd"));
                                            l.AppendFormat("{0}", items.data_lancamento.ToString("yyyyMMdd"));
                                            l.AppendFormat("{0:D12}", extrato_item.nsu_rede_long);
                                            l.AppendFormat("{0}",
                                                (itemsRede.is_numero_autorizacao ?? string.Empty).PadRight(12));
                                            // Autorizacao
                                            l.AppendFormat("{0:D2}", Convert.ToInt64(items.numero_parcela.Substring(3, 2)));
                                            // Plano
                                            l.AppendFormat("{0:D2}", Convert.ToInt64(items.numero_parcela.Substring(0, 2)));
                                            // Parcela
                                            l.AppendFormat("{0:D2}", Convert.ToInt64(Operadora(extrato_item.operadora_desc.ToUpper())));
                                            l.AppendFormat("{0:D2}", Convert.ToInt64(Bandeira((items.bandeira ?? string.Empty).ToUpper().Trim())));
                                            l.AppendFormat("{0}", "C");
                                            l.AppendFormat("{0:D13}", Convert.ToInt64(itemsRede.is_valor_bruto.ToString().Replace(",", "").Replace(".", "")));
                                            l.AppendFormat("{0:D13}", Convert.ToInt64(itemsRede.is_valor_desconto.ToString().Replace(",", "").Replace(".", "")));
                                            l.AppendFormat("{0:D13}", 0);
                                            l.AppendFormat("{0:D20}", itemsRede.is_numero_resumo_vendas_long);
                                            l.AppendFormat("{0}", extrato_item.area_cliente.PadRight(50));
                                            l.AppendFormat("{0:D4}", Convert.ToInt64(items.banco));
                                            l.AppendFormat("{0:D6}", Convert.ToInt64(items.agencia));
                                            l.AppendFormat("{0:D13}", Convert.ToInt64(items.conta_corrente));
                                            l.AppendFormat("{0:D6}", Convert.ToInt64(extrato_item.cod_loja));
                                            // CODIGO DA LOJA
                                            l.AppendFormat("{0}", extrato_item.nm_estabelecimento.Trim().PadRight(20));
                                            // LOJA
                                            l.AppendFormat("{0:D20}",
                                                itemsRede.is_numero_filiacao_pv);
                                            // Estabelecimento
                                            l.AppendFormat("{0}", "PAG");
                                            l.AppendFormat("{0:D13}",
                                                Convert.ToInt64(
                                                    itemsRede.is_valor_liquido.ToString().Replace(",", "").Replace(".", "")));
                                            l.AppendFormat("{0:D15}", Convert.ToInt64(extrato_item.cod_loja));
                                            // CODIGO DA LOJA
                                            l.AppendFormat("{0}", string.Empty.PadLeft(11)); // Reservado
                                            sw.WriteLine(l.ToString());
                                            l.Clear();

                                            totBruto = totBruto + itemsRede.is_valor_bruto;
                                            totLiquido = totLiquido + itemsRede.is_valor_liquido;
                                            //totAntecipado = totAntecipado + CTransacaoAntecipacao.valor_lancamento;


                                            /*  var RVEncontrado = RVVALOR.FirstOrDefault(d => d.RV == items.numero_rv);
                                              if (RVEncontrado != null)
                                              {
                                                  RVEncontrado.valorbruto = RVEncontrado.valorbruto + itemsRede.is_valor_bruto;
                                                  RVEncontrado.valorliquido = RVEncontrado.valorliquido +
                                                                              itemsRede.is_valor_liquido;

                                              }
                                              else
                                              {

                                                  var item = new rvValor
                                                  {
                                                      RV = items.numero_rv,
                                                      valorbruto = itemsRede.is_valor_bruto,
                                                      valorliquido = itemsRede.is_valor_liquido
                                                  };
                                                  RVVALOR.Add(item);

                                              }
                                              RVVALOR.OrderByDescending(x => x.RV);*/


                                        }
                                        else
                                        {
                                            var arq = new ArquivoDeCriticas();
                                            arq.ds_motivo = "Não encontrado Extrato ERP!";
                                            arq.ds_arquivo = arquivo;
                                            arq.dt_credito = items.data_lancamento;
                                            arq.dt_transacao = itemsRede.is_data_cv;
                                            arq.numero_resumo_venda = itemsRede.is_numero_resumo_vendas.ToString();
                                            arq.nsu_rede = itemsRede.is_numero_cv.ToString();
                                            arq.vl_bruto = itemsRede.is_valor_bruto;
                                            arq.vl_liquido = itemsRede.is_valor_liquido;
                                            arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                            ListCriticas.Add(arq);
                                        }



                                    }

                                }
                            }
                        }

                        if ((model.tp_cartao == 1) || (model.tp_cartao == 3))
                        {


                            List<ConciliacaoUseRedeEEVDResumoOperacaoStructListar> CTransacaoResumoOperacaoRedeDebito =
                             DAL.ListarObjetos<ConciliacaoUseRedeEEVDResumoOperacaoStructListar>(

                                string.Format("id_conta = {3} and data_credito between '{0}' and '{1}'  {2} ", (model.DataInicio ?? DateTime.MinValue).ToString("yyyy-MM-dd"),
                                            (model.DataFinal ?? DateTime.MaxValue).ToString("yyyy-MM-dd"),

                                            (model.tp_banco > 0 && model.tp_banco < 1000) ? "and banco =" + Convert.ToInt64(model.tp_banco.ToString().Substring(0, 3)) :
                                            (model.tp_banco > 0 && model.tp_banco > 1000) ? "and banco =" + model.tp_banco.ToString().Substring(0, 3) + " and cast(conta_corrente as unsigned)=" + model.tp_banco.ToString().Substring(3, 6) :


                                            "", UsuarioLogado.IdConta)

                                 );


                            foreach (var itemsResumoRede in CTransacaoResumoOperacaoRedeDebito)
                            {

                                List<ConciliacaoUseRedeEEVDComprovanteVendaStructListar> CTransacaoComprovantesRedeDebito =
                                 DAL.ListarObjetos<ConciliacaoUseRedeEEVDComprovanteVendaStructListar>(string.Format("numero_resumo_venda={0} and id_conta={1}", itemsResumoRede.is_numero_resumo_venda_long, UsuarioLogado.IdConta));

                                foreach (var itemsComprovantesRede in CTransacaoComprovantesRedeDebito)
                                {

                                    TransacaoEstabelecimentoListar extrato_item = DAL.GetObjeto<TransacaoEstabelecimentoListar>(string.Format("nsu_rede={0} and id_conta={1}",
                                         itemsComprovantesRede.is_numero_cv_long, UsuarioLogado.IdConta)) ?? new TransacaoEstabelecimentoListar();

                                    var rtef = TEFDAL.GetStatusTEF(conta.IdConta, itemsComprovantesRede.is_numero_cv_long.ToString()) ??
                                                   new RetornoTEF();

                                    if ((!(rtef.Autorizacao ?? string.Empty).Equals("") ? (rtef.Autorizacao ?? string.Empty) : (extrato_item.is_autorizacao ?? string.Empty)).Equals(""))
                                    {
                                        var arq = new ArquivoDeCriticas();
                                        arq.ds_motivo = "Sem código de Autorização - Débito!";
                                        arq.ds_arquivo = arquivo;
                                        arq.dt_credito = extrato_item.dt_transacao;
                                        arq.dt_transacao = itemsComprovantesRede.is_data_credito;
                                        arq.numero_resumo_venda = itemsComprovantesRede.is_numero_resumo_vendas.ToString();
                                        arq.nsu_rede = extrato_item.nsu_rede.ToString();
                                        arq.vl_bruto = itemsComprovantesRede.is_valor_bruto;
                                        arq.vl_liquido = itemsComprovantesRede.is_valor_liquido;
                                        arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                        ListCriticas.Add(arq);
                                    }
                                    else if (extrato_item.nsu_rede > 0)
                                    {
                                        contador++;

                                        model.ArquivosPagamento.Add(
                                                new ExtratoPagamento
                                                {
                                                    Tipo_registro = 5,
                                                    Data_credito = extrato_item.dt_transacao,
                                                    Data_venda = itemsComprovantesRede.is_data_credito,
                                                    NSU = extrato_item.nsu_rede_long,
                                                    Autorizacao = (!(rtef.Autorizacao ?? string.Empty).Equals("") ? (rtef.Autorizacao ?? string.Empty) : (extrato_item.is_autorizacao ?? string.Empty)),
                                                    Plano = Convert.ToInt32("1"),
                                                    Parcela = Convert.ToInt32(1),
                                                    Rede = Convert.ToInt32(Operadora(extrato_item.operadora_desc.ToUpper())),
                                                    RedeNome = extrato_item.operadora_desc.ToUpper(),
                                                    Bandeira = Convert.ToInt32(Bandeira((itemsComprovantesRede.is_bandeira ?? string.Empty).ToUpper().Trim())),
                                                    BandeiraNome = (itemsComprovantesRede.is_bandeira ?? string.Empty).ToUpper().Trim(),
                                                    Produto = "D",
                                                    Valor_bruto = itemsComprovantesRede.is_valor_bruto,
                                                    Valor_administracao = itemsComprovantesRede.is_valor_desconto,
                                                    Valor_antecipacao = 0,
                                                    Numero_lote = itemsComprovantesRede.is_numero_resumo_vendas_long,
                                                    Area_cliente = extrato_item.area_cliente,
                                                    Banco = Convert.ToInt32(itemsResumoRede.is_banco),
                                                    Agencia = Convert.ToInt32(itemsResumoRede.is_agencia),
                                                    Conta = Convert.ToInt32(itemsResumoRede.is_conta_corrente),
                                                    Cod_loja = Convert.ToInt32(extrato_item.cod_loja),
                                                    Cod_Estabelecimento = extrato_item.nm_estabelecimento.Trim(),
                                                    Loja = itemsComprovantesRede.is_numero_filiacao_pv.ToString(),
                                                    Status = "PAG",
                                                    Valor_liquido = itemsComprovantesRede.is_valor_liquido,
                                                    Cod_loja_2 = extrato_item.cod_loja,
                                                    Reservado = ""
                                                }
                                            );

                                        l.AppendFormat("{0}", 5);
                                        l.AppendFormat("{0}", extrato_item.dt_transacao.ToString("yyyyMMdd"));
                                        l.AppendFormat("{0}", itemsComprovantesRede.is_data_credito.ToString("yyyyMMdd"));
                                        l.AppendFormat("{0:D12}", extrato_item.nsu_rede_long);
                                        l.AppendFormat("{0}", (!(rtef.Autorizacao ?? string.Empty).Equals("") ? (rtef.Autorizacao ?? string.Empty) : (extrato_item.is_autorizacao ?? string.Empty)).PadRight(12)); // Autorizacao
                                        l.AppendFormat("{0:D2}", 1); // Plano
                                        l.AppendFormat("{0}", "01"); // Parcela
                                        l.AppendFormat("{0:D2}", Convert.ToInt64(Operadora(extrato_item.operadora_desc.ToUpper())));
                                        l.AppendFormat("{0:D2}", Convert.ToInt64(Bandeira((itemsComprovantesRede.is_bandeira ?? string.Empty).ToUpper().Trim())));
                                        l.AppendFormat("{0}", "D");
                                        l.AppendFormat("{0:D13}", Convert.ToInt64(itemsComprovantesRede.is_valor_bruto.ToString().Replace(",", "").Replace(".", "")));
                                        l.AppendFormat("{0:D13}", Convert.ToInt64(itemsComprovantesRede.is_valor_desconto.ToString().Replace(",", "").Replace(".", "")));
                                        l.AppendFormat("{0:D13}", 0);
                                        l.AppendFormat("{0:D20}", itemsComprovantesRede.is_numero_resumo_vendas_long);
                                        l.AppendFormat("{0}", extrato_item.area_cliente.PadRight(50));
                                        l.AppendFormat("{0:D4}", Convert.ToInt64(itemsResumoRede.is_banco));
                                        l.AppendFormat("{0:D6}", Convert.ToInt64(itemsResumoRede.is_agencia));
                                        l.AppendFormat("{0:D13}", Convert.ToInt64(itemsResumoRede.is_conta_corrente));
                                        l.AppendFormat("{0:D6}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                        l.AppendFormat("{0}", extrato_item.nm_estabelecimento.Trim().PadRight(20)); // LOJA
                                        l.AppendFormat("{0:D20}", itemsComprovantesRede.is_numero_filiacao_pv); // Estabelecimento
                                        l.AppendFormat("{0}", "PAG");
                                        l.AppendFormat("{0:D13}", Convert.ToInt64(itemsComprovantesRede.is_valor_liquido.ToString().Replace(",", "").Replace(".", "")));
                                        l.AppendFormat("{0:D15}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                        l.AppendFormat("{0}", string.Empty.PadLeft(11)); // Reservado
                                        sw.WriteLine(l.ToString());
                                        l.Clear();

                                        totBruto = totBruto + itemsComprovantesRede.is_valor_bruto;
                                        totLiquido = totLiquido + itemsComprovantesRede.is_valor_liquido;
                                    }
                                    else
                                    {
                                        var arq = new ArquivoDeCriticas();
                                        arq.ds_motivo = "Não encontrado Extrato ERP!";
                                        arq.ds_arquivo = arquivo;
                                        arq.dt_credito = itemsComprovantesRede.is_data_credito;
                                        arq.dt_transacao = itemsComprovantesRede.is_data_cv;
                                        arq.numero_resumo_venda = itemsComprovantesRede.is_numero_resumo_vendas.ToString();
                                        arq.nsu_rede = itemsComprovantesRede.is_numero_cv.ToString();
                                        arq.vl_bruto = itemsComprovantesRede.is_valor_bruto;
                                        arq.vl_liquido = itemsComprovantesRede.is_valor_liquido;
                                        arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                        ListCriticas.Add(arq);
                                    }
                                }
                            }
                        }
                    }

                    if ((model.tp_movimento == 2) || (model.tp_movimento == 0))
                    {
                        List<ConciliacaoUseRedeEEFIAntecipacaoStructListar> MovFin = DAL.ListarObjetos<ConciliacaoUseRedeEEFIAntecipacaoStructListar>(
                        string.Format("id_conta = {3} and data_lancamento between '{0}' and '{1}'  {2} ", (model.DataInicio ?? DateTime.MinValue).ToString("yyyy-MM-dd"),
                            (model.DataFinal ?? DateTime.MaxValue).ToString("yyyy-MM-dd"),

                            (model.tp_banco > 0 && model.tp_banco < 1000) ? "and banco =" + Convert.ToInt64(model.tp_banco.ToString().Substring(0, 3)) :
                            (model.tp_banco > 0 && model.tp_banco > 1000) ? "and banco =" + model.tp_banco.ToString().Substring(0, 3) + " and cast(conta_corrente as unsigned)=" + model.tp_banco.ToString().Substring(3, 6) :


                            "", UsuarioLogado.IdConta));


                        foreach (var items in MovFin)
                        {
                            List<ConciliacaoUseRedeEEVCComprovanteVendaStructListar> CTransacaoRedeCredito =
                                DAL.ListarObjetos<ConciliacaoUseRedeEEVCComprovanteVendaStructListar>(string.Format("numero_resumo_venda={0} and id_conta={1} ", items.numero_rv_correspondente, UsuarioLogado.IdConta/*, items.numero_parcela.Substring(0, 2)*/));

                            /*ConciliacaoUseRedeEEFIAntecipacaoStruct CTransacaoAntecipacao =
                                DAL.GetObjeto<ConciliacaoUseRedeEEFIAntecipacaoStruct>(string.Format("numero_rv_correspondente={0} and numero_parcela='{1}' and id_conta={2}", items.numero_rv, items.numero_parcela, UsuarioLogado.IdConta)) ?? new ConciliacaoUseRedeEEFIAntecipacaoStruct(); */


                            foreach (var itemsRede in CTransacaoRedeCredito)
                            {

                                TransacaoEstabelecimentoListar extrato_item;

                                if (itemsRede.is_tipo_registro == 12)
                                {

                                    extrato_item =
                                         DAL.GetObjeto<TransacaoEstabelecimentoListar>(string.Format("nsu_rede={0} and id_conta={1}",
                                             itemsRede.is_numero_cv_long, UsuarioLogado.IdConta));

                                    if (!(itemsRede.is_numero_parcelas >= Convert.ToInt32(items.numero_parcela.Substring(0, 2))))
                                        continue;

                                    var parcliq = Convert.ToInt64(items.numero_parcela.Substring(0, 2)) == 1 ? itemsRede.is_valor_liquido_primeira_parc : itemsRede.is_valor_liquido_demais_parc;
                                    var valordesconto = Math.Round((itemsRede.is_valor_desconto / itemsRede.is_numero_parcelas), 2);
                                    var parbrut = parcliq + valordesconto;

                                    if (extrato_item != null)
                                    {
                                        contador++;

                                        model.ArquivosPagamento.Add(
                                                new ExtratoPagamento
                                                {
                                                    Tipo_registro = 5,
                                                    Data_credito = extrato_item.dt_transacao,
                                                    Data_venda = items.data_lancamento,
                                                    NSU = extrato_item.nsu_rede_long,
                                                    Autorizacao = itemsRede.is_numero_autorizacao,
                                                    Plano = itemsRede.is_numero_parcelas,
                                                    Parcela = Convert.ToInt32(items.numero_parcela.Substring(0, 2)),
                                                    Rede = Convert.ToInt32(Operadora(extrato_item.operadora_desc.ToUpper())),
                                                    RedeNome = extrato_item.operadora_desc.ToUpper(),
                                                    Bandeira = Convert.ToInt32(Bandeira((items.bandeira ?? string.Empty).ToUpper().Trim())),
                                                    BandeiraNome = (items.bandeira ?? string.Empty).ToUpper().Trim(),
                                                    Produto = "C",
                                                    Valor_bruto = parbrut,
                                                    Valor_administracao = valordesconto,
                                                    Valor_antecipacao = 0,
                                                    Numero_lote = itemsRede.is_numero_resumo_vendas_long,
                                                    Area_cliente = extrato_item.area_cliente,
                                                    Banco = Convert.ToInt32(items.banco),
                                                    Agencia = Convert.ToInt32(items.agencia),
                                                    Conta = Convert.ToInt32(items.conta_corrente),
                                                    Cod_loja = Convert.ToInt32(extrato_item.cod_loja),
                                                    Cod_Estabelecimento = extrato_item.nm_estabelecimento.Trim(),
                                                    Loja = itemsRede.is_numero_filiacao_pv.ToString(),
                                                    Status = "PAG",
                                                    Valor_liquido = parcliq,
                                                    Cod_loja_2 = extrato_item.cod_loja,
                                                    Reservado = ""
                                                }
                                            );

                                        l.AppendFormat("{0}", 5);
                                        l.AppendFormat("{0}", extrato_item.dt_transacao.ToString("yyyyMMdd"));
                                        l.AppendFormat("{0}", items.data_lancamento.ToString("yyyyMMdd"));
                                        l.AppendFormat("{0:D12}", extrato_item.nsu_rede_long);
                                        l.AppendFormat("{0}", (itemsRede.is_numero_autorizacao ?? string.Empty).PadRight(12)); // Autorizacao
                                        l.AppendFormat("{0:D2}", Convert.ToInt64(items.numero_parcela.Substring(3, 2))); // Plano
                                        l.AppendFormat("{0:D2}", Convert.ToInt64(items.numero_parcela.Substring(0, 2))); // Parcela
                                        l.AppendFormat("{0:D2}", Convert.ToInt64(Operadora(extrato_item.operadora_desc.ToUpper())));
                                        l.AppendFormat("{0:D2}", Convert.ToInt64(Bandeira((items.bandeira ?? string.Empty).ToUpper().Trim())));
                                        l.AppendFormat("{0}", "C");
                                        l.AppendFormat("{0:D13}", Convert.ToInt64(parbrut.ToString().Replace(",", "").Replace(".", "")));
                                        l.AppendFormat("{0:D13}", Convert.ToInt64(valordesconto.ToString().Replace(",", "").Replace(".", "")));
                                        l.AppendFormat("{0:D13}", Convert.ToInt64(items.valor_lancamento.ToString().Replace(",", "").Replace(".", "")));
                                        l.AppendFormat("{0:D20}", itemsRede.is_numero_resumo_vendas_long);
                                        l.AppendFormat("{0}", extrato_item.area_cliente.PadRight(50));
                                        l.AppendFormat("{0:D4}", Convert.ToInt64(items.banco));
                                        l.AppendFormat("{0:D6}", Convert.ToInt64(items.agencia));
                                        l.AppendFormat("{0:D13}", Convert.ToInt64(items.conta_corrente));
                                        l.AppendFormat("{0:D6}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                        l.AppendFormat("{0}", extrato_item.nm_estabelecimento.Trim().PadRight(20)); // LOJA
                                        l.AppendFormat("{0:D20}", itemsRede.is_numero_filiacao_pv); // Estabelecimento
                                        l.AppendFormat("{0}", "ANT");
                                        l.AppendFormat("{0:D13}", Convert.ToInt64(parcliq.ToString().Replace(",", "").Replace(".", "")));
                                        l.AppendFormat("{0:D15}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                        l.AppendFormat("{0}", string.Empty.PadLeft(11)); // Reservado
                                        sw.WriteLine(l.ToString());
                                        l.Clear();

                                        totBruto = totBruto + parbrut;
                                        totLiquido = totLiquido + parcliq;
                                        totAntecipado = totAntecipado + items.valor_lancamento;
                                    }
                                    else
                                    {
                                        var arq = new ArquivoDeCriticas();
                                        arq.ds_motivo = "Não encontrado Extrato ERP!";
                                        arq.ds_arquivo = arquivo;
                                        arq.dt_credito = items.data_lancamento;
                                        arq.dt_transacao = itemsRede.is_data_cv;
                                        arq.numero_resumo_venda = itemsRede.is_numero_resumo_vendas.ToString();
                                        arq.nsu_rede = itemsRede.is_numero_cv.ToString();
                                        arq.vl_bruto = parbrut;
                                        arq.vl_liquido = parcliq;
                                        arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                        ListCriticas.Add(arq);
                                    }



                                }
                                else
                                {
                                    extrato_item =
                                       DAL.GetObjeto<TransacaoEstabelecimentoListar>(string.Format("nsu_rede={0} and id_conta={1}",
                                           itemsRede.is_numero_cv_long, UsuarioLogado.IdConta));

                                    if (extrato_item != null)
                                    {
                                        contador++;

                                        model.ArquivosPagamento.Add(
                                                new ExtratoPagamento
                                                {
                                                    Tipo_registro = 5,
                                                    Data_credito = extrato_item.dt_transacao,
                                                    Data_venda = items.data_lancamento,
                                                    NSU = extrato_item.nsu_rede_long,
                                                    Autorizacao = itemsRede.is_numero_autorizacao,
                                                    Plano = Convert.ToInt32(items.numero_parcela.Substring(3, 2)),
                                                    Parcela = Convert.ToInt32(items.numero_parcela.Substring(0, 2)),
                                                    Rede = Convert.ToInt32(Operadora(extrato_item.operadora_desc.ToUpper())),
                                                    RedeNome = extrato_item.operadora_desc.ToUpper(),
                                                    Bandeira = Convert.ToInt32(Bandeira((items.bandeira ?? string.Empty).ToUpper().Trim())),
                                                    BandeiraNome = (items.bandeira ?? string.Empty).ToUpper().Trim(),
                                                    Produto = "C",
                                                    Valor_bruto = itemsRede.is_valor_bruto,
                                                    Valor_administracao = itemsRede.is_valor_desconto,
                                                    Valor_antecipacao = 0,
                                                    Numero_lote = itemsRede.is_numero_resumo_vendas_long,
                                                    Area_cliente = extrato_item.area_cliente,
                                                    Banco = Convert.ToInt32(items.banco),
                                                    Agencia = Convert.ToInt32(items.agencia),
                                                    Conta = Convert.ToInt32(items.conta_corrente),
                                                    Cod_loja = Convert.ToInt32(extrato_item.cod_loja),
                                                    Cod_Estabelecimento = extrato_item.nm_estabelecimento.Trim(),
                                                    Loja = itemsRede.is_numero_filiacao_pv.ToString(),
                                                    Status = "PAG",
                                                    Valor_liquido = itemsRede.is_valor_liquido,
                                                    Cod_loja_2 = extrato_item.cod_loja,
                                                    Reservado = ""
                                                }
                                            );

                                        l.AppendFormat("{0}", 5);
                                        l.AppendFormat("{0}", extrato_item.dt_transacao.ToString("yyyyMMdd"));
                                        l.AppendFormat("{0}", items.data_lancamento.ToString("yyyyMMdd"));
                                        l.AppendFormat("{0:D12}", extrato_item.nsu_rede_long);
                                        l.AppendFormat("{0}", (itemsRede.is_numero_autorizacao ?? string.Empty).PadRight(12)); // Autorizacao
                                        l.AppendFormat("{0:D2}", Convert.ToInt64(items.numero_parcela.Substring(3, 2))); // Plano
                                        l.AppendFormat("{0:D2}", Convert.ToInt64(items.numero_parcela.Substring(0, 2))); // Parcela
                                        l.AppendFormat("{0:D2}", Convert.ToInt64(Operadora(extrato_item.operadora_desc.ToUpper())));
                                        l.AppendFormat("{0:D2}", Convert.ToInt64(Bandeira((items.bandeira ?? string.Empty).ToUpper().Trim())));
                                        l.AppendFormat("{0}", "C");
                                        l.AppendFormat("{0:D13}", Convert.ToInt64(itemsRede.is_valor_bruto.ToString().Replace(",", "").Replace(".", "")));
                                        l.AppendFormat("{0:D13}", Convert.ToInt64(itemsRede.is_valor_desconto.ToString().Replace(",", "").Replace(".", "")));
                                        l.AppendFormat("{0:D13}", Convert.ToInt64(items.valor_lancamento.ToString().Replace(",", "").Replace(".", "")));
                                        l.AppendFormat("{0:D20}", itemsRede.is_numero_resumo_vendas_long);
                                        l.AppendFormat("{0}", extrato_item.area_cliente.PadRight(50));
                                        l.AppendFormat("{0:D4}", Convert.ToInt64(items.banco));
                                        l.AppendFormat("{0:D6}", Convert.ToInt64(items.agencia));
                                        l.AppendFormat("{0:D13}", Convert.ToInt64(items.conta_corrente));
                                        l.AppendFormat("{0:D6}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                        l.AppendFormat("{0}", extrato_item.nm_estabelecimento.Trim().PadRight(20)); // LOJA
                                        l.AppendFormat("{0:D20}", itemsRede.is_numero_filiacao_pv); // Estabelecimento
                                        l.AppendFormat("{0}", "ANT");
                                        l.AppendFormat("{0:D13}", Convert.ToInt64(itemsRede.is_valor_liquido.ToString().Replace(",", "").Replace(".", "")));
                                        l.AppendFormat("{0:D15}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                        l.AppendFormat("{0}", string.Empty.PadLeft(11)); // Reservado
                                        sw.WriteLine(l.ToString());
                                        l.Clear();

                                        totBruto = totBruto + itemsRede.is_valor_bruto;
                                        totLiquido = totLiquido + itemsRede.is_valor_liquido;
                                        totAntecipado = totAntecipado + items.valor_lancamento;

                                    }
                                    else
                                    {
                                        var arq = new ArquivoDeCriticas();
                                        arq.ds_motivo = "Não encontrado Extrato ERP!";
                                        arq.ds_arquivo = arquivo;
                                        arq.dt_credito = items.data_lancamento;
                                        arq.dt_transacao = itemsRede.is_data_cv;
                                        arq.numero_resumo_venda = itemsRede.is_numero_resumo_vendas.ToString();
                                        arq.nsu_rede = itemsRede.is_numero_cv.ToString();
                                        arq.vl_bruto = itemsRede.is_valor_bruto;
                                        arq.vl_liquido = itemsRede.is_valor_liquido;
                                        arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                        ListCriticas.Add(arq);
                                    }

                                }

                            }
                        }



                    }

                    //RVVALOR.OrderByDescending(x => x.RV);
                    l.AppendFormat("{0}", 9);
                    bruto = bruto;
                    l.AppendFormat("{0:D6}", contador + 2);
                    l.AppendFormat("{0:D13}", Convert.ToInt64(totBruto.ToString().Replace(",", "").Replace(".", "")));
                    l.AppendFormat("{0:D13}", Convert.ToInt64((totBruto - totLiquido).ToString().Replace(",", "").Replace(".", "")));
                    l.AppendFormat("{0:D13}", Convert.ToInt64((totAntecipado).ToString().Replace(",", "").Replace(".", "")));
                    l.AppendFormat("{0:D13}", 0);
                    l.AppendFormat("{0:D13}", 0);
                    l.AppendFormat("{0}", " ".PadLeft(168));
                    sw.WriteLine(l.ToString());
                    // swerros.WriteLine(l.ToString());
                    l.Clear();
                }

                // swerros.Dispose();
                
                model.ArquivosCriticas = ListCriticas;
                DAL.GravarList(ListCriticas);

                ViewBag.QtdPagamento = model.ArquivosPagamento.Count();
                ViewBag.TotalPagamentoBruto = model.ArquivosPagamento.Sum(x => x.Valor_bruto).ToString("#,##0.00");
                ViewBag.TotalPagamentoLiquido = model.ArquivosPagamento.Sum(x => x.Valor_liquido).ToString("#,##0.00");

                ViewBag.QtdCriticas = model.ArquivosCriticas.Count();
                ViewBag.TotalCriticasBruto = model.ArquivosCriticas.Sum(x => x.vl_bruto).ToString("#,##0.00");
                ViewBag.TotalCriticasLiquido = model.ArquivosCriticas.Sum(x => x.vl_liquido).ToString("#,##0.00");

                ViewBag.QtdGeral = ViewBag.QtdPagamento + ViewBag.QtdCriticas;
                ViewBag.TotalGeralBruto = (model.ArquivosPagamento.Sum(x => x.Valor_bruto) + model.ArquivosCriticas.Sum(x => x.vl_bruto)).ToString("#,##0.00");
                ViewBag.TotalGeralLiquido = (model.ArquivosPagamento.Sum(x => x.Valor_liquido) + model.ArquivosCriticas.Sum(x => x.vl_liquido)).ToString("#,##0.00");


                /* Response.Clear();
                 Response.ContentType = "text/plain";
                 Response.AppendHeader("Content-Disposition", String.Format("attachment;filename={0}", arquivo));
                 Response.TransmitFile(file);
                 Response.End(); */



            }
            catch (Exception e)
            {
            }

            return View(model);

        }

        public ActionResult TransacoesTEF()
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
                tp_situacao = 2
            };
            ViewBag.tp_administradora = Models.AdministradoraTypes.getAdministradoras("");
            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            return View(model);
        }

        [HttpPost]
        public ActionResult TransacoesTEF(ArquivoDeCartoesTEF obj)
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
                filtro_resumo = obj.filtro_resumo
            };

            var conta = new BaseID();
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


            }

            ViewBag.tp_data = Models.DataTypes.getDataTypes();

            ViewBag.tp_administradora = Models.AdministradoraTypes.getAdministradoras("");

            return View(model);
        }


        private string Bandeira(string bandeira)
        {
            string strprod = "00";
            switch (bandeira.ToUpper().Trim())
            {
                case "VISA":
                    return "01";
                case "VISA ELECTRON":
                    return "01";
                case "MASTERCARD":
                    return "02";
                case "MAESTRO":
                    return "02";
                case "HIPERCARD":
                    return "03";
                case "CREDISHOP":
                    return "04";
                case "DINERS CLUB":
                    return "05";
                case "FORTBRASIL":
                    return "06";
                case "ELO":
                    return "07";
                case "CABAL":
                    return "08";
                case "AMEX":
                    return "09";
                case "AVISTA":
                    return "12";
                case "BANESECARD":
                    return "13";
                case "CREDSYSTEM":
                    return "14";
                case "CREDZ":
                    return "15";
                case "CUP":
                    return "16";
                case "DISCOVER":
                    return "17";
                case "SICRED":
                    return "21";
                case "SODEXO":
                    return "22";
                case "SOROCRED":
                    return "23";
                case "DINERS":
                    return "27";
                case "BANRICARD":
                    return "33";
                case "BANRICOMPRAS":
                    return "35";
                case "SENFF":
                    return "47";
            }
            return strprod;
        }

        private string Operadora(string operadora)
        {
            string strprod = "00";
            switch (operadora.ToUpper().Trim())
            {
                case "AMEX":
                    return "01";
                case "CIELO":
                    return "02";
                case "FORTBRASIL":
                    return "04";
                case "HIPERCARD":
                    return "05";
                case "REDECARD":
                    return "06";
                case "SANTANDER":
                    return "07";
                case "GOODCARD":
                    return "08";
                case "BANESECARD":
                    return "11";
                case "TICKET":
                    return "12";
                case "FIRSTDATA":
                    return "13";
                case "AVISTA":
                    return "14";
                case "TRICARD":
                    return "15";
                case "SODEXO":
                    return "17";
                case "BANESCARD":
                    return "28";
                case "CABAL":
                    return "29";
                case "USECRED":
                    return "30";
                case "SENFF":
                    return "34";
                case "STONE":
                    return "35";
                case "BNB":
                    return "37";
                case "BANRICARD":
                    return "41";
                case "GREENCARD":
                    return "42";
                case "CREDSYSTEM":
                    return "73";
            }
            return strprod;
        }


        [HttpPost]
        public JsonResult MovimentacoesPagamento2(TransacaoRedeViewModel model)
        {
            //  List<rvValor> RVVALOR = new List<rvValor>();
            ViewBag.tp_data = Models.DataTypes.getDataTypes();
            ViewBag.tp_cartao = Models.CartaoTypes.getCartaoTypes();
            ViewBag.tp_banco = Models.BancoTypes.getBancoTypes(model.tp_banco);
            ViewBag.tp_movimento = Models.MovimentacaoTypes.getMovimentacaoTypes(model.tp_movimento);

            var dir = Server.MapPath("~\\TXT");
            var arquivo = "MOVIMENT_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_PAG.txt";
            var file = Path.Combine(dir, arquivo);
            Directory.CreateDirectory(dir);

            Cookie.Delete("arquivoerropagamento");
            Cookie.Set("arquivoerropagamento", arquivo);
            List<ArquivoDeCriticas> ListCriticas = new List<ArquivoDeCriticas>();
            StringBuilder l = new StringBuilder();
            StringBuilder l_erros = new StringBuilder();


            var contador = 0;
            decimal totBruto = 0;
            decimal totLiquido = 0;
            decimal totAntecipado = 0;

            decimal totParcelas = 0;

            string linha = "";

            using (StreamWriter sw = System.IO.File.CreateText(file))
            {
                // StreamWriter swerros = System.IO.File.CreateText(Path.Combine(dir, "MOVIMENT_" + DateTime.Now.ToString("yyyyMMdd") + "_PAG_CRITICA.txt"));


                linha = linha + string.Format("{0}", 0);
                linha = linha + string.Format("{0:D3}", "005");
                linha = linha + string.Format("{0}", (model.DataInicio ?? DateTime.MinValue).ToString("yyyyMMdd"));
                linha = linha + string.Format("{0}", "".PadLeft(16));
                linha = linha + string.Format("{0}", (model.DataInicio ?? DateTime.MinValue).ToString("yyyyMMdd"));
                linha = linha + string.Format("{0}", (model.DataFinal ?? DateTime.MinValue).ToString("yyyyMMdd"));
                linha = linha + string.Format("{0}", "ALLPAYMENTS".PadRight(13));
                linha = linha + string.Format("{0}", "LOGIN".PadRight(13));
                linha = linha + string.Format("{0:D6}", 1);
                linha = linha + string.Format("{0}", " ".PadRight(164));
                sw.WriteLine(linha);
                // swerros.WriteLine(l.ToString());
                //l.Clear();
                linha = "";

                var conta = new BaseID();
                decimal bruto = 0;

                if ((model.tp_movimento == 1) || (model.tp_movimento == 0))
                {


                    if ((model.tp_cartao == 0) || (model.tp_cartao == 3))
                    {

                        List<ConciliacaoUseRedeEEFICreditosStructReduzido> MovFin = DAL.ListarObjetos<ConciliacaoUseRedeEEFICreditosStructReduzido>(
                        string.Format("id_conta = {3} and data_lancamento between '{0}' and '{1}'  {2} ", (model.DataInicio ?? DateTime.MinValue).ToString("yyyy-MM-dd"),
                            (model.DataFinal ?? DateTime.MaxValue).ToString("yyyy-MM-dd"),

                            (model.tp_banco > 0 && model.tp_banco < 1000) ? "and banco =" + Convert.ToInt64(model.tp_banco.ToString().Substring(0, 3)) :
                            (model.tp_banco > 0 && model.tp_banco > 1000) ? "and banco =" + model.tp_banco.ToString().Substring(0, 3) + " and cast(conta_corrente as unsigned)=" + model.tp_banco.ToString().Substring(3, 6) :


                            "", UsuarioLogado.IdConta), "  data_lancamento ");



                        foreach (var items in MovFin)
                        {

                            List<ConciliacaoUseRedeEEVCComprovanteVendaStructListar> CTransacaoRedeCredito =
                                DAL.ListarObjetos<ConciliacaoUseRedeEEVCComprovanteVendaStructListar>(string.Format("cast(numero_resumo_venda as unsigned)={0} and id_conta={1} ", items.numero_rv, UsuarioLogado.IdConta, Convert.ToInt32(items.numero_parcela.Substring(0, 2))));


                            /*ConciliacaoUseRedeEEFIAntecipacaoStruct CTransacaoAntecipacao =
                                DAL.GetObjeto<ConciliacaoUseRedeEEFIAntecipacaoStruct>(string.Format("numero_rv_correspondente={0} and numero_parcela='{1}' and id_conta={2}", items.numero_rv, items.numero_parcela, UsuarioLogado.IdConta)) ?? new ConciliacaoUseRedeEEFIAntecipacaoStruct(); */


                            foreach (var itemsRede in CTransacaoRedeCredito)
                            {

                                TransacaoEstabelecimentoListar extrato_item;

                                if (itemsRede.is_tipo_registro == 12)
                                {

                                    if (!(itemsRede.is_numero_parcelas >= Convert.ToInt32(items.numero_parcela.Substring(0, 2))))
                                        continue;


                                    extrato_item =
                                         DAL.GetObjeto<TransacaoEstabelecimentoListar>(string.Format("cast(nsu_rede as unsigned)={0} and id_conta={1}",
                                             itemsRede.is_numero_cv_long, UsuarioLogado.IdConta));


                                    // List<ConciliacaoUseRedeParcelasStructListar> CTransacaoRedeParcelas = DAL.ListarObjetos<ConciliacaoUseRedeParcelasStructListar>(string.Format("cast(numero_resumo_venda as unsigned)={0} and id_conta={1} and numero_parcela={2} and cast(numero_filiacao_pv as unsigned) ={3} ", items.numero_rv, UsuarioLogado.IdConta, Convert.ToInt32(items.numero_parcela.Substring(0, 2)), items.numero_pv_original ));
                                    var parcliq = Convert.ToInt64(items.numero_parcela.Substring(0, 2)) == 1 ? itemsRede.is_valor_liquido_primeira_parc : itemsRede.is_valor_liquido_demais_parc;
                                    var valordesconto = Math.Round((itemsRede.is_valor_desconto / itemsRede.is_numero_parcelas), 2);
                                    var parbrut = parcliq + valordesconto;

                                    bruto = bruto + valordesconto - (itemsRede.is_valor_desconto / itemsRede.is_numero_parcelas);


                                    if ((itemsRede.is_numero_autorizacao ?? string.Empty).Equals(""))
                                    {
                                        var arq = new ArquivoDeCriticas();
                                        arq.ds_motivo = "Sem código de Autorização - Crédito!";
                                        arq.ds_arquivo = arquivo;
                                        arq.dt_credito = items.data_lancamento;
                                        arq.dt_transacao = itemsRede.is_data_cv;
                                        arq.numero_resumo_venda = itemsRede.is_numero_resumo_vendas.ToString();
                                        arq.nsu_rede = itemsRede.is_numero_cv.ToString();
                                        arq.vl_bruto = parbrut;
                                        arq.vl_liquido = parcliq;
                                        arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                        ListCriticas.Add(arq);
                                    }
                                    else if (extrato_item != null)
                                    {
                                        contador++;
                                        linha = linha + string.Format("{0}", 5);
                                        linha = linha + string.Format("{0}", extrato_item.dt_transacao.ToString("yyyyMMdd"));
                                        linha = linha + string.Format("{0}", items.data_lancamento.ToString("yyyyMMdd"));
                                        linha = linha + string.Format("{0:D12}", extrato_item.nsu_rede_long);
                                        linha = linha + string.Format("{0}", (itemsRede.is_numero_autorizacao ?? string.Empty).PadRight(12)); // Autorizacao
                                        linha = linha + string.Format("{0:D2}", Convert.ToInt64(itemsRede.is_numero_parcelas)); // Plano
                                        linha = linha + string.Format("{0:D2}", Convert.ToInt32(items.numero_parcela.Substring(0, 2))); // Parcela
                                        linha = linha + string.Format("{0:D2}", Convert.ToInt64(Operadora(extrato_item.operadora_desc.ToUpper())));
                                        linha = linha + string.Format("{0:D2}", Convert.ToInt64(Bandeira((items.bandeira ?? string.Empty).ToUpper().Trim())));
                                        linha = linha + string.Format("{0}", "C");
                                        linha = linha + string.Format("{0:D13}", Convert.ToInt64(parbrut.ToString().Replace(",", "").Replace(".", "")));
                                        linha = linha + string.Format("{0:D13}", Convert.ToInt64(valordesconto.ToString().Replace(",", "").Replace(".", "")));
                                        linha = linha + string.Format("{0:D13}", 0);
                                        linha = linha + string.Format("{0:D20}", itemsRede.is_numero_resumo_vendas_long);
                                        linha = linha + string.Format("{0}", extrato_item.area_cliente.PadRight(50));
                                        linha = linha + string.Format("{0:D4}", Convert.ToInt64(items.banco));
                                        linha = linha + string.Format("{0:D6}", Convert.ToInt64(items.agencia));
                                        linha = linha + string.Format("{0:D13}", Convert.ToInt64(items.conta_corrente));
                                        linha = linha + string.Format("{0:D6}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                        linha = linha + string.Format("{0}", extrato_item.nm_estabelecimento.Trim().PadRight(20)); // LOJA
                                        linha = linha + string.Format("{0:D20}", itemsRede.is_numero_filiacao_pv); // Estabelecimento
                                        linha = linha + string.Format("{0}", "PAG");
                                        linha = linha + string.Format("{0:D13}", Convert.ToInt64(parcliq.ToString().Replace(",", "").Replace(".", "")));
                                        linha = linha + string.Format("{0:D15}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                        linha = linha + string.Format("{0}", string.Empty.PadLeft(11)); // Reservado
                                        sw.WriteLine(linha);
                                        //l.Clear();
                                        linha = "";

                                        totBruto = totBruto + parbrut;
                                        totLiquido = totLiquido + parcliq;
                                        // totAntecipado = totAntecipado + CTransacaoAntecipacao.valor_lancamento;

                                        /*  var RVEncontrado = RVVALOR.FirstOrDefault(d => d.RV == items.numero_rv);
                                          if (RVEncontrado != null)
                                          {
                                              RVEncontrado.valorbruto = RVEncontrado.valorbruto + parbrut;
                                              RVEncontrado.valorliquido = RVEncontrado.valorliquido +
                                                                          parcliq;

                                          }
                                          else
                                          {

                                              var item = new rvValor
                                              {
                                                  RV = items.numero_rv,
                                                  valorbruto = parbrut,
                                                  valorliquido = parcliq

                                              };
                                              RVVALOR.Add(item);

                                          }
                                          RVVALOR.OrderByDescending(x => x.RV);*/


                                    }
                                    else
                                    {
                                        var arq = new ArquivoDeCriticas();
                                        arq.ds_motivo = "Não encontrado Extrato ERP!";
                                        arq.ds_arquivo = arquivo;
                                        arq.dt_credito = items.data_lancamento;
                                        arq.dt_transacao = itemsRede.is_data_cv;
                                        arq.numero_resumo_venda = itemsRede.is_numero_resumo_vendas.ToString();
                                        arq.nsu_rede = itemsRede.is_numero_cv.ToString();
                                        arq.vl_bruto = parbrut;
                                        arq.vl_liquido = parcliq;
                                        arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                        ListCriticas.Add(arq);
                                    }
                                }
                                else
                                {
                                    extrato_item =
                                         DAL.GetObjeto<TransacaoEstabelecimentoListar>(string.Format("cast(nsu_rede as unsigned)={0} and id_conta={1}",
                                             itemsRede.is_numero_cv_long, UsuarioLogado.IdConta));


                                    if ((itemsRede.is_numero_autorizacao ?? string.Empty).Equals(""))
                                    {
                                        var arq = new ArquivoDeCriticas();
                                        arq.ds_motivo = "Sem código de Autorização - Crédito!";
                                        arq.ds_arquivo = arquivo;
                                        arq.dt_credito = items.data_lancamento;
                                        arq.dt_transacao = itemsRede.is_data_cv;
                                        arq.numero_resumo_venda = itemsRede.is_numero_resumo_vendas.ToString();
                                        arq.nsu_rede = itemsRede.is_numero_cv.ToString();
                                        arq.vl_bruto = itemsRede.is_valor_bruto;
                                        arq.vl_liquido = itemsRede.is_valor_liquido;
                                        arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                        ListCriticas.Add(arq);
                                        continue;
                                    }
                                    else if (extrato_item != null)
                                    {
                                        contador++;
                                        linha = linha + string.Format("{0}", 5);
                                        linha = linha + string.Format("{0}", extrato_item.dt_transacao.ToString("yyyyMMdd"));
                                        linha = linha + string.Format("{0}", items.data_lancamento.ToString("yyyyMMdd"));
                                        linha = linha + string.Format("{0:D12}", extrato_item.nsu_rede_long);
                                        linha = linha + string.Format("{0}", (itemsRede.is_numero_autorizacao ?? string.Empty).PadRight(12));
                                        // Autorizacao
                                        linha = linha + string.Format("{0:D2}", Convert.ToInt64(items.numero_parcela.Substring(3, 2)));
                                        // Plano
                                        linha = linha + string.Format("{0:D2}", Convert.ToInt64(items.numero_parcela.Substring(0, 2)));
                                        // Parcela
                                        linha = linha + string.Format("{0:D2}", Convert.ToInt64(Operadora(extrato_item.operadora_desc.ToUpper())));
                                        linha = linha + string.Format("{0:D2}", Convert.ToInt64(Bandeira((items.bandeira ?? string.Empty).ToUpper().Trim())));
                                        linha = linha + string.Format("{0}", "C");
                                        linha = linha + string.Format("{0:D13}", Convert.ToInt64(itemsRede.is_valor_bruto.ToString().Replace(",", "").Replace(".", "")));
                                        linha = linha + string.Format("{0:D13}", Convert.ToInt64(itemsRede.is_valor_desconto.ToString().Replace(",", "").Replace(".", "")));
                                        linha = linha + string.Format("{0:D13}", 0);
                                        linha = linha + string.Format("{0:D20}", itemsRede.is_numero_resumo_vendas_long);
                                        linha = linha + string.Format("{0}", extrato_item.area_cliente.PadRight(50));
                                        linha = linha + string.Format("{0:D4}", Convert.ToInt64(items.banco));
                                        linha = linha + string.Format("{0:D6}", Convert.ToInt64(items.agencia));
                                        linha = linha + string.Format("{0:D13}", Convert.ToInt64(items.conta_corrente));
                                        linha = linha + string.Format("{0:D6}", Convert.ToInt64(extrato_item.cod_loja));
                                        // CODIGO DA LOJA
                                        linha = linha + string.Format("{0}", extrato_item.nm_estabelecimento.Trim().PadRight(20));
                                        // LOJA
                                        linha = linha + string.Format("{0:D20}", itemsRede.is_numero_filiacao_pv);
                                        // Estabelecimento
                                        linha = linha + string.Format("{0}", "PAG");
                                        linha = linha + string.Format("{0:D13}", Convert.ToInt64(itemsRede.is_valor_liquido.ToString().Replace(",", "").Replace(".", "")));
                                        linha = linha + string.Format("{0:D15}", Convert.ToInt64(extrato_item.cod_loja));
                                        // CODIGO DA LOJA
                                        linha = linha + string.Format("{0}", string.Empty.PadLeft(11)); // Reservado
                                        sw.WriteLine(linha);
                                        linha = "";
                                        //l.Clear();

                                        totBruto = totBruto + itemsRede.is_valor_bruto;
                                        totLiquido = totLiquido + itemsRede.is_valor_liquido;
                                        //totAntecipado = totAntecipado + CTransacaoAntecipacao.valor_lancamento;


                                        /* var RVEncontrado = RVVALOR.FirstOrDefault(d => d.RV == items.numero_rv);
                                         if (RVEncontrado != null)
                                         {
                                             RVEncontrado.valorbruto = RVEncontrado.valorbruto + itemsRede.is_valor_bruto;
                                             RVEncontrado.valorliquido = RVEncontrado.valorliquido +
                                                                         itemsRede.is_valor_liquido;

                                         }
                                         else
                                         {

                                             var item = new rvValor
                                             {
                                                 RV = items.numero_rv,
                                                 valorbruto = itemsRede.is_valor_bruto,
                                                 valorliquido = itemsRede.is_valor_liquido
                                             };
                                             RVVALOR.Add(item);

                                         }
                                         RVVALOR.OrderByDescending(x => x.RV);*/


                                    }
                                    else
                                    {
                                        var arq = new ArquivoDeCriticas();
                                        arq.ds_motivo = "Não encontrado Extrato ERP!";
                                        arq.ds_arquivo = arquivo;
                                        arq.dt_credito = items.data_lancamento;
                                        arq.dt_transacao = itemsRede.is_data_cv;
                                        arq.numero_resumo_venda = itemsRede.is_numero_resumo_vendas.ToString();
                                        arq.nsu_rede = itemsRede.is_numero_cv.ToString();
                                        arq.vl_bruto = itemsRede.is_valor_bruto;
                                        arq.vl_liquido = itemsRede.is_valor_liquido;
                                        arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                        ListCriticas.Add(arq);
                                        continue;
                                    }



                                }

                            }
                        }
                    }

                    if ((model.tp_cartao == 1) || (model.tp_cartao == 3))
                    {


                        List<ConciliacaoUseRedeEEVDResumoOperacaoStructListar> CTransacaoResumoOperacaoRedeDebito =
                         DAL.ListarObjetos<ConciliacaoUseRedeEEVDResumoOperacaoStructListar>(

                            string.Format("id_conta = {3} and data_credito between '{0}' and '{1}'  {2} ", (model.DataInicio ?? DateTime.MinValue).ToString("yyyy-MM-dd"),
                                        (model.DataFinal ?? DateTime.MaxValue).ToString("yyyy-MM-dd"),

                                        (model.tp_banco > 0 && model.tp_banco < 1000) ? "and banco =" + Convert.ToInt64(model.tp_banco.ToString().Substring(0, 3)) :
                                        (model.tp_banco > 0 && model.tp_banco > 1000) ? "and banco =" + model.tp_banco.ToString().Substring(0, 3) + " and cast(conta_corrente as unsigned)=" + model.tp_banco.ToString().Substring(3, 6) :


                                        "", UsuarioLogado.IdConta)

                             );


                        foreach (var itemsResumoRede in CTransacaoResumoOperacaoRedeDebito)
                        {

                            List<ConciliacaoUseRedeEEVDComprovanteVendaStructListar> CTransacaoComprovantesRedeDebito =
                             DAL.ListarObjetos<ConciliacaoUseRedeEEVDComprovanteVendaStructListar>(string.Format("cast(numero_resumo_venda as unsigned)={0} and id_conta={1}", itemsResumoRede.is_numero_resumo_venda_long, UsuarioLogado.IdConta));

                            foreach (var itemsComprovantesRede in CTransacaoComprovantesRedeDebito)
                            {

                                TransacaoEstabelecimentoListar extrato_item = DAL.GetObjeto<TransacaoEstabelecimentoListar>(string.Format("cast(nsu_rede as unsigned)={0} and id_conta={1}",
                                     itemsComprovantesRede.is_numero_cv_long, UsuarioLogado.IdConta));

                                var rtef = TEFDAL.GetStatusTEF(conta.IdConta, itemsComprovantesRede.is_numero_cv_long.ToString()) ??
                                               new RetornoTEF();

                                if ((!(rtef.Autorizacao ?? string.Empty).Equals("") ? (rtef.Autorizacao ?? string.Empty) : (extrato_item.is_autorizacao ?? string.Empty)).Equals(""))
                                {
                                    var arq = new ArquivoDeCriticas();
                                    arq.ds_motivo = "Sem código de Autorização - Débito!";
                                    arq.ds_arquivo = arquivo;
                                    arq.dt_credito = extrato_item.dt_transacao;
                                    arq.dt_transacao = itemsComprovantesRede.is_data_credito;
                                    arq.numero_resumo_venda = itemsComprovantesRede.is_numero_resumo_vendas.ToString();
                                    arq.nsu_rede = extrato_item.nsu_rede.ToString();
                                    arq.vl_bruto = itemsComprovantesRede.is_valor_bruto;
                                    arq.vl_liquido = itemsComprovantesRede.is_valor_liquido;
                                    arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                    ListCriticas.Add(arq);
                                    continue;
                                }
                                else if (extrato_item != null)
                                {


                                    contador++;
                                    linha = linha + string.Format("{0}", 5);
                                    linha = linha + string.Format("{0}", extrato_item.dt_transacao.ToString("yyyyMMdd"));
                                    linha = linha + string.Format("{0}", itemsComprovantesRede.is_data_credito.ToString("yyyyMMdd"));
                                    linha = linha + string.Format("{0:D12}", extrato_item.nsu_rede_long);
                                    linha = linha + string.Format("{0}", (!(rtef.Autorizacao ?? string.Empty).Equals("") ? (rtef.Autorizacao ?? string.Empty) : (extrato_item.is_autorizacao ?? string.Empty)).PadRight(12)); // Autorizacao
                                    linha = linha + string.Format("{0:D2}", 1); // Plano
                                    linha = linha + string.Format("{0}", "01"); // Parcela
                                    linha = linha + string.Format("{0:D2}", Convert.ToInt64(Operadora(extrato_item.operadora_desc.ToUpper())));
                                    linha = linha + string.Format("{0:D2}", Convert.ToInt64(Bandeira((itemsComprovantesRede.is_bandeira ?? string.Empty).ToUpper().Trim())));
                                    linha = linha + string.Format("{0}", "D");
                                    linha = linha + string.Format("{0:D13}", Convert.ToInt64(itemsComprovantesRede.is_valor_bruto.ToString().Replace(",", "").Replace(".", "")));
                                    linha = linha + string.Format("{0:D13}", Convert.ToInt64(itemsComprovantesRede.is_valor_desconto.ToString().Replace(",", "").Replace(".", "")));
                                    linha = linha + string.Format("{0:D13}", 0);
                                    linha = linha + string.Format("{0:D20}", itemsComprovantesRede.is_numero_resumo_vendas_long);
                                    linha = linha + string.Format("{0}", extrato_item.area_cliente.PadRight(50));
                                    linha = linha + string.Format("{0:D4}", Convert.ToInt64(itemsResumoRede.is_banco));
                                    linha = linha + string.Format("{0:D6}", Convert.ToInt64(itemsResumoRede.is_agencia));
                                    linha = linha + string.Format("{0:D13}", Convert.ToInt64(itemsResumoRede.is_conta_corrente));
                                    linha = linha + string.Format("{0:D6}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                    linha = linha + string.Format("{0}", extrato_item.nm_estabelecimento.Trim().PadRight(20)); // LOJA
                                    linha = linha + string.Format("{0:D20}", itemsComprovantesRede.is_numero_filiacao_pv); // Estabelecimento
                                    linha = linha + string.Format("{0}", "PAG");
                                    linha = linha + string.Format("{0:D13}", Convert.ToInt64(itemsComprovantesRede.is_valor_liquido.ToString().Replace(",", "").Replace(".", "")));
                                    linha = linha + string.Format("{0:D15}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                    linha = linha + string.Format("{0}", string.Empty.PadLeft(11)); // Reservado
                                    sw.WriteLine(linha);
                                    linha = ""; //l.Clear();

                                    totBruto = totBruto + itemsComprovantesRede.is_valor_bruto;
                                    totLiquido = totLiquido + itemsComprovantesRede.is_valor_liquido;
                                }
                                else
                                {
                                    var arq = new ArquivoDeCriticas();
                                    arq.ds_motivo = "Não encontrado Extrato ERP!";
                                    arq.ds_arquivo = arquivo;
                                    arq.dt_credito = extrato_item.dt_transacao;
                                    arq.dt_transacao = itemsComprovantesRede.is_data_cv;
                                    arq.numero_resumo_venda = itemsComprovantesRede.is_numero_resumo_vendas.ToString();
                                    arq.nsu_rede = itemsComprovantesRede.is_numero_cv.ToString();
                                    arq.vl_bruto = itemsComprovantesRede.is_valor_bruto;
                                    arq.vl_liquido = itemsComprovantesRede.is_valor_liquido;
                                    arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                    ListCriticas.Add(arq);
                                    continue;
                                }
                            }
                        }
                    }
                }

                if ((model.tp_movimento == 2) || (model.tp_movimento == 0))
                {
                    List<ConciliacaoUseRedeEEFIAntecipacaoStructListar> MovFin = DAL.ListarObjetos<ConciliacaoUseRedeEEFIAntecipacaoStructListar>(
                    string.Format("id_conta = {3} and data_lancamento between '{0}' and '{1}'  {2} ", (model.DataInicio ?? DateTime.MinValue).ToString("yyyy-MM-dd"),
                        (model.DataFinal ?? DateTime.MaxValue).ToString("yyyy-MM-dd"),

                        (model.tp_banco > 0 && model.tp_banco < 1000) ? "and banco =" + Convert.ToInt64(model.tp_banco.ToString().Substring(0, 3)) :
                        (model.tp_banco > 0 && model.tp_banco > 1000) ? "and banco =" + model.tp_banco.ToString().Substring(0, 3) + " and cast(conta_corrente as unsigned)=" + model.tp_banco.ToString().Substring(3, 6) :


                        "", UsuarioLogado.IdConta));


                    foreach (var items in MovFin)
                    {
                        List<ConciliacaoUseRedeEEVCComprovanteVendaStructListar> CTransacaoRedeCredito =
                            DAL.ListarObjetos<ConciliacaoUseRedeEEVCComprovanteVendaStructListar>(string.Format("cast(numero_resumo_venda as unsigned)={0} and id_conta={1} ", items.numero_rv_correspondente, UsuarioLogado.IdConta, items.numero_parcela.Substring(0, 2)));

                        /*ConciliacaoUseRedeEEFIAntecipacaoStruct CTransacaoAntecipacao =
                            DAL.GetObjeto<ConciliacaoUseRedeEEFIAntecipacaoStruct>(string.Format("numero_rv_correspondente={0} and numero_parcela='{1}' and id_conta={2}", items.numero_rv, items.numero_parcela, UsuarioLogado.IdConta)) ?? new ConciliacaoUseRedeEEFIAntecipacaoStruct(); */


                        foreach (var itemsRede in CTransacaoRedeCredito)
                        {

                            TransacaoEstabelecimentoListar extrato_item;

                            if (itemsRede.is_tipo_registro == 12)
                            {

                                extrato_item =
                                     DAL.GetObjeto<TransacaoEstabelecimentoListar>(string.Format("cast(nsu_rede as unsigned)={0} and id_conta={1}",
                                         itemsRede.is_numero_cv_long, UsuarioLogado.IdConta));

                                if (!(itemsRede.is_numero_parcelas >= Convert.ToInt32(items.numero_parcela.Substring(0, 2))))
                                    continue;

                                var parcliq = Convert.ToInt64(items.numero_parcela.Substring(0, 2)) == 1 ? itemsRede.is_valor_liquido_primeira_parc : itemsRede.is_valor_liquido_demais_parc;
                                var valordesconto = Math.Round((itemsRede.is_valor_desconto / itemsRede.is_numero_parcelas), 2);
                                var parbrut = parcliq + valordesconto;

                                if (extrato_item != null)
                                {
                                    contador++;
                                    linha = linha + string.Format("{0}", 5);
                                    linha = linha + string.Format("{0}", extrato_item.dt_transacao.ToString("yyyyMMdd"));
                                    linha = linha + string.Format("{0}", items.data_lancamento.ToString("yyyyMMdd"));
                                    linha = linha + string.Format("{0:D12}", extrato_item.nsu_rede_long);
                                    linha = linha + string.Format("{0}", (itemsRede.is_numero_autorizacao ?? string.Empty).PadRight(12)); // Autorizacao
                                    linha = linha + string.Format("{0:D2}", Convert.ToInt64(items.numero_parcela.Substring(3, 2))); // Plano
                                    linha = linha + string.Format("{0:D2}", Convert.ToInt64(items.numero_parcela.Substring(0, 2))); // Parcela
                                    linha = linha + string.Format("{0:D2}", Convert.ToInt64(Operadora(extrato_item.operadora_desc.ToUpper())));
                                    linha = linha + string.Format("{0:D2}", Convert.ToInt64(Bandeira((items.bandeira ?? string.Empty).ToUpper().Trim())));
                                    linha = linha + string.Format("{0}", "C");
                                    linha = linha + string.Format("{0:D13}", Convert.ToInt64(parbrut.ToString().Replace(",", "").Replace(".", "")));
                                    linha = linha + string.Format("{0:D13}", Convert.ToInt64(valordesconto.ToString().Replace(",", "").Replace(".", "")));
                                    linha = linha + string.Format("{0:D13}", Convert.ToInt64(items.valor_lancamento.ToString().Replace(",", "").Replace(".", "")));
                                    linha = linha + string.Format("{0:D20}", itemsRede.is_numero_resumo_vendas_long);
                                    linha = linha + string.Format("{0}", extrato_item.area_cliente.PadRight(50));
                                    linha = linha + string.Format("{0:D4}", Convert.ToInt64(items.banco));
                                    linha = linha + string.Format("{0:D6}", Convert.ToInt64(items.agencia));
                                    linha = linha + string.Format("{0:D13}", Convert.ToInt64(items.conta_corrente));
                                    linha = linha + string.Format("{0:D6}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                    linha = linha + string.Format("{0}", extrato_item.nm_estabelecimento.Trim().PadRight(20)); // LOJA
                                    linha = linha + string.Format("{0:D20}", itemsRede.is_numero_filiacao_pv); // Estabelecimento
                                    linha = linha + string.Format("{0}", "ANT");
                                    linha = linha + string.Format("{0:D13}", Convert.ToInt64(parcliq.ToString().Replace(",", "").Replace(".", "")));
                                    linha = linha + string.Format("{0:D15}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                    linha = linha + string.Format("{0}", string.Empty.PadLeft(11)); // Reservado
                                    sw.WriteLine(linha);
                                    //l.Clear();
                                    linha = "";

                                    totBruto = totBruto + parbrut;
                                    totLiquido = totLiquido + parcliq;
                                    totAntecipado = totAntecipado + items.valor_lancamento;
                                }
                                else
                                {
                                    var arq = new ArquivoDeCriticas();
                                    arq.ds_motivo = "Não encontrado Extrato ERP!";
                                    arq.ds_arquivo = arquivo;
                                    arq.dt_credito = items.data_lancamento;
                                    arq.dt_transacao = itemsRede.is_data_cv;
                                    arq.numero_resumo_venda = itemsRede.is_numero_resumo_vendas.ToString();
                                    arq.nsu_rede = itemsRede.is_numero_cv.ToString();
                                    arq.vl_bruto = parbrut;
                                    arq.vl_liquido = parcliq;
                                    arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                    ListCriticas.Add(arq);
                                    continue;
                                }



                            }
                            else
                            {
                                extrato_item =
                                   DAL.GetObjeto<TransacaoEstabelecimentoListar>(string.Format("cast(nsu_rede as unsigned)={0} and id_conta={1}",
                                       itemsRede.is_numero_cv_long, UsuarioLogado.IdConta));

                                if (extrato_item != null)
                                {
                                    contador++;
                                    linha = linha + string.Format("{0}", 5);
                                    linha = linha + string.Format("{0}", extrato_item.dt_transacao.ToString("yyyyMMdd"));
                                    linha = linha + string.Format("{0}", items.data_lancamento.ToString("yyyyMMdd"));
                                    linha = linha + string.Format("{0:D12}", extrato_item.nsu_rede_long);
                                    linha = linha + string.Format("{0}", (itemsRede.is_numero_autorizacao ?? string.Empty).PadRight(12)); // Autorizacao
                                    linha = linha + string.Format("{0:D2}", Convert.ToInt64(items.numero_parcela.Substring(3, 2))); // Plano
                                    linha = linha + string.Format("{0:D2}", Convert.ToInt64(items.numero_parcela.Substring(0, 2))); // Parcela
                                    linha = linha + string.Format("{0:D2}", Convert.ToInt64(Operadora(extrato_item.operadora_desc.ToUpper())));
                                    linha = linha + string.Format("{0:D2}", Convert.ToInt64(Bandeira((items.bandeira ?? string.Empty).ToUpper().Trim())));
                                    linha = linha + string.Format("{0}", "C");
                                    linha = linha + string.Format("{0:D13}", Convert.ToInt64(itemsRede.is_valor_bruto.ToString().Replace(",", "").Replace(".", "")));
                                    linha = linha + string.Format("{0:D13}", Convert.ToInt64(itemsRede.is_valor_desconto.ToString().Replace(",", "").Replace(".", "")));
                                    linha = linha + string.Format("{0:D13}", Convert.ToInt64(items.valor_lancamento.ToString().Replace(",", "").Replace(".", "")));
                                    linha = linha + string.Format("{0:D20}", itemsRede.is_numero_resumo_vendas_long);
                                    linha = linha + string.Format("{0}", extrato_item.area_cliente.PadRight(50));
                                    linha = linha + string.Format("{0:D4}", Convert.ToInt64(items.banco));
                                    linha = linha + string.Format("{0:D6}", Convert.ToInt64(items.agencia));
                                    linha = linha + string.Format("{0:D13}", Convert.ToInt64(items.conta_corrente));
                                    linha = linha + string.Format("{0:D6}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                    linha = linha + string.Format("{0}", extrato_item.nm_estabelecimento.Trim().PadRight(20)); // LOJA
                                    linha = linha + string.Format("{0:D20}", itemsRede.is_numero_filiacao_pv); // Estabelecimento
                                    linha = linha + string.Format("{0}", "ANT");
                                    linha = linha + string.Format("{0:D13}", Convert.ToInt64(itemsRede.is_valor_liquido.ToString().Replace(",", "").Replace(".", "")));
                                    linha = linha + string.Format("{0:D15}", Convert.ToInt64(extrato_item.cod_loja)); // CODIGO DA LOJA
                                    linha = linha + string.Format("{0}", string.Empty.PadLeft(11)); // Reservado
                                    sw.WriteLine(linha);
                                    ///l.Clear();
                                    linha = "";

                                    totBruto = totBruto + itemsRede.is_valor_bruto;
                                    totLiquido = totLiquido + itemsRede.is_valor_liquido;
                                    totAntecipado = totAntecipado + items.valor_lancamento;

                                }
                                else
                                {
                                    var arq = new ArquivoDeCriticas();
                                    arq.ds_motivo = "Não encontrado Extrato ERP!";
                                    arq.ds_arquivo = arquivo;
                                    arq.dt_credito = items.data_lancamento;
                                    arq.dt_transacao = itemsRede.is_data_cv;
                                    arq.numero_resumo_venda = itemsRede.is_numero_resumo_vendas.ToString();
                                    arq.nsu_rede = itemsRede.is_numero_cv.ToString();
                                    arq.vl_bruto = itemsRede.is_valor_bruto;
                                    arq.vl_liquido = itemsRede.is_valor_liquido;
                                    arq.id_conta = Convert.ToInt32(UsuarioLogado.IdConta);
                                    ListCriticas.Add(arq);
                                    continue;
                                }

                            }

                        }
                    }



                }

                //RVVALOR.OrderByDescending(x => x.RV);
                linha = linha + string.Format("{0}", 9);
                bruto = bruto;
                linha = linha + string.Format("{0:D6}", contador + 2);
                linha = linha + string.Format("{0:D13}", Convert.ToInt64(totBruto.ToString().Replace(",", "").Replace(".", "")));
                linha = linha + string.Format("{0:D13}", Convert.ToInt64((totBruto - totLiquido).ToString().Replace(",", "").Replace(".", "")));
                linha = linha + string.Format("{0:D13}", Convert.ToInt64((totAntecipado).ToString().Replace(",", "").Replace(".", "")));
                linha = linha + string.Format("{0:D13}", 0);
                linha = linha + string.Format("{0:D13}", 0);
                linha = linha + string.Format("{0}", " ".PadLeft(168));
                sw.WriteLine(linha);
                // swerros.WriteLine(l.ToString());
                //l.Clear();
                linha = "";
            }

            // swerros.Dispose();

            DAL.GravarList(ListCriticas);
            /* Response.Clear();
             Response.ContentType = "text/plain";
             Response.AppendHeader("Content-Disposition", String.Format("attachment;filename={0}", arquivo));
             Response.TransmitFile(file);
             Response.End();*/

            var result = new { jarquivo = arquivo, jfile = file };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /* public void Baixar(string arquivo, string file)
         {
             Response.Clear();
             Response.ContentType = "text/plain";
             Response.AppendHeader("Content-Disposition", String.Format("attachment;filename={0}", arquivo));
             Response.TransmitFile(file);
             Response.End();

         } */

        [HttpGet]
        public void Baixar(string file, string arquivo)
        {
            Response.Clear();
            Response.ContentType = "text/plain";
            Response.AppendHeader("Content-Disposition", String.Format("attachment;filename={0}", arquivo));
            Response.TransmitFile(file);
            Response.End();
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

    }
}
