using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Conciliacao.Models.conciliacao;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoModelo.model.conciliador.UseRede;
using ConciliacaoPersistencia.model;

namespace Conciliacao.Models.UseRede
{
    public class ConciliacaoUseRedeEEFIDesmontar
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
        private List<ConciliacaoUseRedeEEFICreditosStruct> io_arl_creditos = new List<ConciliacaoUseRedeEEFICreditosStruct>();
        private List<ConciliacaoUseRedeEEFIAjustesDesagendamentoStruct> io_arl_ajustes_desagendamento = new List<ConciliacaoUseRedeEEFIAjustesDesagendamentoStruct>();
        private List<ConciliacaoUseRedeEEFIAntecipacaoStruct> io_arl_ajustes_antecipacao = new List<ConciliacaoUseRedeEEFIAntecipacaoStruct>();
        private List<ConciliacaoUseRedeEEFIDesagendamentoParcelasStruct> io_arl_desagendamento_parcela = new List<ConciliacaoUseRedeEEFIDesagendamentoParcelasStruct>();
        private List<ConciliacaoUseRedeEEFITotCreditosStruct> io_arl_total_credito = new List<ConciliacaoUseRedeEEFITotCreditosStruct>();
        private List<ConciliacaoUseRedeEEFITotMatrizStruct> io_arl_total_matriz = new List<ConciliacaoUseRedeEEFITotMatrizStruct>();
        private List<ConciliacaoUseRedeEEFIResumoOperacaoStruct> io_arl_resumo_op = new List<ConciliacaoUseRedeEEFIResumoOperacaoStruct>();

        public List<ConciliacaoUseRedeEEFIResumoOperacaoStruct> GetResumoArquivo()
        {
            return io_arl_resumo_op;
        }


        public List<ConciliacaoUseRedeEEFICreditosStruct> GetCreditos()
        {
            return io_arl_creditos;
        }

