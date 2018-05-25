using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConciliacaoModelo.model.conciliador
{
    public class ArquivoAntecipado
    {
        public string registro { get; set; }
        public string estabelecimento { get; set; }

        public string num_resumo { get; set; }
        public string dt_credito { get; set; }
        public string sinal_valor_bruto { get; set; }
        public string valor_bruto  { get; set; }
        public string sinal_valor_bruto_parcelado { get; set; }
        public string valor_bruto_parcelado { get; set; }
        public string sinal_valor_bruto_predatado { get; set; }
        public string valor_bruto_predatado { get; set; }
        public string sinal_valor_bruto_total { get; set; }
        public string valor_bruto_total { get; set; }

        public string sinal_valor_liquido { get; set; }
        public string valor_liquido { get; set; }
        public string sinal_valor_liquido_parcelado { get; set; }
        public string valor_liquido_parcelado { get; set; }
        public string sinal_valor_liquido_predatado { get; set; }
        public string valor_liquido_predatado { get; set; }
        public string sinal_valor_liquido_total { get; set; }
        public string valor_liquido_total { get; set; }

        public string taxa_desconto  { get; set; }

        public string banco { get; set; }
        public string agencia { get; set; }
        public string conta_corrente { get; set; }
        
        public string        sinal_valor_liquido_antecipacao { get; set; }
        public string valor_liquido_antecipacao { get; set; }


    }
}
