using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConciliacaoModelo.classes;

namespace ConciliacaoModelo.model.conciliador
{
        [Table("conciliador_criticas")]
        public class ArquivoDeCriticas
        {
            [Key]
            [Column("id")]
            public long id { get; set; }

            [Column("ds_motivo")]
            public string ds_motivo { get; set; }

            [Column("ds_arquivo")]
            public string ds_arquivo { get; set; }

            [Column("data_resumo_venda")]
            public DateTime dt_transacao { get; set; }

            [OnlySelect]
            public string data_string { get { return dt_transacao.ToString("dd/MM/yyyy"); } }

            [Column("data_para_credito")]
            public DateTime dt_credito { get; set; }

            [OnlySelect]
            public string data_credito_string { get { return dt_credito.ToString("dd/MM/yyyy"); } }

            [Column("vl_bruto")]
            public Decimal vl_bruto { get; set; }

            [Column("vl_liquido")]
            public Decimal vl_liquido { get; set; }

            [Column("nsu_rede")]
            public string nsu_rede { get; set; }

            [OnlySelect]
            public decimal nsu_rede_long { get { return Convert.ToInt64(nsu_rede); } }

            [Column("numero_resumo_venda")]
            public string numero_resumo_venda { get; set; }

            [OnlySelect]
            public long numero_resumo_venda_long { get { return Convert.ToInt64(string.IsNullOrEmpty(numero_resumo_venda) ? "0" : numero_resumo_venda); } }
            
            [Column("id_conta")]
            public long id_conta { get; set; }

            public ArquivoDeCriticas()
            {
               
            }
        }

}
