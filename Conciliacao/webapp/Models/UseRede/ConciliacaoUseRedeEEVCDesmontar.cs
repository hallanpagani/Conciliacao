using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Conciliacao.Models.conciliacao;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoModelo.model.conciliador.UseRede;

namespace Conciliacao.Models.UseRede
{
    public class ConciliacaoUseRedeEEVCDesmontar
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

        private string data_pagamento, banco, estabelecimento, produto;


        /*
        Estruturas para desmontagem.
        */
        private List<ConciliacaoUseRedeEEVCResumoOperacaoStruct> io_arl_ro = new List<ConciliacaoUseRedeEEVCResumoOperacaoStruct>();
        private List<ConciliacaoUseRedeEEVCComprovanteVendaStruct> io_arl_cv = new List<ConciliacaoUseRedeEEVCComprovanteVendaStruct>();
        private List<ConciliacaoUseRedeParcelasStruct> io_arl_parcelas = new List<ConciliacaoUseRedeParcelasStruct>();


        public List<ConciliacaoUseRedeEEVCResumoOperacaoStruct> GetResumoArquivo()
        {
            return io_arl_ro;
        }

        public List<ConciliacaoUseRedeEEVCComprovanteVendaStruct> GetComprovanteVenda()
        {
            return io_arl_cv;
        }

        public List<ConciliacaoUseRedeParcelasStruct> GetParcelas()
        {
            return io_arl_parcelas;
        }


        private string TabelaI(string produto)
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
                case "E":
                    strprod = "Elo";
                    break;
                case "J":
                    strprod = "JCB";
                    break;
                case "Z":
                    strprod = "Credz";
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

