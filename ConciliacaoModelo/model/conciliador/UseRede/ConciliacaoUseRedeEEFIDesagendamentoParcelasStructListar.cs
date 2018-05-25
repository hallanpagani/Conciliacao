using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConciliacaoModelo.model.generico;

namespace ConciliacaoModelo.model.conciliador.UseRede
{
    [Table("conciliador_userede_eefi_desagendamento_parcelas")]
    public class ConciliacaoUseRedeEEFIDesagendamentoParcelasStructListar
    {
        [Column("id")]
        public long id { get; set; }

        [Column("numero_pv_ajustado")]
        public int numero_pv_ajustado { get; set; }

        [Column("numero_rv_ajustado")]
        public int numero_rv_ajustado { get; set; }

        [Column("data_credito")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_credito { get; set; }

        [Column("novo_valor_pacela")]
        public Decimal novo_valor_pacela { get; set; }

        [Column("valor_original_pacela")]
        public Decimal valor_original_pacela { get; set; }

        [Column("valor_ajuste")]
        public Decimal valor_ajuste { get; set; }

        [Column("data_cancelamento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_cancelamento { get; set; }

        [Column("valor_rv_original")]
        public Decimal valor_rv_original { get; set; }

        [Column("valor_cancelamento_solicitado")]
        public Decimal valor_cancelamento_solicitado { get; set; }

        [Column("numero_cartao")]
        public string numero_cartao { get; set; }

        [Column("data_transacao")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_transacao { get; set; }

        [Column("nsu")]
        public string nsu { get; set; }

        [Column("numero_pacela")]
        public int numero_pacela { get; set; }

        [Column("bandeira_rv")]
        public string bandeira_rv { get; set; }

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
