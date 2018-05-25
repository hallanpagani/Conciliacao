using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.generico;

namespace ConciliacaoModelo.model.adm
{
    [Table("sistema_usuario")]
    public class Usuario: BaseID
    {
        [Key]
        [AutoInc]
        [Required]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [EmailAddress]
        [Column("ds_login")]
        public string Email { get; set; }

        [Required]
        [Column("nm_usuario")]
        public string NomeDoUsuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Column("ds_senha")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string PasswordConfirmacao { get; set; }

        [Column("ds_perfil")]
        public string perfil { get; set; }

        [OnlySelect]
        [Column("spr_usuario")]
        public bool sprusuario { get; set; }

        [OnlySelect]
        [Column("(select ds_login from sistema_conta r where r.id = a.id_conta) as ds_conta")]
        [Display(Name = "Nome da conta")]
        public string ds_conta { get; set; }

        [OnlySelect]
        [Column("(select ds_chave from sistema_conta r where r.id = a.id_conta) as ds_chave")]
        [Display(Name = "Chave da conta")]
        public string ds_chave { get; set; }

    }
}