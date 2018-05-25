using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Conciliacao.Models.conciliacao;
using ConciliacaoModelo.model.conciliador;

namespace Conciliacao.Models
{
    public class ConciliacaoBanrisulDesmontar
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
        private List<ArquivoTransacao> io_arl_just_cv = new List<ArquivoTransacao>();

        private List<ArquivoDetalhado> io_detalhado = new List<ArquivoDetalhado>();
        private List<ArquivoCompleto> io_completo = new List<ArquivoCompleto>();

        private List<ArquivoAntecipado> io_antecipado = new List<ArquivoAntecipado>();

        private List<TotalizadorProduto> io_totalizador_produto = new List<TotalizadorProduto>();
        private List<TotalizadorBanco> io_totalizador_banco = new List<TotalizadorBanco>();
        private List<TotalizadorEstabelecimento> io_totalizador_estabelecimento = new List<TotalizadorEstabelecimento>();
        /*
        Estrutura CV.
        */
        private Dictionary<string, ArrayList> io_hsm_cv = new Dictionary<string, ArrayList>();            
        private string ls_identificador_arquivo = "";
        private string is_numero_agencia = "";
        private string is_numero_conta = "";

        /*
        Construtor da classe.
        */
        public ConciliacaoBanrisulDesmontar
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
                            case "2":
                            {
                                	//
									// Se contem a estrutura de pagamentos ...
									//
									if	(
											"BJRVCA1".equalsIgnoreCase(ls_identificador_arquivo)
										)
									{
										 /*
                                    	 Desmonta o RO.
                                   	 	 */
                                   		 RODesmontar();
									}
								
									//
									// Se contem a estrutura de lancamento ...
									//
									else if	(
												"BJRVCC1".equalsIgnoreCase(ls_identificador_arquivo)
											)
									{
										//
										// Desmonta o registro de Lançamentos (PAGAMENTOS).
										//
										CVDesmontar();
									}
								
									//
									// Se não contem nenhuma estrutura ...
									//
									else
									{
										throw	new Exception
										(
											"Problemas no processamento do arquivo: - Apenas com Registro Header"
										);
									}
								
									break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("Falha no processamento do arquivo: " + ex.Message);
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
        	
        	//
			// Armazena o identificador do arquivo.
			//
			ls_identificador_arquivo = is_linha_atual.Substring(42, 50).Trim();
											
            /*
            Cria a estrutura.
            */
            io_header = new ConciliacaoHeaderStruct
            {
                /* Tipo de registro. */
                is_tipo_registro = _first.Substring(0, 2),
                /* Estabelecimento Matriz. */
                is_estabelecimento_matriz = _first.Substring(82, 14),
                /* Data de processamento.  */
                is_data_processamento = _first.Substring(58, 8),
                /* Periodo inicial. */
                is_periodo_inicial = _first.Substring(50, 8),
                /* Periodo final. */
                is_periodo_final = _first.Substring(50, 8),
                /* Sequencial. */
                is_sequencia = _first.Substring(58, 8)+_first.Substring(66, 6),
                /* Empresa adquirente.  */
                is_empresa_adquirente = "Banrisul",
                /* Opção extrato. */
                is_opcao_extrato = ls_identificador_arquivo.Equals("BJRVCA1") ? "Resumo de Operações" : "Registro de Pagamento",
                /* Van */
                is_van = "",
                /* Caixa postal */
                is_caixa_postal = "",
                /* Versão layout. */
                is_versao_layout = ""
            };
        }

