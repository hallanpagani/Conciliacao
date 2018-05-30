using ConciliacaoModelo.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConciliacaoModelo.model.conciliador
{
    public class ConciliacaoTransacaoRede
    {
        [Column("ordem")]
        public long ordem { get; set; }

        [Column("resumo")]
        public string is_tipo_registro { get; set; }

        [Column("numero_filiacao_pv")]
        public string is_numero_filiacao_pv { get; set; }

        public string is_status_transacao { get; set; }

        [Column("numero_resumo_venda")]
        public string is_numero_resumo_vendas { get; set; }

        public long is_numero_resumo_vendas_long { get { return Convert.ToInt64(is_numero_resumo_vendas); } }

        [Column("codigo_estabelecimento")]
        public long codigo_estabelecimento { get; set; }

        [Column("nome_estabelecimento")]
        public string nome_estabelecimento { get; set; }

        [Column("data_resumo_venda")]
        public DateTime is_data_cv { get; set; }

        public string data_cv { get { return is_data_cv.ToString("dd/MM/yyyy"); } }

        [Column("valor_bruto")]
        public decimal is_valor_bruto { get; set; }

        [Column("valor_liquido")]
        public decimal is_valor_liquido { get; set; }

        [Column("taxa_cobrada")]
        public decimal taxa_cobrada { get; set; }

        [Column("nsu_rede")] /**/
        public string is_numero_cv { get; set; }

        public long is_numero_cv_long { get { return Convert.ToInt64((is_numero_cv ?? "0").Trim().Equals("") ? "0" : is_numero_cv.Trim()); } }

        [Column("data_credito")]
        public DateTime is_data_credito { get; set; }

        public string data_credito { get { return is_data_credito.ToString("dd/MM/yyyy"); } }

      /*  [Column("status_transacao")]
        public string is_status_transacao { get; set; }*/

        [Column("bandeira")]
        public string is_bandeira { get; set; }

        [Column("produto")]
        public string produto { get; set; }

        [Column("banco")]
        public string banco { get; set; }

        [Column("agencia")]
        public string agencia { get; set; }

        [Column("conta_corrente")]
        public string conta_corrente { get; set; }

        public string banco_trim
        {
            get
            {
                return banco.TrimStart('0');
            }
        }

        public string agencia_trim
        {
            get
            {
                return agencia.TrimStart('0');
            }
        }

        public string conta_corrente_trim
        {
            get
            {
                return conta_corrente.TrimStart('0');
            }
        }

        [Column("numero_cartao")]
        public string numero_cartao { get; set; }

        [Column("tipo_captura")]
        public string tipo_captura { get; set; }     

        public string is_codigo_tef { get; set; }

        public string is_nsu_tef { get; set; }

        [Column("nsu_rede")]
        public string is_nsu_rede { get; set; }

        public ConciliacaoTransacaoRede()
        {
            banco = "0";
            agencia = "0";
            conta_corrente = "0";
        }
    }
}
