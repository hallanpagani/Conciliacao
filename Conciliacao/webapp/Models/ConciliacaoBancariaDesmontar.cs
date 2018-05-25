using System;
using System.Collections.Generic;
using System.Globalization;
using ConciliacaoModelo.model.conciliador;
using RestSharp.Extensions;

namespace Conciliacao.Models
{
    public class ConciliacaoBancariaDesmontar
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
        private List<TransacaoBancaria> io_arl_ban = new List<TransacaoBancaria>();


        /*
       Get List 
       */
        public List<TransacaoBancaria> GetListTransacaoBancaria()
        {
            return io_arl_ban ?? new List<TransacaoBancaria>();
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
        public ConciliacaoBancariaDesmontar
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
                    is_linha_atual = is_linha_atual.Replace("\"","");

                    /*
                    Se linha não vazia.
                    */
                    if (is_linha_atual.Length > 0)
                    {
                        var a = is_linha_atual.Split(';');
                        var t = new TransacaoBancaria
                        {
                            conta = a[0],
                            dt_mvto = Convert.ToDateTime(FormatoDataExecutar(a[1], "yyyyMMdd", "dd/MM/yyyy")),
                            nr_doc = a[2],
                            ds_historico = a[3],
                            vl_mvto = FormatoValorExecutarDouble(a[4]),
                            tp_mvto = a[5]
                        };
                        io_arl_ban.Add(t);
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