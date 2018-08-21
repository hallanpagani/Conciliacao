using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Conciliacao.Models.conciliacao;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoModelo.model.conciliador.UseRede;

namespace Conciliacao.Models
{
    public class ConciliacaoBinDesmontar
    {
        /*
	    Objeto para manipular o arquivo.
	    */
        private ConciliacaoArquivoManipular io_file;

        /*
	    Linha atual do arquivo.
	    */
        private string is_linha_atual;
        private string is_ultimo_ro;

        /*
        Estrutura header.
        */
        private ConciliacaoHeaderStruct io_header;

        /*
        Estruturas para desmontagem.
        */

        private List<ArquivoResumo> io_arl_ro = new List<ArquivoResumo>();


        private ArrayList io_arl_cv = new ArrayList();
        private List<TransacaoRede> io_arl_rede = new List<TransacaoRede>();

        private List<ArquivoTransacao> io_arl_just_cv = new List<ArquivoTransacao>();

        private List<ArquivoDetalhado> io_detalhado = new List<ArquivoDetalhado>();
        private List<ArquivoCompleto> io_completo = new List<ArquivoCompleto>();

        private List<ArquivoAntecipado> io_antecipado = new List<ArquivoAntecipado>();

        private List<TotalizadorProduto> io_totalizador_produto = new List<TotalizadorProduto>();
        private List<TotalizadorBanco> io_totalizador_banco = new List<TotalizadorBanco>();
        private List<TotalizadorEstabelecimento> io_totalizador_estabelecimento = new List<TotalizadorEstabelecimento>();

        /*
        Estruturas para desmontagem.
        */
        private List<ConciliacaoUseRedeEEVCResumoOperacaoStruct> io_arl_ro_credito_bin = new List<ConciliacaoUseRedeEEVCResumoOperacaoStruct>();
        private List<ConciliacaoUseRedeEEVDResumoOperacaoStruct> io_arl_ro_debito_bin = new List<ConciliacaoUseRedeEEVDResumoOperacaoStruct>();
        private List<ConciliacaoUseRedeEEVCComprovanteVendaStruct> io_arl_cv_credito_bin = new List<ConciliacaoUseRedeEEVCComprovanteVendaStruct>();
        private List<ConciliacaoUseRedeEEVDComprovanteVendaStruct> io_arl_cv_debito_bin = new List<ConciliacaoUseRedeEEVDComprovanteVendaStruct>();
        private List<ConciliacaoUseRedeParcelasStruct> io_arl_parcelas = new List<ConciliacaoUseRedeParcelasStruct>();
        private List<ConciliacaoUseRedeEEFIResumoOperacaoStruct> io_arl_resumo_op = new List<ConciliacaoUseRedeEEFIResumoOperacaoStruct>();
        private List<ConciliacaoUseRedeEEFICreditosStruct> io_arl_creditos = new List<ConciliacaoUseRedeEEFICreditosStruct>();

        public List<ConciliacaoUseRedeEEVDComprovanteVendaStruct> GetComprovanteEEVDVenda()
        {
            return io_arl_cv_debito_bin;
        }

        public List<ConciliacaoUseRedeEEVDResumoOperacaoStruct> GetResumoEEVDOperacao()
        {
            return io_arl_ro_debito_bin;
        }

        public List<ConciliacaoUseRedeEEVCResumoOperacaoStruct> GetResumoEEVCArquivo()
        {
            return io_arl_ro_credito_bin;
        }

        public List<ConciliacaoUseRedeEEVCComprovanteVendaStruct> GetComprovanteEEVCCVenda()
        {
            return io_arl_cv_credito_bin;
        }

        public List<ConciliacaoUseRedeParcelasStruct> GetParcelas()
        {
            return io_arl_parcelas;
        }

        public List<ConciliacaoUseRedeEEFIResumoOperacaoStruct> GetResumoArquivo()
        {
            return io_arl_resumo_op;
        }


        public List<ConciliacaoUseRedeEEFICreditosStruct> GetCreditos()
        {
            return io_arl_creditos;
        }

      /*  public List<ConciliacaoUseRedeEEFIAjustesDesagendamentoStruct> GetAjustesDesagendamento()
        {
            return io_arl_ajustes_desagendamento;
        }

        public List<ConciliacaoUseRedeEEFIAntecipacaoStruct> GetAjustesAntecipacao()
        {
            return io_arl_ajustes_antecipacao;
        }

        public List<ConciliacaoUseRedeEEFIDesagendamentoParcelasStruct> GetDesagendamentoParcelas()
        {
            return io_arl_desagendamento_parcela;
        }*/

        /*
        Estrutura CV.
        */
        private Dictionary<string, ArrayList> io_hsm_cv = new Dictionary<string, ArrayList>();

        /*
        Tabela explicativa.
        */

        private IDictionary
            io_hsm_tabela_I = new Hashtable(),
            io_hsm_tabela_II = new Hashtable(),
            io_hsm_tabela_III = new Hashtable(),
            io_hsm_tabela_VI = new Hashtable(),
            io_hsm_totalizador_bruto = new Hashtable(),
            io_hsm_previsao_pagamento = new Hashtable(),
            io_hsm_totalizador_produto = new Hashtable(),
            io_hsm_totalizador_banco = new Hashtable(),
            io_hsm_totalizador_estabelecimento = new Hashtable();

