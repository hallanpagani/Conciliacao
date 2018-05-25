using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.generico;
using System.Collections.Generic;

namespace ConciliacaoModelo.model.cadastros
{
    [Table("cadastro_estabelecimento")]
    public class Estabelecimento : BaseUGrav
    {
        [Key]
        [AutoInc]
        [Required]
        [Column("id_estabelecimento")]
        public long IdEstabelecimento { get; set; }

        [Required]
        [Column("nm_estabelecimento")]
        [Display(Name = "Nome do estabelecimento")]
        public string nome { get; set; }

        [Column("ds_cnpj")]
        [Display(Name = "CNPJ")]
        public string cpfcnpj { get; set; }

        [Column("ds_fone1")]
        [Display(Name = "Fone comercial")]
        public string fone1 { get; set; }

        [Column("ds_fone2")]
        [Display(Name = "Fone comercial 2")]
        public string fone2 { get; set; }

        [Column("ds_cep")]
        [Display(Name = "CEP")]
        public string cep { get; set; }

        [Column("ds_logradouro")]
        [Display(Name = "Endereço")]
        public string logradouro { get; set; }

        [Column("nr_logradouro")]
        [Display(Name = "Número")]
        public int numero { get; set; }

        [Column("ds_complemento")]
        [Display(Name = "Complemento")]
        public string complemento { get; set; }

        [Column("ds_bairro")]
        [Display(Name = "Bairro")]
        public string bairro { get; set; }

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

        [Column("id_unidade")]
        [Display(Name = "Unidade")]
        [Range(0, int.MaxValue, ErrorMessage = "Informe um código de unidade válido!")]
        public long unidade { get; set; }

        [Column("nm_unidade")]
        [Display(Name = "Nome da unidade")]
        public string nmunidade { get; set; }

        [Column("is_sincroniza_tef")]
        [Display(Name = "Sicroniza Tef")]
        public bool is_sincroniza_tef { get; set; }

        public List<EstabelecimentoRedeListar> EstabelecimentoRedeListar { get; set; }
        public EstabelecimentoRede EstabelecimentoRede { get; set; }

        public Estabelecimento()
        {
            email = string.Empty;
            dt_inc = DateTime.Now;
            is_sincroniza_tef = true; 
            EstabelecimentoRedeListar = new List<EstabelecimentoRedeListar>();
            EstabelecimentoRede = new EstabelecimentoRede();
        }
    }
}