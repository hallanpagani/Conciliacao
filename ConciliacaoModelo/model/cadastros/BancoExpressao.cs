using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.generico;


namespace ConciliacaoModelo.model.cadastros
{
    [Table("cadastro_banco_expressoes")]
    public class BancoExpressao
    {
        [Key]
        [AutoInc]
        [Required]
        [Column("id")]
        public long id { get; set; }

        [Column("nm_expressao")]
        [Required]
        [Display(Name = "Expressao")]
        public string nm_expressao { get; set; }

        [Column("bandeira")]
        [Required]
        [Display(Name = "Bandeira")]
        public string bandeira { get; set; }

        [Column("tp_credito_debito")]
        [Required]
        [Display(Name = "Crédito ou Débito")]
        public string tp_credito_debito { get; set; }

        public BancoExpressao()
        {
            nm_expressao = "";
            bandeira = "";
            tp_credito_debito = "C";
        }
    }
}
