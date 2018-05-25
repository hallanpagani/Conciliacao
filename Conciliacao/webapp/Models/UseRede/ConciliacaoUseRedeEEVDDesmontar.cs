using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Conciliacao.Models.conciliacao;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoModelo.model.conciliador.UseRede;

namespace Conciliacao.Models.UseRede
{
    public class ConciliacaoUseRedeEEVDDesmontar
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

        /* totalizadores*/
        private List<TotalizadorProduto> io_totalizador_produto = new List<TotalizadorProduto>();
        private List<TotalizadorBanco> io_totalizador_banco = new List<TotalizadorBanco>();
        private List<TotalizadorEstabelecimento> io_totalizador_estabelecimento = new List<TotalizadorEstabelecimento>();

        private IDictionary
            io_hsm_totalizador_produto = new Hashtable(),
            io_hsm_totalizador_banco = new Hashtable(),
            io_hsm_totalizador_estabelecimento = new Hashtable();

        private string data_pagamento, banco;


        /*
        Estruturas para desmontagem.
        */
        private List<ConciliacaoUseRedeEEVDComprovanteVendaStruct> io_arl_cv = new List<ConciliacaoUseRedeEEVDComprovanteVendaStruct>();
        private List<ConciliacaoUseRedeEEVDResumoOperacaoStruct> io_arl_ro = new List<ConciliacaoUseRedeEEVDResumoOperacaoStruct>();
        private List<ConciliacaoUseRedeEEVDTotalArquivoStruct> io_arl_ta = new List<ConciliacaoUseRedeEEVDTotalArquivoStruct>();
        private List<ConciliacaoUseRedeEEVDTotalMatrizStruct> io_arl_tm = new List<ConciliacaoUseRedeEEVDTotalMatrizStruct>();
        private List<ConciliacaoUseRedeEEVDTotalPontoVendaStruct> io_arl_pv = new List<ConciliacaoUseRedeEEVDTotalPontoVendaStruct>();

        public List<ConciliacaoUseRedeEEVDComprovanteVendaStruct> GetComprovanteVenda()
        {
            return io_arl_cv;
        }

        public List<ConciliacaoUseRedeEEVDResumoOperacaoStruct> GetResumoOperacao()
        {
            return io_arl_ro;
        }

        public List<ConciliacaoUseRedeEEVDTotalArquivoStruct> GetTotalArquivo()
        {
            return io_arl_ta;
        }

        public List<ConciliacaoUseRedeEEVDTotalMatrizStruct> GetTotalMatriz()
        {
            return io_arl_tm;
        }

        public List<ConciliacaoUseRedeEEVDTotalPontoVendaStruct> GetPontoVenda()
        {
            return io_arl_pv;
        }

