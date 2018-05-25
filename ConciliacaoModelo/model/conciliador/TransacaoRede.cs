using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.generico;

namespace ConciliacaoModelo.model.conciliador
{
    [Table("conciliador_rede")]
    public class TransacaoRede : BaseUGrav
    {
        [Key]
        [AutoInc]
        [Column("id_conciliador")]
        public long id { get; set; }

        [Column("ds_rede")]
        public string ds_rede { get; set; }

        [Column("nm_estabelecimento")]
        public string nm_estabelecimento { get; set; }

        [Column("dt_transacao")]
        public DateTime dt_transacao { get; set; }

        [Column("dt_prev_pagto")]
        public DateTime dt_prev_pagto { get; set; }

        [OnlySelect]
        public string data_string { get { return dt_transacao.ToString("dd/MM/yyyy"); } }

        [Column("vl_bruto")]
        public Decimal vl_bruto { get; set; }

        [Column("vl_liquido")]
        public Decimal vl_liquido { get; set; }

        [Column("tot_parcela")]
        public int tot_parcela { get; set; }

        [Column("nsu_rede")]
        public string nsu_rede { get; set; }

        [Column("nsu_tef")]
        public string nsu_tef { get; set; }

        [Column("is_autorizacao")]
        public string is_autorizacao { get; set; }

        [Column("nr_logico")]
        public string nr_logico { get; set; }

    }
}
