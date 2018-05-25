using ConciliacaoModelo.classes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.relatorio
{
    [Table("conciliador_banco")]
    public class TransacaoRedeVsExtratoBancarioListar
    {
        [Column("conta")]
        public string ds_conta { get; set; } // vai no relatorio

        [Column("ds_historico")]
        public string ds_historico { get; set; } // vai no relatorio

        [Column("dt_mvto")]
        public DateTime dt_mvto { get; set; }

        [OnlySelect]
        public string data_string { get { return dt_mvto.ToString("dd/MM/yyyy"); } } // vai no relatorio

        [Column("vl_mvto")]
        public decimal vl_mvto { get; set; } // vai no relatorio

        [Column("coalesce((select 1 from conciliador_rede r where r.vl_liquido = a.vl_mvto and r.dt_prev_pagto = a.dt_mvto and r.id_conta = a.id_conta ), 0) as is_encontrado")]
        public int is_encontrado { get; set; }// vai no relatorio

    }
}