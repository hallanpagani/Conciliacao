using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.generico;


namespace ConciliacaoModelo.model.cadastros
{
    [Table("cadastro_van")]
    public class Vans : BaseUGrav
    {
        [Key]
        [AutoInc]
        [Required]
        [Column("id_van")]
        public long id_van { get; set; }

        [Column("ds_van")]
        [Required]
        [Display(Name = "Identificação da VAN")]
        public string identificacao_van { get; set; }

        [Column("ftp")]
        [Required]
        [Display(Name = "FTP da Rede")]
        public string rede_ftp { get; set; }

        [Column("porta")]
        [Required]
        [Display(Name = "Porta")]
        public string rede_porta { get; set; }

        [Column("usuario")]
        [Required]
        [Display(Name = "Usuário FTP")]
        public string usuario_rede { get; set; }

        [Column("senha")]
        [Required]
        [Display(Name = "Senha FTP")]
        public string senha_rede { get; set; }

        [Display(Name = "Rede")]
        [Required]
        [Column("id_rede")]
        public long id_rede { get; set; }

        [Display(Name = "Diretório base dos arquivos")]
        [Required]
        [Column("dir_base_arquivos_ftp")]
        public string dir_base_arquivos_ftp { get; set; }

        [OnlySelect]
        [Column("(select ds_rede from cadastro_rede r where r.id_rede = a.id_rede) as ds_rede")]
        public string ds_rede { get; set; }

        public Vans()
        {
            rede_porta = "0";
        }

    }
}
