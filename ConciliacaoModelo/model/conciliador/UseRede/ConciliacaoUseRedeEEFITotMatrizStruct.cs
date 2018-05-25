using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.conciliador.UseRede
{
    [Table("conciliador_userede_eefi_totalizador_matriz")]
    public class ConciliacaoUseRedeEEFITotMatrizStruct 
    {
        [Column("id")]
        public long id { get; set; }

        [Column("numero_pv")]
        public int numero_pv { get; set; }

        [Column("quantidade_total_resumos")]
        public int quantidade_total_resumos { get; set; }

        [Column("valor_total_creditos_normais")]
        public decimal valor_total_creditos_normais { get; set; }

        [Column("quantidade_creditos_antecipados")]
        public int quantidade_creditos_antecipados { get; set; }

        [Column("valor_total_antecipado")]
        public decimal valor_total_antecipado { get; set; }

        [Column("quantidade_ajustes_credito")]
        public int quantidade_ajustes_credito { get; set; }

        [Column("valor_total_ajustes_credito")]
        public decimal valor_total_ajustes_credito { get; set; }

        [Column("quantidade_ajustes_debito")]
        public int quantidade_ajustes_debito { get; set; }

        [Column("valor_total_ajustes_debito")]
        public decimal valor_total_ajustes_debito { get; set; }

        [Column("rede")]
        public int rede { get; set; }
    }
}