        /**
	    Desmonta o resumo de vendas.
	    */
        private
        void RODesmontar()
        {
            /*
            Cria a estrutura de registro.
            */
            var resumo = new ArquivoResumo()
            {
                resumo = is_linha_atual.Substring(311, 9),
                estabelecimento = is_linha_atual.Substring(17, 7)+is_linha_atual.Substring(38, 4),
                parcela = is_linha_atual.Substring(66, 2),
                transacao = "Venda",
                prev_pagamento = FormatoDataExecutar(is_linha_atual.Substring(140, 8), "yyyyMMdd", "dd/MM/yyyy"),
                valor_bruto = FormatoValorExecutar(is_linha_atual.Substring(110, 15)),
                taxa_comissao = "",
                vl_comissao = "",
                vl_rejeitado = "",
                vl_liquido = FormatoValorExecutar(is_linha_atual.Substring(176, 13)),
                situacao = SituacaoParcelaGet(is_linha_atual.substring(108, 2), false),
                produto = is_linha_atual.Substring(98, 6).Equals("002000")
								?	"Pagamento à vista"
										:	is_linha_atual.Substring(98, 6).Equals("002800")
												?	"Pagamento parcelado com parcela à vista"
												:	is_linha_atual.Substring(98, 6).Equals("002890")
														?	"Pagamento parcelado sem parcela à vista"
														:	is_linha_atual.Substring(98, 6).Equals("002900")
																?	"Pagamento pré_datado"
																:	is_linha_atual.Substring(98, 6).Equals("002899")
																		?	"Pagamento c/ Crédito 1 Minuto"
																		:	is_linha_atual.Substring(98, 6).Equals("003900")
																				?	"Pagamento c/ Banco Sim Parcelado"
																				:	is_linha_atual.Substring(98, 6),
                terminal = is_linha_atual.Substring(42, 8),
                chave = ""
            };
            
            
            Decimal valor_total = 0;
            Decimal valor_total_liquido = 0;

            if (io_hsm_totalizador_produto.Contains(resumo.produto))
            {
                TotalizadorProduto totalizador_produto = (TotalizadorProduto)io_hsm_totalizador_produto[resumo.produto];
                valor_total = totalizador_produto.valor_bruto + FormatoValorExecutarDouble(resumo.valor_bruto);
                valor_total_liquido = totalizador_produto.vl_liquido + FormatoValorExecutarDouble(resumo.vl_liquido);
                io_hsm_totalizador_produto.Remove(resumo.produto);
                io_hsm_totalizador_produto.Add(resumo.produto, new TotalizadorProduto
                                                                {
                                                                    prev_pagamento = Convert.ToDateTime(FormatoDataExecutar(is_linha_atual.Substring(140, 8), "yyyyMMdd", "dd/MM/yyyy")),
                                                                    valor_bruto = valor_total,
                                                                    vl_liquido = valor_total_liquido,
                                                                    produto = resumo.produto
                                                                });
            }
            else
            {
                io_hsm_totalizador_produto.Add(resumo.produto, new TotalizadorProduto
                                                               {
                                                                   prev_pagamento = Convert.ToDateTime(FormatoDataExecutar(is_linha_atual.Substring(140, 8), "yyyyMMdd", "dd/MM/yyyy")),
                                                                   valor_bruto = FormatoValorExecutarDouble(resumo.valor_bruto),
                                                                   vl_liquido = FormatoValorExecutarDouble(resumo.vl_liquido),
                                                                   produto = resumo.produto
                                                               });
            }
            
            valor_total = 0;
            valor_total_liquido = 0;

            string estabelecimento = is_linha_atual.Substring(17, 7)+is_linha_atual.Substring(38, 4);

            if (io_hsm_totalizador_estabelecimento.Contains(estabelecimento))
            {
                TotalizadorEstabelecimento totalizador_estabelecimento = (TotalizadorEstabelecimento)io_hsm_totalizador_estabelecimento[estabelecimento];
                valor_total_liquido = totalizador_estabelecimento.total_realizado + FormatoValorExecutarDouble(resumo.vl_liquido);
                io_hsm_totalizador_estabelecimento.Remove(estabelecimento);
                io_hsm_totalizador_estabelecimento.Add(estabelecimento, new TotalizadorEstabelecimento
                {
                    prev_pagamento = Convert.ToDateTime(FormatoDataExecutar(is_linha_atual.Substring(140, 8), "yyyyMMdd", "dd/MM/yyyy")),
                    total_realizado = valor_total_liquido,
                    estabelecimento = Convert.ToInt32(estabelecimento)
                });
            }
            else
            {
                io_hsm_totalizador_estabelecimento.Add(estabelecimento, new TotalizadorEstabelecimento
                {
                    prev_pagamento = Convert.ToDateTime(FormatoDataExecutar(is_linha_atual.Substring(140, 8), "yyyyMMdd", "dd/MM/yyyy")),
                    total_realizado = FormatoValorExecutarDouble(resumo.vl_liquido),
                    estabelecimento = Convert.ToInt32(estabelecimento)
                });
            }
            
            /*
            GUARDA O ULTIMO RO.
            */
            is_ultimo_ro = is_linha_atual.Substring(311, 9);

            /*
            Adiciona o ro na lista.
            */
            io_arl_ro.Add(resumo);
            io_arl_cv = new ArrayList();

            var lo_detalhado = new ArquivoDetalhado()
            {
                registro = "Resumo do Pagamento",
                estabelecimento = resumo.estabelecimento,
                transacao = resumo.transacao,
                RO = resumo.resumo,
                produto = resumo.produto,
                cartao = "",
                vl_bruto = resumo.valor_bruto,
                vl_liquido = resumo.vl_liquido,
                parcela = "",
                total_parcela = "",
                autorizacao = "",
                nsu = ""
            };
            io_detalhado.Add(lo_detalhado);


            var lo_completo = new ArquivoCompleto()
            {
                registro = "Resumo do Pagamento",
                estabelecimento = resumo.estabelecimento,
                RO = resumo.resumo,
                parcela = resumo.parcela,
                total_parcela = is_linha_atual.Substring(68, 2),
                filer = "",
                plano = "",
                tipo_transacao = resumo.transacao,
                apresentacao = "",
                prev_pagamento = resumo.prev_pagamento,
                envio_banco = "",
                cartao = "",
                sinal_valor_bruto = "+",
                valor_bruto = resumo.valor_bruto,
                sinal_comissao = "",
                comissao = "",
                sinal_rejeitado = "",
                valor_rejeitado = "",
                sinal_liquido = "",
                valor_liquido = resumo.vl_liquido,
                valor_total_venda = "",
                valor_prox_parcela = "",
                taxas = "",
                autorizacao = "",
                nsu_doc = is_linha_atual.Substring(88, 6),
                banco = "",
                agencia = "",
                conta_corrente = "",
                status_pagamento = resumo.situacao,
                motivo_rejeicao = "",
                cvs_aceitos = "",
                produto = resumo.produto,
                cvs_rejeitados = "",
                data_venda = "",
                data_captura = FormatoDataExecutar(is_linha_atual.Substring(76, 8), "yyyyMMdd", "dd/MM/yyyy"),
                origem_ajuste = "",
                valor_complementar = "",
                produto_financeiro = ls_identificador_arquivo.Equals("BJRVCA1") ? "Resumo de Operações" : "Registro de Pagamento",
                valor_antecipado = "",
                bandeira = "",
                registro_unico_RO = resumo.resumo,
                taxas_comissao = "",
                tarifa = "",
                meio_captura = "",
                terminal = is_linha_atual.Substring(42, 8)
            };
            io_completo.Add(lo_completo);
        }

