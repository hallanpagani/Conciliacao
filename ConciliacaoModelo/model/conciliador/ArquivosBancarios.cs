using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ConciliacaoModelo.model.conciliador
{
    public class ArquivosBancarios
    {
        [Column("tp_arquivo")]
        [Display(Name = "Descrição")]
        public string tp_arquivo { get; set; }

        [Column("ds_arquivo")]
        [Display(Name = "Arquivo processado")]
        public string ds_arquivo { get; set; }
        
        [Display(Name = "Dt.Inicio")]
        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]
        public DateTime datainicio { get; set; }

        [Display(Name = "Dt.Final")]
        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]
        public DateTime datafinal { get; set; }

        public HttpPostedFileBase arquivo { get; set; }

        public List<TransacaoBancaria> ArquivoBancario { get; set; }
    }
}
