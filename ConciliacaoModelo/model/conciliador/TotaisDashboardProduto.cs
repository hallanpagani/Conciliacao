using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.generico;

namespace ConciliacaoModelo.model.conciliador
{
    [Table("totalizador_previsao")]
    public class TotaisDashboardProduto 
    {
        [Column("ds_produto")]
        public string ds_produto { get; set; }
        
        [Column("total_bruto")]
        public decimal total_bruto { get; set; }
        
        [Column("total_liquido")]
        public decimal total_liquido { get; set; }

        public decimal valor_taxa
        {
            get
            {
                return 100-((this.total_liquido * 100) / this.total_bruto);
            }
        }

        public TotaisDashboardProduto()
        {
            ds_produto = "";
            total_bruto = 0;
            total_liquido = 0;
        }
    }
}
