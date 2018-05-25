using System;
using System.Collections.Generic;
using System.Globalization;
using ConciliacaoModelo.model.conciliador;

namespace Conciliacao.Models
{
    public class ConciliacaoTEFDesmontar
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
        private List<TransacoesTEF> io_arl_tef = new List<TransacoesTEF>();


        /*
       Get List 
       */
        public List<TransacoesTEF> GetListTransacaoTEF()
        {
            return io_arl_tef ?? new List<TransacoesTEF>();
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
            DateTime data_arquivo = DateTime.ParseExact(as_texto.Substring(0,10), as_mascara, CultureInfo.InvariantCulture);
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
            try { 
                as_texto = as_texto.Replace(",", "").Replace(".", "");

                if (as_texto.Equals("NULL")) {
                    return 0;
                }
                

                String MilharCentena = "";
                String Centavos = "";
                if (as_texto.Length < 2) {
                    MilharCentena = as_texto;
                    Centavos = "00";
                } else { 
                    MilharCentena = as_texto.Substring(0, as_texto.Length - 2);
                    Centavos = as_texto.Substring(MilharCentena.Length, 2);
                }
                
                String aux = MilharCentena + "," + Centavos;
                Decimal Valor = Convert.ToDecimal(aux);
                return Valor;
            }
            catch (Exception ex)
            {
                Console.Write("Falha no processamento do arquivo: " + ex.Message);
                throw;
            }
        }


        /*
        Construtor da classe.
        */
        public ConciliacaoTEFDesmontar
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
                io_arl_tef.Clear();
                while ((is_linha_atual = io_file.LerLinha(true)) != null)
                {
                    /*
                    Se linha não vazia.
                    */
                    if (is_linha_atual.Length > 0)
                    {
                        var a = is_linha_atual.Split(',');
                        try
                        {
                            var tra = new TransacoesTEF(); 
                            tra.sequencial = a[0];
                            tra.data_atual = Convert.ToDateTime(a[1]);
                            tra.estabelecimento = a[2];
                            tra.loja = a[3];
                            tra.terminal = a[4];
                            tra.terminal_validade = Convert.ToDateTime(a[5]);
                            tra.rede = a[6];
                            tra.tipo_cartao = a[7];
                            tra.administrador = a[8];
                            tra.tipo_transacao = a[9];
                            tra.produto = a[10];
                            tra.cartao_bin = Convert.ToInt32(a[11]);
                            tra.cartao_numero = Convert.ToInt64(a[12]);
                            tra.cartao_validade = Convert.ToDateTime(a[13]);
                            tra.cartao_entrada = a[14];
                            tra.transacao_inicio = Convert.ToDateTime(a[15]);
                            tra.transacao_fim = Convert.ToDateTime(a[16]);
                            tra.transacao_conclusao = Convert.ToDateTime(a[17]); 
                            tra.transacao_pagamento = a[18];
                            tra.transacao_financiado = a[19];
                            tra.erro = a[20];
                            tra.transacao_identificacao = a[21];
                            tra.transacao_nsu = Convert.ToInt64(a[22]);
                            tra.transacao_nsu_rede = Convert.ToInt64(a[23]);
                            tra.transacao_valor = FormatoValorExecutarDouble(a[24]);
                            tra.transacao_parcela = Convert.ToInt32(a[25]);
                            tra.transacao_autorizacao = a[26];
                            tra.transacao_resposta = a[27];
                            tra.transacao_situacao = a[28];
                            io_arl_tef.Add(tra);
                        }
                        catch (Exception e)
                        {
                            throw e;
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
    }
}