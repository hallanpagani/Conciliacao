using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Antlr.Runtime.Tree;
using Conciliacao.Models.conciliacao;
using ConciliacaoModelo.model.conciliador;
using OFXSharp;
using ConciliacaoModelo.model.conciliador.UseRede;

namespace Conciliacao.Models
{
    public class ConciliacaoBaneseDesmontar
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
        private List<ConciliacaoUseRedeEEVCResumoOperacaoStruct> io_arl_ro_credito_banese = new List<ConciliacaoUseRedeEEVCResumoOperacaoStruct>();
        private List<ConciliacaoUseRedeEEVDResumoOperacaoStruct> io_arl_ro_debito_banese = new List<ConciliacaoUseRedeEEVDResumoOperacaoStruct>();
        private List<ConciliacaoUseRedeEEVCComprovanteVendaStruct> io_arl_cv_credito_banese = new List<ConciliacaoUseRedeEEVCComprovanteVendaStruct>();
        private List<ConciliacaoUseRedeEEVDComprovanteVendaStruct> io_arl_cv_debito_banese = new List<ConciliacaoUseRedeEEVDComprovanteVendaStruct>();
        private List<ConciliacaoUseRedeParcelasStruct> io_arl_parcelas = new List<ConciliacaoUseRedeParcelasStruct>();
        private List<ConciliacaoUseRedeEEFIResumoOperacaoStruct> io_arl_resumo_op = new List<ConciliacaoUseRedeEEFIResumoOperacaoStruct>();
        private List<ConciliacaoUseRedeEEFICreditosStruct> io_arl_creditos = new List<ConciliacaoUseRedeEEFICreditosStruct>();

        public List<ConciliacaoUseRedeEEVDComprovanteVendaStruct> GetComprovanteEEVDVenda()
        {
            return io_arl_cv_debito_banese;
        }

        public List<ConciliacaoUseRedeEEVDResumoOperacaoStruct> GetResumoEEVDOperacao()
        {
            return io_arl_ro_debito_banese;
        }

        public List<ConciliacaoUseRedeEEVCResumoOperacaoStruct> GetResumoEEVCArquivo()
        {
            return io_arl_ro_credito_banese;
        }

