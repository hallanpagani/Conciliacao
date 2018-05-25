using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;

namespace ConciliacaoModelo.model.cadastros
{
    [Table("cadastro_estabelecimento")]
    public class EstabelecimentoListar
    {

        [Key]
        [AutoInc]
        [Required]
        [Column("id_estabelecimento")]
        public long id { get; set; }

        [Required]
        [Column("nm_estabelecimento")]
        [Display(Name = "Nome do cliente")]
        public string nome { get; set; }

        [Column("ds_cnpj")]
        [Display(Name = "CPF ou CNPJ")]
        public string cpfcnpj { get; set; }

        [Column("ds_fone1")]
        [Display(Name = "Fone residencial")]
        public string fone1 { get; set; }

        [Column("ds_fone2")]
        [Display(Name = "Fone celular")]
        public string fone2 { get; set; }

        [Column("ds_cidade")]
        [Display(Name = "Cidade")]
        public string cidade { get; set; }

        [Column("ds_estado")]
        [Display(Name = "Estado")]
        public string estado { get; set; }

        [Column("ds_email")]
        [Display(Name = "Email")]
        public string email { get; set; }

        [DataType(DataType.Date)]
        [Column("dt_inc")]
        [Display(Name = "Data do cadastro")]
        public DateTime dt_inc { get; set; }

        public EstabelecimentoListar()
        {
            email = string.Empty;
        }
    }
}