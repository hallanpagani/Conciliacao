using ConciliacaoModelo.model.generico;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Conciliacao.Models
{
    [Table("cadastro_transacoes_tef")]
    public class TransacoesTEF : BaseID
    {
        [Column("id")]
        public long id { get; set; }

        [Column("sequencial")]
        public string sequencial { get; set; }

        [Column("data_atual")]
        public DateTime data_atual { get; set; }

        [Column("estabelecimento")]
        public string estabelecimento { get; set; }

        [Column("loja")]
        public string loja { get; set; }

        [Column("terminal")]
        public string terminal { get; set; }

        [Column("terminal_validade")]
        public DateTime terminal_validade { get; set; }

        [Column("rede")]
        public string rede { get; set; }

        [Column("tipo_cartao")]
        public string tipo_cartao { get; set; }

        [Column("administrador")]
        public string administrador { get; set; }

        [Column("tipo_transacao")]
        public string tipo_transacao { get; set; }

        [Column("produto")]
        public string produto { get; set; }

        [Column("cartao_bin")]
        public long cartao_bin { get; set; }

        [Column("cartao_numero")]
        public long cartao_numero { get; set; }

        [Column("cartao_validade")]
        public DateTime cartao_validade { get; set; }

        [Column("cartao_entrada")]
        public string cartao_entrada { get; set; }

        [Column("transacao_inicio")]
        public DateTime transacao_inicio { get; set; }

        [Column("transacao_fim")]
        public DateTime transacao_fim { get; set; }

        [Column("transacao_conclusao")]
        public DateTime transacao_conclusao { get; set; }

        [Column("transacao_pagamento")]
        public string transacao_pagamento { get; set; }

        [Column("transacao_financiado")]
        public string transacao_financiado { get; set; }

        [Column("erro")]
        public string erro { get; set; }

        [Column("transacao_identificacao")]
        public string transacao_identificacao { get; set; }

        [Column("transacao_nsu")]
        public long transacao_nsu { get; set; }

        [Column("transacao_nsu_rede")]
        public long transacao_nsu_rede { get; set; }

        [Column("transacao_valor")]
        public decimal transacao_valor { get; set; }

        [Column("transacao_parcela")]
        public int transacao_parcela { get; set; }

        [Column("transacao_autorizacao")]
        public string transacao_autorizacao { get; set; }

        [Column("transacao_resposta")]
        public string transacao_resposta { get; set; }

        [Column("transacao_situacao")]
        public string transacao_situacao { get; set; }
    }
}