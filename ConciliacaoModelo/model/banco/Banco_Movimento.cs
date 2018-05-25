using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.flags;
using ConciliacaoModelo.model.generico;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.banco
{
    [Table("banco_movimento")]
    public class Banco_Movimento : BaseUGrav
    {
        [Key]
        [AutoInc]
        [Required]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column("vl_mvto")]
        [Display(Name = "Valor")]
        public decimal? Valor { get; set; }

        [Required]
        [Column("id_historico")]
        public int IdHistorico { get; set; }
        [Display(Name = "Histórico")]
        public string DsHistorico { get; set; }

        [Column("ds_documento")]
        [Display(Name = "Documento")]
        public string DsDocumento { get; set; }

        [DataType(DataType.Date)]
        [Column("dt_emissao")]
        [Display(Name = "Emissão")]
        public DateTime DataEmissao { get; set; }

        [DataType(DataType.Date)]
        [Column("dt_vencimento")]
        [Display(Name = "Vencimento")]
        public DateTime DataVencimento { get; set; }

        public Financeiro_Flags.Tipo FinanceiroTipo;

        [OnlyInsert]
        [Required]
        [Column("dt_inclusao")]
        public DateTime DataInc { get; set; }

        [OnlyInsert]
        [Required]
        [Column("hr_inclusao")]
        public TimeSpan HoraInc { get; set; }

        public Banco_Movimento()
        {
            DataInc = Fuso.GetDateNow();
            HoraInc = Fuso.GetTimeNow();
            FinanceiroTipo = Financeiro_Flags.Tipo.Tranferencia;
            DataEmissao = Fuso.GetDateNow();
            DataVencimento = Fuso.GetDateNow();
            DsDocumento = null;
        }
    }
}
