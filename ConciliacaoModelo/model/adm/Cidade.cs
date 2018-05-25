using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.adm
{
    [Table("cadastro_cidade")]
    public class Cidade
    {
        [Key]
        [Column("id")]
        public int idCidade { get; set; }

        [Column("nm_cidade")]
        public string NmCidade { get; set; }

    }
}
