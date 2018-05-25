using ConciliacaoModelo.classes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.relatorio
{
    [Table("conciliador_rede")]
    public class TransacaoRedeVsTEFListar
    {
        [Column("ds_rede")]
        public string ds_rede { get; set; } // vai no relatorio

        [Column("nm_estabelecimento")]
        public string nm_estabelecimento { get; set; } // vai no relatorio

        [Column("dt_transacao")]
        public DateTime dt_transacao { get; set; }

        [OnlySelect]
        public string data_string { get { return dt_transacao.ToString("dd/MM/yyyy"); } } // vai no relatorio

        [Column("vl_bruto")]
        public decimal vl_bruto { get; set; } // vai no relatorio

        [Column("nsu_tef")]
        public string nsu_tef { get; set; }// vai no relatorio

        [Column("coalesce((select 1 from conciliador_tef t where t.id_conta = a.id_conta and t.ds_rede = a.ds_rede and t.dt_transacao = a.dt_transacao and t.nsu_rede = a.nsu_rede),0) as is_encontrado")]
        public long is_encontrado { get; set; }// vai no relatorio

    }
}