        public List<ConciliacaoUseRedeEEFIAjustesDesagendamentoStruct> GetAjustesDesagendamento()
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
        }

        public List<ConciliacaoUseRedeEEFITotCreditosStruct> GetTotalCredito()
        {
            return io_arl_total_credito;
        }

        public List<ConciliacaoUseRedeEEFITotMatrizStruct> GetTotalMatriz()
        {
            return io_arl_total_matriz;
        }

        /*
        Construtor da classe.
        */
        public ConciliacaoUseRedeEEFIDesmontar
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
                        switch (is_linha_atual.Substring(0, 3))
                        {
                            case "034":
                                {
                                    CreditosDesmontar(is_linha_atual);

                                    break;
                                }

                            case "035":
                                {
                                    AjustesDesagendamentoDesmontar(is_linha_atual);

                                    break;
                                }


                            case "036":
                                {
                                    AntecipacoesDesmontar(is_linha_atual);

                                    break;
                                }

                            case "037":
                                {
                                    TotalCreditosDesmontar(is_linha_atual);

                                    break;
                                }

                            case "049":
                                {
                                    DesagendamentoParcelasDesmontar(is_linha_atual);

                                    break;
                                }

                            case "50":
                                {
                                    TotalMatrizDesmontar(is_linha_atual);

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

        }

        private void CreditosDesmontar(string a)
        {
            try
            {

                var creditos = new ConciliacaoUseRedeEEFICreditosStruct()
                {
                    numero_pv_centralizador = Convert.ToDecimal(a.Substring(3, 9)),
                    numero_documento = Convert.ToDecimal(a.Substring(12, 11)),
                    data_lancamento = a.Substring(23, 8).Equals("00000000") ? DateTime.Now : Convert.ToDateTime(a.Substring(23, 2) + "/" + a.Substring(25, 2) + "/" + a.Substring(27, 4)),
                    valor_lancamento = Convert.ToDecimal(a.Substring(31, 15)) / 100,
                    banco = Convert.ToInt32(a.Substring(47, 3)),
                    agencia = a.Substring(50, 6),
                    conta_corrente = a.Substring(56, 11),
                    data_movimento = a.Substring(67, 8).Equals("00000000") ? DateTime.Now : Convert.ToDateTime(a.Substring(67, 2) + "/" + a.Substring(69, 2) + "/" + a.Substring(71, 4)),
                    numero_rv = Convert.ToInt32(a.Substring(75, 9)),
                    data_rv = a.Substring(84, 8).Equals("00000000") ? DateTime.Now : Convert.ToDateTime(a.Substring(84, 2) + "/" + a.Substring(86, 2) + "/" + a.Substring(88, 4)),
                    bandeira = Bandeira(a.Substring(92, 1)),
                    tipo_transacao = TabelaTpTrans(a.Substring(93, 1)),
                    valor_bruto_rv = Convert.ToDecimal(a.Substring(94, 15)) / 100,
                    valor_taxa_desconto = Convert.ToDecimal(a.Substring(109, 15)) / 100,
                    numero_parcela = a.Substring(124, 5),
                    situacao = Tabela2(a.Substring(129, 2)),
                    numero_pv_original = Convert.ToInt32(a.Substring(131, 9)),
                    rede = 1
                };

                var resumo = new ConciliacaoUseRedeEEFIResumoOperacaoStruct()
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
                    rede = 1
                };

                io_arl_resumo_op.Add(resumo);

                io_arl_creditos.Add(creditos);

            }
            catch (Exception ex)
            {
                Console.Write("Falha no processamento do arquivo: " + ex.Message);
                throw;
            }
        }

        private void AjustesDesagendamentoDesmontar(string a)
        {
            try
            {
                var ajustes_desagendamento = new ConciliacaoUseRedeEEFIAjustesDesagendamentoStruct()
                {
                    numero_pv_ajustado = Convert.ToInt32(a.Substring(3, 9)),
                    numero_rv_ajustado = Convert.ToInt32(a.Substring(12, 9)),
                    data_ajuste = a.Substring(21, 8).Equals("00000000") ? DateTime.Now : Convert.ToDateTime(a.Substring(21, 2) + "/" + a.Substring(23, 2) + "/" + a.Substring(25, 4)),
                    valor_ajuste = Convert.ToDecimal(a.Substring(29, 15)) / 100,
                    motivo_ajuste = a.Substring(47, 28),
                    numero_cartao = a.Substring(75, 16),
                    data_transacao = a.Substring(91, 8).Equals("00000000") ? DateTime.Now : Convert.ToDateTime(a.Substring(91, 2) + "/" + a.Substring(93, 2) + "/" + a.Substring(95, 4)),
                    numero_rv_original = Convert.ToInt32(a.Substring(99, 9)),
                    numero_referencia_carta = a.Substring(108, 15),
                    data_carta = a.Substring(123, 8).Equals("00000000") ? DateTime.Now : Convert.ToDateTime(a.Substring(123, 2) + "/" + a.Substring(125, 2) + "/" + a.Substring(127, 4)),
                    mes_referencia = Convert.ToInt32(a.Substring(131, 6)),
                    numero_pv_original = Convert.ToInt32(a.Substring(137, 9)),
                    data_rv_original = a.Substring(146, 8).Equals("00000000") ? DateTime.Now : Convert.ToDateTime(a.Substring(146, 2) + "/" + a.Substring(148, 2) + "/" + a.Substring(150, 4)),
                    valor_transacao = Convert.ToDecimal(a.Substring(154, 15)) / 100,
                    identificador = a.Substring(169, 1),
                    data_credito = a.Substring(170, 8).Equals("00000000") ? DateTime.Now : Convert.ToDateTime(a.Substring(170, 2) + "/" + a.Substring(172, 2) + "/" + a.Substring(174, 4)),
                    novo_valor_parcela = Convert.ToDecimal(a.Substring(178, 15)) / 100,
                    valor_original_parcela = Convert.ToDecimal(a.Substring(193, 15)) / 100,
                    valor_bruto_resumo = Convert.ToDecimal(a.Substring(208, 15)) / 100,
                    valor_cancelado = Convert.ToDecimal(a.Substring(223, 15)) / 100,
                    nsu = Convert.ToInt32(a.Substring(238, 12)),
                    numero_autorizacao = a.Substring(250, 6),
                    valor_debito_total = Convert.ToDecimal(a.Substring(268, 15)) / 100,
                    valor_pendente = Convert.ToDecimal(a.Substring(283, 15)) / 100,
                    rede = 1
                };

                var resumo = new ConciliacaoUseRedeEEFIResumoOperacaoStruct()
                {
                    data = ajustes_desagendamento.data_ajuste,
                    data_venda = ajustes_desagendamento.data_transacao,
                    nsu = ajustes_desagendamento.nsu.ToString(),
                    descricao = "Ajustes e Desagendamentos",
                    numero_pv = ajustes_desagendamento.numero_pv_original.ToString(),
                    situacao = ajustes_desagendamento.motivo_ajuste,
                    valor = ajustes_desagendamento.novo_valor_parcela,
                    valor_cancelado = ajustes_desagendamento.valor_cancelado,
                    valor_venda = ajustes_desagendamento.valor_original_parcela,
                    rede = 1
                };

                io_arl_resumo_op.Add(resumo);

                io_arl_ajustes_desagendamento.Add(ajustes_desagendamento);

            }
            catch (Exception ex)
            {
                Console.Write("Falha no processamento do arquivo: " + ex.Message);
                throw;
            }

        }

        private void AntecipacoesDesmontar(string a)
        {
            try
            {
                var antecipados = new ConciliacaoUseRedeEEFIAntecipacaoStruct()
                {
                    numero_pv = Convert.ToInt32(a.Substring(3, 9)),
                    numero_documento = a.Substring(12, 11),
                    data_lancamento = a.Substring(23, 8).Equals("00000000") ? DateTime.Now :  Convert.ToDateTime(a.Substring(23, 2) + "/" + a.Substring(25, 2) + "/" + a.Substring(27, 4)),
                    valor_lancamento = Convert.ToDecimal(a.Substring(31, 15)) / 100,
                    banco = Convert.ToInt32(a.Substring(47, 3)),
                    agencia = a.Substring(50, 6),
                    conta_corrente = a.Substring(56, 11),
                    numero_rv_correspondente = Convert.ToInt32(a.Substring(67, 9)),
                    data_rv_correspondente = a.Substring(76, 8).Equals("00000000") ? DateTime.Now : Convert.ToDateTime(a.Substring(76, 2) + "/" + a.Substring(78, 2) + "/" + a.Substring(80, 4)),
                    valor_credito_original = Convert.ToDecimal(a.Substring(84, 15)) / 100,
                    data_vencimento_original = a.Substring(99, 8).Equals("00000000") ? DateTime.Now : Convert.ToDateTime(a.Substring(99, 2) + "/" + a.Substring(101, 2) + "/" + a.Substring(103, 4)),
                    numero_parcela = a.Substring(107, 5),
                    valor_bruto = Convert.ToDecimal(a.Substring(112, 15)) / 100,
                    valor_taxa_desconto = Convert.ToDecimal(a.Substring(127, 15)) / 100,
                    numero_pv_original = a.Substring(142, 9),
                    bandeira = Bandeira(a.Substring(151, 1)),
                    rede = 1
                };

                var resumo = new ConciliacaoUseRedeEEFIResumoOperacaoStruct()
                {
                    data = antecipados.data_lancamento,
                    data_venda = antecipados.data_rv_correspondente,
                    descricao = "Antecipações",
                    numero_pv = antecipados.numero_pv_original.ToString(),
                    situacao = "Crédito",
                    valor = antecipados.valor_lancamento,
                    valor_venda = antecipados.valor_credito_original,
                    banco = antecipados.banco,
                    agencia = antecipados.agencia,
                    conta_corrente = antecipados.conta_corrente
                };

                io_arl_resumo_op.Add(resumo);

                io_arl_ajustes_antecipacao.Add(antecipados);

            }
            catch (Exception ex)
            {
                Console.Write("Falha no processamento do arquivo: " + ex.Message);
                throw;
            }
        }

        private void TotalCreditosDesmontar(string a)
        {
            try
            {
                var totalizador_creditos = new ConciliacaoUseRedeEEFITotCreditosStruct()
                {
                    numero_pv = Convert.ToInt32(a.Substring(3, 9)),

                    data_credito = a.Substring(19, 8).Equals("00000000") ? DateTime.MinValue : Convert.ToDateTime(a.Substring(19, 2) + "/" + a.Substring(21, 2) + "/" + a.Substring(23, 4)),
                    valor_total_credito = Convert.ToDecimal(a.Substring(27, 15)) / 100,
                    banco = Convert.ToInt32(a.Substring(43, 3)),
                    agencia = Convert.ToInt32(a.Substring(46, 6)),
                    conta = a.Substring(52, 11),
                    data_arquivo = Convert.ToDateTime(a.Substring(63, 2) + "/" + a.Substring(65, 2) + "/" + a.Substring(67, 4)),

                    valor_total_antecipado = Convert.ToDecimal(a.Substring(79, 15)) / 100,
                    rede = 1
                };

                if (!a.Substring(71, 8).Equals("00000000"))
                {
                    totalizador_creditos.data_credito_antecipado = Convert.ToDateTime(a.Substring(71, 2) + "/" + a.Substring(73, 2) + "/" + a.Substring(75, 4));
                }
                io_arl_total_credito.Add(totalizador_creditos);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DesagendamentoParcelasDesmontar(string a)
        {
            var parcelas = new ConciliacaoUseRedeEEFIDesagendamentoParcelasStruct()
            {
                numero_pv_ajustado = Convert.ToInt32(a.Substring(3, 9)),
                numero_rv_ajustado = Convert.ToInt32(a.Substring(12, 9)),
                data_credito = Convert.ToDateTime(a.Substring(36, 2) + "/" + a.Substring(38, 2) + "/" + a.Substring(40, 4)),
                novo_valor_pacela = Convert.ToDecimal(a.Substring(44, 15)) / 100,
                valor_original_pacela = Convert.ToDecimal(a.Substring(59, 15)) / 100,
                valor_ajuste = Convert.ToDecimal(a.Substring(74, 15)) / 100,
                data_cancelamento = Convert.ToDateTime(a.Substring(89, 2) + "/" + a.Substring(91, 2) + "/" + a.Substring(93, 4)),
                valor_rv_original = Convert.ToDecimal(a.Substring(97, 15)) / 100,
                valor_cancelamento_solicitado = Convert.ToDecimal(a.Substring(112, 15)) / 100,
                numero_cartao = a.Substring(127, 16),
                data_transacao = Convert.ToDateTime(a.Substring(143, 2) + "/" + a.Substring(145, 2) + "/" + a.Substring(147, 4)),
                nsu = a.Substring(151, 12),
                numero_pacela = Convert.ToInt32(a.Substring(164, 2)),
                bandeira_rv = Bandeira(a.Substring(166, 1)),
                rede = 1
            };

            var resumo = new ConciliacaoUseRedeEEFIResumoOperacaoStruct()
            {
                data = parcelas.data_credito,
                data_venda = parcelas.data_transacao,
                descricao = "Desagendamento de parcelas",
                numero_pv = parcelas.numero_pv_ajustado.ToString(),
                situacao = "Desagendamento",
                valor = parcelas.valor_ajuste,
                valor_venda = parcelas.valor_rv_original,
                valor_cancelado = parcelas.valor_cancelamento_solicitado,
                nsu = parcelas.nsu,
                rede = 1
            };

            io_arl_resumo_op.Add(resumo);

            io_arl_desagendamento_parcela.Add(parcelas);
        }

        private void TotalMatrizDesmontar(string a)
        {
            var total_matriz = new ConciliacaoUseRedeEEFITotMatrizStruct()
            {
                numero_pv = Convert.ToInt32(a.Substring(3, 9)),
                quantidade_total_resumos = Convert.ToInt32(a.Substring(12, 6)),
                valor_total_ajustes_credito = Convert.ToDecimal(a.Substring(18, 15)) / 100,
                quantidade_creditos_antecipados = Convert.ToInt32(a.Substring(33, 6)),
                valor_total_antecipado = Convert.ToDecimal(a.Substring(39, 15)) / 100,
                quantidade_ajustes_credito = Convert.ToInt32(a.Substring(54, 4)),
                valor_total_creditos_normais = Convert.ToDecimal(a.Substring(58, 15)) / 100,
                quantidade_ajustes_debito = Convert.ToInt32(a.Substring(73, 6)),
                valor_total_ajustes_debito = Convert.ToDecimal(a.Substring(79, 15)) / 100
            };

            io_arl_total_matriz.Add(total_matriz);
        }

        private string TabelaTpTrans(string produto)
        {
            string strprod = "";
            switch (produto)
            {
                case "01":
                    strprod = "À vista";
                    break;
                case "02":
                    strprod = "Parcelado sem juros";
                    break;
                case "03":
                    strprod = "Parcelado IATA";
                    break;
                case "04":
                    strprod = "Rotativo dólar";
                    break;
                case "05":
                    strprod = "CDC";
                    break;
                case "06":
                    strprod = "Pré-datada";
                    break;
                case "07":
                    strprod = "Trishop";
                    break;
                case "08":
                    strprod = "Construcard";
                    break;
            }
            return strprod;
        }

        private string Tabela2(string produto)
        {
            string strprod = "";
            switch (produto)
            {
                case "CV":
                    strprod = "CV";
                    break;
                case "01":
                    strprod = "A emitir";
                    break;
                case "02":
                    strprod = "Trânsito";
                    break;
                case "03":
                    strprod = "Pendente banco";
                    break;
                case "04":
                    strprod = "Pendente matriz";
                    break;
                case "05":
                    strprod = "Pendente filial";
                    break;
                case "06":
                    strprod = "Baixada";
                    break;
                case "07":
                    strprod = "Trânsito em fita";
                    break;
                case "08":
                    strprod = "Baixa automática";
                    break;
                case "09":
                    strprod = "Baixado para penhora ou retenção";
                    break;
                case "11":
                    strprod = "Suspenso";
                    break;
                case "12":
                    strprod = "Penhorado";
                    break;
                case "13":
                    strprod = "Retido";
                    break;
            }
            return strprod;
        }

        private string Bandeira(string produto)
        {
            string strprod = "";
            switch (produto)
            {
                case "0":
                    strprod = "Outras bandeiras";
                    break;
                case "1":
                    strprod = "Mastercard";
                    break;
                case "2":
                    strprod = "Diners Club";
                    break;
                case "3":
                    strprod = "Visa";
                    break;
                case "4":
                    strprod = "Cabal";
                    break;
                case "5":
                    strprod = "Hipercard";
                    break;
                case "6":
                    strprod = "Sorocred";
                    break;
                case "7":
                    strprod = "CUP";
                    break;
                case "8":
                    strprod = "Credsystem";
                    break;
                case "9":
                    strprod = "Sicred";
                    break;
                case "A":
                    strprod = "Avista";
                    break;
                case "B":
                    strprod = "Banescard";
                    break;
                case "J":
                    strprod = "JCB";
                    break;
                case "Z":
                    strprod = "Credz";
                    break;

            }
            return strprod;
        }


    }
}