        /*
        Construtor da classe.
        */
        public ConciliacaoBinDesmontar
                        (
                            ConciliacaoArquivoManipular
                                        ao_file,
                            string _first
                        )
        {
            /*
		    Guarda o arquivo.
		    */
            io_file = ao_file;

            /*
            Volta para a primeira linha.
            */
            //io_file.Seek(0);

            /*
            Inicializa as tabelas explicativas dos registros.
            */
            TabelasIniciar();

            /*
            Desmonta o header.
            */
            HeaderDesmontar(_first);

            /*
            Percorre toda a linha.
            */
            try
            {
                while ((is_linha_atual = io_file.LerLinha(true)) != null)
                {
                    /*
                    Se linha não vazia.
                    */
                    if (is_linha_atual.Length > 0)
                    {

                        string[] arquivo = is_linha_atual.Split(',');

                        /*
                        Identifica qual o tipo de registro.
                        */
                        switch (Convert.ToInt32(arquivo[0]))
                        {
                            case 10:
                                {
                                    RODesmontarDebitoMV(arquivo);
                                    break;
                                }
                            case 11:
                                {
                                    CVDesmontarDebitoMV(arquivo);
                                    break;
                                }
                            case 12:
                                {
                                    RODesmontarCreditoMV(arquivo);
                                    break;
                                }
                            case 13:
                                {
                                    CVDesmontarCreditoMV(arquivo);
                                    break;
                                }
                             case 14:
                                 {
                                     RODesmontarVendasCreditoParceladosMVInformacoes(arquivo);
                                     break;
                                 }
                            /* case 15:
                                 {
                                     CVDesmontarVendasCreditoParceladosMV(arquivo);
                                     break;
                                 } */
                             case 16:
                                 {
                                     RODesmontarVendasCreditoParceladosMV(arquivo);
                                     break;
                                 }
                             case 17:
                                 {
                                     CVDesmontarVendasCreditoParceladosMV(arquivo);
                                     break;
                                 } 

                            /*
                        Se for o RO.
                        */
                            case 20:
                                {
                                    /*
                                    Desmonta o RO.
                                    */
                                    RODesmontarDebito(arquivo);

                                    break;
                                }
                            /*
                            Se for o CV.
                            */
                            case 21:
                                {
                                    /*
                                    Desmonta o RO.
                                    */
                                    CVDesmontarDebito(arquivo);

                                    break;
                                }
                            case 22:
                                    {
                                        /*
                                        Desmonta o RO.
                                        */
                                        RODesmontarCredito(arquivo);

                                        break;
                                    }
                            /*  Se for o CV.
                                */
                            case 23:
                                {
                                    /*
                                    Desmonta o RO.
                                    */
                                    CVDesmontarCredito(arquivo);

                                    break;
                                }

                            case 24:
                                {
                                    /*
                                    Desmonta o RO.
                                    */
                                    RODesmontarParcelados(arquivo);

                                    break;
                                }
                            case 26:
                                {
                                    /*
                                    Desmonta o RO.
                                    */
                                    CVDesmontarParcelados(arquivo);

                                    break;
                                }
                           
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("Falha no processamento do arquivo: " + ex.Message);
                throw;
            }

            IDictionaryEnumerator totalizador_produto = io_hsm_totalizador_produto.GetEnumerator();
            while (totalizador_produto.MoveNext())
            {
                io_totalizador_produto.Add((TotalizadorProduto)totalizador_produto.Value);
            }

            IDictionaryEnumerator totalizador_banco = io_hsm_totalizador_banco.GetEnumerator();
            while (totalizador_banco.MoveNext())
            {
                io_totalizador_banco.Add((TotalizadorBanco)totalizador_banco.Value);
            }

            IDictionaryEnumerator totalizador_estabelecimento = io_hsm_totalizador_estabelecimento.GetEnumerator();
            while (totalizador_estabelecimento.MoveNext())
            {
                io_totalizador_estabelecimento.Add((TotalizadorEstabelecimento)totalizador_estabelecimento.Value);
            }

        }

        private void RODesmontarDebitoMV(string[] a)
        {
            /*
            Cria a estrutura de registro.
            */
            var resumo = new ArquivoResumo();

            try
            {
                DateTime data_rv = Convert.ToDateTime(FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"));
                DateTime dataprevista = Convert.ToDateTime(FormatoDataExecutar(a[10], "ddMMyyyy", "dd/MM/yyyy"));
                lvalor_bruto = FormatoValorExecutarDouble(a[7]);
                lvalor_liquido = FormatoValorExecutarDouble(a[9]);
                ltaxa = Math.Round(Math.Abs(((lvalor_liquido * 100 / lvalor_bruto) - 100)), 2); 

                resumo = new ArquivoResumo()
                {
                    resumo = a[3], //
                    estabelecimento = a[1], // 
                    parcela = "01", //
                    transacao = "Débito", //
                    prev_pagamento = FormatoDataExecutar(a[10], "ddMMyyyy", "dd/MM/yyyy"), //
                    valor_bruto = lvalor_bruto, //
                    taxa_comissao = ltaxa.ToString("#,##0.00"), // is_linha_atual.Substring(57, 1),
                    vl_comissao = FormatoValorExecutarDouble(a[8]), //
                    vl_rejeitado = "0", //
                    vl_liquido = lvalor_liquido, //
                    situacao = "", //
                    produto = "Vendas Cartão de Débito ", //
                    terminal = "",
                    chave = ""

                };

                taxa = ltaxa;
                bandeira = (string)io_hsm_tabela_VI[a[5]]; // (string) io_hsm_tabela_VI[is_linha_atual.Substring(184, 3)];
                data_pagamento = dataprevista.ToString(); //
                data_movimento = data_rv.ToString(); //
                tipo_transacao = "Débito";
                status_pagamento = "";
                meio_captura = "";
                banco_ro = a[11]; //
                agencia = a[12]; //
                conta = a[13]; // 
                parcela = "01";  //
                bruto_rv = lvalor_bruto;
                produto_ro = resumo.produto;

                var resumo_debito_bin = new ConciliacaoUseRedeEEVDResumoOperacaoStruct()
                {
                    is_tipo_registro = 10,
                    nm_tipo_registro = tipo_transacao,
                    is_numero_filiacao_pv = Convert.ToDecimal(a[1]),
                    is_numero_resumo_venda = a[3],
                    is_banco = banco_ro,
                    is_agencia = agencia,
                    is_conta_corrente = conta,
                    is_data_resumo_venda = Convert.ToDateTime(data_movimento),

                    is_quantidade_resumo_vendas = "0",
                    is_valor_bruto = bruto_rv,
                    is_valor_desconto = bruto_rv - lvalor_liquido,
                    is_valor_liquido = lvalor_liquido,

                    is_data_credito = dataprevista,
                    is_bandeira = (string)io_hsm_tabela_VI[a[5]],
                    rede = 4
                };

                io_arl_ro_debito_bin.Add(resumo_debito_bin);

                Decimal valor_total = 0;
                Decimal valor_total_liquido = 0;

                if (io_hsm_totalizador_produto.Contains(resumo.produto + dataprevista))
                {
                    var totalizador_produto = (TotalizadorProduto)io_hsm_totalizador_produto[resumo.produto + dataprevista];
                    valor_total = totalizador_produto.valor_bruto + resumo.valor_bruto;
                    valor_total_liquido = totalizador_produto.valor_liquido + resumo.vl_liquido;
                    io_hsm_totalizador_produto.Remove(resumo.produto + dataprevista);
                    io_hsm_totalizador_produto.Add(resumo.produto + dataprevista, new TotalizadorProduto
                    {
                        data_prevista = dataprevista,
                        valor_bruto = valor_total,
                        valor_liquido = valor_total_liquido,
                        ds_produto = resumo.produto,
                        rede = "Bin"
                    });
                }
                else
                {
                    io_hsm_totalizador_produto.Add(resumo.produto + dataprevista, new TotalizadorProduto
                    {
                        data_prevista = dataprevista,
                        valor_bruto = resumo.valor_bruto,
                        valor_liquido = resumo.vl_liquido,
                        ds_produto = resumo.produto,
                        rede = "Bin"
                    });
                }

                valor_total = 0;
                valor_total_liquido = 0;

                string banco = banco_ro;

                if (io_hsm_totalizador_banco.Contains(banco + dataprevista))
                {
                    TotalizadorBanco totalizador_banco = (TotalizadorBanco)io_hsm_totalizador_banco[banco + dataprevista];
                    valor_total_liquido = totalizador_banco.total_realizado + resumo.vl_liquido;
                    io_hsm_totalizador_banco.Remove(banco + dataprevista);
                    io_hsm_totalizador_banco.Add(banco + dataprevista, new TotalizadorBanco
                    {
                        data_prevista = dataprevista,
                        total_realizado = valor_total_liquido,
                        id_banco = Convert.ToInt32(banco)
                    });
                }
                else
                {
                    io_hsm_totalizador_banco.Add(banco + dataprevista, new TotalizadorBanco
                    {
                        data_prevista = dataprevista,
                        total_realizado = resumo.vl_liquido,
                        id_banco = Convert.ToInt32(banco)
                    });
                }


                valor_total = 0;
                valor_total_liquido = 0;

                string estabelecimento = resumo.estabelecimento;

                if (io_hsm_totalizador_estabelecimento.Contains(estabelecimento + dataprevista))
                {
                    var totalizador_estabelecimento = (TotalizadorEstabelecimento)io_hsm_totalizador_estabelecimento[estabelecimento + dataprevista];
                    valor_total_liquido = totalizador_estabelecimento.total_realizado + resumo.vl_liquido;
                    io_hsm_totalizador_estabelecimento.Remove(estabelecimento + dataprevista);
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + dataprevista, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = dataprevista,
                        total_realizado = valor_total_liquido,
                        estabelecimento = Convert.ToInt64(estabelecimento)
                    });
                }
                else
                {
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + dataprevista, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = dataprevista,
                        total_realizado = resumo.vl_liquido,
                        estabelecimento = Convert.ToInt64(estabelecimento)
                    });
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            /*
            GUARDA O ULTIMO RO.
            */
            is_ultimo_ro = resumo.resumo;

            /*
            Adiciona o ro na lista.
            */
            io_arl_ro.Add(resumo);
            io_arl_cv = new ArrayList();

            try
            {
                var lo_detalhado = new ArquivoDetalhado()
                {
                    registro = "Resumo do Pagamento",
                    estabelecimento = resumo.estabelecimento,
                    transacao = resumo.transacao,
                    RO = resumo.resumo,
                    produto = resumo.produto,
                    cartao = "",
                    vl_bruto = resumo.valor_bruto.ToString(),
                    vl_liquido = resumo.vl_liquido.ToString(),
                    parcela = "",
                    total_parcela = "",
                    autorizacao = "",
                    nsu = ""
                };


                io_detalhado.Add(lo_detalhado);
            }
            catch (Exception e)
            {
                throw e;
            }
            try
            {
                var lo_completo = new ArquivoCompleto()
                {
                    registro = "Resumo do Pagamento",
                    estabelecimento = resumo.estabelecimento,
                    RO = resumo.resumo,
                    parcela = "",
                    total_parcela = "",
                    filer = "",
                    plano = "",
                    tipo_transacao = resumo.transacao,
                    apresentacao = data_movimento,
                    prev_pagamento = resumo.prev_pagamento,
                    envio_banco = a[10].Equals("00000000") ? "00/00/00000" : FormatoDataExecutar(a[10], "ddMMyyyy", "dd/MM/yyyy"),
                    cartao = "",
                    sinal_valor_bruto = "",
                    valor_bruto = resumo.valor_bruto.ToString(),
                    sinal_comissao = "",
                    comissao = resumo.vl_comissao.ToString(),
                    sinal_rejeitado = "",
                    valor_rejeitado = resumo.vl_rejeitado,
                    sinal_liquido = "",
                    valor_liquido = resumo.vl_liquido.ToString(),
                    valor_total_venda = "",
                    valor_prox_parcela = "",
                    taxas = "",
                    autorizacao = "",
                    nsu_doc = "",
                    banco = banco_ro,
                    agencia = agencia,
                    conta_corrente = conta,
                    status_pagamento = "Débito",
                    motivo_rejeicao = "",
                    cvs_aceitos = a[4],
                    produto = resumo.produto,
                    cvs_rejeitados = "0",
                    data_venda = resumo.prev_pagamento,
                    data_captura = "",
                    origem_ajuste = "",
                    valor_complementar = "",
                    produto_financeiro = "",
                    valor_antecipado = "",
                    bandeira = "",
                    registro_unico_RO = resumo.resumo,
                    taxas_comissao = ltaxa.ToString("#,##0.00"),
                    tarifa = Math.Round(Math.Abs(((resumo.vl_liquido * 100) / (resumo.valor_bruto) - 100))).ToString("#,##0.00"),
                    meio_captura = "",
                    terminal = ""
                };

                var lo_rede = new TransacaoRede
                {
                    ds_rede = "Bin",
                    dt_transacao = Convert.ToDateTime(data_movimento),
                    dt_prev_pagto = Convert.ToDateTime(resumo.prev_pagamento),
                    nr_logico = lo_completo.terminal,
                    nsu_rede = "",
                    nsu_tef = "",
                    vl_liquido = Convert.ToDecimal(lo_completo.valor_liquido),
                    vl_bruto = Convert.ToDecimal(lo_completo.valor_bruto),
                    nm_estabelecimento = lo_completo.estabelecimento
                };
                io_arl_rede.Add(lo_rede);

                io_completo.Add(lo_completo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*
        Comprovante de venda débito.
        */
        private
        void CVDesmontarDebitoMV(string[] a)
        {
            try
            {
                var lo_cv = new ArquivoTransacao()
                {
                    num_resumo = a[3],
                    registro = "Comprovante de Venda",
                    estabelecimento = a[1],
                    num_logico = "",
                    dt_venda = FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"),
                    cartao = a[7],
                    vl_venda = FormatoValorExecutar(a[14]),
                    parcela = "01",
                    total_parcela = "01",
                    autorizacao = a[9],
                    nsu = a[4]
                };

                io_arl_cv.Add(lo_cv);
                io_arl_just_cv.Add(lo_cv);

                var resumo = new ConciliacaoUseRedeEEVDComprovanteVendaStruct()
                {
                    is_tipo_registro = 11,
                    nm_tipo_registro = "CV - Detalhamento",
                    is_numero_filiacao_pv = Convert.ToDecimal(a[1]),
                    is_numero_resumo_vendas = Convert.ToInt64(a[3]),
                    is_data_cv = Convert.ToDateTime(FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy")),
                    is_valor_bruto = Convert.ToDecimal(a[14]) / 100,
                    is_valor_liquido = (Convert.ToDecimal(a[16]) / 100),
                    is_valor_desconto = (Convert.ToDecimal(a[15]) / 100),

                    is_parcela = "01",
                    is_numero_parcelas = 1,
                    is_numero_cartao = a[7],
                    is_tipo_transacao = tipo_transacao,
                    is_numero_cv = Convert.ToDecimal(a[4]),
                    is_data_credito = Convert.ToDateTime(data_pagamento),
                    is_status_transacao = status_pagamento,
                    is_hora_transacao = a[10],
                    is_numero_terminal = a[11],
                    is_numero_autorizacao = a[9],
                    is_tipo_captura = meio_captura,
                    is_reservado = "",
                    is_valor_compra = Convert.ToDecimal(a[14]) / 100,
                    is_valor_liquido_primeira_parc = Convert.ToDecimal(a[16]) / 100,
                    is_valor_liquido_demais_parc = Convert.ToDecimal(a[16]) / 100,
                    numero_nota_fiscal = "",
                    numero_unico_transacao = a[4],
                    is_valor_saque = 0,
                    is_bandeira = bandeira,
                    taxa_cobrada = taxa,
                    rede = 4

                    //   is_codigo_tef = achou.Codigt
                };

                io_arl_cv_debito_bin.Add(resumo);

                //io_hsm_cv.Add(is_ultimo_ro, io_arl_cv);

                var lo_detalhado1 = new ArquivoDetalhado()
                {
                    registro = "Comprovante de Venda",
                    estabelecimento = lo_cv.estabelecimento,
                    transacao = "",
                    RO = lo_cv.num_resumo,
                    produto = "",
                    cartao = lo_cv.cartao,
                    vl_bruto = lo_cv.vl_venda,
                    vl_liquido = "",
                    parcela = lo_cv.parcela,
                    total_parcela = lo_cv.total_parcela,
                    autorizacao = lo_cv.autorizacao,
                    nsu = lo_cv.nsu
                };

                io_detalhado.Add(lo_detalhado1);

                var lo_completo = new ArquivoCompleto()
                {
                    registro = "Comprovante de Venda",
                    estabelecimento = lo_cv.estabelecimento,
                    RO = lo_cv.num_resumo,
                    parcela = lo_cv.parcela,
                    total_parcela = lo_cv.total_parcela,
                    filer = "",
                    plano = "",
                    tipo_transacao = "",
                    apresentacao = "",
                    prev_pagamento = "",
                    envio_banco = "",
                    cartao = lo_cv.cartao,
                    sinal_valor_bruto = "",
                    valor_bruto = lvalor_bruto.ToString(),
                    sinal_comissao = "",
                    comissao = ltaxa.ToString(),
                    sinal_rejeitado = "",
                    valor_rejeitado = "",
                    sinal_liquido = "",
                    valor_liquido = lvalor_liquido.ToString(),
                    valor_total_venda = lvalor_bruto.ToString(),
                    valor_prox_parcela = "",
                    taxas = "",
                    autorizacao = lo_cv.autorizacao,
                    nsu_doc = lo_cv.nsu,
                    banco = "",
                    agencia = "",
                    conta_corrente = "",
                    status_pagamento = "",
                    motivo_rejeicao = "",
                    cvs_aceitos = "",
                    produto = "",
                    cvs_rejeitados = "",
                    data_venda = lo_cv.dt_venda,
                    data_captura = "",
                    origem_ajuste = "",
                    valor_complementar = "",
                    produto_financeiro = "",
                    valor_antecipado = "",
                    bandeira = "",
                    registro_unico_RO = lo_cv.num_resumo, // is_linha_atual.Substring(11, 7),
                    taxas_comissao = "",
                    tarifa = "",
                    meio_captura = "",
                    terminal = ""
                };
                io_completo.Add(lo_completo);

                var lo_rede = new TransacaoRede
                {
                    ds_rede = "Bin",
                    dt_transacao = Convert.ToDateTime(lo_completo.data_venda),
                    nr_logico = lo_completo.terminal,
                    nsu_rede = lo_completo.nsu_doc,
                    nsu_tef = lo_completo.nsu_doc,

                    vl_bruto = Convert.ToDecimal(lo_completo.valor_bruto),
                    nm_estabelecimento = lo_completo.estabelecimento
                };
                io_arl_rede.Add(lo_rede);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /**
       Desmonta o resumo de vendas crédito.
       */
        private
        void RODesmontarCreditoMV(string[] a)
        {
            /*
            Cria a estrutura de registro.
            */
            var resumo = new ArquivoResumo();

            try
            {
                DateTime data_rv = Convert.ToDateTime(FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"));
                DateTime dataprevista = Convert.ToDateTime(FormatoDataExecutar(a[9], "ddMMyyyy", "dd/MM/yyyy"));
                lvalor_bruto = FormatoValorExecutarDouble(a[6]);
                lvalor_liquido = FormatoValorExecutarDouble(a[8]);
                ltaxa = Math.Round(Math.Abs(((lvalor_liquido * 100 / lvalor_bruto) - 100)), 2); 

                resumo = new ArquivoResumo()
                {
                    resumo = a[3], //
                    estabelecimento = a[1], // 
                    parcela = "01", //
                    transacao = "A vista", //
                    prev_pagamento = FormatoDataExecutar(a[9], "ddMMyyyy", "dd/MM/yyyy"), //
                    valor_bruto = lvalor_bruto, //
                    taxa_comissao = ltaxa.ToString("#,##0.00"), // is_linha_atual.Substring(57, 1),
                    vl_comissao = FormatoValorExecutarDouble(a[7]), //
                    vl_rejeitado = "0", //
                    vl_liquido = lvalor_liquido, //
                    situacao = "", //
                    produto = "Crédito A Vista", //
                    terminal = "",
                    chave = ""

                };

                taxa = ltaxa;
                bandeira = (string)io_hsm_tabela_VI[a[5]];  // (string) io_hsm_tabela_VI[is_linha_atual.Substring(184, 3)];
                data_pagamento = data_rv.ToString(); //
                data_movimento = dataprevista.ToString(); //
                tipo_transacao = "A vista";
                status_pagamento = "";
                meio_captura = "";
                banco_ro = a[10]; //
                agencia = a[11]; //
                conta = a[12]; // 
                parcela = "01";  //
                bruto_rv = lvalor_bruto;
                produto_ro = resumo.produto;

                var resumo_bin = new ConciliacaoUseRedeEEVCResumoOperacaoStruct()
                {
                    is_tipo_registro = 12,
                    nm_tipo_registro = tipo_transacao,
                    is_numero_filiacao_pv = Convert.ToDecimal(a[1]),
                    is_numero_resumo_venda = a[3],
                    is_banco = banco_ro,
                    is_agencia = agencia,
                    is_conta_corrente = conta,
                    is_data_resumo_venda = data_rv,

                    is_quantidade_resumo_vendas = a[4],
                    is_valor_bruto = bruto_rv,

                    is_valor_gorjeta = 0,

                    is_valor_rejeitado = 0,
                    is_valor_desconto = bruto_rv - lvalor_liquido,
                    is_valor_liquido = lvalor_liquido,

                    is_data_credito = dataprevista,
                    is_bandeira = (string)io_hsm_tabela_VI[a[5]],
                    rede = 4
                };


                io_arl_ro_credito_bin.Add(resumo_bin);



                Decimal valor_total = 0;
                Decimal valor_total_liquido = 0;

                if (io_hsm_totalizador_produto.Contains(resumo.produto + dataprevista))
                {
                    var totalizador_produto = (TotalizadorProduto)io_hsm_totalizador_produto[resumo.produto + dataprevista];
                    valor_total = totalizador_produto.valor_bruto + resumo.valor_bruto;
                    valor_total_liquido = totalizador_produto.valor_liquido + resumo.vl_liquido;
                    io_hsm_totalizador_produto.Remove(resumo.produto + dataprevista);
                    io_hsm_totalizador_produto.Add(resumo.produto + dataprevista, new TotalizadorProduto
                    {
                        data_prevista = dataprevista,
                        valor_bruto = valor_total,
                        valor_liquido = valor_total_liquido,
                        ds_produto = resumo.produto,
                        rede = "Bin"
                    });
                }
                else
                {
                    io_hsm_totalizador_produto.Add(resumo.produto + dataprevista, new TotalizadorProduto
                    {
                        data_prevista = dataprevista,
                        valor_bruto = resumo.valor_bruto,
                        valor_liquido = resumo.vl_liquido,
                        ds_produto = resumo.produto,
                        rede = "Bin"
                    });
                }


                valor_total = 0;
                valor_total_liquido = 0;

                string banco = banco_ro;

                if (io_hsm_totalizador_banco.Contains(banco + dataprevista))
                {
                    TotalizadorBanco totalizador_banco = (TotalizadorBanco)io_hsm_totalizador_banco[banco + dataprevista];
                    valor_total_liquido = totalizador_banco.total_realizado + resumo.vl_liquido;
                    io_hsm_totalizador_banco.Remove(banco + dataprevista);
                    io_hsm_totalizador_banco.Add(banco + dataprevista, new TotalizadorBanco
                    {
                        data_prevista = dataprevista,
                        total_realizado = valor_total_liquido,
                        id_banco = Convert.ToInt32(banco)
                    });
                }
                else
                {
                    io_hsm_totalizador_banco.Add(banco + dataprevista, new TotalizadorBanco
                    {
                        data_prevista = dataprevista,
                        total_realizado = resumo.vl_liquido,
                        id_banco = Convert.ToInt32(banco)
                    });
                }


                valor_total = 0;
                valor_total_liquido = 0;

                string estabelecimento = resumo.estabelecimento;

                if (io_hsm_totalizador_estabelecimento.Contains(estabelecimento + dataprevista))
                {
                    var totalizador_estabelecimento = (TotalizadorEstabelecimento)io_hsm_totalizador_estabelecimento[estabelecimento + dataprevista];
                    valor_total_liquido = totalizador_estabelecimento.total_realizado + resumo.vl_liquido;
                    io_hsm_totalizador_estabelecimento.Remove(estabelecimento + dataprevista);
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + dataprevista, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = dataprevista,
                        total_realizado = valor_total_liquido,
                        estabelecimento = Convert.ToInt64(estabelecimento)
                    });
                }
                else
                {
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + dataprevista, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = dataprevista,
                        total_realizado = resumo.vl_liquido,
                        estabelecimento = Convert.ToInt64(estabelecimento)
                    });
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            /*
                GUARDA O ULTIMO RO.
           */
            is_ultimo_ro = resumo.resumo;

            /*
            Adiciona o ro na lista.
            */
            io_arl_ro.Add(resumo);
            io_arl_cv = new ArrayList();

            try
            {
                var lo_detalhado = new ArquivoDetalhado()
                {
                    registro = "Resumo do Pagamento",
                    estabelecimento = resumo.estabelecimento,
                    transacao = resumo.transacao,
                    RO = resumo.resumo,
                    produto = resumo.produto,
                    cartao = "",
                    vl_bruto = resumo.valor_bruto.ToString(),
                    vl_liquido = resumo.vl_liquido.ToString(),
                    parcela = "",
                    total_parcela = "",
                    autorizacao = "",
                    nsu = ""
                };


                io_detalhado.Add(lo_detalhado);
            }
            catch (Exception e)
            {
                throw e;
            }

            try
            {
                var lo_completo = new ArquivoCompleto()
                {
                    registro = "Resumo do Pagamento",
                    estabelecimento = resumo.estabelecimento,
                    RO = resumo.resumo,
                    parcela = "",
                    total_parcela = "",
                    filer = "",
                    plano = "",
                    tipo_transacao = resumo.transacao,
                    apresentacao = data_movimento,
                    prev_pagamento = resumo.prev_pagamento,
                    envio_banco = "",
                    cartao = "",
                    sinal_valor_bruto = "",
                    valor_bruto = resumo.valor_bruto.ToString(),
                    sinal_comissao = "",
                    comissao = resumo.vl_comissao.ToString(),
                    sinal_rejeitado = "",
                    valor_rejeitado = resumo.vl_rejeitado,
                    sinal_liquido = "",
                    valor_liquido = resumo.vl_liquido.ToString(),
                    valor_total_venda = "",
                    valor_prox_parcela = "",
                    taxas = "",
                    autorizacao = "",
                    nsu_doc = "",
                    banco = banco_ro,
                    agencia = agencia,
                    conta_corrente = conta,
                    status_pagamento = "",
                    motivo_rejeicao = "",
                    cvs_aceitos = a[4],
                    produto = resumo.produto,
                    cvs_rejeitados = "0",
                    data_venda = resumo.prev_pagamento,
                    data_captura = "",
                    origem_ajuste = "",
                    valor_complementar = "",
                    produto_financeiro = "",
                    valor_antecipado = "",
                    bandeira = "",
                    registro_unico_RO = resumo.resumo,
                    taxas_comissao = (resumo.valor_bruto- resumo.vl_liquido).ToString("#,##0.00"),
                    tarifa = Math.Round(Math.Abs(((resumo.vl_liquido * 100) / (resumo.valor_bruto) - 100)),2).ToString("#,##0.00"),
                    meio_captura = "",
                    terminal = ""
                };

                var lo_rede = new TransacaoRede
                {
                    ds_rede = "Bin",
                    dt_transacao = Convert.ToDateTime(data_movimento),
                    dt_prev_pagto = Convert.ToDateTime(resumo.prev_pagamento),
                    nr_logico = lo_completo.terminal,
                    nsu_rede = "",
                    nsu_tef = "",
                    vl_liquido = Convert.ToDecimal(lo_completo.valor_liquido),
                    vl_bruto = Convert.ToDecimal(lo_completo.valor_bruto),
                    nm_estabelecimento = lo_completo.estabelecimento
                };
                io_arl_rede.Add(lo_rede);

                io_completo.Add(lo_completo);
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        private
        void CVDesmontarCreditoMV(string[] a)
        {
            try
            {
                var lo_cv = new ArquivoTransacao()
                {
                    num_resumo = a[3],
                    registro = "Comprovante de Venda",
                    estabelecimento = a[1],
                    num_logico = "",
                    dt_venda = FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"),
                    cartao = a[6],
                    vl_venda = FormatoValorExecutar(a[13]),
                    parcela = "01",
                    total_parcela = "01",
                    autorizacao = a[8],
                    nsu = a[4]
                };

                io_arl_cv.Add(lo_cv);
                io_arl_just_cv.Add(lo_cv);


                var comprovante = new ConciliacaoUseRedeEEVCComprovanteVendaStruct()
                {
                    is_tipo_registro = 13,
                    nm_tipo_registro = "CV/NSU",
                    is_numero_filiacao_pv = Convert.ToDecimal(a[1]),
                    is_numero_resumo_vendas = Convert.ToInt64(a[3]),

                    is_data_cv =
                        Convert.ToDateTime(FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy")),

                    is_valor_bruto = Convert.ToDecimal(a[13]) / 100,
                    is_valor_gorjeta = 0, //Convert.ToDecimal(is_linha_atual.Substring(52, 15)) / 100,
                    is_numero_cartao = a[6],

                    is_status_cv = "",

                    is_parcela = 1.ToString(),
                    is_numero_parcelas = 1,
                    is_numero_cv = Convert.ToDecimal(a[4]),

                    is_numero_referencia = "0", //a.Substring(100, 13),


                    is_valor_desconto = (Convert.ToDecimal(a[14]) / 100),

                    is_numero_autorizacao = a[8],

                    is_hora_transcao = "",

                    is_tipo_captura = (string)io_hsm_tabela_I[a[11]],

                    is_valor_liquido =
                        (Convert.ToDecimal(a[15]) / 100),
                    // Convert.ToDecimal(a.Substring(205, 15)) / 100,#VER

                    //   is_valor_liquido_primeira_parc =  Convert.ToDecimal(is_linha_atual.Substring(113, 13)) / 100,
                    //   is_valor_liquido_demais_parc = Convert.ToDecimal(is_linha_atual.Substring(126, 13)) / 100,
                    //   numero_nota_fiscal = is_linha_atual.Substring(139, 9),
                    numero_unico_transacao = a[4],
                    is_numero_terminal = a[10],

                    is_bandeira = bandeira, //TabelaI(a.Substring(261, 1)), #VER

                    taxa_cobrada = Math.Round(Math.Abs(((Convert.ToDecimal(a[15]) * 100) / (Convert.ToDecimal(a[13])) - 100)),2),
                    is_data_credito = Convert.ToDateTime(FormatoDataExecutar(a[16], "ddMMyyyy", "dd/MM/yyyy")),
                    rede = 4

                };

                io_arl_cv_credito_bin.Add(comprovante);

                var creditos = new ConciliacaoUseRedeEEFICreditosStruct()
                {
                    numero_pv_centralizador = comprovante.is_numero_filiacao_pv,
                    numero_documento = comprovante.is_numero_cv,
                    data_lancamento = comprovante.is_data_credito,
                    valor_lancamento = comprovante.is_valor_liquido,
                    banco = Convert.ToInt32(banco_ro),
                    agencia = agencia,
<<<<<<< HEAD
                    conta_corrente = conta,
=======
                    conta_corrente = Convert.ToInt64(conta),
>>>>>>> 7d4f72e90771fdbe2ddce8e4d5c620260095bf10
                    data_movimento = Convert.ToDateTime(data_movimento),
                    numero_rv = Convert.ToInt64(comprovante.is_numero_resumo_vendas.ToString()),
                    data_rv = comprovante.is_data_cv,
                    bandeira = bandeira,
                    tipo_transacao = tipo_transacao,
                    valor_bruto_rv = bruto_rv,
                    valor_taxa_desconto = taxa,
                    numero_parcela = parcela,
                    situacao = "",
                    numero_pv_original = Convert.ToInt64(comprovante.is_numero_filiacao_pv),
                    rede = 4
                };

                var resumo_fi = new ConciliacaoUseRedeEEFIResumoOperacaoStruct()
                {
                    agencia = creditos.agencia,
                    banco = creditos.banco,
                    conta_corrente = creditos.conta_corrente,
                    data = creditos.data_lancamento,
                    data_venda = creditos.data_rv,
                    nsu = creditos.numero_rv.ToString(),
                    descricao = "Créditos",
                    numero_pv = creditos.numero_pv_original.ToString(),
                    situacao = creditos.situacao,
                    valor = creditos.valor_lancamento,
                    valor_cancelado = 0,
                    valor_venda = creditos.valor_bruto_rv,
                    rede = 4
                };

                io_arl_resumo_op.Add(resumo_fi);

                io_arl_creditos.Add(creditos);

                //io_hsm_cv.Add(is_ultimo_ro, io_arl_cv);

                var lo_detalhado1 = new ArquivoDetalhado()
                {
                    registro = "Comprovante de Venda",
                    estabelecimento = lo_cv.estabelecimento,
                    transacao = "",
                    RO = lo_cv.num_resumo,
                    produto = "",
                    cartao = lo_cv.cartao,
                    vl_bruto = lo_cv.vl_venda,
                    vl_liquido = "",
                    parcela = lo_cv.parcela,
                    total_parcela = lo_cv.total_parcela,
                    autorizacao = lo_cv.autorizacao,
                    nsu = lo_cv.nsu
                };

                io_detalhado.Add(lo_detalhado1);

                var lo_completo = new ArquivoCompleto()
                {
                    registro = "Comprovante de Venda",
                    estabelecimento = lo_cv.estabelecimento,
                    RO = lo_cv.num_resumo,
                    parcela = lo_cv.parcela,
                    total_parcela = lo_cv.total_parcela,
                    filer = "",
                    plano = "",
                    tipo_transacao = "",
                    apresentacao = "",
                    prev_pagamento = "",
                    envio_banco = "",
                    cartao = lo_cv.cartao,
                    sinal_valor_bruto = "",
                    valor_bruto = lvalor_bruto.ToString(),
                    sinal_comissao = "",
                    comissao = ltaxa.ToString(),
                    sinal_rejeitado = "",
                    valor_rejeitado = "",
                    sinal_liquido = "",
                    valor_liquido = lvalor_liquido.ToString(),
                    valor_total_venda = lvalor_bruto.ToString(),
                    valor_prox_parcela = "",
                    taxas = "",
                    autorizacao = lo_cv.autorizacao,
                    nsu_doc = lo_cv.nsu,
                    banco = "",
                    agencia = "",
                    conta_corrente = "",
                    status_pagamento = "",
                    motivo_rejeicao = "",
                    cvs_aceitos = "",
                    produto = "",
                    cvs_rejeitados = "",
                    data_venda = lo_cv.dt_venda,
                    data_captura = "",
                    origem_ajuste = "",
                    valor_complementar = "",
                    produto_financeiro = "",
                    valor_antecipado = "",
                    bandeira = bandeira,
                    registro_unico_RO = lo_cv.num_resumo, // is_linha_atual.Substring(11, 7),
                    taxas_comissao = "",
                    tarifa = "",
                    meio_captura = "",
                    terminal = "",
                };
                io_completo.Add(lo_completo);

                var lo_rede = new TransacaoRede
                {
                    ds_rede = "Bin",
                    dt_transacao = Convert.ToDateTime(lo_completo.data_venda),
                    nr_logico = lo_completo.terminal,
                    nsu_rede = lo_completo.nsu_doc,
                    nsu_tef = lo_completo.nsu_doc,

                    vl_bruto = Convert.ToDecimal(lo_completo.valor_bruto),
                    nm_estabelecimento = lo_completo.estabelecimento
                };
                io_arl_rede.Add(lo_rede);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /**
          Desmonta o resumo de vendas parcelados. 14
        */
        private
        void RODesmontarVendasCreditoParceladosMVInformacoes(string[] a)
        {
            /*
           Cria a estrutura de registro.
           */
            banco_ro = a[12]; //
            agencia = a[13]; //
            conta = a[14]; // 
            parcela = a[9].ToString();  //
            quantidade_resumo_vendas = a[4];
           
        }

        /**
          Desmonta o resumo de vendas parcelados. 16
        */
        private
        void RODesmontarVendasCreditoParceladosMV(string[] a)
        {
            /*
           Cria a estrutura de registro.
           */
            var resumo = new ArquivoResumo();

            try
            {
                DateTime data_rv = Convert.ToDateTime(FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"));
                DateTime dataprevista = Convert.ToDateTime(FormatoDataExecutar(a[18], "ddMMyyyy", "dd/MM/yyyy"));
                lvalor_bruto = FormatoValorExecutarDouble(a[14]);
                lvalor_liquido = FormatoValorExecutarDouble(a[16]);
                ltaxa = Math.Round(Math.Abs(((lvalor_liquido * 100 / lvalor_bruto) - 100)), 2); 

                resumo = new ArquivoResumo()
                {
                    resumo = a[3], //
                    estabelecimento = a[1], // 
                    parcela = a[13], //
                    transacao = "Crédito parcelado", //
                    prev_pagamento = FormatoDataExecutar(a[18], "ddMMyyyy", "dd/MM/yyyy"), //
                    valor_bruto = lvalor_bruto, //
                    taxa_comissao = ltaxa.ToString("#,##0.00"), // is_linha_atual.Substring(57, 1),
                    vl_comissao = FormatoValorExecutarDouble(a[15]), //
                    vl_rejeitado = "0", //
                    vl_liquido = lvalor_liquido, //
                    situacao = "", //
                    produto = "Crédito Parcelado", //
                    terminal = "",
                    chave = ""

                };

                taxa = ltaxa;
                bandeira = (string)io_hsm_tabela_VI[a[5]];  // (string) io_hsm_tabela_VI[is_linha_atual.Substring(184, 3)];
                data_pagamento = data_rv.ToString(); //
                data_movimento = dataprevista.ToString(); //
                tipo_transacao = "Crédito parcelado";
                status_pagamento = "";
                meio_captura = "";
                
         
                bruto_rv = lvalor_bruto;
                produto_ro = resumo.produto;

                var resumo_bin = new ConciliacaoUseRedeEEVCResumoOperacaoStruct()
                {
                    is_tipo_registro = 16,
                    nm_tipo_registro = tipo_transacao,
                    is_numero_filiacao_pv = Convert.ToDecimal(a[1]),
                    is_numero_resumo_venda = a[3],
                    is_banco = banco_ro,
                    is_agencia = agencia,
                    is_conta_corrente = conta,
                    is_data_resumo_venda = data_rv,

                    is_quantidade_resumo_vendas = quantidade_resumo_vendas,
                    is_valor_bruto = bruto_rv,

                    is_valor_gorjeta = 0,

                    is_valor_rejeitado = 0,
                    is_valor_desconto = bruto_rv - lvalor_liquido,
                    is_valor_liquido = lvalor_liquido,
                    
                    is_data_credito = dataprevista,
                    is_bandeira = (string)io_hsm_tabela_VI[a[5]],
                    rede = 4
                };


                io_arl_ro_credito_bin.Add(resumo_bin);



                Decimal valor_total = 0;
                Decimal valor_total_liquido = 0;

                if (io_hsm_totalizador_produto.Contains(resumo.produto + dataprevista))
                {
                    var totalizador_produto = (TotalizadorProduto)io_hsm_totalizador_produto[resumo.produto + dataprevista];
                    valor_total = totalizador_produto.valor_bruto + resumo.valor_bruto;
                    valor_total_liquido = totalizador_produto.valor_liquido + resumo.vl_liquido;
                    io_hsm_totalizador_produto.Remove(resumo.produto + dataprevista);
                    io_hsm_totalizador_produto.Add(resumo.produto + dataprevista, new TotalizadorProduto
                    {
                        data_prevista = dataprevista,
                        valor_bruto = valor_total,
                        valor_liquido = valor_total_liquido,
                        ds_produto = resumo.produto,
                        rede = "Bin"
                    });
                }
                else
                {
                    io_hsm_totalizador_produto.Add(resumo.produto + dataprevista, new TotalizadorProduto
                    {
                        data_prevista = dataprevista,
                        valor_bruto = resumo.valor_bruto,
                        valor_liquido = resumo.vl_liquido,
                        ds_produto = resumo.produto,
                        rede = "Bin"
                    });
                }


                valor_total = 0;
                valor_total_liquido = 0;

                string banco = banco_ro;

                if (io_hsm_totalizador_banco.Contains(banco + dataprevista))
                {
                    TotalizadorBanco totalizador_banco = (TotalizadorBanco)io_hsm_totalizador_banco[banco + dataprevista];
                    valor_total_liquido = totalizador_banco.total_realizado + resumo.vl_liquido;
                    io_hsm_totalizador_banco.Remove(banco + dataprevista);
                    io_hsm_totalizador_banco.Add(banco + dataprevista, new TotalizadorBanco
                    {
                        data_prevista = dataprevista,
                        total_realizado = valor_total_liquido,
                        id_banco = Convert.ToInt32(banco)
                    });
                }
                else
                {
                    io_hsm_totalizador_banco.Add(banco + dataprevista, new TotalizadorBanco
                    {
                        data_prevista = dataprevista,
                        total_realizado = resumo.vl_liquido,
                        id_banco = Convert.ToInt32(banco)
                    });
                }


                valor_total = 0;
                valor_total_liquido = 0;

                string estabelecimento = resumo.estabelecimento;

                if (io_hsm_totalizador_estabelecimento.Contains(estabelecimento + dataprevista))
                {
                    var totalizador_estabelecimento = (TotalizadorEstabelecimento)io_hsm_totalizador_estabelecimento[estabelecimento + dataprevista];
                    valor_total_liquido = totalizador_estabelecimento.total_realizado + resumo.vl_liquido;
                    io_hsm_totalizador_estabelecimento.Remove(estabelecimento + dataprevista);
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + dataprevista, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = dataprevista,
                        total_realizado = valor_total_liquido,
                        estabelecimento = Convert.ToInt64(estabelecimento)
                    });
                }
                else
                {
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + dataprevista, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = dataprevista,
                        total_realizado = resumo.vl_liquido,
                        estabelecimento = Convert.ToInt64(estabelecimento)
                    });
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            /*
               GUARDA O ULTIMO RO.
            */
            is_ultimo_ro = resumo.resumo;

            /*
            Adiciona o ro na lista.
            */
            io_arl_ro.Add(resumo);
            io_arl_cv = new ArrayList();

            try
            {
                var lo_detalhado = new ArquivoDetalhado()
                {
                    registro = "Resumo do Pagamento",
                    estabelecimento = resumo.estabelecimento,
                    transacao = resumo.transacao,
                    RO = resumo.resumo,
                    produto = resumo.produto,
                    cartao = "",
                    vl_bruto = resumo.valor_bruto.ToString(),
                    vl_liquido = resumo.vl_liquido.ToString(),
                    parcela = "",
                    total_parcela = "",
                    autorizacao = "",
                    nsu = ""
                };


                io_detalhado.Add(lo_detalhado);
            }
            catch (Exception e)
            {
                throw e;
            }


            try
            {
                var lo_completo = new ArquivoCompleto()
                {
                    registro = "Resumo do Pagamento",
                    estabelecimento = resumo.estabelecimento,
                    RO = resumo.resumo,
                    parcela = "",
                    total_parcela = "",
                    filer = "",
                    plano = "",
                    tipo_transacao = resumo.transacao,
                    apresentacao = data_movimento,
                    prev_pagamento = "",
                    envio_banco = a[2].Equals("00000000") ? "00/00/00000" : FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"),
                    cartao = "",
                    sinal_valor_bruto = "",
                    valor_bruto = resumo.valor_bruto.ToString(),
                    sinal_comissao = "",
                    comissao = resumo.vl_comissao.ToString(),
                    sinal_rejeitado = "",
                    valor_rejeitado = resumo.vl_rejeitado,
                    sinal_liquido = "",
                    valor_liquido = resumo.vl_liquido.ToString(),
                    valor_total_venda = "",
                    valor_prox_parcela = "",
                    taxas = "",
                    autorizacao = "",
                    nsu_doc = "",
                    banco = banco_ro,
                    agencia = agencia,
                    conta_corrente = conta,
                    status_pagamento = (string)io_hsm_tabela_III[a[7]],
                    motivo_rejeicao = "",
                    cvs_aceitos = a[13],
                    produto = resumo.produto,
                    cvs_rejeitados = "0",
                    data_venda = resumo.prev_pagamento,
                    data_captura = "",
                    origem_ajuste = "",
                    valor_complementar = "",
                    produto_financeiro = "",
                    valor_antecipado = "",
                    bandeira = "",
                    registro_unico_RO = resumo.resumo,
                    taxas_comissao = (resumo.valor_bruto-resumo.vl_liquido).ToString("#,##0.00"),
                    tarifa = Math.Round(Math.Abs(((resumo.vl_liquido * 100) / (resumo.valor_bruto) - 100)),2).ToString(),
                    meio_captura = "",
                    terminal = ""
                };

                var lo_rede = new TransacaoRede
                {
                    ds_rede = "Bin",
                    dt_transacao = Convert.ToDateTime(data_movimento),
                    dt_prev_pagto = Convert.ToDateTime(resumo.prev_pagamento),
                    nr_logico = lo_completo.terminal,
                    nsu_rede = "",
                    nsu_tef = "",
                    vl_liquido = Convert.ToDecimal(lo_completo.valor_liquido),
                    vl_bruto = Convert.ToDecimal(lo_completo.valor_bruto),
                    nm_estabelecimento = lo_completo.estabelecimento
                };
                io_arl_rede.Add(lo_rede);

                io_completo.Add(lo_completo);
            }
            catch (Exception e)
            {
                throw e;
            }

            cartao = a[6];
            autorizacao = a[8];
            meio_captura = (string)io_hsm_tabela_II[a[11]];
            hr_transacao = a[9];
        }


        /*
       Comprovante de venda parcelados.
       */
        private
        void CVDesmontarVendasCreditoParceladosMV(string[] a)
        {
            try
            {
                var lo_cv = new ArquivoTransacao()
                {
                    num_resumo = a[3],
                    registro = "Comprovante de Venda Parcelados",
                    estabelecimento = a[1],
                    num_logico = "",
                    dt_venda = FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"),
                    cartao = cartao,
                    vl_venda = FormatoValorExecutar(a[6]),
                    parcela = a[7],
                    total_parcela = a[8],
                    autorizacao = autorizacao,
                    nsu = a[4]
                };

                io_arl_cv.Add(lo_cv);
                io_arl_just_cv.Add(lo_cv);


                var comprovante = new ConciliacaoUseRedeEEVCComprovanteVendaStruct()
                {
                    is_tipo_registro = 17,
                    nm_tipo_registro = "CV/NSU",
                    is_numero_filiacao_pv = Convert.ToDecimal(a[1]),
                    is_numero_resumo_vendas = Convert.ToInt64(a[3]),

                    is_data_cv =
                        Convert.ToDateTime(FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy")),

                    is_valor_bruto = lvalor_bruto,
                    is_valor_gorjeta = 0, //Convert.ToDecimal(is_linha_atual.Substring(52, 15)) / 100,
                    is_numero_cartao = cartao,

                    is_status_cv = "",

                    is_parcela = a[7],
                    is_numero_parcelas = Convert.ToInt32(a[8]),
                    is_numero_cv = Convert.ToDecimal(a[4]),

                    is_numero_referencia = "0", //a.Substring(100, 13),


                    is_valor_desconto = lvalor_bruto-lvalor_liquido,

                    is_numero_autorizacao = autorizacao,

                    is_hora_transcao = hr_transacao,

                    is_tipo_captura = meio_captura,

                    is_valor_liquido =
                        (Convert.ToDecimal(a[6]) / 100),
                    // Convert.ToDecimal(a.Substring(205, 15)) / 100,#VER

                    //   is_valor_liquido_primeira_parc =  Convert.ToDecimal(is_linha_atual.Substring(113, 13)) / 100,
                    //   is_valor_liquido_demais_parc = Convert.ToDecimal(is_linha_atual.Substring(126, 13)) / 100,
                    //   numero_nota_fiscal = is_linha_atual.Substring(139, 9),
                    numero_unico_transacao = a[4],
                    is_numero_terminal = a[10],

                    is_bandeira = bandeira, //TabelaI(a.Substring(261, 1)), #VER

                    taxa_cobrada = taxa,
                    is_data_credito = Convert.ToDateTime(FormatoDataExecutar(a[10], "ddMMyyyy", "dd/MM/yyyy")),
                    rede = 4

                };

                io_arl_cv_credito_bin.Add(comprovante);

                var creditos = new ConciliacaoUseRedeEEFICreditosStruct()
                {
                    numero_pv_centralizador = comprovante.is_numero_filiacao_pv,
                    numero_documento = comprovante.is_numero_cv,
                    data_lancamento = comprovante.is_data_credito,
                    valor_lancamento = comprovante.is_valor_liquido,
                    banco = Convert.ToInt32(banco_ro),
                    agencia = agencia,
<<<<<<< HEAD
                    conta_corrente = conta,
=======
                    conta_corrente = Convert.ToInt64(conta),
>>>>>>> 7d4f72e90771fdbe2ddce8e4d5c620260095bf10
                    data_movimento = Convert.ToDateTime(data_movimento),
                    numero_rv = Convert.ToInt64(comprovante.is_numero_resumo_vendas.ToString()),
                    data_rv = comprovante.is_data_cv,
                    bandeira = bandeira,
                    tipo_transacao = tipo_transacao,
                    valor_bruto_rv = bruto_rv,
                    valor_taxa_desconto = taxa,
                    numero_parcela = parcela,
                    situacao = "",
                    numero_pv_original = Convert.ToInt64(comprovante.is_numero_filiacao_pv),
                    rede = 4
                };

                var resumo_fi = new ConciliacaoUseRedeEEFIResumoOperacaoStruct()
                {
                    agencia = creditos.agencia,
                    banco = creditos.banco,
                    conta_corrente = creditos.conta_corrente,
                    data = creditos.data_lancamento,
                    data_venda = creditos.data_rv,
                    nsu = creditos.numero_rv.ToString(),
                    descricao = "Créditos",
                    numero_pv = creditos.numero_pv_original.ToString(),
                    situacao = creditos.situacao,
                    valor = creditos.valor_lancamento,
                    valor_cancelado = 0,
                    valor_venda = creditos.valor_bruto_rv,
                    rede = 4
                };

                io_arl_resumo_op.Add(resumo_fi);

                io_arl_creditos.Add(creditos);

                //io_hsm_cv.Add(is_ultimo_ro, io_arl_cv);

                var lo_detalhado1 = new ArquivoDetalhado()
                {
                    registro = "Comprovante de Venda",
                    estabelecimento = lo_cv.estabelecimento,
                    transacao = "",
                    RO = lo_cv.num_resumo,
                    produto = "",
                    cartao = lo_cv.cartao,
                    vl_bruto = lo_cv.vl_venda,
                    vl_liquido = "",
                    parcela = lo_cv.parcela,
                    total_parcela = lo_cv.total_parcela,
                    autorizacao = lo_cv.autorizacao,
                    nsu = lo_cv.nsu
                };

                io_detalhado.Add(lo_detalhado1);

                var lo_completo = new ArquivoCompleto()
                {
                    registro = "Comprovante de Venda",
                    estabelecimento = lo_cv.estabelecimento,
                    RO = lo_cv.num_resumo,
                    parcela = lo_cv.parcela,
                    total_parcela = lo_cv.total_parcela,
                    filer = "",
                    plano = "",
                    tipo_transacao = "",
                    apresentacao = "",
                    prev_pagamento = "",
                    envio_banco = "",
                    cartao = lo_cv.cartao,
                    sinal_valor_bruto = "",
                    valor_bruto = lvalor_bruto.ToString(),
                    sinal_comissao = "",
                    comissao = ltaxa.ToString(),
                    sinal_rejeitado = "",
                    valor_rejeitado = "",
                    sinal_liquido = "",
                    valor_liquido = lvalor_liquido.ToString(),
                    valor_total_venda = lvalor_bruto.ToString(),
                    valor_prox_parcela = "",
                    taxas = "",
                    autorizacao = lo_cv.autorizacao,
                    nsu_doc = lo_cv.nsu,
                    banco = "",
                    agencia = "",
                    conta_corrente = "",
                    status_pagamento = "",
                    motivo_rejeicao = "",
                    cvs_aceitos = "",
                    produto = "",
                    cvs_rejeitados = "",
                    data_venda = lo_cv.dt_venda,
                    data_captura = "",
                    origem_ajuste = "",
                    valor_complementar = "",
                    produto_financeiro = "",
                    valor_antecipado = "",
                    bandeira = bandeira,
                    registro_unico_RO = lo_cv.num_resumo, // is_linha_atual.Substring(11, 7),
                    taxas_comissao = "",
                    tarifa = "",
                    meio_captura = "",
                    terminal = "",
                };
                io_completo.Add(lo_completo);

                var lo_rede = new TransacaoRede
                {
                    ds_rede = "Bin",
                    dt_transacao = Convert.ToDateTime(lo_completo.data_venda),
                    nr_logico = lo_completo.terminal,
                    nsu_rede = lo_completo.nsu_doc,
                    nsu_tef = lo_completo.nsu_doc,

                    vl_bruto = Convert.ToDecimal(lo_completo.valor_bruto),
                    nm_estabelecimento = lo_completo.estabelecimento
                };
                io_arl_rede.Add(lo_rede);
            }
            catch (Exception e)
            {
                throw e;
            }


        }


        /*
        Comprovante de venda parcelados.
        */
        private
        void CVDesmontarParceladosMV(string[] a)
        {
            try
            {
                var lo_cv = new ArquivoTransacao()
                {
                    num_resumo = a[3],
                    registro = "Comprovante de Venda Parcelados",
                    estabelecimento = a[1],
                    num_logico = "",
                    dt_venda = FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"),
                    cartao = a[6],
                    vl_venda = FormatoValorExecutar(a[14]),
                    parcela = "1",
                    total_parcela = a[13],
                    autorizacao = a[8],
                    nsu = a[4]
                };

                io_arl_cv.Add(lo_cv);
                io_arl_just_cv.Add(lo_cv);


                var comprovante = new ConciliacaoUseRedeEEVCComprovanteVendaStruct()
                {
                    is_tipo_registro = 16,
                    nm_tipo_registro = "CV/NSU",
                    is_numero_filiacao_pv = Convert.ToDecimal(a[1]),
                    is_numero_resumo_vendas = Convert.ToInt64(a[3]),

                    is_data_cv =
                        Convert.ToDateTime(FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy")),

                    is_valor_bruto = Convert.ToDecimal(a[13]) / 100,
                    is_valor_gorjeta = 0, //Convert.ToDecimal(is_linha_atual.Substring(52, 15)) / 100,
                    is_numero_cartao = a[6],

                    is_status_cv = "",

                    is_parcela = 1.ToString(),
                    is_numero_parcelas = 1,
                    is_numero_cv = Convert.ToDecimal(a[4]),

                    is_numero_referencia = "0", //a.Substring(100, 13),


                    is_valor_desconto = (Convert.ToDecimal(a[14]) / 100),

                    is_numero_autorizacao = a[8],

                    is_hora_transcao = "",

                    is_tipo_captura = (string)io_hsm_tabela_I[a[11]],

                    is_valor_liquido =
                        (Convert.ToDecimal(a[15]) / 100),
                    // Convert.ToDecimal(a.Substring(205, 15)) / 100,#VER

                    //   is_valor_liquido_primeira_parc =  Convert.ToDecimal(is_linha_atual.Substring(113, 13)) / 100,
                    //   is_valor_liquido_demais_parc = Convert.ToDecimal(is_linha_atual.Substring(126, 13)) / 100,
                    //   numero_nota_fiscal = is_linha_atual.Substring(139, 9),
                    numero_unico_transacao = a[4],
                    is_numero_terminal = a[10],

                    is_bandeira = bandeira, //TabelaI(a.Substring(261, 1)), #VER

                    taxa_cobrada = Math.Round(Math.Abs(((Convert.ToDecimal(a[15]) * 100) / (Convert.ToDecimal(a[13])) - 100)),2),
                    is_data_credito = Convert.ToDateTime(FormatoDataExecutar(a[16], "ddMMyyyy", "dd/MM/yyyy")),
                    rede = 4

                };

                io_arl_cv_credito_bin.Add(comprovante);

                var creditos = new ConciliacaoUseRedeEEFICreditosStruct()
                {
                    numero_pv_centralizador = comprovante.is_numero_filiacao_pv,
                    numero_documento = comprovante.is_numero_cv,
                    data_lancamento = comprovante.is_data_credito,
                    valor_lancamento = comprovante.is_valor_liquido,
                    banco = Convert.ToInt32(banco_ro),
                    agencia = agencia,
<<<<<<< HEAD
                    conta_corrente = conta,
=======
                    conta_corrente = Convert.ToInt64(conta),
>>>>>>> 7d4f72e90771fdbe2ddce8e4d5c620260095bf10
                    data_movimento = Convert.ToDateTime(data_movimento),
                    numero_rv = Convert.ToInt64(comprovante.is_numero_resumo_vendas.ToString()),
                    data_rv = comprovante.is_data_cv,
                    bandeira = bandeira,
                    tipo_transacao = tipo_transacao,
                    valor_bruto_rv = bruto_rv,
                    valor_taxa_desconto = taxa,
                    numero_parcela = parcela,
                    situacao = "",
                    numero_pv_original = Convert.ToInt64(comprovante.is_numero_filiacao_pv),
                    rede = 4
                };

                var resumo_fi = new ConciliacaoUseRedeEEFIResumoOperacaoStruct()
                {
                    agencia = creditos.agencia,
                    banco = creditos.banco,
                    conta_corrente = creditos.conta_corrente,
                    data = creditos.data_lancamento,
                    data_venda = creditos.data_rv,
                    nsu = creditos.numero_rv.ToString(),
                    descricao = "Créditos",
                    numero_pv = creditos.numero_pv_original.ToString(),
                    situacao = creditos.situacao,
                    valor = creditos.valor_lancamento,
                    valor_cancelado = 0,
                    valor_venda = creditos.valor_bruto_rv,
                    rede = 4
                };

                io_arl_resumo_op.Add(resumo_fi);

                io_arl_creditos.Add(creditos);

                //io_hsm_cv.Add(is_ultimo_ro, io_arl_cv);

                var lo_detalhado1 = new ArquivoDetalhado()
                {
                    registro = "Comprovante de Venda",
                    estabelecimento = lo_cv.estabelecimento,
                    transacao = "",
                    RO = lo_cv.num_resumo,
                    produto = "",
                    cartao = lo_cv.cartao,
                    vl_bruto = lo_cv.vl_venda,
                    vl_liquido = "",
                    parcela = lo_cv.parcela,
                    total_parcela = lo_cv.total_parcela,
                    autorizacao = lo_cv.autorizacao,
                    nsu = lo_cv.nsu
                };

                io_detalhado.Add(lo_detalhado1);

                var lo_completo = new ArquivoCompleto()
                {
                    registro = "Comprovante de Venda",
                    estabelecimento = lo_cv.estabelecimento,
                    RO = lo_cv.num_resumo,
                    parcela = lo_cv.parcela,
                    total_parcela = lo_cv.total_parcela,
                    filer = "",
                    plano = "",
                    tipo_transacao = "",
                    apresentacao = "",
                    prev_pagamento = "",
                    envio_banco = "",
                    cartao = lo_cv.cartao,
                    sinal_valor_bruto = "",
                    valor_bruto = lvalor_bruto.ToString(),
                    sinal_comissao = "",
                    comissao = ltaxa.ToString(),
                    sinal_rejeitado = "",
                    valor_rejeitado = "",
                    sinal_liquido = "",
                    valor_liquido = lvalor_liquido.ToString(),
                    valor_total_venda = lvalor_bruto.ToString(),
                    valor_prox_parcela = "",
                    taxas = "",
                    autorizacao = lo_cv.autorizacao,
                    nsu_doc = lo_cv.nsu,
                    banco = "",
                    agencia = "",
                    conta_corrente = "",
                    status_pagamento = "",
                    motivo_rejeicao = "",
                    cvs_aceitos = "",
                    produto = "",
                    cvs_rejeitados = "",
                    data_venda = lo_cv.dt_venda,
                    data_captura = "",
                    origem_ajuste = "",
                    valor_complementar = "",
                    produto_financeiro = "",
                    valor_antecipado = "",
                    bandeira = bandeira,
                    registro_unico_RO = lo_cv.num_resumo, // is_linha_atual.Substring(11, 7),
                    taxas_comissao = "",
                    tarifa = "",
                    meio_captura = "",
                    terminal = "",
                };
                io_completo.Add(lo_completo);

                var lo_rede = new TransacaoRede
                {
                    ds_rede = "Bin",
                    dt_transacao = Convert.ToDateTime(lo_completo.data_venda),
                    nr_logico = lo_completo.terminal,
                    nsu_rede = lo_completo.nsu_doc,
                    nsu_tef = lo_completo.nsu_doc,

                    vl_bruto = Convert.ToDecimal(lo_completo.valor_bruto),
                    nm_estabelecimento = lo_completo.estabelecimento
                };
                io_arl_rede.Add(lo_rede);
            }
            catch (Exception e)
            {
                throw e;
            }


        }


        /*
         * 
        Desmonta o header do arquivo.
        */
        private void HeaderDesmontar(string _first)
        {
            /*
            Cria a estrutura.
            */
            io_header = new ConciliacaoHeaderStruct
            {
                /* Tipo de registro. */
                is_tipo_registro = _first.Substring(0, 1),
                /* Estabelecimento Matriz. */
                is_estabelecimento_matriz = _first.Substring(1, 10),
                /* Data de processamento.  */
                is_data_processamento = _first.Substring(11, 8),
                /* Periodo inicial. */
                is_periodo_inicial = _first.Substring(19, 8),
                /* Periodo final. */
                is_periodo_final = _first.Substring(27, 8),
                /* Sequencial. */
                is_sequencia = _first.Substring(35, 7),
                /* Empresa adquirente.  */
                is_empresa_adquirente = _first.Substring(42, 5),
                /* Opção extrato. */
                is_opcao_extrato = _first.Substring(47, 2),
                /* Van */
                is_van = _first.Substring(49, 1),
                /* Caixa postal */
                is_caixa_postal = _first.Substring(50, 20),
                /* Versão layout. */
                is_versao_layout = _first.Substring(70, 3)
            };
        }


        private Decimal taxa;
        private Decimal lvalor_bruto;
        private Decimal lvalor_liquido;
        private Decimal ltaxa;

        private string bandeira;
        private string data_pagamento;
        private string data_movimento;
        private string tipo_transacao;
        private string hr_transacao;
        private string status_pagamento;
        private string meio_captura;
        private string produto_ro;
        private string banco_ro;
        private string quantidade_resumo_vendas;
        private string agencia;
        private string conta;
        private string cartao;
        private string autorizacao;
        private Decimal bruto_rv;
        private string parcela;


        private string rv_ajuste;
        private string rv_dt_ajuste;
        private string rv_dt_ajuste_pagamento;
        private string rv_ajuste_banco;
        private string rv_ajuste_agencia;
        private string rv_ajuste_conta;
        private string rv_ajuste_sinal_bruto;


        /**
	    Desmonta o resumo de vendas débito.
	    */
        private
        void RODesmontarDebito(string[] a)
        {
            /*
            Cria a estrutura de registro.
            */
            var resumo = new ArquivoResumo();

            try
            {
                DateTime data_rv = Convert.ToDateTime(FormatoDataExecutar(a[12], "ddMMyyyy", "dd/MM/yyyy")); 
                DateTime dataprevista = Convert.ToDateTime(FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"));
                lvalor_bruto = FormatoValorExecutarDouble(a[15]);
                lvalor_liquido = FormatoValorExecutarDouble(a[4]);
                ltaxa = Math.Round(Math.Abs(((lvalor_liquido * 100 / lvalor_bruto) - 100)), 2);

                resumo = new ArquivoResumo()
                {
                    resumo = a[11], //
                    estabelecimento = a[1], // 
                    parcela = a[13], //
                    transacao = (string)io_hsm_tabela_III[a[7]], //
                    prev_pagamento = FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"), //
                    valor_bruto = lvalor_bruto, //
                    taxa_comissao = ltaxa.ToString("#,##0.00"), // is_linha_atual.Substring(57, 1),
                    vl_comissao = FormatoValorExecutarDouble(a[16]), //
                    vl_rejeitado = "0", //
                    vl_liquido = lvalor_liquido, //
                    situacao = "", //
                    produto = "Vendas Cartão de Débito ", //
                    terminal = "",
                    chave = ""
                    
                };

                taxa = ltaxa;
                bandeira = (string)io_hsm_tabela_VI[a[14]]; // (string) io_hsm_tabela_VI[is_linha_atual.Substring(184, 3)];
                data_pagamento = dataprevista.ToString(); //
                data_movimento = data_rv.ToString(); //
                tipo_transacao = (string)io_hsm_tabela_III[a[7]];
                status_pagamento = "";
                meio_captura = "";
                banco_ro = a[8]; //
                agencia = a[9]; //
                conta = a[10]; // 
                parcela = a[13];  //
                bruto_rv = lvalor_bruto;
                produto_ro = resumo.produto;

                var resumo_debito_bin = new ConciliacaoUseRedeEEVDResumoOperacaoStruct()
                {
                    is_tipo_registro = 1,
                    nm_tipo_registro = tipo_transacao,
                    is_numero_filiacao_pv = Convert.ToDecimal(a[1]),
                    is_numero_resumo_venda = a[11],
                    is_banco = banco_ro,
                    is_agencia = agencia,
                    is_conta_corrente = conta,
                    is_data_resumo_venda = Convert.ToDateTime(data_movimento),

                    is_quantidade_resumo_vendas = "0",
                    is_valor_bruto = bruto_rv,
                    is_valor_desconto = bruto_rv - lvalor_liquido,
                    is_valor_liquido = lvalor_liquido,

                    is_data_credito = dataprevista,
                    is_bandeira = (string)io_hsm_tabela_VI[a[14]],
                    rede = 4
                };

                io_arl_ro_debito_bin.Add(resumo_debito_bin);

                Decimal valor_total = 0;
                Decimal valor_total_liquido = 0;

                if (io_hsm_totalizador_produto.Contains(resumo.produto + dataprevista))
                {
                    var totalizador_produto = (TotalizadorProduto)io_hsm_totalizador_produto[resumo.produto + dataprevista];
                    valor_total = totalizador_produto.valor_bruto + resumo.valor_bruto;
                    valor_total_liquido = totalizador_produto.valor_liquido + resumo.vl_liquido;
                    io_hsm_totalizador_produto.Remove(resumo.produto + dataprevista);
                    io_hsm_totalizador_produto.Add(resumo.produto + dataprevista, new TotalizadorProduto
                    {
                        data_prevista = dataprevista,
                        valor_bruto = valor_total,
                        valor_liquido = valor_total_liquido,
                        ds_produto = resumo.produto,
                        rede = "Bin"
                    });
                }
                else
                {
                    io_hsm_totalizador_produto.Add(resumo.produto + dataprevista, new TotalizadorProduto
                    {
                        data_prevista = dataprevista,
                        valor_bruto = resumo.valor_bruto,
                        valor_liquido = resumo.vl_liquido,
                        ds_produto = resumo.produto,
                        rede = "Bin"
                    });
                }

                valor_total = 0;
                valor_total_liquido = 0;

                string banco = banco_ro;

                if (io_hsm_totalizador_banco.Contains(banco + dataprevista))
                {
                    TotalizadorBanco totalizador_banco = (TotalizadorBanco)io_hsm_totalizador_banco[banco + dataprevista];
                    valor_total_liquido = totalizador_banco.total_realizado + resumo.vl_liquido;
                    io_hsm_totalizador_banco.Remove(banco + dataprevista);
                    io_hsm_totalizador_banco.Add(banco + dataprevista, new TotalizadorBanco
                    {
                        data_prevista = dataprevista,
                        total_realizado = valor_total_liquido,
                        id_banco = Convert.ToInt32(banco)
                    });
                }
                else
                {
                    io_hsm_totalizador_banco.Add(banco + dataprevista, new TotalizadorBanco
                    {
                        data_prevista = dataprevista,
                        total_realizado = resumo.vl_liquido,
                        id_banco = Convert.ToInt32(banco)
                    });
                }


                valor_total = 0;
                valor_total_liquido = 0;

                string estabelecimento = resumo.estabelecimento;

                if (io_hsm_totalizador_estabelecimento.Contains(estabelecimento + dataprevista))
                {
                    var totalizador_estabelecimento = (TotalizadorEstabelecimento)io_hsm_totalizador_estabelecimento[estabelecimento + dataprevista];
                    valor_total_liquido = totalizador_estabelecimento.total_realizado + resumo.vl_liquido;
                    io_hsm_totalizador_estabelecimento.Remove(estabelecimento + dataprevista);
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + dataprevista, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = dataprevista,
                        total_realizado = valor_total_liquido,
                        estabelecimento = Convert.ToInt64(estabelecimento)
                    });
                }
                else
                {
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + dataprevista, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = dataprevista,
                        total_realizado = resumo.vl_liquido,
                        estabelecimento = Convert.ToInt64(estabelecimento)
                    });
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            /*
            GUARDA O ULTIMO RO.
            */
            is_ultimo_ro = resumo.resumo;

            /*
            Adiciona o ro na lista.
            */
            io_arl_ro.Add(resumo);
            io_arl_cv = new ArrayList();

            try
            {
                var lo_detalhado = new ArquivoDetalhado()
                {
                    registro = "Resumo do Pagamento",
                    estabelecimento = resumo.estabelecimento,
                    transacao = resumo.transacao,
                    RO = resumo.resumo,
                    produto = resumo.produto,
                    cartao = "",
                    vl_bruto = resumo.valor_bruto.ToString(),
                    vl_liquido = resumo.vl_liquido.ToString(),
                    parcela = "",
                    total_parcela = "",
                    autorizacao = "",
                    nsu = ""
                };


                io_detalhado.Add(lo_detalhado);
            }
            catch (Exception e)
            {
                throw e;
            }
            try
            {
                var lo_completo = new ArquivoCompleto()
                {
                    registro = "Resumo do Pagamento",
                    estabelecimento = resumo.estabelecimento,
                    RO = resumo.resumo,
                    parcela = "",
                    total_parcela = "",
                    filer = "",
                    plano = "",
                    tipo_transacao = resumo.transacao,
                    apresentacao = data_movimento,
                    prev_pagamento = resumo.prev_pagamento,
                    envio_banco = a[2].Equals("00000000") ? "00/00/00000" : FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"),
                    cartao = "",
                    sinal_valor_bruto = "",
                    valor_bruto = resumo.valor_bruto.ToString(),
                    sinal_comissao = "",
                    comissao = resumo.vl_comissao.ToString(),
                    sinal_rejeitado = "",
                    valor_rejeitado = resumo.vl_rejeitado,
                    sinal_liquido ="",
                    valor_liquido = resumo.vl_liquido.ToString(),
                    valor_total_venda = "",
                    valor_prox_parcela = "",
                    taxas = "",
                    autorizacao = "",
                    nsu_doc = "",
                    banco = banco_ro,
                    agencia = agencia,
                    conta_corrente = conta,
                    status_pagamento = (string)io_hsm_tabela_III[a[7]],
                    motivo_rejeicao = "",
                    cvs_aceitos = a[13],
                    produto = resumo.produto,
                    cvs_rejeitados = "0",
                    data_venda = resumo.prev_pagamento,
                    data_captura = "",
                    origem_ajuste = "",
                    valor_complementar = "",
                    produto_financeiro = "",
                    valor_antecipado = "",
                    bandeira = "",
                    registro_unico_RO = resumo.resumo,
                    taxas_comissao = FormatoValorExecutar(a[16]),
                    tarifa = Math.Round(Math.Abs(((resumo.vl_liquido * 100) / (resumo.valor_bruto) -100)),2).ToString("#,#0.00"),
                    meio_captura = "",
                    terminal = ""
                };

                var lo_rede = new TransacaoRede
                {
                    ds_rede = "Bin",
                    dt_transacao = Convert.ToDateTime(data_movimento),
                    dt_prev_pagto = Convert.ToDateTime(resumo.prev_pagamento),
                    nr_logico = lo_completo.terminal,
                    nsu_rede = "",
                    nsu_tef = "",
                    vl_liquido = Convert.ToDecimal(lo_completo.valor_liquido),
                    vl_bruto = Convert.ToDecimal(lo_completo.valor_bruto),
                    nm_estabelecimento = lo_completo.estabelecimento
                };
                io_arl_rede.Add(lo_rede);

                io_completo.Add(lo_completo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /**
        Desmonta o resumo de vendas crédito.
        */
        private
        void RODesmontarCredito(string[] a)
        {
            /*
            Cria a estrutura de registro.
            */
            var resumo = new ArquivoResumo();

            try
            {
                DateTime data_rv = Convert.ToDateTime(FormatoDataExecutar(a[12], "ddMMyyyy", "dd/MM/yyyy"));
                DateTime dataprevista = Convert.ToDateTime(FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"));
                lvalor_bruto = FormatoValorExecutarDouble(a[15]);
                lvalor_liquido = FormatoValorExecutarDouble(a[4]);
                ltaxa = Math.Round(Math.Abs(((lvalor_liquido * 100 / lvalor_bruto) - 100)), 2);

                resumo = new ArquivoResumo()
                {
                    resumo = a[11], //
                    estabelecimento = a[1], // 
                    parcela = a[13], //
                    transacao = (string)io_hsm_tabela_III[a[7]], //
                    prev_pagamento = FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"), //
                    valor_bruto = lvalor_bruto, //
                    taxa_comissao = ltaxa.ToString("#,##0.00"), // is_linha_atual.Substring(57, 1),
                    vl_comissao = FormatoValorExecutarDouble(a[16]), //
                    vl_rejeitado = "0", //
                    vl_liquido = lvalor_liquido, //
                    situacao = "", //
                    produto = "Vendas Cartão Crédito", //
                    terminal = "",
                    chave = ""

                };

                taxa = ltaxa;
                bandeira = (string)io_hsm_tabela_VI[a[14]];  // (string) io_hsm_tabela_VI[is_linha_atual.Substring(184, 3)];
                data_pagamento = data_rv.ToString(); //
                data_movimento = dataprevista.ToString(); //
                tipo_transacao = (string)io_hsm_tabela_III[a[7]];
                status_pagamento = "";
                meio_captura = "";
                banco_ro = a[8]; //
                agencia = a[9]; //
                conta = a[10]; // 
                parcela = a[13];  //
                bruto_rv = lvalor_bruto;
                produto_ro = resumo.produto;

                var resumo_bin = new ConciliacaoUseRedeEEVCResumoOperacaoStruct()
                {
                    is_tipo_registro = 22,
                    nm_tipo_registro = tipo_transacao,
                    is_numero_filiacao_pv = Convert.ToDecimal(a[1]),
                    is_numero_resumo_venda = a[11],
                    is_banco = banco_ro,
                    is_agencia = agencia,
                    is_conta_corrente = conta,
                    is_data_resumo_venda = data_rv,

                    is_quantidade_resumo_vendas = a[13],
                    is_valor_bruto = bruto_rv,

                    is_valor_gorjeta = 0,

                    is_valor_rejeitado = 0,
                    is_valor_desconto = bruto_rv - lvalor_liquido,
                    is_valor_liquido = lvalor_liquido,

                    is_data_credito = dataprevista,
                    is_bandeira = (string)io_hsm_tabela_VI[a[14]],
                    rede = 4
                };

                
                io_arl_ro_credito_bin.Add(resumo_bin);
                


                Decimal valor_total = 0;
                Decimal valor_total_liquido = 0;

                if (io_hsm_totalizador_produto.Contains(resumo.produto + dataprevista))
                {
                    var totalizador_produto = (TotalizadorProduto)io_hsm_totalizador_produto[resumo.produto + dataprevista];
                    valor_total = totalizador_produto.valor_bruto + resumo.valor_bruto;
                    valor_total_liquido = totalizador_produto.valor_liquido + resumo.vl_liquido;
                    io_hsm_totalizador_produto.Remove(resumo.produto + dataprevista);
                    io_hsm_totalizador_produto.Add(resumo.produto + dataprevista, new TotalizadorProduto
                    {
                        data_prevista = dataprevista,
                        valor_bruto = valor_total,
                        valor_liquido = valor_total_liquido,
                        ds_produto = resumo.produto,
                        rede = "Bin"
                    });
                }
                else
                {
                    io_hsm_totalizador_produto.Add(resumo.produto + dataprevista, new TotalizadorProduto
                    {
                        data_prevista = dataprevista,
                        valor_bruto = resumo.valor_bruto,
                        valor_liquido = resumo.vl_liquido,
                        ds_produto = resumo.produto,
                        rede = "Bin"
                    });
                }


                valor_total = 0;
                valor_total_liquido = 0;

                string banco = banco_ro;

                if (io_hsm_totalizador_banco.Contains(banco + dataprevista))
                {
                    TotalizadorBanco totalizador_banco = (TotalizadorBanco)io_hsm_totalizador_banco[banco + dataprevista];
                    valor_total_liquido = totalizador_banco.total_realizado + resumo.vl_liquido;
                    io_hsm_totalizador_banco.Remove(banco + dataprevista);
                    io_hsm_totalizador_banco.Add(banco + dataprevista, new TotalizadorBanco
                    {
                        data_prevista = dataprevista,
                        total_realizado = valor_total_liquido,
                        id_banco = Convert.ToInt32(banco)
                    });
                }
                else
                {
                    io_hsm_totalizador_banco.Add(banco + dataprevista, new TotalizadorBanco
                    {
                        data_prevista = dataprevista,
                        total_realizado = resumo.vl_liquido,
                        id_banco = Convert.ToInt32(banco)
                    });
                }


                valor_total = 0;
                valor_total_liquido = 0;

                string estabelecimento = resumo.estabelecimento;

                if (io_hsm_totalizador_estabelecimento.Contains(estabelecimento + dataprevista))
                {
                    var totalizador_estabelecimento = (TotalizadorEstabelecimento)io_hsm_totalizador_estabelecimento[estabelecimento + dataprevista];
                    valor_total_liquido = totalizador_estabelecimento.total_realizado + resumo.vl_liquido;
                    io_hsm_totalizador_estabelecimento.Remove(estabelecimento + dataprevista);
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + dataprevista, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = dataprevista,
                        total_realizado = valor_total_liquido,
                        estabelecimento = Convert.ToInt64(estabelecimento)
                    });
                }
                else
                {
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + dataprevista, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = dataprevista,
                        total_realizado = resumo.vl_liquido,
                        estabelecimento = Convert.ToInt64(estabelecimento)
                    });
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            /*
            GUARDA O ULTIMO RO.
            */
            is_ultimo_ro = is_linha_atual.Substring(18, 9);

            /*
            Adiciona o ro na lista.
            */
            io_arl_ro.Add(resumo);
            io_arl_cv = new ArrayList();

            try
            {
                var lo_detalhado = new ArquivoDetalhado()
                {
                    registro = "Resumo do Pagamento",
                    estabelecimento = resumo.estabelecimento,
                    transacao = resumo.transacao,
                    RO = resumo.resumo,
                    produto = resumo.produto,
                    cartao = "",
                    vl_bruto = resumo.valor_bruto.ToString(),
                    vl_liquido = resumo.vl_liquido.ToString(),
                    parcela = "",
                    total_parcela = "",
                    autorizacao = "",
                    nsu = ""
                };


                io_detalhado.Add(lo_detalhado);
            }
            catch (Exception e)
            {
                throw e;
            }
            try
            {
                var lo_completo = new ArquivoCompleto()
                {
                    registro = "Resumo do Pagamento",
                    estabelecimento = resumo.estabelecimento,
                    RO = resumo.resumo,
                    parcela = "",
                    total_parcela = "",
                    filer = "",
                    plano = "",
                    tipo_transacao = resumo.transacao,
                    apresentacao = data_movimento,
                    prev_pagamento = resumo.prev_pagamento,
                    envio_banco = a[2].Equals("00000000") ? "00/00/00000" : FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"),
                    cartao = "",
                    sinal_valor_bruto = "",
                    valor_bruto = resumo.valor_bruto.ToString(),
                    sinal_comissao = "",
                    comissao = resumo.vl_comissao.ToString(),
                    sinal_rejeitado = "",
                    valor_rejeitado = resumo.vl_rejeitado,
                    sinal_liquido = "",
                    valor_liquido = resumo.vl_liquido.ToString(),
                    valor_total_venda = "",
                    valor_prox_parcela = "",
                    taxas = "",
                    autorizacao = "",
                    nsu_doc = "",
                    banco = banco_ro,
                    agencia = agencia,
                    conta_corrente = conta,
                    status_pagamento = (string)io_hsm_tabela_III[a[7]],
                    motivo_rejeicao = "",
                    cvs_aceitos = a[13],
                    produto = resumo.produto,
                    cvs_rejeitados = "0",
                    data_venda = resumo.prev_pagamento,
                    data_captura = "",
                    origem_ajuste = "",
                    valor_complementar = "",
                    produto_financeiro = "",
                    valor_antecipado = "",
                    bandeira = "",
                    registro_unico_RO = resumo.resumo,
                    taxas_comissao = FormatoValorExecutar(a[16]),
                    tarifa = Math.Round(Math.Abs(((resumo.vl_liquido * 100) / (resumo.valor_bruto) - 100)),2).ToString("#,##0.00"),
                    meio_captura = "",
                    terminal = ""
                };

                var lo_rede = new TransacaoRede
                {
                    ds_rede = "Bin",
                    dt_transacao = Convert.ToDateTime(data_movimento),
                    dt_prev_pagto = Convert.ToDateTime(resumo.prev_pagamento),
                    nr_logico = lo_completo.terminal,
                    nsu_rede = "",
                    nsu_tef = "",
                    vl_liquido = Convert.ToDecimal(lo_completo.valor_liquido),
                    vl_bruto = Convert.ToDecimal(lo_completo.valor_bruto),
                    nm_estabelecimento = lo_completo.estabelecimento
                };
                io_arl_rede.Add(lo_rede);

                io_completo.Add(lo_completo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /*
        Comprovante de venda débito.
        */
        private
        void CVDesmontarDebito(string[] a)
        {
            try
            {
                var lo_cv = new ArquivoTransacao()
                {
                    num_resumo = a[13],
                    registro = "Comprovante de Venda",
                    estabelecimento = a[1],
                    num_logico = "",
                    dt_venda = FormatoDataExecutar(a[7], "ddMMyyyy", "dd/MM/yyyy"),
                    cartao = a[8],
                    vl_venda = FormatoValorExecutar(a[5]),
                    parcela = "1",
                    total_parcela = "1",
                    autorizacao = a[10],
                    nsu = a[3]
                };

                io_arl_cv.Add(lo_cv);
                io_arl_just_cv.Add(lo_cv);

                var resumo = new ConciliacaoUseRedeEEVDComprovanteVendaStruct()
                {
                    is_tipo_registro = 21,
                    nm_tipo_registro = "CV - Detalhamento",
                    is_numero_filiacao_pv = Convert.ToDecimal(a[1]),
                    is_numero_resumo_vendas = Convert.ToInt64(a[13]),
                    is_data_cv = Convert.ToDateTime(FormatoDataExecutar(a[7], "ddMMyyyy", "dd/MM/yyyy")),
                    is_valor_bruto = Convert.ToDecimal(a[5]) / 100,
                    is_valor_liquido = (Convert.ToDecimal(a[4]) / 100),
                    is_valor_desconto = (Convert.ToDecimal(a[6]) / 100),

                    is_parcela = 1.ToString(),
                    is_numero_parcelas = 1,
                    is_numero_cartao = a[8],
                    is_tipo_transacao = tipo_transacao,
                    is_numero_cv = Convert.ToDecimal(a[3]),
                    is_data_credito = Convert.ToDateTime(data_pagamento),
                    is_status_transacao = status_pagamento,
                    is_hora_transacao = "",
                    is_numero_terminal = "",
                    is_numero_autorizacao = a[10],
                    is_tipo_captura = meio_captura,
                    is_reservado = "",
                    is_valor_compra = Convert.ToDecimal(a[5]) / 100,
                    is_valor_liquido_primeira_parc = Convert.ToDecimal(a[4]) / 100,
                    is_valor_liquido_demais_parc = Convert.ToDecimal(a[4]) / 100,
                    numero_nota_fiscal = "",
                    numero_unico_transacao = a[9],
                    is_valor_saque = 0,
                    is_bandeira = bandeira,
                    taxa_cobrada = taxa,
                    rede = 4

                    //   is_codigo_tef = achou.Codigt
                };

                io_arl_cv_debito_bin.Add(resumo);

                //io_hsm_cv.Add(is_ultimo_ro, io_arl_cv);

                var lo_detalhado1 = new ArquivoDetalhado()
                {
                    registro = "Comprovante de Venda",
                    estabelecimento = lo_cv.estabelecimento,
                    transacao = "",
                    RO = lo_cv.num_resumo,
                    produto = "",
                    cartao = lo_cv.cartao,
                    vl_bruto = lo_cv.vl_venda,
                    vl_liquido = "",
                    parcela = lo_cv.parcela,
                    total_parcela = lo_cv.total_parcela,
                    autorizacao = lo_cv.autorizacao,
                    nsu = lo_cv.nsu
                };

                io_detalhado.Add(lo_detalhado1);

                var lo_completo = new ArquivoCompleto()
                {
                    registro = "Comprovante de Venda",
                    estabelecimento = lo_cv.estabelecimento,
                    RO = lo_cv.num_resumo,
                    parcela = lo_cv.parcela,
                    total_parcela = lo_cv.total_parcela,
                    filer = "",
                    plano = "",
                    tipo_transacao = "",
                    apresentacao = "",
                    prev_pagamento = "",
                    envio_banco = "",
                    cartao = lo_cv.cartao,
                    sinal_valor_bruto = "",
                    valor_bruto = lvalor_bruto.ToString(),
                    sinal_comissao = "",
                    comissao = ltaxa.ToString(),
                    sinal_rejeitado = "",
                    valor_rejeitado = "",
                    sinal_liquido = "",
                    valor_liquido = lvalor_liquido.ToString(),
                    valor_total_venda = lvalor_bruto.ToString(),
                    valor_prox_parcela = "",
                    taxas = "",
                    autorizacao = lo_cv.autorizacao,
                    nsu_doc = lo_cv.nsu,
                    banco = "",
                    agencia = "",
                    conta_corrente = "",
                    status_pagamento = "",
                    motivo_rejeicao = "",
                    cvs_aceitos = "",
                    produto = "",
                    cvs_rejeitados = "",
                    data_venda = lo_cv.dt_venda,
                    data_captura = "",
                    origem_ajuste = "",
                    valor_complementar = "",
                    produto_financeiro = "",
                    valor_antecipado = "",
                    bandeira = "",
                    registro_unico_RO = lo_cv.num_resumo, // is_linha_atual.Substring(11, 7),
                    taxas_comissao = "",
                    tarifa = "",
                    meio_captura = "",
                    terminal = ""
                };
                io_completo.Add(lo_completo);

                var lo_rede = new TransacaoRede
                {
                    ds_rede = "Bin",
                    dt_transacao = Convert.ToDateTime(lo_completo.data_venda),
                    nr_logico = lo_completo.terminal,
                    nsu_rede = lo_completo.nsu_doc,
                    nsu_tef = lo_completo.nsu_doc,

                    vl_bruto = Convert.ToDecimal(lo_completo.valor_bruto),
                    nm_estabelecimento = lo_completo.estabelecimento
                };
                io_arl_rede.Add(lo_rede);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /*
          Comprovante de venda crédito.
          */
        private
        void CVDesmontarCredito(string[] a)
        {
            try
            {
                var lo_cv = new ArquivoTransacao()
                {
                    num_resumo = a[13],
                    registro = "Comprovante de Venda",
                    estabelecimento = a[1],
                    num_logico = "",
                    dt_venda = FormatoDataExecutar(a[7], "ddMMyyyy", "dd/MM/yyyy"),
                    cartao = a[8],
                    vl_venda = FormatoValorExecutar(a[5]),
                    parcela = "1",
                    total_parcela = "1",
                    autorizacao = a[10],
                    nsu = a[3]
                };

                io_arl_cv.Add(lo_cv);
                io_arl_just_cv.Add(lo_cv);


                var comprovante = new ConciliacaoUseRedeEEVCComprovanteVendaStruct()
                {
                    is_tipo_registro = 23,
                    nm_tipo_registro = "CV/NSU",
                    is_numero_filiacao_pv = Convert.ToDecimal(a[1]),
                    is_numero_resumo_vendas = Convert.ToInt64(a[13]),

                    is_data_cv =
                        Convert.ToDateTime(FormatoDataExecutar(a[7], "ddMMyyyy", "dd/MM/yyyy")),

                    is_valor_bruto = Convert.ToDecimal(a[5]) / 100,
                    is_valor_gorjeta = 0, //Convert.ToDecimal(is_linha_atual.Substring(52, 15)) / 100,
                    is_numero_cartao = a[8],

                    is_status_cv = "",

                    is_parcela = 1.ToString(),
                    is_numero_parcelas =1,
                    is_numero_cv = Convert.ToDecimal(a[3]),

                    is_numero_referencia = "0", //a.Substring(100, 13),


                    is_valor_desconto = (Convert.ToDecimal(a[6]) / 100),

                    is_numero_autorizacao = a[10],

                    is_hora_transcao = "",

                    is_tipo_captura = (string)io_hsm_tabela_I[a[11]],

                    is_valor_liquido =
                        (Convert.ToDecimal(a[4]) / 100),
                    // Convert.ToDecimal(a.Substring(205, 15)) / 100,#VER

                    //   is_valor_liquido_primeira_parc =  Convert.ToDecimal(is_linha_atual.Substring(113, 13)) / 100,
                    //   is_valor_liquido_demais_parc = Convert.ToDecimal(is_linha_atual.Substring(126, 13)) / 100,
                    //   numero_nota_fiscal = is_linha_atual.Substring(139, 9),
                    numero_unico_transacao = a[9],
                    //  is_numero_terminal = is_linha_atual.Substring(52, 8),

                    is_bandeira = bandeira, //TabelaI(a.Substring(261, 1)), #VER

                    taxa_cobrada = Math.Round(Math.Abs(((Convert.ToDecimal(a[4]) * 100) / (Convert.ToDecimal(a[5])) - 100)),2),
                    is_data_credito = Convert.ToDateTime(FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy")),
                    rede = 4

                };

                io_arl_cv_credito_bin.Add(comprovante);

                var creditos = new ConciliacaoUseRedeEEFICreditosStruct()
                {
                    numero_pv_centralizador = comprovante.is_numero_filiacao_pv,
                    numero_documento = comprovante.is_numero_cv,
                    data_lancamento = comprovante.is_data_credito,
                    valor_lancamento = comprovante.is_valor_liquido,
                    banco = Convert.ToInt32(banco_ro),
                    agencia = agencia,
<<<<<<< HEAD
                    conta_corrente = conta,
=======
                    conta_corrente = Convert.ToInt64(conta),
>>>>>>> 7d4f72e90771fdbe2ddce8e4d5c620260095bf10
                    data_movimento = Convert.ToDateTime(data_movimento),
                    numero_rv = Convert.ToInt64(comprovante.is_numero_resumo_vendas.ToString()),
                    data_rv = comprovante.is_data_cv,
                    bandeira = bandeira,
                    tipo_transacao = tipo_transacao,
                    valor_bruto_rv = bruto_rv,
                    valor_taxa_desconto = taxa,
                    numero_parcela = parcela,
                    situacao = "",
                    numero_pv_original = Convert.ToInt64(comprovante.is_numero_filiacao_pv),
                    rede = 4
                };

                var resumo_fi = new ConciliacaoUseRedeEEFIResumoOperacaoStruct()
                {
                    agencia = creditos.agencia,
                    banco = creditos.banco,
                    conta_corrente = creditos.conta_corrente,
                    data = creditos.data_lancamento,
                    data_venda = creditos.data_rv,
                    nsu = creditos.numero_rv.ToString(),
                    descricao = "Créditos",
                    numero_pv = creditos.numero_pv_original.ToString(),
                    situacao = creditos.situacao,
                    valor = creditos.valor_lancamento,
                    valor_cancelado = 0,
                    valor_venda = creditos.valor_bruto_rv,
                    rede = 4
                };

                io_arl_resumo_op.Add(resumo_fi);

                io_arl_creditos.Add(creditos);

                //io_hsm_cv.Add(is_ultimo_ro, io_arl_cv);

                var lo_detalhado1 = new ArquivoDetalhado()
                {
                    registro = "Comprovante de Venda",
                    estabelecimento = lo_cv.estabelecimento,
                    transacao = "",
                    RO = lo_cv.num_resumo,
                    produto = "",
                    cartao = lo_cv.cartao,
                    vl_bruto = lo_cv.vl_venda,
                    vl_liquido = "",
                    parcela = lo_cv.parcela,
                    total_parcela = lo_cv.total_parcela,
                    autorizacao = lo_cv.autorizacao,
                    nsu = lo_cv.nsu
                };

                io_detalhado.Add(lo_detalhado1);

                var lo_completo = new ArquivoCompleto()
                {
                    registro = "Comprovante de Venda",
                    estabelecimento = lo_cv.estabelecimento,
                    RO = lo_cv.num_resumo,
                    parcela = lo_cv.parcela,
                    total_parcela = lo_cv.total_parcela,
                    filer = "",
                    plano = "",
                    tipo_transacao = "",
                    apresentacao = "",
                    prev_pagamento = "",
                    envio_banco = "",
                    cartao = lo_cv.cartao,
                    sinal_valor_bruto = "",
                    valor_bruto = lvalor_bruto.ToString(),
                    sinal_comissao = "",
                    comissao = ltaxa.ToString(),
                    sinal_rejeitado = "",
                    valor_rejeitado = "",
                    sinal_liquido = "",
                    valor_liquido = lvalor_liquido.ToString(),
                    valor_total_venda = lvalor_bruto.ToString(),
                    valor_prox_parcela = "",
                    taxas = "",
                    autorizacao = lo_cv.autorizacao,
                    nsu_doc = lo_cv.nsu,
                    banco = "",
                    agencia = "",
                    conta_corrente = "",
                    status_pagamento = "",
                    motivo_rejeicao = "",
                    cvs_aceitos = "",
                    produto = "",
                    cvs_rejeitados = "",
                    data_venda = lo_cv.dt_venda,
                    data_captura = "",
                    origem_ajuste = "",
                    valor_complementar = "",
                    produto_financeiro = "",
                    valor_antecipado = "",
                    bandeira = bandeira,
                    registro_unico_RO = lo_cv.num_resumo, // is_linha_atual.Substring(11, 7),
                    taxas_comissao = "",
                    tarifa = "",
                    meio_captura = "",
                    terminal = "",
                };
                io_completo.Add(lo_completo);

                var lo_rede = new TransacaoRede
                {
                    ds_rede = "Bin",
                    dt_transacao = Convert.ToDateTime(lo_completo.data_venda),
                    nr_logico = lo_completo.terminal,
                    nsu_rede = lo_completo.nsu_doc,
                    nsu_tef = lo_completo.nsu_doc,

                    vl_bruto = Convert.ToDecimal(lo_completo.valor_bruto),
                    nm_estabelecimento = lo_completo.estabelecimento
                };
                io_arl_rede.Add(lo_rede);
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        /**
          Desmonta o resumo de vendas parcelados.
        */
        private
        void RODesmontarParcelados(string[] a)
        {
            /*
            Cria a estrutura de registro.
            */
            var resumo = new ArquivoResumo();

            try
            {
                DateTime data_rv = Convert.ToDateTime(FormatoDataExecutar(a[14], "ddMMyyyy", "dd/MM/yyyy"));
                DateTime dataprevista = Convert.ToDateTime(FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"));
                lvalor_bruto = FormatoValorExecutarDouble(a[17]);
                lvalor_liquido = FormatoValorExecutarDouble(a[19]);
                ltaxa = Math.Round(Math.Abs(((lvalor_liquido * 100 / lvalor_bruto) - 100)), 2); 

                resumo = new ArquivoResumo()
                {
                    resumo = a[13], //
                    estabelecimento = a[1], // 
                    parcela = a[15], //
                    transacao = (string)io_hsm_tabela_III[a[6]], //
                    prev_pagamento = dataprevista.ToString(), //
                    valor_bruto = lvalor_bruto, //
                    taxa_comissao = ltaxa.ToString("#,##0.00"), // is_linha_atual.Substring(57, 1),
                    vl_comissao = FormatoValorExecutarDouble(a[16]), //
                    vl_rejeitado = "0", //
                    vl_liquido = lvalor_liquido, //
                    situacao = "", //
                    produto = "Vendas Crédito Parcelado ", //
                    terminal = "",
                    chave = ""

                };

                taxa = ltaxa;
                bandeira = (string)io_hsm_tabela_VI[a[16]];  // (string) io_hsm_tabela_VI[is_linha_atual.Substring(184, 3)];
                data_pagamento = data_rv.ToString(); //
                data_movimento = dataprevista.ToString(); //
                tipo_transacao = (string)io_hsm_tabela_I[a[6]];
                status_pagamento = "";
                meio_captura = "";
                banco_ro = a[10]; //
                agencia = a[11]; //
                conta = a[12]; // 
                parcela = a[9];  //
                bruto_rv = lvalor_bruto;
                produto_ro = resumo.produto;

                var resumo_bin = new ConciliacaoUseRedeEEVCResumoOperacaoStruct()
                {
                    is_tipo_registro = 24,
                    nm_tipo_registro = tipo_transacao,
                    is_numero_filiacao_pv = Convert.ToDecimal(a[1]),
                    is_numero_resumo_venda = a[13],
                    is_banco = banco_ro,
                    is_agencia = agencia,
                    is_conta_corrente = conta,
                    is_data_resumo_venda = data_rv,

                    is_quantidade_resumo_vendas = a[9],
                    is_valor_bruto = bruto_rv,

                    is_valor_gorjeta = 0,

                    is_valor_rejeitado = 0,
                    is_valor_desconto = bruto_rv - lvalor_liquido,
                    is_valor_liquido = lvalor_liquido,

                    is_data_credito = dataprevista,
                    is_bandeira = (string)io_hsm_tabela_VI[a[16]],
                    rede = 4
                };


                io_arl_ro_credito_bin.Add(resumo_bin);



                Decimal valor_total = 0;
                Decimal valor_total_liquido = 0;

                if (io_hsm_totalizador_produto.Contains(resumo.produto + dataprevista))
                {
                    var totalizador_produto = (TotalizadorProduto)io_hsm_totalizador_produto[resumo.produto + dataprevista];
                    valor_total = totalizador_produto.valor_bruto + resumo.valor_bruto;
                    valor_total_liquido = totalizador_produto.valor_liquido + resumo.vl_liquido;
                    io_hsm_totalizador_produto.Remove(resumo.produto + dataprevista);
                    io_hsm_totalizador_produto.Add(resumo.produto + dataprevista, new TotalizadorProduto
                    {
                        data_prevista = dataprevista,
                        valor_bruto = valor_total,
                        valor_liquido = valor_total_liquido,
                        ds_produto = resumo.produto,
                        rede = "Bin"
                    });
                }
                else
                {
                    io_hsm_totalizador_produto.Add(resumo.produto + dataprevista, new TotalizadorProduto
                    {
                        data_prevista = dataprevista,
                        valor_bruto = resumo.valor_bruto,
                        valor_liquido = resumo.vl_liquido,
                        ds_produto = resumo.produto,
                        rede = "Bin"
                    });
                }


                valor_total = 0;
                valor_total_liquido = 0;

                string banco = banco_ro;

                if (io_hsm_totalizador_banco.Contains(banco + dataprevista))
                {
                    TotalizadorBanco totalizador_banco = (TotalizadorBanco)io_hsm_totalizador_banco[banco + dataprevista];
                    valor_total_liquido = totalizador_banco.total_realizado + resumo.vl_liquido;
                    io_hsm_totalizador_banco.Remove(banco + dataprevista);
                    io_hsm_totalizador_banco.Add(banco + dataprevista, new TotalizadorBanco
                    {
                        data_prevista = dataprevista,
                        total_realizado = valor_total_liquido,
                        id_banco = Convert.ToInt32(banco)
                    });
                }
                else
                {
                    io_hsm_totalizador_banco.Add(banco + dataprevista, new TotalizadorBanco
                    {
                        data_prevista = dataprevista,
                        total_realizado = resumo.vl_liquido,
                        id_banco = Convert.ToInt32(banco)
                    });
                }


                valor_total = 0;
                valor_total_liquido = 0;

                string estabelecimento = resumo.estabelecimento;

                if (io_hsm_totalizador_estabelecimento.Contains(estabelecimento + dataprevista))
                {
                    var totalizador_estabelecimento = (TotalizadorEstabelecimento)io_hsm_totalizador_estabelecimento[estabelecimento + dataprevista];
                    valor_total_liquido = totalizador_estabelecimento.total_realizado + resumo.vl_liquido;
                    io_hsm_totalizador_estabelecimento.Remove(estabelecimento + dataprevista);
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + dataprevista, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = dataprevista,
                        total_realizado = valor_total_liquido,
                        estabelecimento = Convert.ToInt64(estabelecimento)
                    });
                }
                else
                {
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + dataprevista, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = dataprevista,
                        total_realizado = resumo.vl_liquido,
                        estabelecimento = Convert.ToInt64(estabelecimento)
                    });
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            /*
            GUARDA O ULTIMO RO.
            */
            is_ultimo_ro = is_linha_atual.Substring(18, 9);

            /*
            Adiciona o ro na lista.
            */
            io_arl_ro.Add(resumo);
            io_arl_cv = new ArrayList();

            try
            {
                var lo_detalhado = new ArquivoDetalhado()
                {
                    registro = "Resumo do Pagamento",
                    estabelecimento = resumo.estabelecimento,
                    transacao = resumo.transacao,
                    RO = resumo.resumo,
                    produto = resumo.produto,
                    cartao = "",
                    vl_bruto = resumo.valor_bruto.ToString(),
                    vl_liquido = resumo.vl_liquido.ToString(),
                    parcela = "",
                    total_parcela = "",
                    autorizacao = "",
                    nsu = ""
                };


                io_detalhado.Add(lo_detalhado);
            }
            catch (Exception e)
            {
                throw e;
            }
            try
            {
                var lo_completo = new ArquivoCompleto()
                {
                    registro = "Resumo do Pagamento",
                    estabelecimento = resumo.estabelecimento,
                    RO = resumo.resumo,
                    parcela = "",
                    total_parcela = "",
                    filer = "",
                    plano = "",
                    tipo_transacao = resumo.transacao,
                    apresentacao = data_movimento,
                    prev_pagamento = resumo.prev_pagamento,
                    envio_banco = a[2].Equals("00000000") ? "00/00/00000" : FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy"),
                    cartao = "",
                    sinal_valor_bruto = "",
                    valor_bruto = resumo.valor_bruto.ToString(),
                    sinal_comissao = "",
                    comissao = resumo.vl_comissao.ToString(),
                    sinal_rejeitado = "",
                    valor_rejeitado = resumo.vl_rejeitado,
                    sinal_liquido = "",
                    valor_liquido = resumo.vl_liquido.ToString(),
                    valor_total_venda = "",
                    valor_prox_parcela = "",
                    taxas = "",
                    autorizacao = "",
                    nsu_doc = "",
                    banco = banco_ro,
                    agencia = agencia,
                    conta_corrente = conta,
                    status_pagamento = (string)io_hsm_tabela_III[a[7]],
                    motivo_rejeicao = "",
                    cvs_aceitos = a[9],
                    produto = resumo.produto,
                    cvs_rejeitados = "0",
                    data_venda = resumo.prev_pagamento,
                    data_captura = "",
                    origem_ajuste = "",
                    valor_complementar = "",
                    produto_financeiro = "",
                    valor_antecipado = "",
                    bandeira = "",
                    registro_unico_RO = resumo.resumo,
                    taxas_comissao = FormatoValorExecutar(a[18]),
                    tarifa = Math.Round(Math.Abs(((resumo.vl_liquido * 100) / (resumo.valor_bruto) - 100)),2).ToString("#,##0.00"),
                    meio_captura = "",
                    terminal = ""
                };

                var lo_rede = new TransacaoRede
                {
                    ds_rede = "Bin",
                    dt_transacao = Convert.ToDateTime(data_movimento),
                    dt_prev_pagto = Convert.ToDateTime(resumo.prev_pagamento),
                    nr_logico = lo_completo.terminal,
                    nsu_rede = "",
                    nsu_tef = "",
                    vl_liquido = Convert.ToDecimal(lo_completo.valor_liquido),
                    vl_bruto = Convert.ToDecimal(lo_completo.valor_bruto),
                    nm_estabelecimento = lo_completo.estabelecimento
                };
                io_arl_rede.Add(lo_rede);

                io_completo.Add(lo_completo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*
        Comprovante de venda parcelados.
        */
        private
        void CVDesmontarParcelados(string[] a)
        {
            try
            {
                var lo_cv = new ArquivoTransacao()
                {
                    num_resumo = a[15],
                    registro = "Comprovante de Venda",
                    estabelecimento = a[1],
                    num_logico = "",
                    dt_venda = FormatoDataExecutar(a[9], "ddMMyyyy", "dd/MM/yyyy"),
                    cartao = a[10],
                    vl_venda = FormatoValorExecutar(a[7]),
                    parcela = a[5],
                    total_parcela = a[6],
                    autorizacao = a[12],
                    nsu = a[3]
                };

                io_arl_cv.Add(lo_cv);
                io_arl_just_cv.Add(lo_cv);


                var comprovante = new ConciliacaoUseRedeEEVCComprovanteVendaStruct()
                {
                    is_tipo_registro = 26,
                    nm_tipo_registro = "CV/NSU",
                    is_numero_filiacao_pv = Convert.ToDecimal(lo_cv.estabelecimento),
                    is_numero_resumo_vendas = Convert.ToInt64(lo_cv.num_resumo),

                    is_data_cv =
                        Convert.ToDateTime(lo_cv.dt_venda),

                    is_valor_bruto = Convert.ToDecimal(a[7]) / 100,
                    is_valor_gorjeta = 0, //Convert.ToDecimal(is_linha_atual.Substring(52, 15)) / 100,
                    is_numero_cartao = lo_cv.cartao,

                    is_status_cv = "",

                    is_parcela = 1.ToString(),
                    is_numero_parcelas = 1,
                    is_numero_cv = Convert.ToDecimal(lo_cv.nsu),

                    is_numero_referencia = "0", //a.Substring(100, 13),


                    is_valor_desconto = (Convert.ToDecimal(a[8]) / 100),

                    is_numero_autorizacao = a[12],

                    is_hora_transcao = "",

                    is_tipo_captura = (string)io_hsm_tabela_I[a[13]],

                    is_valor_liquido =
                        (Convert.ToDecimal(a[4]) / 100),
                    // Convert.ToDecimal(a.Substring(205, 15)) / 100,#VER

                    //   is_valor_liquido_primeira_parc =  Convert.ToDecimal(is_linha_atual.Substring(113, 13)) / 100,
                    //   is_valor_liquido_demais_parc = Convert.ToDecimal(is_linha_atual.Substring(126, 13)) / 100,
                    //   numero_nota_fiscal = is_linha_atual.Substring(139, 9),
                    numero_unico_transacao = a[11],
                    //  is_numero_terminal = is_linha_atual.Substring(52, 8),

                    is_bandeira = bandeira, //TabelaI(a.Substring(261, 1)), #VER

                    taxa_cobrada = Math.Round(Math.Abs(((Convert.ToDecimal(a[4]) * 100) / (Convert.ToDecimal(a[7])) - 100)),2),
                    is_data_credito = Convert.ToDateTime(FormatoDataExecutar(a[2], "ddMMyyyy", "dd/MM/yyyy")),
                    rede = 4

                };

                io_arl_cv_credito_bin.Add(comprovante);

                var creditos = new ConciliacaoUseRedeEEFICreditosStruct()
                {
                    numero_pv_centralizador = comprovante.is_numero_filiacao_pv,
                    numero_documento = comprovante.is_numero_cv,
                    data_lancamento = comprovante.is_data_credito,
                    valor_lancamento = comprovante.is_valor_liquido,
                    banco = Convert.ToInt32(banco_ro),
                    agencia = agencia,
<<<<<<< HEAD
                    conta_corrente = conta,
=======
                    conta_corrente = Convert.ToInt64(conta),
>>>>>>> 7d4f72e90771fdbe2ddce8e4d5c620260095bf10
                    data_movimento = Convert.ToDateTime(data_movimento),
                    numero_rv = Convert.ToInt64(comprovante.is_numero_resumo_vendas.ToString()),
                    data_rv = comprovante.is_data_cv,
                    bandeira = bandeira,
                    tipo_transacao = tipo_transacao,
                    valor_bruto_rv = bruto_rv,
                    valor_taxa_desconto = taxa,
                    numero_parcela = parcela,
                    situacao = "",
                    numero_pv_original = Convert.ToInt64(comprovante.is_numero_filiacao_pv),
                    rede = 4
                };

                var resumo_fi = new ConciliacaoUseRedeEEFIResumoOperacaoStruct()
                {
                    agencia = creditos.agencia,
                    banco = creditos.banco,
                    conta_corrente = creditos.conta_corrente,
                    data = creditos.data_lancamento,
                    data_venda = creditos.data_rv,
                    nsu = creditos.numero_rv.ToString(),
                    descricao = "Créditos",
                    numero_pv = creditos.numero_pv_original.ToString(),
                    situacao = creditos.situacao,
                    valor = creditos.valor_lancamento,
                    valor_cancelado = 0,
                    valor_venda = creditos.valor_bruto_rv,
                    rede = 4
                };

                io_arl_resumo_op.Add(resumo_fi);

                io_arl_creditos.Add(creditos);

                //io_hsm_cv.Add(is_ultimo_ro, io_arl_cv);

                var lo_detalhado1 = new ArquivoDetalhado()
                {
                    registro = "Comprovante de Venda",
                    estabelecimento = lo_cv.estabelecimento,
                    transacao = "",
                    RO = lo_cv.num_resumo,
                    produto = "",
                    cartao = lo_cv.cartao,
                    vl_bruto = lo_cv.vl_venda,
                    vl_liquido = "",
                    parcela = lo_cv.parcela,
                    total_parcela = lo_cv.total_parcela,
                    autorizacao = lo_cv.autorizacao,
                    nsu = lo_cv.nsu
                };

                io_detalhado.Add(lo_detalhado1);

                var lo_completo = new ArquivoCompleto()
                {
                    registro = "Comprovante de Venda",
                    estabelecimento = lo_cv.estabelecimento,
                    RO = lo_cv.num_resumo,
                    parcela = lo_cv.parcela,
                    total_parcela = lo_cv.total_parcela,
                    filer = "",
                    plano = "",
                    tipo_transacao = "",
                    apresentacao = "",
                    prev_pagamento = "",
                    envio_banco = "",
                    cartao = lo_cv.cartao,
                    sinal_valor_bruto = "",
                    valor_bruto = lvalor_bruto.ToString(),
                    sinal_comissao = "",
                    comissao = ltaxa.ToString(),
                    sinal_rejeitado = "",
                    valor_rejeitado = "",
                    sinal_liquido = "",
                    valor_liquido = lvalor_liquido.ToString(),
                    valor_total_venda = lvalor_bruto.ToString(),
                    valor_prox_parcela = "",
                    taxas = "",
                    autorizacao = lo_cv.autorizacao,
                    nsu_doc = lo_cv.nsu,
                    banco = "",
                    agencia = "",
                    conta_corrente = "",
                    status_pagamento = "",
                    motivo_rejeicao = "",
                    cvs_aceitos = "",
                    produto = "",
                    cvs_rejeitados = "",
                    data_venda = lo_cv.dt_venda,
                    data_captura = "",
                    origem_ajuste = "",
                    valor_complementar = "",
                    produto_financeiro = "",
                    valor_antecipado = "",
                    bandeira = "",
                    registro_unico_RO = lo_cv.num_resumo, // is_linha_atual.Substring(11, 7),
                    taxas_comissao = "",
                    tarifa = "",
                    meio_captura = "",
                    terminal = ""
                };
                io_completo.Add(lo_completo);

                var lo_rede = new TransacaoRede
                {
                    ds_rede = "Bin",
                    dt_transacao = Convert.ToDateTime(lo_completo.data_venda),
                    nr_logico = lo_completo.terminal,
                    nsu_rede = lo_completo.nsu_doc,
                    nsu_tef = lo_completo.nsu_doc,

                    vl_bruto = Convert.ToDecimal(lo_completo.valor_bruto),
                    nm_estabelecimento = lo_completo.estabelecimento
                };
                io_arl_rede.Add(lo_rede);
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        /**
        @return
            O header desmontado.
        */
        public ConciliacaoHeaderStruct HeaderGet()
        {
            return (io_header);
        }

        /**
        @return
            Toda a estrutura de RO.
        */
        public
        List<ArquivoResumo> ROGet()
        {
            return (io_arl_ro);
        }

        public
        List<TransacaoRede> RedeGet()
        {
            return (io_arl_rede);
        }

        /**
        @return
            Toda a estrutura de RO.
        */
        public
        IDictionary
                    CVGet()
        {
            return (io_hsm_cv);
        }

        /**
        @return
            Toda a estrutura de RO.
        */
        public
        List<ArquivoTransacao> CVArrayGet()
        {
            return (io_arl_just_cv);
        }

        /**
        @return
            Toda a estrutura de RO.
        */
        public
        List<ArquivoAntecipado> ARGet()
        {
            return (io_antecipado);
        }

        /**
       @return
           Toda a estrutura de TotalizadorProduto.
       */
        public
        List<TotalizadorProduto> TotalizadorProdutoGet()
        {
            return (io_totalizador_produto);
        }

        /**
        @return
           Toda a estrutura de TotalizadorBanco
        */
        public
        List<TotalizadorBanco> TotalizadorBancoGet()
        {
            return (io_totalizador_banco);
        }

        /**
        @return
           Toda a estrutura de TotalizadorEstabelecimento
        */
        public
        List<TotalizadorEstabelecimento> TotalizadorEstabelecimentoGet()
        {
            return (io_totalizador_estabelecimento);
        }

        /**
       @return
           Toda a estrutura de detalhado.
       */

        public
            List<ArquivoDetalhado> DetalhadoGet()
        {
            /*foreach (ArquivoResumo resumao in io_arl_ro)
            {
                var lo_detalhado = new ArquivoDetalhado()
                {
                    registro = "Resumo do Pagamento",
                    estabelecimento = resumao.estabelecimento,
                    transacao = resumao.transacao,
                    RO = resumao.resumo,
                    produto = resumao.produto,
                    cartao = "",
                    vl_bruto = resumao.valor_bruto,
                    vl_liquido = resumao.vl_liquido,
                    parcela = "",
                    total_parcela = "",
                    autorizacao = "",
                    nsu = ""
                };

                io_detalhado.Add(lo_detalhado);

                ArrayList teste = null;
                if (io_hsm_cv.TryGetValue(resumao.chave, out teste))
                {
                    foreach (ArquivoTransacao transacao in teste)
                    {
                        var lo_detalhado1 = new ArquivoDetalhado()
                        {
                            registro = "Comprovante de Venda",
                            estabelecimento = transacao.estabelecimento,
                            transacao = "",
                            RO = transacao.num_resumo,
                            produto = "",
                            cartao = transacao.cartao,
                            vl_bruto = resumao.valor_bruto,
                            vl_liquido = "",
                            parcela = transacao.parcela,
                            total_parcela = transacao.total_parcela,
                            autorizacao = transacao.autorizacao,
                            nsu = transacao.nsu
                        };

                        io_detalhado.Add(lo_detalhado1);
                    }
                }
            }*/

            return (io_detalhado);
        }

        public
           List<ArquivoCompleto> CompletoGet()
        {
            return (io_completo);
        }


        public
        IDictionary
                    TotalizadorGet()
        {
            return (io_hsm_totalizador_bruto);
        }

        public
        IDictionary
                    PrevisaoGet()
        {
            return (io_hsm_previsao_pagamento);
        }

        /*
        Inicializa as tabelas com sues respectivos valores.
        */
        private
        void TabelasIniciar()
        {

            io_hsm_tabela_I.Add("1", "Normal");
            io_hsm_tabela_I.Add("2", "Antecipado");
            io_hsm_tabela_I.Add("3", "Liberado de Suspensão");

            /*
            Tabela II.
            */
            io_hsm_tabela_II.Add("300", "POS ISO");
            io_hsm_tabela_II.Add("301", "POS GPRS Mobile");
            io_hsm_tabela_II.Add("302", "POS Desktop");
            io_hsm_tabela_II.Add("303", "POS Eth +Desktop");
            io_hsm_tabela_II.Add("304", "POS GPRS Desktop");
            io_hsm_tabela_II.Add("305", "POS GPRS Dt + Dial + Eth");
            io_hsm_tabela_II.Add("306", "PINPAD USB");
            io_hsm_tabela_II.Add("307", "TEF Dial(discado)");
            io_hsm_tabela_II.Add("308", "E - Commerce Gateway 1");
            io_hsm_tabela_II.Add("309", "Arquivo de Transações(Lote)");
            io_hsm_tabela_II.Add("330", "TEF dedicada");
            io_hsm_tabela_II.Add("331", "TEF IP");
            io_hsm_tabela_II.Add("333", "E - Comm Gateway 2");
            io_hsm_tabela_II.Add("334", "E - Comm Gateway 3");
            io_hsm_tabela_II.Add("335", "E - Comm Gateway 4");
            io_hsm_tabela_II.Add("336", "E - Comm Gateway 5");
            io_hsm_tabela_II.Add("337", "E - Comm Subadquirente");
            io_hsm_tabela_II.Add("338", "Solução própr ecomm");
            io_hsm_tabela_II.Add("352", "Mobile POS");
            io_hsm_tabela_II.Add("356", "IPG - Ecommerce");

            /*
            Tabela III - Situação do Pagamento.
           */
            io_hsm_tabela_III.Add("0", "OK");
            io_hsm_tabela_III.Add("1", "Retido");
            io_hsm_tabela_III.Add("2", "Suspenso");

            /*
            Tabela VI - Bandeira.
            */
            io_hsm_tabela_VI.Add("001", "Visa");
            io_hsm_tabela_VI.Add("002", "Mastercard");
            io_hsm_tabela_VI.Add("003", "Cabal");
            io_hsm_tabela_VI.Add("004", "Hipercard");
            io_hsm_tabela_VI.Add("005", "Elo");
            io_hsm_tabela_VI.Add("006", "Amex");

        }

        /**
	    Formata o texto em data.
	    @param as_texto
	    @param as_mascara
	    @return
	     * @throws ParseException 
	    */
        private
        string FormatoDataExecutar
                    (
                        string as_texto,
                        string as_mascara,
                        string as_mascara_nova

                    )
        {
            try
            {
                if (Convert.ToDouble(as_texto) == 0)
                {
                    return "";
                }
                DateTime data_arquivo = DateTime.ParseExact(as_texto, as_mascara, CultureInfo.InvariantCulture);
                return (data_arquivo.ToString(as_mascara_nova));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /**
	    Formato o texto em valor.
	    @param as_texto
	    @return
	    */
        private
        String FormatoValorExecutar
                    (
                        string as_texto

                    )
        {
            try
            {
                as_texto = as_texto.Replace(" ", string.Empty);
                String MilharCentena = as_texto.Substring(0, as_texto.Length - 2);
                String Centavos = as_texto.Substring(MilharCentena.Length, 2);
                String aux = MilharCentena + "," + Centavos;
                double Valor = Convert.ToDouble(aux);
                return Valor == 0 ? "" : Valor.ToString("#,##0.00");
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /**
	    Formato o texto em valor.
	    @param as_texto
	    @return
	    */
        private
        Decimal FormatoValorExecutarDouble
                    (
                        string as_texto

                    )
        {
            as_texto = as_texto.Replace(",", "").Replace(".", "");
            String MilharCentena = as_texto.Substring(0, as_texto.Length - 2);
            String Centavos = as_texto.Substring(MilharCentena.Length, 2);
            String aux = MilharCentena + "," + Centavos;
            Decimal Valor = Convert.ToDecimal(aux);
            return Valor;
        }

    }
}
