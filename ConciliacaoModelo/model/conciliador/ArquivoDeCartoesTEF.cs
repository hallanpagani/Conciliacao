using Conciliacao.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ConciliacaoModelo.model.conciliador
{
    public class ArquivoDeCartoesTEF
    {
        public int? filtro_tef { get; set; }
        public string filtro_nm_tef { get; set; }

        public int? filtro_rede { get; set; }
        public string filtro_nm_rede { get; set; }

        public int? filtro_estabelecimento { get; set; }
        public string filtro_nm_estabelecimento { get; set; }

        public int? filtro_administrador { get; set; }
        public string filtro_nm_administrador { get; set; }

        public string filtro_nm_loja { get; set; }

        public string filtro_tp_operacao { get; set; }

        public string filtro_tp_transacao { get; set; }

        public decimal filtro_valor { get; set; }
        public string filtro_resumo { get; set; }

        public int tp_situacao { get; set; }

        public int tp_data { get; set; } // 0 - Emissao

        public string tp_administradora { get; set; } 

        [DataType(DataType.Date, ErrorMessage = "Data inicial em formato inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataInicio { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Data final em formato inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataFinal { get; set; }

        [Column("tp_arquivo")]
        [Display(Name = "Descrição")]
        public string tp_arquivo { get; set; }

        [Column("ds_arquivo")]
        [Display(Name = "Arquivo processado")]
        public string ds_arquivo { get; set; }

        public HttpPostedFileBase arquivo { get; set; }

        public List<TransacaoTEFListar> ArquivosTEf { get; set; }

        public List<TransacoesTEF> TEF { get; set; }

    }
}
