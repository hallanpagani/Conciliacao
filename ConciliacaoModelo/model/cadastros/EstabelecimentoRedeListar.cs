using ConciliacaoModelo.classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.cadastros
{
    [Table("cadastro_estabelecimento_rede")]
    public class EstabelecimentoRedeListar 
    {

        [Key]
        [AutoInc]
        [Required]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("id_estabelecimento")]
        public long IdEstabelecimento { get; set; }
        
        [Required]
        [Column("id_estabelecimento_rede")]
        [Display(Name = "Id.Estabelecimento")]
        public string IdEstabelecimentoRede { get; set; }

        [Column("codigo_estabelecimento")]
        [Display(Name = "Código.Estabelecimento")]
        public long CodigoEstabelecimento { get; set; }

        [Column("nome_estabelecimento")]
        [Display(Name = "Nome.Estabelecimento")]
        public string NomeEstabelecimento { get; set; }

        [Required]
        [Column("id_rede")]
        [Display(Name = "Rede")]
        public long id_rede { get; set; }

        [OnlySelect]
        [Column("(select ds_rede from cadastro_rede r where r.id_rede = a.id_rede) as ds_rede")]
        [Display(Name = "Nome da Rede")]
        public string ds_rede { get; set; }

        [Required]
        [Column("id_van")]
        [Display(Name = "VAN")]
        public long id_van { get; set; }

        [OnlySelect]
        [Column("(select ds_van from cadastro_van v where v.id_van = a.id_van) as ds_van")]
        [Display(Name = "Nome da VAN")]
        public string ds_van { get; set; }
    }
}
