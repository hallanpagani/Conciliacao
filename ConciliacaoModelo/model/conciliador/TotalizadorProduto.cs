using ConciliacaoModelo.model.generico;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.conciliador
{
    [Table("totalizador_previsao")]
    public class TotalizadorProduto : BaseUGrav
    {
        [Key]
        [Required]
        [Column("id")]
        public long id { get; set; }

        [Required]
        [Column("ds_produto")]
        public string ds_produto { get; set; }

        [Column("total_bruto")]
        public decimal valor_bruto { get; set; }

        [Column("total_liquido")]
        public decimal valor_liquido { get; set; }

        [Column("data_prevista")]
        public DateTime data_prevista { get; set; }

        [Column("ds_rede")]
        public string rede { get; set; }

        public TotalizadorProduto()
        {
            ds_produto = "";
            valor_bruto = 0;
            valor_liquido = 0;
        }
    }
}
