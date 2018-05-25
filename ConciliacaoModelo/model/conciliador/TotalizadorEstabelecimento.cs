using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConciliacaoModelo.model.generico;

namespace ConciliacaoModelo.model.conciliador
{
    [Table("totalizador_estabelecimento")]
    public class TotalizadorEstabelecimento : BaseUGrav
    {
        [Column("id_estabelecimento")]
        public long estabelecimento { get; set; }

        [Column("data_prevista")]
        public DateTime prev_pagamento { get; set; }

        [Column("total")]
        public decimal total_realizado { get; set; }
    }
}