        private string TabelaIII(int captura)
        {
            string strcap = "";
            switch (captura)
            {
                case 0: strcap = "CV/NSU OK"; break;
                case 1: strcap = "VENDA JA PROCESSADA"; break;
                case 2: strcap = "MOEDA INVALIDA"; break;
                case 3: strcap = "TT PREST DIF VR VD"; break;
                case 4: strcap = "CV/NSU SEM VR PREST"; break;
                case 5: strcap = "CP ACIMA LIM REDESHO"; break;
                case 7: strcap = "CV/NSU IRREGULAR"; break;
                case 8: strcap = "ESTAB BORDERO INVALI"; break;
                case 10: strcap = "CARTAO EM BP"; break;
                case 11: strcap = "CV/NSU SEM ASSINATURA"; break;
                case 12: strcap = "CV/NSU ILEGIVEL"; break;
                case 13: strcap = "CV/NSU SEM AUTORIZAÇAO"; break;
                case 14: strcap = "CARTAO N CRED/DINERS"; break;
                case 15: strcap = "FALTA 2A VIA PROCESS"; break;
                case 16: strcap = "ESTAB S/ IATA N AUTO"; break;
                case 17: strcap = "CV/NSU IRREGULAR"; break;
                case 18: strcap = "CARTAO EM BP"; break;
                case 19: strcap = "TX EMBARQUE VLR EXCD"; break;
                case 20: strcap = "CARTAO VENCIDO"; break;
                case 21: strcap = "CV/NSU PARA CARTAO JURID"; break;
                case 22: strcap = "ACIMA LIMITE C/AUTOR"; break;
                case 23: strcap = "DATA CV/NSU POSTERIOR RV"; break;
                case 25: strcap = "CARTAO N CRED/DINERS"; break;
                case 27: strcap = "CV/NSU PARA CARTAO INTER"; break;
                case 28: strcap = "CARTAO EM BP"; break;
                case 29: strcap = "PROMOCAO MASTERCARD®"; break;
                case 30: strcap = "TR INV P/TIPO CARTAO"; break;
                case 31: strcap = "PLAN PAGTO BIG TICKE"; break;
                case 32: strcap = "COD AUT N ENCONTRADO"; break;
                case 34: strcap = "DATA CV/NSU POSTERIOR RV"; break;
                case 35: strcap = "CV/NSU COM VR INCORRETO"; break;
                case 36: strcap = "CV/NSU SEM AUTORIZACAO"; break;
                case 37: strcap = "APRES. TARDIA CV/NSU"; break;
                case 41: strcap = "PARC. INVAL.VIAC.AERE"; break;
                case 42: strcap = "NAC. INVAL.- DUTY FRE"; break;
                case 43: strcap = "MQ INVA/NAO TR VISTA"; break;
                case 44: strcap = "CV/NSU S/JRS EM RV ROTAT"; break;
                case 45: strcap = " CV/NSU ROT EM RV S/JUROS"; break;
                case 47: strcap = "CARTAO N PODE PARCEL"; break;
                case 48: strcap = "ESTAB. NAO AUTORIZADO"; break;
                case 49: strcap = "DESCUMPRIM DE PROCED"; break;
                case 102: strcap = "COD AUT N ENCONTRADO"; break;
                case 106: strcap = "VR CV/NSU MAIOR QUE AUT"; break;
                case 108: strcap = "AUT CARTAO DIFERENTE"; break;
                case 109: strcap = "ESTAB BORDERO INVALI"; break;
                case 110: strcap = "TIPO TRANS DF DA AUT"; break;
                case 111: strcap = "CV/NSU SEM ASSINATURA"; break;
                case 112: strcap = "CV/NSU ILEGIVEL"; break;
                case 114: strcap = "CARTAO N CRED/DINERS"; break;
                case 115: strcap = "FALTA 2A VIA PROCESS"; break;
                case 117: strcap = "CV/NSU IRREGULAR"; break;
                case 125: strcap = "CARTAO N CRED/DINERS"; break;
                case 137: strcap = "APRES. TARDIA CV/NSU"; break;
                case 141: strcap = "PARC. INVAL. VIAC. AERE"; break;
                case 142: strcap = "NAC. INVAL. – DUTY FRE"; break;
                case 143: strcap = "MQ INVA/NÃO TR VISTA"; break;
                case 144: strcap = "CV/NSU S/ JRS EM RV ROTAT"; break;
                case 145: strcap = "CV/NSU ROT EM RV S/ JUROS"; break;
                case 147: strcap = "CARTAO N PODE PARCEL"; break;
                case 148: strcap = "ESTAB. NAO AUTORIZADO"; break;
                case 149: strcap = "DESCUMPRIM DE PROCED"; break;
                case 194: strcap = "CV/NSU SEM AUTORIZAÇAO"; break;
                case 198: strcap = "COD AUT REUTILIZADO"; break;
            }
            return strcap;
        }

