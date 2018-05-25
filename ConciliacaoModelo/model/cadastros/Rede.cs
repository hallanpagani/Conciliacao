using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.generico;


namespace ConciliacaoModelo.model.cadastros
{
    [Table("cadastro_rede")]
    public class Rede : BaseUGrav
    {
        [Key]
        [AutoInc]
        [Required]
        [Column("id_rede")]
        [Display(Name = "Cód.")]
        public long id_rede { get; set; }

        [Column("ds_rede")]
        [Display(Name = "Descrição da rede")]
        public string Nome { get; set; }

        [Column("is_conciliacao_automatica")]
        [Display(Name = "Conciliação automática")]
        public bool HabilitaConciliacaoAutomatica { get; set; }

        public Rede()
        {
            HabilitaConciliacaoAutomatica = false;
        }

    }
}
