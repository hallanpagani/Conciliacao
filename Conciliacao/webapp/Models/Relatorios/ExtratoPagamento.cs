using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Conciliacao.Models.Relatorios
{

    public class ExtratoPagamento
    {

        public int Tipo_registro { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Data_venda { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Data_credito { get; set; }


        public long NSU { get; set; }


        public string Autorizacao { get; set; }


        public int Plano { get; set; }


        public int Parcela { get; set; }


        public int Rede { get; set; }


        public string RedeNome { get; set; }


        public int Bandeira { get; set; }


        public string BandeiraNome { get; set; }


        public string Produto { get; set; }


        public decimal Valor_bruto { get; set; }


        public decimal Valor_administracao { get; set; }


        public decimal Valor_antecipacao  { get; set; }


        public decimal Numero_lote { get; set; }

        public string Numero_lote_string { get { return Numero_lote.ToString(); } }


        public string Area_cliente { get; set; }


        public int Banco { get; set; }


        public int Agencia { get; set; }


        public int Conta { get; set; }


        public int Cod_loja { get; set; }


        public string Loja { get; set; }


        public string Cod_Estabelecimento { get; set; }


        public string Status { get; set; }


        public decimal Valor_liquido { get; set; }


        public int Cod_loja_2 { get; set; }


        public string Reservado { get; set; }

    }
}