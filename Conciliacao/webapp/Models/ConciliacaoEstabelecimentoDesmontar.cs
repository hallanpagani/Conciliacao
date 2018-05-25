using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using ConciliacaoModelo.model.conciliador;
using RestSharp.Extensions;

namespace Conciliacao.Models
{
    public class ConciliacaoEstabelecimentoDesmontar
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
        Estruturas para desmontagem.
        */
        private List<TransacaoEstabelecimento> io_arl_est = new List<TransacaoEstabelecimento>();


        private IDictionary
            io_hsm_tabela_I = new Hashtable(),
            io_hsm_tabela_II = new Hashtable();


        /*
       Get List 
       */
        public List<TransacaoEstabelecimento> GetListTransacaoEstabelecimento()
        {
            return io_arl_est ?? new List<TransacaoEstabelecimento>();
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


        /*
        Construtor da classe.
        */
        public ConciliacaoEstabelecimentoDesmontar
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

            /*Inicializa as tabelas explicativas dos registros.
           */
            TabelasIniciar();


            /*
            Percorre toda a linha.
            */
            try
            {
                io_arl_est.Clear();
                while ((is_linha_atual = io_file.LerLinha(true)) != null)
                {
                    /*
                    Se linha não vazia.
                    */
                    if (is_linha_atual.Length > 0)
                    {
                        var a = is_linha_atual.Substring(0,1);
                        if ((a.Equals("0")) || (a.Equals("9"))) continue;
                        var t = new TransacaoEstabelecimento
                        {
                            dt_transacao = Convert.ToDateTime(FormatoDataExecutar(is_linha_atual.Substring(1, 8), "yyyyMMdd", "dd/MM/yyyy")),
                            nsu_rede = string.IsNullOrEmpty(is_linha_atual.Substring(9, 12).Trim()) ? "0" : is_linha_atual.Substring(9, 12).Trim(),
                            is_autorizacao = is_linha_atual.Substring(21, 12).Trim(),
                            tot_parcela = Convert.ToInt32(is_linha_atual.Substring(33, 2)),


                            operadora = is_linha_atual.Substring(35, 2),
                            bandeira = is_linha_atual.Substring(37, 2),
                            operadora_desc = (string)io_hsm_tabela_I[is_linha_atual.Substring(35, 2)],
                            bandeira_desc = (string)io_hsm_tabela_II[is_linha_atual.Substring(37, 2)],
                            produto = is_linha_atual.Substring(39, 1),
                            vl_bruto = FormatoValorExecutarDouble(is_linha_atual.Substring(40, 13)),
                            cod_loja = Convert.ToInt32(is_linha_atual.Substring(53, 6)),
                            nm_estabelecimento = is_linha_atual.Substring(59, 20).Trim(),
                            caixa = Convert.ToInt32(is_linha_atual.Substring(79, 6)),
                            nr_maquineta = is_linha_atual.Substring(85, 20).Trim(),
                            area_cliente = is_linha_atual.Substring(105, 50).Trim(),
                            reservado_cliente = is_linha_atual.Substring(155, 85).Trim()
                               
                            // nsu_tef = a[6] ,
                            // nr_logico = a[8]
                        };
                        io_arl_est.Add(t);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("Falha no processamento do arquivo: " + ex.Message);
                throw;
            }
        }

        private
            void TabelasIniciar()
        {
            /*
            Tabela I Operadoras.
            */
            io_hsm_tabela_I.Add("01", "Amex");
            io_hsm_tabela_I.Add("02", "Cielo");
            io_hsm_tabela_I.Add("03", "CrediShop");
            io_hsm_tabela_I.Add("04", "FortBrasil");
            io_hsm_tabela_I.Add("05", "HiperCard");
            io_hsm_tabela_I.Add("06", "RedeCard");
            io_hsm_tabela_I.Add("07", "Santander");
            io_hsm_tabela_I.Add("08", "GoodCard");
            io_hsm_tabela_I.Add("09", "LiberCard");
            io_hsm_tabela_I.Add("10", "Elavon");
            io_hsm_tabela_I.Add("11", "BaneseCard");
            io_hsm_tabela_I.Add("12", "Ticket");
            io_hsm_tabela_I.Add("13", "FirstData");
            io_hsm_tabela_I.Add("14", "Avista");
            io_hsm_tabela_I.Add("15", "TriCard");
            io_hsm_tabela_I.Add("16", "Cetelem");
            io_hsm_tabela_I.Add("17", "Sodexo");
            io_hsm_tabela_I.Add("18", "VR Benefícios");
            io_hsm_tabela_I.Add("19", "ProtegeCard");
            io_hsm_tabela_I.Add("20", "Vero");
            io_hsm_tabela_I.Add("21", "Losango");
            io_hsm_tabela_I.Add("22", "Dacasa");
            io_hsm_tabela_I.Add("23", "Policard");
            io_hsm_tabela_I.Add("24", "Valecard");
            io_hsm_tabela_I.Add("25", "Comprocard");
            io_hsm_tabela_I.Add("26", "Fancard");
            io_hsm_tabela_I.Add("27", "Ecxcard");
            io_hsm_tabela_I.Add("28", "Banescard");
            io_hsm_tabela_I.Add("29", "Cabal");
            io_hsm_tabela_I.Add("30", "Usecred");
            io_hsm_tabela_I.Add("32", "Planvale");
            io_hsm_tabela_I.Add("33", "Abrapetite");
            io_hsm_tabela_I.Add("34", "Senff");
            io_hsm_tabela_I.Add("35", "Stone");
            io_hsm_tabela_I.Add("37", "Bnb");
            io_hsm_tabela_I.Add("41", "Banricard");
            io_hsm_tabela_I.Add("42", "Greencard");
            io_hsm_tabela_I.Add("47", "Paypal");
            io_hsm_tabela_I.Add("63", "Nutricash");
            io_hsm_tabela_I.Add("65", "Verocheque");
            io_hsm_tabela_I.Add("66", "Bigcard");
            io_hsm_tabela_I.Add("68", "Unik");
            io_hsm_tabela_I.Add("69", "Mercadolivre");
            io_hsm_tabela_I.Add("70", "Mgcard");
            io_hsm_tabela_I.Add("71", "Coopelife");
            io_hsm_tabela_I.Add("72", "Brasilcard");
            io_hsm_tabela_I.Add("73", "Credsystem");
            io_hsm_tabela_I.Add("74", "Boa Alimentação");
            io_hsm_tabela_I.Add("77", "Valemais");
            io_hsm_tabela_I.Add("78", "Dmcard");
            
            /*
            Tabela II Bandeiras.
            */
            io_hsm_tabela_II.Add("01", "Visa");
            io_hsm_tabela_II.Add("02", "MasterCard");
            io_hsm_tabela_II.Add("03", "HiperCard");
            io_hsm_tabela_II.Add("04", "CrediShop");
            io_hsm_tabela_II.Add("05", "DinersClub");
            io_hsm_tabela_II.Add("06", "FortBrasil");
            io_hsm_tabela_II.Add("07", "Elo");
            io_hsm_tabela_II.Add("08", "Cabal");
            io_hsm_tabela_II.Add("09", "Amex");
            io_hsm_tabela_II.Add("10", "Agiplan");
            io_hsm_tabela_II.Add("11", "Aura");
            io_hsm_tabela_II.Add("12", "Avista");
            io_hsm_tabela_II.Add("13", "BaneseCard");
            io_hsm_tabela_II.Add("14", "Credsystem");
            io_hsm_tabela_II.Add("15", "CredZ");
            io_hsm_tabela_II.Add("16", "CUP");
            io_hsm_tabela_II.Add("17", "Discover");
            io_hsm_tabela_II.Add("18", "Esplanada");
            io_hsm_tabela_II.Add("19", "GoodCard");
            io_hsm_tabela_II.Add("20", "LiberCard");
            io_hsm_tabela_II.Add("21", "Sicredi");
            io_hsm_tabela_II.Add("22", "Sodexo");
            io_hsm_tabela_II.Add("23", "Sorocred");
            io_hsm_tabela_II.Add("24", "Ticket Restaurante");
            io_hsm_tabela_II.Add("25", "Ticket Car");
            io_hsm_tabela_II.Add("26", "TriCard");
            io_hsm_tabela_II.Add("27", "Diners");
            io_hsm_tabela_II.Add("28", "BanesCard");
            io_hsm_tabela_II.Add("29", "VR Benefícios");
            io_hsm_tabela_II.Add("30", "ProtegeCard");
            io_hsm_tabela_II.Add("33", "BanriCard");
            io_hsm_tabela_II.Add("34", "VerdeCard");
            io_hsm_tabela_II.Add("35", "BanriCompras");
            io_hsm_tabela_II.Add("36", "Losango");
            io_hsm_tabela_II.Add("37", "Dacasa");
            io_hsm_tabela_II.Add("38", "Policard");
            io_hsm_tabela_II.Add("39", "Comprocard");
            io_hsm_tabela_II.Add("41", "Ticket Alimentação");
            io_hsm_tabela_II.Add("42", "Fancard");
            io_hsm_tabela_II.Add("43", "Ecxcard");
            io_hsm_tabela_II.Add("44", "Valecard");
            io_hsm_tabela_II.Add("45", "Usecred");
            io_hsm_tabela_II.Add("46", "Planvale");
            io_hsm_tabela_II.Add("47", "Senff");
            io_hsm_tabela_II.Add("48", "Abrapetite");
            io_hsm_tabela_II.Add("49", "Bnb");
            io_hsm_tabela_II.Add("52", "Greencard");
            io_hsm_tabela_II.Add("53", "Jcb");
            io_hsm_tabela_II.Add("69", "Paypal");
            io_hsm_tabela_II.Add("76", "Nutricash");
            io_hsm_tabela_II.Add("78", "Verocheque");
            io_hsm_tabela_II.Add("79", "Bigcard");
            io_hsm_tabela_II.Add("81", "Unik");
            io_hsm_tabela_II.Add("84", "Mercado Livre");
            io_hsm_tabela_II.Add("85", "MgCard");
            io_hsm_tabela_II.Add("86", "Coopelife");
            io_hsm_tabela_II.Add("87", "Brasilcard");
            io_hsm_tabela_II.Add("88", "Boa Alimentação");
            io_hsm_tabela_II.Add("89", "Valemais");
            io_hsm_tabela_II.Add("95", "Dmcard");
                                    
        }                           
                                    
                                    
    }                               
                                    
                                    
                                    
                                    
}                                   
                                    