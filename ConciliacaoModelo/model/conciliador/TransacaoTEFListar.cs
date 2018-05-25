using System;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.generico;

namespace ConciliacaoModelo.model.conciliador
{
    
    public class TransacaoTEFListar 
    {

        [Column("id_conciliador")]
        public long id { get; set; }

        [Column("ds_rede")]
        public string ds_rede { get; set; } // vai no relatorio

        [Column("nm_estabelecimento")]
        public string nm_estabelecimento { get; set; } // vai no relatorio

        [Column("cnpj_estabelecimento")]
        public string cnpj_estabelecimento { get; set; } // vai no relatorio

        [Column("dt_transacao")]
        public DateTime dt_transacao { get; set; }

        [OnlySelect]
        public string data_string { get { return dt_transacao.ToString("dd/MM/yyyy"); } } // vai no relatorio

        [Column("vl_bruto")]
        public Decimal vl_bruto { get; set; } // vai no relatorio

        [Column("tot_parcela")]
        public int tot_parcela { get; set; }

        [Column("nsu_rede")]
        public string nsu_rede { get; set; } 

        [Column("nsu_tef")]
        public string nsu_tef { get; set; }// vai no relatorio

        [Column("is_autorizacao")]
        public string is_autorizacao { get; set; }

        [Column("nr_logico")]
        public string nr_logico { get; set; }

        [Column("transacao_situacao")]
        public string transacao_situacao { get; set; }

        [Column("codigo_tef")]
        public string codigo_tef { get; set; }// vai no relatorio

        [Column("adquirente")]
        public string adquirente { get; set; }// vai no relatorio

        [Column("bandeira")]
        public string bandeira { get; set; }// vai no relatorio

        [Column("tp_transacao")]
        public string tp_transacao { get; set; }

        [Column("forma_pagto")]
        public int forma_pagto { get; set; }

        [Column("nr_cartao")]
        public string nr_cartao { get; set; }

        [Column("cd_cartao_sistema_tef")]
        public string cd_cartao_sistema_tef { get; set; }

        [Column("cd_unidade_estabelecimento")]
        public int cd_unidade_estabelecimento { get; set; }

        [Column("loja")]
        public string loja { get; set; }

        [Column("transacao_pagamento")]
        public string transacao_pagamento { get; set; }

        [Column("tipo_cartao")]
        public string tipo_cartao { get; set; }

        [Column("administrador")]
        public string administrador { get; set; }

        [Column("terminal")]
        public string terminal { get; set; }

        //[Column("conciliou")]
        public int conciliado { get; set; }

    }
}
