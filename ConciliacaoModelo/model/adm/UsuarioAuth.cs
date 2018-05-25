using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;

namespace ConciliacaoModelo.model.adm
{
    [Table("sistema_usuario")]
    public class UsuarioAuth 
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

        [Required]
        [Column("id_conta")]
        public long IdConta { get; set; }

        [Column("ds_perfil")]
        public string perfil { get; set; }

    }
}