using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.conciliador
{
    public class TotaisDashboardPorData
    {
       // [Column("tipo")]
       // public string tipo { get; set; }

        [Column("rede")]
        public string rede { get; set; }

        [Column("data_prevista")]
        public DateTime data_prevista { get; set; }

        [Column("total_bruto")]
        public decimal total_bruto { get; set; }

        [Column("total_liquido")]
        public decimal total_liquido { get; set; }

        public TotaisDashboardPorData()
        {
            total_bruto = 0;
            total_liquido = 0;
        }

    }
}
