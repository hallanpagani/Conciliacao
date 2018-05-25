using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.conciliador
{
    public class TotaisDashboardBanco
    {

        [Column("id_banco")]
        public int banco { get; set; }
        
        [Column("total_bruto")]
        public decimal total_bruto { get; set; }

        [Column("total_pendente")]
        public decimal total_pendente { get; set; }

        [Column("total_hoje")]
        public decimal total_hoje { get; set; }

        public TotaisDashboardBanco()
        {
            banco = 0;
            total_bruto = 0;
            total_pendente = 0;
            total_hoje = 0;
        }
    }
}