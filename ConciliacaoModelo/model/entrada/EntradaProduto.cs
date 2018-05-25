using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;

namespace ConciliacaoModelo.model.entrada
{
    [Table("tb_entrada_produto")]
    public class EntradaProduto
    {
        [Key]
        [AutoInc]
        [Required]
        [Column("id_entrada_produto")]
        public int Id { get; set; }

        [ForeignKey("id_entrada")]
        [Required]
        [Column("id_entrada")]
        public int IdEntrada { get; set; }

        [Required]
        [Column("id_produto")]
        public int IdProduto { get; set; }

        [Required]
        [Column("qtd")]
        public int Qtd { get; set; }

        [Required]
        [Column("vl_custo")]
        public decimal Custo { get; set; }

        [Required]
        [Column("vl_venda")]
        public decimal Venda { get; set; }

        public EntradaProduto()
        {
        }
    }
}