        private string TabelaV(int ajustes)
        {
            string straju = "";
            switch (ajustes)
            {
                case 2: straju = "CONSULTA DE CHEQUES"; break;
                case 3: straju = "DEBITO PARCELADO"; break;
                case 4: straju = "DEBITO TX TRIBUTO"; break;
                case 5: straju = "TX MAN DO TEF"; break;
                case 6: straju = "POS-INATIV/CONEC/PIN"; break;
                case 7: straju = "CREDENC/ADESAO"; break;
                case 8: straju = "REPOS/ADIC MAQUINETA"; break;
                case 9: straju = "CANCEL/CHBK MAESTRO"; break;
                case 10: straju = "CANCELAMENTOS POR DISPUTAS"; break;
                case 11: straju = "MENS.SecureCode"; break;
                case 12: straju = "MENSALIDADE HIPERCARD"; break;
                case 13: straju = "MAQUININHA CONTA CERTA"; break;
                case 14: straju = "TARIFA DEBITO"; break;
                case 15: straju = "CBK CARTAO CHIP"; break;
                case 16: straju = "ESTORNO CR.INDEV.CI"; break;
                case 17: straju = "INDENIZA POS PERDIDO"; break;
                case 18: straju = "CANCEL.DE VENDAS"; break;
                case 19: straju = "SEGUNDA VIA EXTRATO"; break;
                case 20: straju = "POS-INATIV/CONEC/PIN"; break;
                case 21: straju = "CANCELAMENTO MAESTRO"; break;
                case 22: straju = "CONTESTAÇAO DE VENDA"; break;
                case 23: straju = "CONTESTAÇAO DE VENDA"; break;
                case 24: straju = "TRF AD EXCESSO CBACK"; break;
                case 28: straju = "AL.POS/PINPAD/TX CONECT"; break;
                case 29: straju = "DEBITO RECARGA"; break;
                case 30: straju = "CANCEL DESP DOLAR"; break;
                case 32: straju = "CANCEL VENDA DEBITO"; break;
                case 34: straju = "MODELO TARIFARIO"; break;
                case 35: straju = "CONSULTA AVS"; break;
                case 36: straju = "DEVOLUÇAO CV"; break;
                case 37: straju = "ESTORNO CR.INDEV."; break;
                case 38: straju = "ESTORNO TAXA ADM."; break;
                case 39: straju = "CONTEST VENDAS HIPER"; break;
                case 40: straju = "TARIFA EXT. MENSAL"; break;
                case 44: straju = "MENSALIDADE CONTROL REDE"; break;
                case 45: straju = "RETROATIVO CONTROL REDE"; break;
                case 46: straju = "TAXA PARC. ESPECIAL"; break;
                case 48: straju = "AL.POS/PINPAD/TX CONECT"; break;
                case 49: straju = "POS-INATIV/CONEC/PIN"; break;
                case 51: straju = "CRED SecureCode"; break;
                case 52: straju = "REVERSAO DEBITO CBK"; break;
                case 53: straju = "CREDITO RECARGA"; break;
                case 54: straju = "TOT. LIQID. A MENOR"; break;
                case 55: straju = "FCV N CONSIDERADO RV"; break;
                case 56: straju = "PREMIO PROMOCAO PARCELADO"; break;
                case 57: straju = "PAGTO DESAGIO"; break;
                case 58: straju = "CREDITO ALUGUEL"; break;
                case 59: straju = "REBATE MENSAL"; break;
                case 60: straju = "REBATE FINAL"; break;
                case 61: straju = "DEV CRED PGTO MAIOR"; break;
                case 62: straju = "REGULARIZACAO DIF. TAXA"; break;
                case 63: straju = "REGUL.DB ANTERIOR"; break;
                case 64: straju = "PGTO DE RV"; break;
                case 65: straju = "DEV MENSAL HIPERCARD"; break;
                case 66: straju = "PGT.JURO CORREÇAO"; break;

            }
            return straju;
        }


        /*
        Construtor da classe.
        */
        public ConciliacaoUseRedeEEVCDesmontar
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
            // HeaderDesmontar(_first);

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
                            case "010":
                                {
                                    ROParceladoSemJurosDesmontar(is_linha_atual);

                                    break;
                                }

                            case "012":
                                {
                                    CVParceladoSemJurosDesmontar(is_linha_atual);

                                    break;
                                }

                            case "014":
                                {
                                    ParcelasDesmontar(is_linha_atual);

                                    break;
                                }

                            case "006":
                                {
                                    RORotativo(is_linha_atual);

                                    break;
                                }

                            case "008":
                                {
                                    CVRotativo(is_linha_atual);

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
            string arquivo = _first;

            /*
           Cria a estrutura.
           */
            io_header = new ConciliacaoHeaderStruct
            {

            };
        }

