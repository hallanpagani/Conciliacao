using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConciliacaoModelo.model.conciliador
{
    public class ArquivoDetalhado
    {

        public string registro { get; set; }
        public string estabelecimento { get; set; }
        public string RO { get; set; }
        public string transacao { get; set; }
        public string produto { get; set; }
        public string cartao { get; set; }
        public string vl_bruto { get; set; }
        public string vl_liquido { get; set; }
        public string parcela { get; set; }
        public string total_parcela { get; set; }
        public string autorizacao { get; set; }
        public string nsu { get; set; }
    }
}
