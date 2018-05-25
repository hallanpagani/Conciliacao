using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConciliacaoModelo.model.generico
{
    public class Lista
    {
        [Column("id")]
        public long id { get; set; }

        [Column("text")]
        public string text { get; set; }

    }
}
