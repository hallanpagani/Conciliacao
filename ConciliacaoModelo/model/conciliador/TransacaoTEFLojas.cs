using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConciliacaoModelo.model.conciliador
{
    public class TransacaoTEFLojas
    {
        [Column("loja")]
        public string loja { get; set; }
    }
}
