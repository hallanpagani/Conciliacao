using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConciliacaoModelo.model.conciliador
{
    public class ArquivoCompleto
    {
        public string registro { get; set; }
        public string estabelecimento { get; set; }
        public string RO { get; set; }
        public string tipo_transacao { get; set; }
        public string produto { get; set; }
        public string parcela { get; set; }
        public string total_parcela { get; set; }
        public string filer { get; set; }
        public string plano { get; set; }
        
        public string apresentacao { get; set; }
        public string prev_pagamento { get; set; }
        public string envio_banco { get; set; }
        public string cartao { get; set; }
        public string sinal_valor_bruto { get; set; }
        public string valor_bruto { get; set; }

        public string sinal_comissao { get; set; }
        public string comissao { get; set; }
        public string sinal_rejeitado { get; set; }
        public string valor_rejeitado { get; set; }
        public string sinal_liquido { get; set; }
        public string valor_liquido { get; set; }
        public string valor_total_venda { get; set; }
        public string valor_prox_parcela { get; set; }
        public string taxas { get; set; }
        public string autorizacao { get; set; }
        public string nsu_doc { get; set; }
        public string banco { get; set; }
        public string agencia { get; set; }
        public string conta_corrente { get; set; }
        public string status_pagamento { get; set; }

        public string motivo_rejeicao { get; set; }
        public string cvs_aceitos { get; set; } // ver
        
        public string cvs_rejeitados { get; set; } // ver
        public string data_venda { get; set; }

        public string data_captura { get; set; }
        public string origem_ajuste { get; set; }
        public string valor_complementar { get; set; }
        public string produto_financeiro { get; set; }
        public string valor_antecipado { get; set; }
        public string bandeira { get; set; }
        public string meio_captura { get; set; }
        public string taxas_comissao { get; set; }
        public string tarifa { get; set; }
        public string taxa_garantia { get; set; }
        public string registro_unico_RO { get; set; }
        public string terminal { get; set; }

    }
}
