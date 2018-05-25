using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.generico;

namespace ConciliacaoModelo.model.cadastros
{
    [Table("cadastro_banco_tef")]
    public class BancoTEF : BaseUGrav
    {
        [Key]
        [AutoInc]
        [Required]
        [Column("id_tef")]
        public long id_tef { get; set; }

        [Column("ds_tef")]
        [Required]
        [Display(Name = "Identificação do TEF")]
        public string identificacao_tef { get; set; }

        [Column("ip_banco")]
        [Required]
        [Display(Name = "IP Banco")]
        public string ip_banco { get; set; }

        [Column("porta")]
        [Required]
        [Display(Name = "Porta")]
        public string porta_banco { get; set; }

        [Column("usuario")]
        [Required]
        [Display(Name = "Usuário Banco")]
        public string usuario_banco { get; set; }

        [Column("senha")]
        [Required]
        [Display(Name = "Senha Banco")]
        public string senha_banco { get; set; }

        [Display(Name = "SID Banco")]
        [Required]
        [Column("sid_banco")]
        public string sid_banco { get; set; }

        public BancoTEF()
        {
            porta_banco = "0";
        }

    }
}
