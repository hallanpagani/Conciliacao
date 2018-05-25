using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;

namespace ConciliacaoModelo.model.adm
{
    [Table("sistema_conta")]
    public class Conta
    {
        [Key]
        [AutoInc]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("dh_inc")]
        public DateTime DhInc { get; set; }

        [Required]
        [Column("dt_ativacao")]
        public DateTime DtAtivacao { get; set; }

        [Required]
        [Column("ds_login")]
        public string DsLogin { get; set; }

        [Column("is_verificada")]
        public bool is_verificada { get; set; }

        public Conta()
        {
            DhInc = DateTime.Now;
            DtAtivacao = DateTime.Now;
            is_verificada = true;
        }
    }
}