        /*
        Comprovante de venda.
        */
        private
        void CVDesmontar()
        {
        	if (is_linha_atual.Substring(0, 2).Equals("10")) {
        		is_numero_agencia = is_linha_atual.Substring(56, 4);
        		is_numero_conta = is_linha_atual.Substring(60, 10);
        	}
        	else {
        		
        		var lo_cv = new ArquivoTransacao()
           		{
                	num_resumo = is_linha_atual.Substring(311, 9),
                	registro = "Registro de Pagamento",
                	estabelecimento = is_linha_atual.Substring(3, 7)+is_linha_atual.Substring(24, 4),
                	num_logico = is_linha_atual.Substring(3, 7)+is_linha_atual.Substring(24, 4),
                	dt_venda = FormatoDataExecutar(is_linha_atual.Substring(42, 8), "yyyyMMdd", "dd/MM/yyyy"),
                	cartao = "",
                	vl_venda = FormatoValorExecutar(is_linha_atual.Substring(60, 15)),
               	 	parcela = is_linha_atual.Substring(58, 2),
                	total_parcela = "",
                	autorizacao = "",
                	nsu = is_linha_atual.Substring(50, 8)
            	};

            	io_arl_cv.Add(lo_cv);
           		 io_arl_just_cv.Add(lo_cv);
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
                	sinal_valor_bruto = "+",
                	valor_bruto = lo_cv.vl_venda,
                	sinal_comissao = "",
                	comissao = "",
                	sinal_rejeitado = "",
                	valor_rejeitado = "",
                	sinal_liquido = "+",
                	valor_liquido = FormatoValorExecutar(is_linha_atual.Substring(107, 15)),
                	valor_total_venda = "",
                	valor_prox_parcela = "",
                	taxas = "",
                	autorizacao = "",
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
            
            	valor_total = 0;
            	valor_total_liquido = 0;

            	if (io_hsm_totalizador_banco.Contains(is_numero_agencia))
            	{
               	 	TotalizadorBanco totalizador_banco = (TotalizadorBanco)io_hsm_totalizador_banco[is_numero_agencia];
                	valor_total_liquido = totalizador_banco.total_realizado + FormatoValorExecutarDouble(resumo.vl_liquido);
                	io_hsm_totalizador_banco.Remove(is_numero_agencia);
                	io_hsm_totalizador_banco.Add(is_numero_agencia, new TotalizadorBanco
                	{
                   		prev_pagamento = Convert.ToDateTime(FormatoDataExecutar(is_linha_atual.Substring(31, 6), "yyMMdd", "dd/MM/yy")),
                    	total_realizado = valor_total_liquido,
                    	banco = Convert.ToInt32(is_numero_agencia)
                	});
            	}
            	else
            	{
                	io_hsm_totalizador_banco.Add(is_numero_agencia, new TotalizadorBanco
                	{
                  	  prev_pagamento = Convert.ToDateTime(FormatoDataExecutar(is_linha_atual.Substring(31, 6), "yyMMdd", "dd/MM/yy")),
                    	total_realizado = FormatoValorExecutarDouble(resumo.vl_liquido),
                    	banco = Convert.ToInt32(is_numero_agencia)
                	});
            	}
        			
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
		Retorna a situação da parcela ou o status
		vindo do arquivo de conciliação.
		@param	as_conteudo_arquivo
			Conteudo presente no arquivo.
		@param	ab_situacao
			true	para retornar a situação padronizada da parcela.
			false	para retornar o status vindo do arquivo.
		*/
		public final
		String			SituacaoParcelaGet
					(
						final String		as_conteudo_arquivo,
						final boolean		ab_situacao
					)
		{
			//
			// Armazenará a situação da parcela.
			//
			String
			ls_situacao_status	=	"";
		
			switch	(Integer.parseInt(as_conteudo_arquivo)) 
			{
				case	00:
				{				
					ls_situacao_status	= "00-Ok por confirmação"
							;
				
					break;
				}
			
				case	1:
				{	
					ls_situacao_status	= "01-Ok pela resolução de pendências on-line"
							;
				
					break;
				}
			
				case	3:
				{	
					ls_situacao_status	=	"03-Ok pela resolução de pendências na operação seguinte"
							;
				
					break;
				}
			
				case	4:
				{	
					ls_situacao_status	=	"04-Ok pela resolução de pendências manual"
							;
				
					break;
				}
			
				case	10:
				{	
					ls_situacao_status	=	"10-Pendente"
							;
				
					break;
				}
			
				case	20:
				{	
					ls_situacao_status	=	"20-Não autorizada"
							;
				
					break;
				}
			
				case	40:
				{	
					ls_situacao_status	=	"40-Cancelada pela origem"
							;
				
					break;
				}
			
				case	41:
				{	
					ls_situacao_status	=	"41-Cancelada por resolução de pendências on-line"
							;
				
					break;
				}
			
				case	43:
				{	
					ls_situacao_status	=	"43-Cancelada por resolução de pendências na operação seguinte"
							;
				
					break;
				}
			
				case	44:
				{	
					ls_situacao_status	=		"44-Cancelada por resolução de pendências manual"
							;
				
					break;
				}
			
				case	50:
				{	
					ls_situacao_status	=		"50-Estornada"
							;
				
					break;
				}
					
				default:
				{	
					ls_situacao_status	=		as_conteudo_arquivo+"-Inexistente"
						;
					
					break;
				}
			}
		
			return	(ls_situacao_status);
		}

    }
}