        private void ROParceladoSemJurosDesmontar(string a)
        {
            try
            {
                var resumo = new ConciliacaoUseRedeEEVCResumoOperacaoStruct()
                {
                    is_tipo_registro = 10,
                    nm_tipo_registro = "RV - Pacelado sem Juros",
                    is_numero_filiacao_pv = Convert.ToDecimal(a.Substring(3, 9)),
                    is_numero_resumo_venda = a.Substring(12, 9),
                    is_banco = a.Substring(21, 3),
                    is_agencia = a.Substring(24, 5),
                    is_conta_corrente = a.Substring(29, 11),
                    is_data_resumo_venda = Convert.ToDateTime(a.Substring(40, 2) + "/" + a.Substring(42, 2) + "/" + a.Substring(44, 4)),

                    is_quantidade_resumo_vendas = a.Substring(48, 5),
                    is_valor_bruto = Convert.ToDecimal(a.Substring(53, 15)) / 100,

                    is_valor_gorjeta = Convert.ToDecimal(a.Substring(68, 15)) / 100,

                    is_valor_rejeitado = Convert.ToDecimal(a.Substring(83, 15)) / 100,
                    is_valor_desconto = Convert.ToDecimal(a.Substring(98, 15)) / 100,
                    is_valor_liquido = Convert.ToDecimal(a.Substring(113, 15)) / 100,

                    is_data_credito = Convert.ToDateTime(a.Substring(128, 2) + "/" + a.Substring(130, 2) + "/" + a.Substring(132, 4)),
                    is_bandeira = TabelaI(a.Substring(136, 1)),
                    rede = 1
                   // adquirente = 1

                };

                data_pagamento = (a.Substring(128, 2) + "/" + a.Substring(130, 2) + "/" + a.Substring(132, 4));
                banco = resumo.is_banco;
                estabelecimento = a.Substring(3, 9);
                produto = "Crédito parcelado";

                io_arl_ro.Add(resumo);

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private void CVParceladoSemJurosDesmontar(string a)
        {

            try
            {
                /* 12  */
                var resumo = new ConciliacaoUseRedeEEVCComprovanteVendaStruct()
                {
                    is_tipo_registro = 12,
                    nm_tipo_registro = "CV/NSU Parcelado s/Juros",
                    is_numero_filiacao_pv = Convert.ToDecimal(a.Substring(3, 9)),
                    is_numero_resumo_vendas = Convert.ToInt64( string.IsNullOrEmpty(a.Substring(12, 9)) ? "0" : a.Substring(12, 9) ),

                    is_data_cv = Convert.ToDateTime(a.Substring(21, 2) + "/" + a.Substring(23, 2) + "/" + a.Substring(25, 4)),

                    is_valor_bruto = Convert.ToDecimal(a.Substring(37, 15)) / 100,
                    is_valor_gorjeta = Convert.ToDecimal(a.Substring(52, 15)) / 100,
                    is_numero_cartao = a.Substring(67, 16),

                    is_status_cv = TabelaIII(Convert.ToInt32(a.Substring(83, 3))),

                    is_numero_parcelas = Convert.ToInt32(a.Substring(86, 2)),
                    is_numero_cv = Convert.ToDecimal(a.Substring(88, 12)),

                    is_numero_referencia = a.Substring(100, 13),

                    is_valor_desconto = Convert.ToDecimal(a.Substring(113, 15)) / 100,

                    is_numero_autorizacao = a.Substring(128, 6),

                    is_hora_transcao = a.Substring(134, 2) + ":" + a.Substring(136, 2) + ":" + a.Substring(138, 2),

                    is_tipo_captura = TabelaII(Convert.ToInt32(a.Substring(204, 1))),

                    is_valor_liquido = Convert.ToDecimal(a.Substring(205, 15)) / 100,

                    is_valor_liquido_primeira_parc = Convert.ToDecimal(a.Substring(220, 15)) / 100,
                    is_valor_liquido_demais_parc = Convert.ToDecimal(a.Substring(235, 15)) / 100,

                    is_numero_terminal = a.Substring(250, 8),

                    is_bandeira = TabelaI(a.Substring(261, 1)),

                    taxa_cobrada = Math.Abs(Convert.ToDecimal(((Convert.ToDecimal(a.Substring(205, 15)) / 100) * 100) / (Convert.ToDecimal(a.Substring(37, 15)) / 100)) - 100),
                    is_data_credito = Convert.ToDateTime(data_pagamento),
                    rede = 1

                };

                io_arl_cv.Add(resumo);

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
                is_tipo_registro = Convert.ToInt32(a.Substring(0, 3)),
                is_numero_filiacao_pv = Convert.ToDecimal(a.Substring(3,9)),
                is_numero_resumo_vendas = a.Substring(12, 9),
                is_data_rv = Convert.ToDateTime(a.Substring(21, 2) + "/" + a.Substring(23, 2) + "/" + a.Substring(25, 4)),
                numero_parcela = Convert.ToInt32(a.Substring(37, 2)),
                valor_parcela_bruto = (Convert.ToDecimal(a.Substring(39, 15)) / 100),
                valor_desconto_parcela = (Convert.ToDecimal(a.Substring(54, 15)) / 100),
                valor_parcela_liquida = (Convert.ToDecimal(a.Substring(69, 15)) / 100),
                data_credito = Convert.ToDateTime(a.Substring(84, 2) + "/" + a.Substring(86, 2) + "/" + a.Substring(88, 4)),
                rede = 1
            };

            io_arl_parcelas.Add(parcelas);

            /* 14  */
            try
            {
                data_pagamento = a.Substring(84, 2) + "/" + a.Substring(86, 2) + "/" + a.Substring(88, 4);
                estabelecimento = a.Substring(3, 9);
                if (io_hsm_totalizador_estabelecimento.Contains(estabelecimento + data_pagamento))
                {
                    TotalizadorEstabelecimento totalizador_estabelecimento = (TotalizadorEstabelecimento)io_hsm_totalizador_estabelecimento[estabelecimento + data_pagamento];
                    io_hsm_totalizador_estabelecimento.Remove(estabelecimento + data_pagamento);
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + data_pagamento, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = Convert.ToDateTime(data_pagamento),
                        total_realizado = totalizador_estabelecimento.total_realizado + (Convert.ToDecimal(a.Substring(69, 15)) / 100),
                        estabelecimento = Convert.ToInt32(estabelecimento)
                    });
                }
                else
                {
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + data_pagamento, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = Convert.ToDateTime(data_pagamento),
                        total_realizado = (Convert.ToDecimal(a.Substring(69, 15)) / 100),
                        estabelecimento = Convert.ToInt32(estabelecimento)
                    });
                }