        public List<ConciliacaoUseRedeEEVCComprovanteVendaStruct> GetComprovanteEEVCCVenda()
        {
            return io_arl_cv_credito_banese;
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
            io_hsm_tabela_IV = new Hashtable(),
            io_hsm_tabela_V = new Hashtable(),
            io_hsm_tabela_VI = new Hashtable(),
            io_hsm_tabela_VII = new Hashtable(),
            io_hsm_tabela_VIII = new Hashtable(),
            io_hsm_totalizador_bruto = new Hashtable(),
            io_hsm_previsao_pagamento = new Hashtable(),
            io_hsm_totalizador_produto = new Hashtable(),
            io_hsm_totalizador_banco = new Hashtable(),
            io_hsm_totalizador_estabelecimento = new Hashtable();

        /*
        Construtor da classe.
        */
        public ConciliacaoBaneseDesmontar
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
                        /*
                        Identifica qual o tipo de registro.
                        */
                        switch (is_linha_atual.Substring(0, 1))
                        {
                            /*
                            Se for o RO.
                            */
                            case "1":
                                {
                                    /*
                                    Desmonta o RO.
                                    */
                                    RODesmontar();

                                    break;
                                }

                            /*
                            Se for o CV.
                            */
                            case "2":
                                {
                                    /*
                                    Desmonta o RO.
                                    */
                                    CVDesmontar();

                                    break;
                                }

                            case "3":
                                {
                                    ParcelasDesmontar(is_linha_atual);

                                    break;
                                }

                            case "4":
                                {
                                    /*
                                    Desmonta o AR - Antecipação de recebiveis.
                                    */
                                    ARRODesmontar();

                                    break;
                                }
                            /*
                            Se for o Antecipacao.
                            */
                            case "5":
                                {
                                    /*
                                    Desmonta o AR - Antecipação de recebiveis.
                                    */
                                    ARDesmontar();

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

        /*
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
        private string status_pagamento;
        private string meio_captura;
        private string produto_ro;
        private string banco_ro;
        private string agencia;
        private string conta;
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
	    Desmonta o resumo de vendas.
	    */
        private
        void RODesmontar()
        {
            /*
            Cria a estrutura de registro.
            */
            var resumo = new ArquivoResumo();

            try
            {
                DateTime data_rv = Convert.ToDateTime(FormatoDataExecutar(is_linha_atual.Substring(27, 8), "ddMMyyyy", "dd/MM/yyyy")); 
                DateTime dataprevista = Convert.ToDateTime(FormatoDataExecutar(is_linha_atual.Substring(123, 8), "ddMMyyyy", "dd/MM/yyyy"));
                lvalor_bruto = FormatoValorExecutarDouble(is_linha_atual.Substring(37, 12));
                lvalor_liquido = FormatoValorExecutarDouble(is_linha_atual.Substring(49, 12));
                ltaxa = Math.Abs(((lvalor_liquido*100/lvalor_bruto)-100));

                resumo = new ArquivoResumo()
                {
                    resumo = is_linha_atual.Substring(18, 9), //
                    estabelecimento = is_linha_atual.Substring(1, 15), // 
                    parcela = is_linha_atual.Substring(35, 2).Equals("") ? "1" : is_linha_atual.Substring(35, 2).Trim(), //
                    transacao = (string)io_hsm_tabela_III[is_linha_atual.Substring(23, 2)], //
                    prev_pagamento = FormatoDataExecutar(is_linha_atual.Substring(123, 8), "ddMMyyyy", "dd/MM/yyyy"), //
                    valor_bruto = lvalor_bruto, //
                    taxa_comissao = ltaxa.ToString("#,##0.00"), // is_linha_atual.Substring(57, 1),
                    vl_comissao = FormatoValorExecutarDouble(is_linha_atual.Substring(61, 12)), //
                    vl_rejeitado = FormatoValorExecutar(is_linha_atual.Substring(111, 12)), //
                    vl_liquido = lvalor_liquido, //
                    situacao = "", //
                    produto = (string)io_hsm_tabela_I[is_linha_atual.Substring(16, 2)], //
                    terminal = "",
                    chave = ""
                    
                };

                taxa = ltaxa;
                bandeira = "Banese"; // (string) io_hsm_tabela_VI[is_linha_atual.Substring(184, 3)];
                data_pagamento = FormatoDataExecutar(is_linha_atual.Substring(123, 8), "ddMMyyyy", "dd/MM/yyyy"); //
                data_movimento = FormatoDataExecutar(is_linha_atual.Substring(27, 8), "ddMMyyyy", "dd/MM/yyyy"); //
                tipo_transacao = (string)io_hsm_tabela_IV[is_linha_atual.Substring(143, 2)];
                status_pagamento = "";
                meio_captura = "";
                banco_ro = is_linha_atual.Substring(73, 3); //
                agencia = is_linha_atual.Substring(76, 6); //
                conta = is_linha_atual.Substring(82, 11); // 
                parcela = is_linha_atual.Substring(35, 2).Equals("") ? "1" : is_linha_atual.Substring(35, 2).Trim(); //
                bruto_rv = lvalor_bruto;
                produto_ro = resumo.produto;

                var resumo_banese = new ConciliacaoUseRedeEEVCResumoOperacaoStruct()
                {
                    is_tipo_registro = 1,
                    nm_tipo_registro = tipo_transacao,
                    is_numero_filiacao_pv = Convert.ToDecimal(is_linha_atual.Substring(1, 15)),
                    is_numero_resumo_venda = is_linha_atual.Substring(18, 9),
                    is_banco = banco_ro,
                    is_agencia = agencia,
                    is_conta_corrente = conta,
                    is_data_resumo_venda = Convert.ToDateTime(data_movimento),

                    is_quantidade_resumo_vendas = "0",
                    is_valor_bruto = bruto_rv,

                    is_valor_gorjeta = 0,

                    is_valor_rejeitado = FormatoValorExecutarDouble(is_linha_atual.Substring(111, 12)),
                    is_valor_desconto = bruto_rv - lvalor_liquido,
                    is_valor_liquido = lvalor_liquido,

                    is_data_credito = dataprevista,
                    is_bandeira = "Banese",
                    rede = 3
                };

                var resumo_debito_banese = new ConciliacaoUseRedeEEVDResumoOperacaoStruct()
                {
                    is_tipo_registro = 1,
                    nm_tipo_registro = tipo_transacao,
                    is_numero_filiacao_pv = Convert.ToDecimal(is_linha_atual.Substring(1, 15)),
                    is_numero_resumo_venda = is_linha_atual.Substring(18, 9),
                    is_banco = banco_ro,
                    is_agencia = agencia,
                    is_conta_corrente = conta,
                    is_data_resumo_venda = Convert.ToDateTime(data_movimento),

                    is_quantidade_resumo_vendas = "0",
                    is_valor_bruto = bruto_rv,
                    is_valor_desconto = bruto_rv - lvalor_liquido,
                    is_valor_liquido = lvalor_liquido,

                    is_data_credito = dataprevista,
                    is_bandeira = "Banese",
                    rede = 3
                };

                TimeSpan date = dataprevista - data_rv;

                if ( (date.Days) > 10)
                {
                    io_arl_ro_credito_banese.Add(resumo_banese);
                }
                else
                {
                    io_arl_ro_debito_banese.Add(resumo_debito_banese);
                }


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
                        rede = "Banese"
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
                        rede = "Banese"
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

                string estabelecimento = is_linha_atual.Substring(1, 10);

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
                    envio_banco = is_linha_atual.Substring(123, 8).Equals("00000000") ? "00/00/00000" : FormatoDataExecutar(is_linha_atual.Substring(123, 8), "ddMMyyyy", "dd/MM/yyyy"),
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
                    banco = is_linha_atual.Substring(73, 3),
                    agencia = is_linha_atual.Substring(76, 6),
                    conta_corrente = is_linha_atual.Substring(82, 11),
                    status_pagamento = (string)io_hsm_tabela_IV[is_linha_atual.Substring(143, 2)],
                    motivo_rejeicao = "",
                    cvs_aceitos = is_linha_atual.Substring(93, 9),
                    produto = (string)io_hsm_tabela_I[is_linha_atual.Substring(16, 2)],
                    cvs_rejeitados = is_linha_atual.Substring(102, 9),
                    data_venda = is_linha_atual.Substring(27, 8).Equals("00000000") ? "00/00/00000" : FormatoDataExecutar(is_linha_atual.Substring(27, 8), "ddMMyyyy", "dd/MM/yyyy"),
                    data_captura = "",
                    origem_ajuste = "",
                    valor_complementar = "",
                    produto_financeiro = "",
                    valor_antecipado = "",
                    bandeira = "",
                    registro_unico_RO = resumo.resumo,
                    taxas_comissao = FormatoValorExecutar(is_linha_atual.Substring(61, 12)),
                    tarifa = Math.Abs(((resumo.vl_liquido * 100) / (resumo.valor_bruto) -100)).ToString()  ,
                    meio_captura = "",
                    terminal = ""
                };

                var lo_rede = new TransacaoRede
                {
                    ds_rede = "Banese",
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
        Comprovante de venda.
        */
        private
        void CVDesmontar()
        {
            try
            {
                var lo_cv = new ArquivoTransacao()
                {
                    num_resumo = is_linha_atual.Substring(16, 9),
                    registro = "Comprovante de Venda",
                    estabelecimento = is_linha_atual.Substring(1, 15),
                    num_logico = "",
                    dt_venda = FormatoDataExecutar(is_linha_atual.Substring(37, 8), "ddMMyyyy", "dd/MM/yyyy"),
                    cartao = is_linha_atual.Substring(51, 19),
                    vl_venda = FormatoValorExecutar(is_linha_atual.Substring(70, 12)),
                    parcela = is_linha_atual.Substring(108, 2),
                    total_parcela = is_linha_atual.Substring(106, 2),
                    autorizacao = is_linha_atual.Substring(130, 10),
                    nsu = is_linha_atual.Substring(25, 12)
                };

                io_arl_cv.Add(lo_cv);
                io_arl_just_cv.Add(lo_cv);


                if (Convert.ToInt32(is_linha_atual.Substring(106, 2).Trim().Equals("") ? "0" : is_linha_atual.Substring(106, 2).Trim()) > 1)
                {
                    var comprovante = new ConciliacaoUseRedeEEVCComprovanteVendaStruct()
                    {
                        is_tipo_registro = 2,
                        nm_tipo_registro = "CV/NSU",
                        is_numero_filiacao_pv = Convert.ToDecimal(is_linha_atual.Substring(1, 15)),
                        is_numero_resumo_vendas =
                            Convert.ToInt64(string.IsNullOrEmpty(is_linha_atual.Substring(16, 9))
                                ? "0"
                                : is_linha_atual.Substring(16, 9)),

                        is_data_cv =
                            Convert.ToDateTime(FormatoDataExecutar(is_linha_atual.Substring(37, 8), "ddMMyyyy", "dd/MM/yyyy")),

                        is_valor_bruto = Convert.ToDecimal(is_linha_atual.Substring(70, 12))/100,
                        is_valor_gorjeta = 0, //Convert.ToDecimal(is_linha_atual.Substring(52, 15)) / 100,
                        is_numero_cartao = is_linha_atual.Substring(51, 19),

                        is_status_cv = "",

                        is_parcela = is_linha_atual.Substring(108, 2),
                        is_numero_parcelas = Convert.ToInt32(is_linha_atual.Substring(106, 2)),
                        is_numero_cv = Convert.ToDecimal(is_linha_atual.Substring(25, 12)),

                        is_numero_referencia = "0", //a.Substring(100, 13),

                        
                        is_valor_desconto = (Convert.ToDecimal(is_linha_atual.Substring(94, 12)) / 100),

                        is_numero_autorizacao = is_linha_atual.Substring(130, 10),

                        is_hora_transcao =
                            is_linha_atual.Substring(45, 2) + ":" + is_linha_atual.Substring(47, 2) + ":" +
                            is_linha_atual.Substring(49, 2),

                        is_tipo_captura = (string)io_hsm_tabela_VI[is_linha_atual.Substring(140, 3)],

                        is_valor_liquido =
                            (Convert.ToDecimal(is_linha_atual.Substring(82, 12))/100),
                        // Convert.ToDecimal(a.Substring(205, 15)) / 100,#VER

                     //   is_valor_liquido_primeira_parc =  Convert.ToDecimal(is_linha_atual.Substring(113, 13)) / 100,
                     //   is_valor_liquido_demais_parc = Convert.ToDecimal(is_linha_atual.Substring(126, 13)) / 100,
                     //   numero_nota_fiscal = is_linha_atual.Substring(139, 9),
                        numero_unico_transacao = is_linha_atual.Substring(16, 9),
                      //  is_numero_terminal = is_linha_atual.Substring(52, 8),

                        is_bandeira = bandeira, //TabelaI(a.Substring(261, 1)), #VER

                        taxa_cobrada = (Convert.ToDecimal(is_linha_atual.Substring(94, 12)) / 100),
                        is_data_credito = Convert.ToDateTime(FormatoDataExecutar(is_linha_atual.Substring(122, 8), "ddMMyyyy", "dd/MM/yyyy")),
                        rede = 3

                    };

                    io_arl_cv_credito_banese.Add(comprovante);

                    var creditos = new ConciliacaoUseRedeEEFICreditosStruct()
                    {
                        numero_pv_centralizador = comprovante.is_numero_filiacao_pv,
                        numero_documento = comprovante.is_numero_cv,
                        data_lancamento = comprovante.is_data_credito,
                        valor_lancamento = comprovante.is_valor_liquido,
                        banco = Convert.ToInt32(banco_ro),
                        agencia = agencia,
                        conta_corrente = Convert.ToInt64(conta),
                        data_movimento = Convert.ToDateTime(data_movimento),
                        numero_rv = Convert.ToInt32(comprovante.is_numero_resumo_vendas.ToString()),
                        data_rv = comprovante.is_data_cv,
                        bandeira = bandeira,
                        tipo_transacao = tipo_transacao,
                        valor_bruto_rv = bruto_rv,
                        valor_taxa_desconto = taxa,
                        numero_parcela = parcela,
                        situacao = "",
                        numero_pv_original = Convert.ToInt64(comprovante.is_numero_filiacao_pv),
                        rede = 3
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
                        rede = 3
                    };

                    io_arl_resumo_op.Add(resumo_fi);

                    io_arl_creditos.Add(creditos);
                }
                else
                {
                    var resumo = new ConciliacaoUseRedeEEVDComprovanteVendaStruct()
                    {
                        is_tipo_registro = 2,
                        nm_tipo_registro = "CV - Detalhamento",
                        is_numero_filiacao_pv = Convert.ToDecimal(is_linha_atual.Substring(1, 15)),
                        is_numero_resumo_vendas = Convert.ToInt64(is_linha_atual.Substring(16, 9)),
                        is_data_cv = Convert.ToDateTime(FormatoDataExecutar(is_linha_atual.Substring(37, 8), "ddMMyyyy", "dd/MM/yyyy")),
                        is_valor_bruto = Convert.ToDecimal(is_linha_atual.Substring(70, 12)) / 100,
                        is_valor_liquido = (Convert.ToDecimal(is_linha_atual.Substring(82, 12)) / 100),
                        is_valor_desconto = (Convert.ToDecimal(is_linha_atual.Substring(94, 12)) / 100) ,

                        is_parcela = is_linha_atual.Substring(108, 2),
                        is_numero_parcelas = Convert.ToInt32(is_linha_atual.Substring(106, 2).Trim().Equals("") ? "0" : is_linha_atual.Substring(106, 2).Trim()),
                        is_numero_cartao = is_linha_atual.Substring(51, 19),
                        is_tipo_transacao = tipo_transacao,
                        is_numero_cv = Convert.ToDecimal(is_linha_atual.Substring(25, 12)),
                        is_data_credito = Convert.ToDateTime(data_pagamento),
                        is_status_transacao = status_pagamento,
                        is_hora_transacao = is_linha_atual.Substring(45, 2) + ":" + is_linha_atual.Substring(47, 2) + ":" +
                            is_linha_atual.Substring(49, 2),
                        is_numero_terminal = "",
                        is_numero_autorizacao = is_linha_atual.Substring(130, 10),
                        is_tipo_captura = meio_captura,
                        is_reservado ="",
                        is_valor_compra = Convert.ToDecimal(is_linha_atual.Substring(70, 12)) / 100,
                        is_valor_liquido_primeira_parc = Convert.ToDecimal(is_linha_atual.Substring(82, 12)) / 100,
                        is_valor_liquido_demais_parc = Convert.ToDecimal(is_linha_atual.Substring(94, 12)) / 100,
                        numero_nota_fiscal = "",
                        numero_unico_transacao = is_linha_atual.Substring(16, 9),
                        is_valor_saque = 0,
                        is_bandeira = bandeira,
                        taxa_cobrada = taxa,
                        
                        rede = 3

                        //   is_codigo_tef = achou.Codigt
                    };

                    io_arl_cv_debito_banese.Add(resumo);
                }
                
                
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
                    valor_bruto = (Convert.ToDecimal(is_linha_atual.Substring(70, 12)) / 100).ToString(),
                    sinal_comissao = "",
                    comissao = ltaxa.ToString(),
                    sinal_rejeitado = "",
                    valor_rejeitado = "",
                    sinal_liquido = "",
                    valor_liquido = (Convert.ToDecimal(is_linha_atual.Substring(82, 12)) / 100).ToString(),
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
                    ds_rede = "Banese",
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

        private void ParcelasDesmontar(string a)
        {


            var parcelas = new ConciliacaoUseRedeParcelasStruct()
            {
                is_tipo_registro = 3,
                is_numero_filiacao_pv = Convert.ToDecimal(a.Substring(1, 15)),
                is_numero_resumo_vendas = a.Substring(16, 9),
                is_data_rv = Convert.ToDateTime(a.Substring(37, 2) + "/" + a.Substring(39, 2) + "/" + a.Substring(41, 4)),
                numero_parcela = Convert.ToInt32(a.Substring(108, 2)),
                valor_parcela_bruto = (Convert.ToDecimal(a.Substring(70, 12)) / 100),
                valor_desconto_parcela = (Convert.ToDecimal(a.Substring(94, 12)) / 100),
                valor_parcela_liquida = (Convert.ToDecimal(a.Substring(82, 12)) / 100),
                data_credito = Convert.ToDateTime(a.Substring(110, 2) + "/" + a.Substring(112, 2) + "/" + a.Substring(114, 4)),
                rede = 3
            };

            io_arl_parcelas.Add(parcelas);

            /* 14  */
          /*  try
            {
                var data_pagamento = Convert.ToDateTime(a.Substring(110, 2) + "/" + a.Substring(112, 2) + "/" + a.Substring(114, 4));
                var estabelecimento = a.Substring(1, 15);
                if (io_hsm_totalizador_estabelecimento.Contains(estabelecimento + data_pagamento))
                {
                    TotalizadorEstabelecimento totalizador_estabelecimento = (TotalizadorEstabelecimento)io_hsm_totalizador_estabelecimento[estabelecimento + data_pagamento];
                    io_hsm_totalizador_estabelecimento.Remove(estabelecimento + data_pagamento);
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + data_pagamento, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = Convert.ToDateTime(data_pagamento),
                        total_realizado = totalizador_estabelecimento.total_realizado + (Convert.ToDecimal(a.Substring(82, 12)) / 100),
                        estabelecimento = Convert.ToInt32(estabelecimento)
                    });
                }
                else
                {
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + data_pagamento, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = Convert.ToDateTime(data_pagamento),
                        total_realizado = (Convert.ToDecimal(a.Substring(82, 12)) / 100),
                        estabelecimento = Convert.ToInt32(estabelecimento)
                    });
                }

                if (io_hsm_totalizador_banco.Contains(banco_ro + data_pagamento))
                {
                    TotalizadorBanco totalizador_banco = (TotalizadorBanco)io_hsm_totalizador_banco[banco_ro + data_pagamento];
                    io_hsm_totalizador_banco.Remove(banco_ro + data_pagamento);
                    io_hsm_totalizador_banco.Add(banco_ro + data_pagamento, new TotalizadorBanco
                    {
                        data_prevista = Convert.ToDateTime(data_pagamento),
                        total_realizado = totalizador_banco.total_realizado + (Convert.ToDecimal(a.Substring(82, 12)) / 100),
                        id_banco = Convert.ToInt32(banco_ro)
                    });
                }
                else
                {
                    io_hsm_totalizador_banco.Add(banco_ro + data_pagamento, new TotalizadorBanco
                    {
                        data_prevista = Convert.ToDateTime(data_pagamento),
                        total_realizado = (Convert.ToDecimal(a.Substring(82, 12)) / 100),
                        id_banco = Convert.ToInt32(banco_ro)
                    });
                }

                Decimal valor_total = 0;
                Decimal valor_total_liquido = 0;

                if (io_hsm_totalizador_produto.Contains(produto_ro + data_pagamento))
                {
                    TotalizadorProduto totalizador_produto = (TotalizadorProduto)io_hsm_totalizador_produto[produto_ro + data_pagamento];
                    valor_total = totalizador_produto.valor_bruto + (Convert.ToDecimal(a.Substring(70, 12)) / 100);
                    valor_total_liquido = totalizador_produto.valor_liquido + (Convert.ToDecimal(a.Substring(82, 12)) / 100);
                    io_hsm_totalizador_produto.Remove(produto_ro + data_pagamento);
                    io_hsm_totalizador_produto.Add(produto_ro + data_pagamento, new TotalizadorProduto
                    {
                        data_prevista = Convert.ToDateTime(data_pagamento),
                        valor_bruto = valor_total,
                        valor_liquido = valor_total_liquido,
                        ds_produto = produto_ro,
                        rede = "Banese"
                    });
                }
                else
                {
                    io_hsm_totalizador_produto.Add(produto_ro + data_pagamento, new TotalizadorProduto
                    {
                        data_prevista = Convert.ToDateTime(data_pagamento),
                        valor_bruto = (Convert.ToDecimal(a.Substring(70, 12)) / 100),
                        valor_liquido = (Convert.ToDecimal(a.Substring(82, 12)) / 100),
                        ds_produto = produto_ro,
                        rede = "Banese"
                    });
                } 

            }
            catch (Exception e)
            {
                throw e;
            }*/

        }

        private void ARRODesmontar()
        {
            try
            {
                rv_ajuste = is_linha_atual.Substring(16, 9);
                rv_dt_ajuste = FormatoDataExecutar(is_linha_atual.Substring(25, 8), "ddMMyyyy", "dd/MM/yyyy");
                rv_dt_ajuste_pagamento = FormatoDataExecutar(is_linha_atual.Substring(33, 8), "ddMMyyyy", "dd/MM/yyyy");

                rv_ajuste_sinal_bruto = is_linha_atual.Substring(61, 1);
                rv_ajuste_banco = is_linha_atual.Substring(150, 3);
                rv_ajuste_agencia = is_linha_atual.Substring(153, 6);
                rv_ajuste_conta = is_linha_atual.Substring(159, 11);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

            private void ARDesmontar()
        {
            try
            {
                var lo_ar = new ArquivoAntecipado()
                {

                    registro = "Antecipação de Recebíveis",
                    estabelecimento = is_linha_atual.Substring(1, 15),
                    num_resumo = rv_ajuste,
                    dt_credito = rv_dt_ajuste_pagamento,
                    sinal_valor_bruto = rv_ajuste_sinal_bruto,
                    valor_bruto = FormatoValorExecutar(is_linha_atual.Substring(36, 12)),
                    sinal_valor_bruto_parcelado = rv_ajuste_sinal_bruto,
                    valor_bruto_parcelado = FormatoValorExecutar(is_linha_atual.Substring(43, 13)),
                    sinal_valor_bruto_predatado = is_linha_atual.Substring(56, 1),
                    valor_bruto_predatado = FormatoValorExecutar(is_linha_atual.Substring(57, 13)),
                    sinal_valor_bruto_total = is_linha_atual.Substring(70, 1),
                    valor_bruto_total = FormatoValorExecutar(is_linha_atual.Substring(71, 13)),

                    sinal_valor_liquido = rv_ajuste_sinal_bruto,
                    valor_liquido = FormatoValorExecutar(is_linha_atual.Substring(48, 12)),

                    taxa_desconto = FormatoValorExecutar(is_linha_atual.Substring(60, 12)),

                    banco = rv_ajuste_banco,
                    agencia = rv_ajuste_agencia,
                    conta_corrente = rv_ajuste_conta,

                    sinal_valor_liquido_antecipacao = rv_ajuste_sinal_bruto,
                    valor_liquido_antecipacao = FormatoValorExecutar(is_linha_atual.Substring(154, 13))

                };

                io_antecipado.Add(lo_ar);
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

            io_hsm_tabela_I.Add("01", "Título");
            io_hsm_tabela_I.Add("02", "Convênio");
            io_hsm_tabela_I.Add("CC", "Cartão de Crédito");

            /*
            Tabela II.
            */
            io_hsm_tabela_II.Add("01", "Venda");
            io_hsm_tabela_II.Add("02", "Ajuste a Crédito");
            io_hsm_tabela_II.Add("03", "Ajuste a Débito");
            io_hsm_tabela_II.Add("04", "Pacote banese");
            io_hsm_tabela_II.Add("05", "Reagendamento");

            /*
            Tabela III.
            */
            io_hsm_tabela_III.Add("C", "Confirmada");
            io_hsm_tabela_III.Add("M", "Confirmada (Manual)");
            io_hsm_tabela_III.Add("P", "Pendente");
            io_hsm_tabela_III.Add("X", "Cancelada");
            io_hsm_tabela_III.Add("D", "Desfeita");
            io_hsm_tabela_III.Add("E", "Estorno");

            /*
            Tabela IV - Forma de pagamento.
            */
            io_hsm_tabela_IV.Add("PF", "Pagamento Futuro");
            io_hsm_tabela_IV.Add("PG", "Pagamento Normal");
            io_hsm_tabela_IV.Add("ES", "Estorno de Venda");
            io_hsm_tabela_IV.Add("AL", "Aluguel de POS");
            io_hsm_tabela_IV.Add("AG", "Agen.Crédito/Débito");
            io_hsm_tabela_IV.Add("AC", "Antec.deCrédito");
            /*
            Tabela V.
            */
            io_hsm_tabela_V.Add("01", "Acerto de Correção Monetária");
            io_hsm_tabela_V.Add("02", "Acerto de data de pagamento");
            io_hsm_tabela_V.Add("03", "Acerto de taxa de comissão");
            io_hsm_tabela_V.Add("04", "Acerto de valores não processados");
            io_hsm_tabela_V.Add("05", "Acerto de valores não recebidos");
            io_hsm_tabela_V.Add("06", "Acerto de valores não reconhecidos");
            io_hsm_tabela_V.Add("07", "Acerto de valores negociados");
            io_hsm_tabela_V.Add("08", "Acerto de valores processados indevidamente");
            io_hsm_tabela_V.Add("09", "Acerto lancamento nao compensado em conta corrente");
            io_hsm_tabela_V.Add("10", "Acerto referente a valores contestados");
            io_hsm_tabela_V.Add("11", "Acerto temporário de valores contestados");
            io_hsm_tabela_V.Add("12", "Acertos diversos");
            io_hsm_tabela_V.Add("13", "Acordo de cobrança");
            io_hsm_tabela_V.Add("14", "Acordo juridico");
            io_hsm_tabela_V.Add("15", "Aplicação de multa programa monitória chargeback");
            io_hsm_tabela_V.Add("16", "Bloqueio de valor por ordem judicial");
            io_hsm_tabela_V.Add("17", "Cancelamento de venda");
            io_hsm_tabela_V.Add("18", "Cobrança de tarifa operacional");
            io_hsm_tabela_V.Add("19", "Cobrança mensal linx comercio");
            io_hsm_tabela_V.Add("20", "Cobrança plano banese");
            io_hsm_tabela_V.Add("21", "Contrato de calcão");
            io_hsm_tabela_V.Add("22", "Crédito de devolução cancelamento - Banco Emissor");
            io_hsm_tabela_V.Add("23", "Crédito EC - Referente contestacao portador");
            io_hsm_tabela_V.Add("24", "Crédito por cancelamento rejeitado - banese");
            io_hsm_tabela_V.Add("25", "Débito processamento dupicado - Visa Pedagio");
            io_hsm_tabela_V.Add("26", "Débito por venda realizada sem a leitura do chip");
            io_hsm_tabela_V.Add("27", "Débito por venda rejeitada no sistema");
            io_hsm_tabela_V.Add("28", "Débito referente a contestação do portador");
            io_hsm_tabela_V.Add("29", "Estorno acordo juridico");
            io_hsm_tabela_V.Add("30", "Estorno contrato de calcao");
            io_hsm_tabela_V.Add("31", "Estorno de acordo de cobranca");
            io_hsm_tabela_V.Add("32", "Estorno de bloqueio de valor por ordem judicial");
            io_hsm_tabela_V.Add("33", "Estorno de cancelamento de venda");
            io_hsm_tabela_V.Add("34", "Estorno de cobranca de tarifa operacional");
            io_hsm_tabela_V.Add("35", "Estorno de cobranca mensal linx comercio");
            io_hsm_tabela_V.Add("36", "Estorno de cobranca plano banese");
            io_hsm_tabela_V.Add("37", "Estorno de débito venda sem a leitura do chip");
            io_hsm_tabela_V.Add("38", "Estorno de insentivo comercial");
            io_hsm_tabela_V.Add("39", "Estorno de multa programa monitória chargeback");
            io_hsm_tabela_V.Add("40", "Estorno de rejeicao ARV");
            io_hsm_tabela_V.Add("41", "Estorno de reversão pagamento duplicidade - ARV");
            io_hsm_tabela_V.Add("42", "Estorno de tarifa de cadastro");
            io_hsm_tabela_V.Add("43", "Estorno de tarifa extrato de papel");
            io_hsm_tabela_V.Add("44", "Estorno deb processamento duplicado - Visa Pedagio");
            io_hsm_tabela_V.Add("45", "Incentivo comercial");
            io_hsm_tabela_V.Add("46", "Incentivo por venda de recarga");
            io_hsm_tabela_V.Add("47", "Regularização de rejeição ARV");
            io_hsm_tabela_V.Add("48", "Reversão pagto duplicidade - ARV");
            io_hsm_tabela_V.Add("49", "Tarifa de cadastro");
            io_hsm_tabela_V.Add("50", "Tarifa Extrato papel");
            io_hsm_tabela_V.Add("51", "Aceleração de Débito de Antecipação");

            /*
            Tabela VI - Bandeira.
            */
            io_hsm_tabela_VI.Add("001", "Visa");
            io_hsm_tabela_VI.Add("002", "Mastercard");
            io_hsm_tabela_VI.Add("006", "SoroCred");
            io_hsm_tabela_VI.Add("007", "Elo");
            io_hsm_tabela_VI.Add("009", "Diners");

            io_hsm_tabela_VI.Add("011", "Agiplan");
            io_hsm_tabela_VI.Add("015", "Banescard");
            io_hsm_tabela_VI.Add("023", "Cabal");
            io_hsm_tabela_VI.Add("029", "CredSystem");
            io_hsm_tabela_VI.Add("035", "Esplanada");
            io_hsm_tabela_VI.Add("064", "CredZ");


            /*
            Tabela VII - Ponto de venda.
            */
            io_hsm_tabela_VII.Add("01", "POS-Point of Sales");
            io_hsm_tabela_VII.Add("02", "PDV-Ponto de Venda ou TEF");
            io_hsm_tabela_VII.Add("03", "E-Comerce");
            io_hsm_tabela_VII.Add("04", "EDI");
            io_hsm_tabela_VII.Add("05", "ADP/BSP-Empresa Capturadora");
            io_hsm_tabela_VII.Add("06", "Manual");
            io_hsm_tabela_VII.Add("07", "URA-CVA");
            io_hsm_tabela_VII.Add("08", "Mobile");
            io_hsm_tabela_VII.Add("09", "Moedeiro eletrônico em rede");

            /*
            Tabela VIII - Erros.
            */
            io_hsm_tabela_VIII.Add("002", "Cartão Inváido");
            io_hsm_tabela_VIII.Add("023", "Outros erros");
            io_hsm_tabela_VIII.Add("031", "Transação de saque com cartão electron valor zerado");
            io_hsm_tabela_VIII.Add("039", "Banco emissor inválido");
            io_hsm_tabela_VIII.Add("044", "Data da transação inválida");
            io_hsm_tabela_VIII.Add("045", "Código de autorização inválido");
            io_hsm_tabela_VIII.Add("055", "Número de Parcelas Inválido");
            io_hsm_tabela_VIII.Add("056", "Transação financiada para EC não autorizado");
            io_hsm_tabela_VIII.Add("057", "Cartão em boletim protetor");
            io_hsm_tabela_VIII.Add("061", "Número de cartão inválido");
            io_hsm_tabela_VIII.Add("066", "Transação não autorizada");
            io_hsm_tabela_VIII.Add("067", "Transação não autorizada");
            io_hsm_tabela_VIII.Add("069", "Transação não autorizada");
            io_hsm_tabela_VIII.Add("070", "Transação não autorizada");
            io_hsm_tabela_VIII.Add("071", "Transação não autorizada");
            io_hsm_tabela_VIII.Add("072", "Transação não autorizada");
            io_hsm_tabela_VIII.Add("073", "Transação inválida");
            io_hsm_tabela_VIII.Add("074", "Valor de Transação inválido");
            io_hsm_tabela_VIII.Add("075", "Número de cartão inválido");
            io_hsm_tabela_VIII.Add("077", "Transação não autorizada");
            io_hsm_tabela_VIII.Add("078", "Transação não autorizada");
            io_hsm_tabela_VIII.Add("079", "Transação não autorizada");
            io_hsm_tabela_VIII.Add("080", "Transação não autorizada");
            io_hsm_tabela_VIII.Add("081", "Cartão Vencido");
            io_hsm_tabela_VIII.Add("082", "Transação Não Autorizada");
            io_hsm_tabela_VIII.Add("083", "Transação não autorizada");
            io_hsm_tabela_VIII.Add("084", "Transação não autorizada");
            io_hsm_tabela_VIII.Add("086", "Transação não autorizada");
            io_hsm_tabela_VIII.Add("092", "Banco Emissor sem Comunicação");
            io_hsm_tabela_VIII.Add("093", "Desbalanceamento no plano parcelado");
            io_hsm_tabela_VIII.Add("094", "Venda parcelada para cartão emitido no exterior");
            io_hsm_tabela_VIII.Add("097", "Valor de parcela menor do que o permitido");
            io_hsm_tabela_VIII.Add("099", "Banco emissor inválido");
            io_hsm_tabela_VIII.Add("100", "Transação não autorizada");
            io_hsm_tabela_VIII.Add("101", "Transação duplicada");
            io_hsm_tabela_VIII.Add("102", "Transação duplicada");
            io_hsm_tabela_VIII.Add("124", "BIN não cadastrado");
            io_hsm_tabela_VIII.Add("126", "Transação de saque com cartão Electron inválido");
            io_hsm_tabela_VIII.Add("128", "Transação de saque com cartão Electron inválido");
            io_hsm_tabela_VIII.Add("129", "Transação de saque com cartão Electron inválido");
            io_hsm_tabela_VIII.Add("130", "Transação de saque com cartão Electron inválido");
            io_hsm_tabela_VIII.Add("133", "Transação de saque com cartão Electron inválido");
            io_hsm_tabela_VIII.Add("134", "Transação de saque com cartão Electron inválido");
            io_hsm_tabela_VIII.Add("145", "Estabelecimento inválido para destribuição");
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
