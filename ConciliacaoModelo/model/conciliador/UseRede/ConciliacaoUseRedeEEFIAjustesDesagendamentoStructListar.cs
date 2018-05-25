using ConciliacaoModelo.model.generico;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.conciliador.UseRede
{
    [Table("conciliador_userede_eefi_ajustes_desagendamento")]
    public class ConciliacaoUseRedeEEFIAjustesDesagendamentoStructListar : BaseUGrav
    {
        [Column("id")]
        public long id { get; set; }
        
        [Column("numero_pv_ajustado")]
        public int numero_pv_ajustado { get; set; }

        [Column("numero_rv_ajustado")]
        public int numero_rv_ajustado { get; set; }

        [Column("data_ajuste")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_ajuste { get; set; }

        [Column("valor_ajuste")]
        public decimal valor_ajuste { get; set; }

        [Column("motivo_ajuste")]
        public string motivo_ajuste { get; set; }

        [Column("numero_cartao")]
        public string numero_cartao { get; set; }

        [Column("data_transacao")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_transacao { get; set; }

        [Column("numero_rv_original")]
        public int numero_rv_original { get; set; }

        [Column("numero_referencia_carta")]
        public string numero_referencia_carta { get; set; }

        [Column("data_carta")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_carta { get; set; }

        [Column("mes_referencia")]
        public int mes_referencia { get; set; }

        [Column("numero_pv_original")]
        public int numero_pv_original { get; set; }

        [Column("data_rv_original")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_rv_original { get; set; }

        [Column("valor_transacao")]
        public decimal valor_transacao { get; set; }

        [Column("identificador")]
        public string identificador { get; set; }

        [Column("data_credito")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_credito { get; set; }

        [Column("novo_valor_parcela")]
        public decimal novo_valor_parcela { get; set; }

        [Column("valor_original_parcela")]
        public decimal valor_original_parcela { get; set; }

        [Column("valor_bruto_resumo")]
        public decimal valor_bruto_resumo { get; set; }

        [Column("valor_cancelado")]
        public decimal valor_cancelado { get; set; }

        [Column("nsu")]
        public long nsu { get; set; }

        [Column("numero_autorizacao")]
        public string numero_autorizacao { get; set; }

        [Column("valor_debito_total")]
        public decimal valor_debito_total { get; set; }

        [Column("valor_pendente")]
        public decimal valor_pendente { get; set; }

        [Column("rede")]
        public int rede { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Data inicial em formato inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataInicio { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Data final em formato inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataFinal { get; set; }
    }
}
