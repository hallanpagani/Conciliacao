using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConciliacaoModelo.model.generico;

namespace ConciliacaoModelo.model.conciliador.UseRede
{
    [Table("conciliador_userede_eefi_credito")]
    public class ConciliacaoUseRedeEEFICreditosStructReduzido
    {
        [Column("data_lancamento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_lancamento { get; set; }

        [Column("banco")]
        public int banco { get; set; }

        [Column("agencia")]
        public string agencia { get; set; }

        [Column("conta_corrente")]
        public long conta_corrente { get; set; }

        [Column("numero_rv")]
        public int numero_rv { get; set; }

        [Column("bandeira")]
        public string bandeira { get; set; }

        [Column("numero_parcela")]
        public string numero_parcela { get; set; }

        [Column("rede")]
        public int rede { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Data inicial em formato inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataInicio { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Data final em formato inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataFinal { get; set; }

    }
}
