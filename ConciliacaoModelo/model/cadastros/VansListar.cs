using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;

namespace ConciliacaoModelo.model.cadastros
{
    [Table("cadastro_van")]
    public class VansListar 
    {
        [Key]
        [AutoInc]
        [Required]
        [Column("id_van")]
        public long id_van { get; set; }

        [Column("ds_van")]
        [Display(Name = "Identificação")]
        public string identificacao_van { get; set; }

        [Column("ftp")]
        [Display(Name = "FTP da Rede")]
        public string rede_ftp { get; set; }

        [Column("porta")]
        [Display(Name = "Porta do FTP")]
        public string rede_porta { get; set; }

        [Column("usuario")]
        [Display(Name = "Usuário FTP")]
        public string usuario_rede { get; set; }

        [Column("senha")]
        [Display(Name = "Senha FTP")]
        public string senha_rede { get; set; }

        [Column("id_rede")]
        public long id_rede { get; set; }

        [Column("dir_base_arquivos_ftp")]
        public string dir_base_arquivos_ftp { get; set; }

        [Column("(select ds_rede from cadastro_rede r where r.id_rede = a.id_rede) as ds_rede")]
        [Display(Name = "Rede")]
        public string ds_rede { get; set; }

        public VansListar()
        {
            rede_porta = "0";
        }

    }
}