        /*
        Construtor da classe.
        */
        public ConciliacaoUseRedeEEVDDesmontar
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
                        switch (arquivo[0])
                        {
                            case "01":
                                {
                                    RODesmontar(arquivo);

                                    break;
                                }

                            case "02":
                                {
                                    TotalPVDesmontar(arquivo);

                                    break;
                                }


                            case "03":
                                {
                                    TotalMatrizDesmontar(arquivo);

                                    break;
                                }

                            case "04":
                                {
                                    TotalArquivoDesmontar(arquivo);

                                    break;
                                }

                            case "05":
                                {
                                    CVDesmontar(arquivo);

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
            string[] arquivo = _first.Split(',');

            /*
           Cria a estrutura.
           */
            io_header = new ConciliacaoHeaderStruct
            {
                /* Tipo de registro. */
                is_tipo_registro = arquivo[0],
                /* Estabelecimento Matriz. */
                is_estabelecimento_matriz = arquivo[1],
                /* Data de processamento.  */
                is_data_processamento = arquivo[2],
                /* Periodo inicial. */
                is_periodo_inicial = arquivo[3],
                /* Periodo final. */
                is_periodo_final = arquivo[3],
                /* Sequencial. */
                is_sequencia = "",
                /* Empresa adquirente.  */
                is_empresa_adquirente = "",
                /* Opção extrato. */
                is_opcao_extrato = arquivo[4],
                /* Van */
                is_van = "",
                /* Caixa postal */
                is_caixa_postal = arquivo[5],
                /* Versão layout. */
                is_versao_layout = arquivo[9]
            };
        }

        private void RODesmontar(string[] a)
        {
            var resumo = new ConciliacaoUseRedeEEVDResumoOperacaoStruct()
            {
                is_tipo_registro = Convert.ToInt32(a[0]),
                nm_tipo_registro = "Resumo de vendas",
                is_numero_filiacao_pv = Convert.ToDecimal(a[1]),
                is_data_credito = Convert.ToDateTime(a[2].Substring(0, 2) + "/" + a[2].Substring(2, 2) + "/" + a[2].Substring(4, 4)),
                is_data_resumo_venda = Convert.ToDateTime(a[3].Substring(0, 2) + "/" + a[3].Substring(2, 2) + "/" + a[3].Substring(4, 4)),
                is_numero_resumo_venda = Convert.ToDecimal(a[4]).ToString(),
                is_quantidade_resumo_vendas = a[5],
                is_valor_bruto = Convert.ToDecimal(a[6])/100,
                is_valor_desconto = Convert.ToDecimal(a[7]) / 100,
                is_valor_liquido = Convert.ToDecimal(a[8]) / 100,
                is_tipo_resumo = a[9].Equals("D") ? "Débito A vista" : "Pré-Datado",
                is_banco = a[10],
                is_agencia = a[11],
                is_conta_corrente = a[12],
                is_bandeira = Bandeira(a[13]),
                rede = 1
            };

            data_pagamento = a[2].Substring(0, 2) + "/" + a[2].Substring(2, 2) + "/" + a[2].Substring(4, 4);
            banco = resumo.is_banco;

            io_arl_ro.Add(resumo);

        }

        private void TotalPVDesmontar(string[] a)
        {
            var resumo = new ConciliacaoUseRedeEEVDTotalPontoVendaStruct()
            {
                is_tipo_registro = Convert.ToInt32(a[0]),
                nm_tipo_registro = "Total ponto de venda",
                is_numero_ponto_venda = Convert.ToDecimal(a[1]).ToString(),
                is_quantidade_resumo_vendas_acatados = Convert.ToDecimal(a[2]),
                is_quantidade_comprovante_vendas = Convert.ToDecimal(a[3]),
                is_total_bruto = Convert.ToDecimal(a[4]) /100,
                is_total_desconto = Convert.ToDecimal(a[5]) / 100,
                is_total_liquido = Convert.ToDecimal(a[6]) / 100,
                is_valor_bruto_predatado = Convert.ToDecimal(a[7]) / 100,
                is_valor_desconto_predatado = Convert.ToDecimal(a[8]) / 100,
                is_valor_liquido_predatado = Convert.ToDecimal(a[9]) / 100,
                rede = 1
            };

            io_arl_pv.Add(resumo);

            string estabelecimento = a[1];

            if (io_hsm_totalizador_estabelecimento.Contains(estabelecimento))
            {
                TotalizadorEstabelecimento totalizador_estabelecimento = (TotalizadorEstabelecimento)io_hsm_totalizador_estabelecimento[estabelecimento];
                io_hsm_totalizador_estabelecimento.Remove(estabelecimento);
                io_hsm_totalizador_estabelecimento.Add(estabelecimento, new TotalizadorEstabelecimento
                {
                    prev_pagamento = Convert.ToDateTime(data_pagamento),
                    total_realizado = totalizador_estabelecimento.total_realizado + resumo.is_total_liquido,
                    estabelecimento = Convert.ToInt32(estabelecimento)
                });
            }
            else
            {
                io_hsm_totalizador_estabelecimento.Add(estabelecimento, new TotalizadorEstabelecimento
                {
                    prev_pagamento = Convert.ToDateTime(data_pagamento),
                    total_realizado = resumo.is_total_liquido,
                    estabelecimento = Convert.ToInt32(estabelecimento)
                });
            }

            if (io_hsm_totalizador_banco.Contains(banco))
            {
                TotalizadorBanco totalizador_banco = (TotalizadorBanco)io_hsm_totalizador_banco[banco];
                io_hsm_totalizador_banco.Remove(banco);
                io_hsm_totalizador_banco.Add(banco, new TotalizadorBanco
                {
                    data_prevista = Convert.ToDateTime(data_pagamento),
                    total_realizado = totalizador_banco.total_realizado + resumo.is_total_liquido,
                    id_banco = Convert.ToInt32(banco)
                });
            }
            else
            {
                io_hsm_totalizador_banco.Add(banco, new TotalizadorBanco
                {
                    data_prevista = Convert.ToDateTime(data_pagamento),
                    total_realizado = resumo.is_total_liquido,
                    id_banco = Convert.ToInt32(banco)
                });
            }

            Decimal valor_total = 0;
            Decimal valor_total_liquido = 0;

            if (io_hsm_totalizador_produto.Contains("debito"))
            {
                TotalizadorProduto totalizador_produto = (TotalizadorProduto)io_hsm_totalizador_produto["debito"];
                valor_total = totalizador_produto.valor_bruto + resumo.is_total_bruto;
                valor_total_liquido = totalizador_produto.valor_liquido + resumo.is_total_liquido;
                io_hsm_totalizador_produto.Remove("debito");
                io_hsm_totalizador_produto.Add("debito", new TotalizadorProduto
                {
                    data_prevista = Convert.ToDateTime(data_pagamento),
                    valor_bruto = valor_total,
                    valor_liquido = valor_total_liquido,
                    ds_produto = "Débito",
                    rede = "Userede"
                });
            }
            else
            {
                io_hsm_totalizador_produto.Add("debito", new TotalizadorProduto
                {
                    data_prevista = Convert.ToDateTime(data_pagamento),
                    valor_bruto = resumo.is_total_bruto,
                    valor_liquido = resumo.is_total_liquido,
                    ds_produto = "Débito",
                    rede = "Userede"
                });
            }

        }

        private void TotalMatrizDesmontar(string[] a)
        {
            var resumo = new ConciliacaoUseRedeEEVDTotalMatrizStruct()
            {
                is_tipo_registro = Convert.ToInt32(a[0]),
                is_numero_ponto_venda = Convert.ToDecimal(a[1]).ToString(),
                is_quantidade_resumo_vendas_acatados = Convert.ToDecimal(a[2]),
                is_quantidade_comprovante_vendas = Convert.ToDecimal(a[3]),
                is_total_bruto = Convert.ToDecimal(a[4]) / 100,
                is_total_desconto = Convert.ToDecimal(a[5]) / 100,
                is_total_liquido = Convert.ToDecimal(a[6]) / 100,
                is_valor_bruto_predatado = Convert.ToDecimal(a[7]) / 100,
                is_valor_desconto_predatado = Convert.ToDecimal(a[8]) / 100,
                is_valor_liquido_predatado = Convert.ToDecimal(a[9]) / 100,
                rede = 1
            };

            io_arl_tm.Add(resumo);

            string estabelecimento = a[1];

            if (io_hsm_totalizador_estabelecimento.Contains(estabelecimento))
            {
                TotalizadorEstabelecimento totalizador_estabelecimento = (TotalizadorEstabelecimento)io_hsm_totalizador_estabelecimento[estabelecimento];
                io_hsm_totalizador_estabelecimento.Remove(estabelecimento);
                io_hsm_totalizador_estabelecimento.Add(estabelecimento, new TotalizadorEstabelecimento
                {
                    prev_pagamento = Convert.ToDateTime(data_pagamento),
                    total_realizado = totalizador_estabelecimento.total_realizado + resumo.is_total_liquido,
                    estabelecimento = Convert.ToInt32(estabelecimento)
                });
            }
            else
            {
                io_hsm_totalizador_estabelecimento.Add(estabelecimento, new TotalizadorEstabelecimento
                {
                    prev_pagamento = Convert.ToDateTime(data_pagamento),
                    total_realizado = resumo.is_total_liquido,
                    estabelecimento = Convert.ToInt32(estabelecimento)
                });
            }
        }

        private void TotalArquivoDesmontar(string[] a)
        {
            var resumo = new ConciliacaoUseRedeEEVDTotalArquivoStruct()
            {
                is_tipo_registro = Convert.ToInt32(a[0]),
                is_numero_ponto_venda = Convert.ToDecimal(a[1]).ToString(),
                is_quantidade_resumo_vendas_acatados = Convert.ToDecimal(a[2]),
                is_quantidade_comprovante_vendas = Convert.ToDecimal(a[3]),
                is_total_bruto = Convert.ToDecimal(a[4]) /100,
                is_total_desconto = Convert.ToDecimal(a[5]) / 100,
                is_total_liquido = Convert.ToDecimal(a[6]) / 100,
                is_valor_bruto_predatado = Convert.ToDecimal(a[7]) / 100,
                is_valor_desconto_predatado = Convert.ToDecimal(a[8]) / 100,
                is_valor_liquido_predatado = Convert.ToDecimal(a[9]) / 100,
                is_total_registro = Convert.ToDecimal(a[10]),
                rede = 1
            };

            io_arl_ta.Add(resumo);
        }

        private void CVDesmontar(string[] a)
        {

           // var achou = TEFDAL.GetStatusTEF(Convert.ToDecimal(a[9]).ToString()); //numero_resumo_vendas

            var resumo = new ConciliacaoUseRedeEEVDComprovanteVendaStruct()
            {
                is_tipo_registro = Convert.ToInt32(a[0]),
                nm_tipo_registro = "CV - Detalhamento",
                is_numero_filiacao_pv = Convert.ToDecimal(a[1]),
                is_numero_resumo_vendas = Convert.ToInt64(a[2]),
                is_data_cv = Convert.ToDateTime(a[3].Substring(0, 2) + "/" + a[3].Substring(2, 2) + "/" + a[3].Substring(4, 4)),
                is_valor_bruto = Convert.ToDecimal(a[4]) / 100,
                is_valor_desconto = Convert.ToDecimal(a[5]) / 100,
                is_valor_liquido = Convert.ToDecimal(a[6]) / 100,
                is_numero_cartao = a[7],
                is_tipo_transacao = TipoTransacao(a[8]),
                is_numero_cv = Convert.ToDecimal(a[9]),
                is_data_credito = Convert.ToDateTime(a[10].Substring(0, 2) + "/" + a[10].Substring(2, 2) + "/" + a[10].Substring(4, 4)),
                is_status_transacao = a[11].Equals("01") ? "Aprovado" : a[11],
                is_hora_transacao = a[12].Substring(0, 2) + ":" + a[12].Substring(2, 2) + ":" + a[12].Substring(4, 2),
                is_numero_terminal = a[13],
                is_tipo_captura = TabelaII(Convert.ToInt32(a[14])),
                is_reservado = a[15],
                is_valor_compra = Convert.ToDecimal(a[16]) / 100,
                is_valor_saque = Convert.ToDecimal(a[17]) / 100,
                is_bandeira = Bandeira(a[18]),
                taxa_cobrada = Math.Abs(Convert.ToDecimal(((Convert.ToDecimal(a[6]) / 100) * 100) / (Convert.ToDecimal(a[4]) / 100)) - 100),
                rede = 1

                //   is_codigo_tef = achou.Codigt
            };

            io_arl_cv.Add(resumo);

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
            if (Convert.ToDouble(as_texto) == 0)
            {
                return "";
            }
            DateTime data_arquivo = DateTime.ParseExact(as_texto, as_mascara, CultureInfo.InvariantCulture);
            return (data_arquivo.ToString(as_mascara_nova));
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
            String MilharCentena = as_texto.Substring(0, as_texto.Length - 2);
            String Centavos = as_texto.Substring(MilharCentena.Length, 2);
            String aux = MilharCentena + "," + Centavos;
            double Valor = Convert.ToDouble(aux);
            return Valor == 0 ? "" : Valor.ToString("#,##0.00");
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

        private string Bandeira(string bandeira)
        {
            string strprod = "";
            switch (bandeira)
            {
                case "0":
                    strprod = "Outras bandeiras";
                    break;
                case "1":
                    strprod = "Maestro";
                    break;
                case "3":
                    strprod = "Visa Electron";
                    break;
                case "4":
                    strprod = "Cabal";
                    break;
                case "9":
                    strprod = "Sicred";
                    break;
                case "H":
                    strprod = "Hipercard";
                    break;
                case "A":
                    strprod = "Avista";
                    break;
                case "B":
                    strprod = "Banescard";
                    break;
                case "E":
                    strprod = "Elo";
                    break;
                case "X":
                    strprod = "Amex";
                    break;
            }
            return strprod;
        }

        private string TabelaII(int captura)
        {
            string strcap = "";
            switch (captura)
            {
                case 1:
                    strcap = "Manual";
                    break;
                case 2:
                    strcap = "POS";
                    break;
                case 3:
                    strcap = "PDV";
                    break;
                case 4:
                    strcap = "TO";
                    break;
                case 5:
                    strcap = "Internet";
                    break;
                case 6:
                    strcap = "Leitor de trilha";
                    break;
                case 9:
                    strcap = "Outros";
                    break;
            }
            return strcap;
        }

        private string TipoTransacao(string transacao)
        {
            string strcap = "";
            switch (transacao)
            {
                case "V":
                    strcap = "Cartão de Débito Avista";
                    break;
                case "C":
                    strcap = "CDC";
                    break;
                case "T":
                    strcap = "Trishop";
                    break;
                case "S":
                    strcap = "Construcard";
                    break;
                case "P":
                    strcap = "Pré-Datado";
                    break;
                case "F":
                    strcap = "Pagamento de fatura";
                    break;
            }
            return strcap;
        }

    }
}