                if (io_hsm_totalizador_banco.Contains(banco + data_pagamento))
                {
                    TotalizadorBanco totalizador_banco = (TotalizadorBanco)io_hsm_totalizador_banco[banco + data_pagamento];
                    io_hsm_totalizador_banco.Remove(banco + data_pagamento);
                    io_hsm_totalizador_banco.Add(banco + data_pagamento, new TotalizadorBanco
                    {
                        data_prevista = Convert.ToDateTime(data_pagamento),
                        total_realizado = totalizador_banco.total_realizado + (Convert.ToDecimal(a.Substring(69, 15)) / 100),
                        id_banco = Convert.ToInt32(banco)
                    });
                }
                else
                {
                    io_hsm_totalizador_banco.Add(banco + data_pagamento, new TotalizadorBanco
                    {
                        data_prevista = Convert.ToDateTime(data_pagamento),
                        total_realizado = (Convert.ToDecimal(a.Substring(69, 15)) / 100),
                        id_banco = Convert.ToInt32(banco)
                    });
                }

                Decimal valor_total = 0;
                Decimal valor_total_liquido = 0;

                if (io_hsm_totalizador_produto.Contains(produto + data_pagamento))
                {
                    TotalizadorProduto totalizador_produto = (TotalizadorProduto)io_hsm_totalizador_produto[produto + data_pagamento];
                    valor_total = totalizador_produto.valor_bruto + (Convert.ToDecimal(a.Substring(39, 15)) / 100);
                    valor_total_liquido = totalizador_produto.valor_liquido + (Convert.ToDecimal(a.Substring(69, 15)) / 100);
                    io_hsm_totalizador_produto.Remove(produto + data_pagamento);
                    io_hsm_totalizador_produto.Add(produto + data_pagamento, new TotalizadorProduto
                    {
                        data_prevista = Convert.ToDateTime(data_pagamento),
                        valor_bruto = valor_total,
                        valor_liquido = valor_total_liquido,
                        ds_produto = produto,
                        rede = "Userede"
                    });
                }
                else
                {
                    io_hsm_totalizador_produto.Add(produto + data_pagamento, new TotalizadorProduto
                    {
                        data_prevista = Convert.ToDateTime(data_pagamento),
                        valor_bruto = (Convert.ToDecimal(a.Substring(39, 15)) / 100),
                        valor_liquido = (Convert.ToDecimal(a.Substring(69, 15)) / 100),
                        ds_produto = produto,
                        rede = "Userede"
                    });
                }

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private void RORotativo(string a)
        {
            try
            {
                var resumo = new ConciliacaoUseRedeEEVCResumoOperacaoStruct()
                {
                    is_tipo_registro = 6,
                    nm_tipo_registro = "RV - Rotativo",
                    is_numero_filiacao_pv = Convert.ToDecimal(a.Substring(3, 9)),
                    is_numero_resumo_venda = Convert.ToDecimal(a.Substring(12, 9)).ToString(),
                    is_banco = a.Substring(21, 3),
                    is_agencia = a.Substring(24, 5),
                    is_conta_corrente = a.Substring(29, 11),
                    is_data_resumo_venda = Convert.ToDateTime(a.Substring(40, 2) + "/" + a.Substring(42, 2) + "/" + a.Substring(44, 4)),

                    is_quantidade_resumo_vendas = a.Substring(48, 5),
                    is_valor_bruto = Convert.ToDecimal(a.Substring(53, 15)) / 100,

                    is_valor_gorjeta = Convert.ToDecimal(a.Substring(68, 15)) / 100,

                    is_valor_rejeitado = Convert.ToDecimal(a.Substring(83, 15)) / 100,
                    is_valor_desconto = Convert.ToDecimal(a.Substring(98, 15)) / 100,
                    is_valor_liquido = Convert.ToDecimal(a.Substring(113, 15)) / 100,

                    is_data_credito = Convert.ToDateTime(a.Substring(128, 2) + "/" + a.Substring(130, 2) + "/" + a.Substring(132, 4)),
                    is_bandeira = TabelaI(a.Substring(136, 1)),
                    rede = 1

                };

                io_arl_ro.Add(resumo);
                produto = "Crédito rotativo";
                banco = resumo.is_banco;
                data_pagamento = (a.Substring(128, 2) + "/" + a.Substring(130, 2) + "/" + a.Substring(132, 4));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void CVRotativo(string a)
        {
            try
            {
                /* 8  */
                var resumo = new ConciliacaoUseRedeEEVCComprovanteVendaStruct()
                {
                    is_tipo_registro = 8,
                    nm_tipo_registro = "CV - Rotativo",
                    is_numero_filiacao_pv = Convert.ToDecimal(a.Substring(3, 9)),
                    is_numero_resumo_vendas = Convert.ToInt64(string.IsNullOrEmpty(a.Substring(12, 9)) ? "0" : a.Substring(12, 9)),
                    is_data_cv = Convert.ToDateTime(a.Substring(21, 2) + "/" + a.Substring(23, 2) + "/" + a.Substring(25, 4)),
                    is_valor_bruto = Convert.ToDecimal(a.Substring(37, 15)) / 100,
                    is_valor_gorjeta = Convert.ToDecimal(a.Substring(52, 15)) / 100,
                    is_numero_cartao = a.Substring(67, 16),
                    is_status_cv = TabelaIII(Convert.ToInt32(a.Substring(83, 3))),
                    is_numero_cv = Convert.ToDecimal(a.Substring(86, 12)),
                    is_numero_referencia = a.Substring(98, 13),
                    is_valor_desconto = Convert.ToDecimal(a.Substring(111, 15)) / 100,
                    is_numero_autorizacao = a.Substring(126, 6),
                    is_hora_transcao = a.Substring(132, 2) + ":" + a.Substring(134, 2) + ":" + a.Substring(136, 2),
                    is_tipo_captura = TabelaII(Convert.ToInt32(a.Substring(202, 1))),
                    is_valor_liquido = Convert.ToDecimal(a.Substring(203, 15)) / 100,
                    is_numero_terminal = a.Substring(218, 8),
                    is_bandeira = TabelaI(a.Substring(229, 1)),
                    taxa_cobrada = Math.Abs(Convert.ToDecimal(((Convert.ToDecimal(a.Substring(203, 15)) / 100) * 100) / (Convert.ToDecimal(a.Substring(37, 15)) / 100)) - 100),
                    is_data_credito = Convert.ToDateTime(data_pagamento),
                    rede = 1
                };

                io_arl_cv.Add(resumo);



                
                estabelecimento = a.Substring(3, 9);
                if (io_hsm_totalizador_estabelecimento.Contains(estabelecimento + data_pagamento))
                {
                    TotalizadorEstabelecimento totalizador_estabelecimento = (TotalizadorEstabelecimento)io_hsm_totalizador_estabelecimento[estabelecimento + data_pagamento];
                    io_hsm_totalizador_estabelecimento.Remove(estabelecimento + data_pagamento);
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + data_pagamento, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = Convert.ToDateTime(data_pagamento),
                        total_realizado = totalizador_estabelecimento.total_realizado + (Convert.ToDecimal(a.Substring(203, 15)) / 100),
                        estabelecimento = Convert.ToInt32(estabelecimento)
                    });
                }
                else
                {
                    io_hsm_totalizador_estabelecimento.Add(estabelecimento + data_pagamento, new TotalizadorEstabelecimento
                    {
                        prev_pagamento = Convert.ToDateTime(data_pagamento),
                        total_realizado = (Convert.ToDecimal(a.Substring(203, 15)) / 100),
                        estabelecimento = Convert.ToInt32(estabelecimento)
                    });
                }

                if (io_hsm_totalizador_banco.Contains(banco + data_pagamento))
                {
                    TotalizadorBanco totalizador_banco = (TotalizadorBanco)io_hsm_totalizador_banco[banco + data_pagamento];
                    io_hsm_totalizador_banco.Remove(banco + data_pagamento);
                    io_hsm_totalizador_banco.Add(banco + data_pagamento, new TotalizadorBanco
                    {
                        data_prevista = Convert.ToDateTime(data_pagamento),
                        total_realizado = totalizador_banco.total_realizado + (Convert.ToDecimal(a.Substring(203, 15)) / 100),
                        id_banco = Convert.ToInt32(banco)
                    });
                }
                else
                {
                    io_hsm_totalizador_banco.Add(banco + data_pagamento, new TotalizadorBanco
                    {
                        data_prevista = Convert.ToDateTime(data_pagamento),
                        total_realizado = (Convert.ToDecimal(a.Substring(203, 15)) / 100),
                        id_banco = Convert.ToInt32(banco)
                    });
                }

                Decimal valor_total = 0;
                Decimal valor_total_liquido = 0;

                if (io_hsm_totalizador_produto.Contains(produto + data_pagamento))
                {
                    TotalizadorProduto totalizador_produto = (TotalizadorProduto)io_hsm_totalizador_produto[produto + data_pagamento];
                    valor_total = totalizador_produto.valor_bruto + (Convert.ToDecimal(a.Substring(37, 15)) / 100);
                    valor_total_liquido = totalizador_produto.valor_liquido + (Convert.ToDecimal(a.Substring(203, 15)) / 100);
                    io_hsm_totalizador_produto.Remove(produto + data_pagamento);
                    io_hsm_totalizador_produto.Add(produto + data_pagamento, new TotalizadorProduto
                    {
                        data_prevista = Convert.ToDateTime(data_pagamento),
                        valor_bruto = valor_total,
                        valor_liquido = valor_total_liquido,
                        ds_produto = produto,
                        rede = "Userede"
                    });
                }
                else
                {
                    io_hsm_totalizador_produto.Add(produto + data_pagamento, new TotalizadorProduto
                    {
                        data_prevista = Convert.ToDateTime(data_pagamento),
                        valor_bruto = (Convert.ToDecimal(a.Substring(37, 15)) / 100),
                        valor_liquido = (Convert.ToDecimal(a.Substring(203, 15)) / 100),
                        ds_produto = produto,
                        rede = "Userede"
                    });
                }



            }
            catch (Exception e)
            {
                throw e;
            }


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

    }
}
