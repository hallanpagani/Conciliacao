using ConciliacaoModelo.model.generico;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.conciliador
{
    [Table("totalizador_banco")]
    public class TotalizadorBanco : BaseUGrav
    {
        [Key]
        [Required]
        [Column("id")]
        public long id { get; set; }

        [Column("id_banco")]
        public int id_banco { get; set; }

        [Column("total")]
        public decimal total_realizado { get; set; }

        [Column("data_prevista")]
        public DateTime data_prevista { get; set; }

        public TotalizadorBanco()
        {
            id_banco = 0;
            total_realizado = 0;
        }
    }
}
