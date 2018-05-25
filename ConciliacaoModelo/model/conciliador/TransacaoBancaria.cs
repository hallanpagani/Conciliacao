using System;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.generico;

namespace ConciliacaoModelo.model.conciliador
{
    [Table("conciliador_banco")]
    public class TransacaoBancaria : BaseUGrav
    {
        /*[Column("id")]
        public long id { get; set; }*/

        [Column("dt_mvto")]
        public DateTime dt_mvto { get; set; }

        [OnlySelect]
        public string data_string { get { return dt_mvto.ToString("dd/MM/yyyy"); }  }

        [Column("vl_mvto")]
        public Decimal vl_mvto { get; set; }

        [Column("nr_doc")]
        public string nr_doc { get; set; }

        [Column("ds_historico")]
        public string ds_historico { get; set; }

        [Column("conta")]
        public string conta { get; set; }

        [Column("tp_mvto")]
        public string tp_mvto { get; set; }

    }
}
