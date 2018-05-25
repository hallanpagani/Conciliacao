using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConciliacaoModelo.model.conciliador.UseRede
{
    public class ConciliacaoUseRedeEEFIResumoOperacaoStruct
    {
        public string descricao { get; set; }

        public string numero_pv { get; set; }

        public DateTime data { get; set; }

        public string data_str { get { return data.ToString("dd/MM/yyyy"); } }

        public decimal valor { get; set; }

        public DateTime data_venda { get; set; }

        public string data_venda_str { get { return data_venda.ToString("dd/MM/yyyy"); } }

        public decimal valor_venda { get; set; }

        public int banco { get; set; }
    
        public string agencia { get; set; }

        public long conta_corrente { get; set; }

        public string nsu { get; set; }

        public decimal valor_cancelado { get; set; }

        public string autorizacao { get; set; }

        public string situacao { get; set; }

        public int rede { get; set; }
    }

}
