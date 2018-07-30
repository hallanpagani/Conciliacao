using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.conciliador.UseRede
{
    [Table("conciliador_userede_eefi_tot_creditos")]
    public class ConciliacaoUseRedeEEFITotCreditosStruct
    {
        [Column("id")]
        public long id { get; set; }

        [Column("numero_pv")]
        public int numero_pv { get; set; }

        [Column("data_credito")]
        public DateTime data_credito { get; set; }

        [Column("valor_total_credito")]
        public decimal valor_total_credito { get; set; }

        [Column("banco")]
        public int banco { get; set; }

        [Column("agencia")]
        public int agencia { get; set; }

        [Column("conta_corrente")]
        public string conta { get; set; }

        [Column("data_arquivo")]
        public DateTime data_arquivo { get; set; }

        [Column("data_credito_antecipado")]
        public DateTime data_credito_antecipado { get; set; }

        [Column("valor_total_antecipado")]
        public decimal valor_total_antecipado { get; set; }

        [Column("rede")]
        public int rede { get; set; }

        
    }
}
