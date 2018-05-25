using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.model.generico;

namespace ConciliacaoModelo.model.conciliador.UseRede
{
    [Table("conciliador_userede_eefi_antecipacao")]
    public class ConciliacaoUseRedeEEFIAntecipacaoStructListar
    {
        [Column("id")]
        public long id { get; set; }

        [Column("numero_pv")]
        public int numero_pv { get; set; }

        [Column("numero_documento")]
        public string numero_documento { get; set; }

        [Column("data_lancamento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_lancamento { get; set; }

        [Column("valor_lancamento")]
        public decimal valor_lancamento { get; set; }

        [Column("banco")]
        public int banco { get; set; }

        [Column("agencia")]
        public string agencia { get; set; }

        [Column("conta_corrente")]
        public string conta_corrente { get; set; }

        [Column("numero_rv_correspondente")]
        public int numero_rv_correspondente { get; set; }

        [Column("data_rv_correspondente")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_rv_correspondente { get; set; }

        [Column("valor_credito_original")]
        public decimal valor_credito_original { get; set; }

        [Column("data_vencimento_original")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_vencimento_original { get; set; }

        [Column("numero_parcela")]
        public string numero_parcela { get; set; }

        [Column("valor_bruto")]
        public decimal valor_bruto { get; set; }

        [Column("valor_taxa_desconto")]
        public decimal valor_taxa_desconto { get; set; }

        [Column("numero_pv_original")]
        public string numero_pv_original { get; set; }

        [Column("bandeira")]
        public string bandeira { get; set; }

        [Column("rede")]
        public int rede { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Data inicial em formato inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataInicio { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Data final em formato inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataFinal { get; set; }

        public ConciliacaoUseRedeEEFIAntecipacaoStructListar()
        {
            valor_lancamento = 0;
        }

    }
}
