﻿using Conciliacao.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Conciliacao.Models
{
    public class ConciliacaoArquivoManipular 
    {
        /**
	    Arquivo. 
	    */
        private StreamReader _file;

        /**
        Indica o final do arquivo.
        */
        private int in_end_of_file = -1;

        /*======================================================================

        Construtores

        ======================================================================*/

        /**
        Cria um arquivo sem nome.
        */
        public ConciliacaoArquivoManipular(StreamReader file)
        {
            _file = file;
        }

        /*======================================================================
    
    	Métodos de manipulação.
    
    	======================================================================*/

        /**
    	Faz a leitura linha por linha
	    */
        public string LerLinha()
        {
            /*
            Carrega o resultado.
            */
            if (!_file.EndOfStream)
            {
                return _file.ReadLine();

            }
            return (null);
        }

        /**
    	Faz a leitura linha por linha
	    */
        public
        string LerLinha(Boolean ab)
        {
            /*
            Carrega o resultado.
            */
            string
            ls_line = null;

            try
            {
                /*
                Se leu alguma coisa ...
                */
                if (
                        in_end_of_file < 0
                        &&
                        (ls_line = _file.ReadLine()
                        ) != null
                            &&
                            (in_end_of_file = ls_line.IndexOf("0x1A")
                            ) != 0
                    )
                {
                    /*
                    Se não for final do arquivo ...
                    */
                    if (
                            in_end_of_file > 0
                        )
                    {
                        /*
                        Alimenta a linha.
                        */
                        ls_line = ls_line.Substring(0, in_end_of_file);
                    }
                }
            }

            catch (IOException ex)
            {

            }

            /*
            Retorna a linha lida.
            */
            return (ls_line);
        }

        /**
       Set a position into the file for next read/write operation.
       @param	an_pointer
           New file pointer's position.
       */
        public
        void Seek
                    (
                        long an_pointer
                    )
        {
            try
            {
                //
                // Setting file pointer's position ...
                //
                _file.BaseStream.Seek(an_pointer, SeekOrigin.Begin);

                //
                // Reset end-of-file ...
                //
                in_end_of_file = -1;
            }

            catch (IOException ex)
            {

            }
        }

        /**
        Verifica se pode processar o arquivo Cielo.
        */
        public bool ProcessarArquivoCielo(string linha)
        {
            /// LAYOUT CIELO TIPO DE OPCAO DE EXTRATO
            int extrato = Convert.ToInt32(linha.Substring(47, 02).Trim() == "" ? "0" : linha.Substring(47, 02).Trim());

            return extrato == 1 || extrato == 3 ||  extrato == 4;

        }

        public bool ProcessarArquivoBanese(string linha)
        {
            /// LAYOUT CIELO TIPO DE OPCAO DE EXTRATO
            int extrato = Convert.ToInt32(linha.Substring(47, 02).Trim() == "" ? "0" : linha.Substring(47, 02).Trim());

            return extrato == 1 || extrato == 3 || extrato == 4;

        }


    }
}
