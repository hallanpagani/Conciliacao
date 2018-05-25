using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ConciliacaoModelo.model.conciliador
{
    public class ArquivosTEF
    {
        [Column("tp_arquivo")]
        [Display(Name = "Descrição")]
        public string tp_arquivo { get; set; }

        [Column("ds_arquivo")]
        [Display(Name = "Arquivo processado")]
        public string ds_arquivo { get; set; }
        
        [Display(Name = "Dt.Inicio")]
        public DateTime dt_inicio { get; set; }

        [Display(Name = "Dt.Final")]
        public DateTime dt_final { get; set; }

        public HttpPostedFileBase arquivo { get; set; }

        public List<TransacaoTEFListar> ArquivoTEF { get; set; }

        public ArquivosTEF()
        {
            
        }
    }
}
