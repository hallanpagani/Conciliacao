using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.conciliador
{
    public class TotaisDashboard
    {
        [Column("total_bruto")]
        public decimal total_bruto { get; set; }

        [Column("total_liquido")]
        public decimal total_liquido { get; set; }

        public TotaisDashboard()
        {
            total_bruto = 0;
            total_liquido = 0;
        }
    }
}