using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConciliacaoModelo.model.conciliador
{
    public class ArquivoTransacao
    {
        public string registro { get; set; }
        public string estabelecimento { get; set; }
        public string num_resumo { get; set; }
        public string num_logico { get; set; }
        public string dt_venda { get; set; }
        public string cartao { get; set; }
        public string vl_venda { get; set; }
        public string parcela { get; set; }
        public string total_parcela { get; set; }
        public string autorizacao { get; set; }
        public string nsu { get; set; }
    }
}
