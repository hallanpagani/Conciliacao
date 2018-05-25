using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.generico;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.banco
{
    [Table("banco_historico")]
    public class Banco_Historico
    {
        [Key]
        [AutoInc]
        [Required]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("ds_historico")]
        [Display(Name = "Histórico")]
        public string DsHistorico { get; set; }

        [OnlyInsert]
        [Required]
        [Column("dt_inclusao")]
        public DateTime DataInc { get; set; }

        [OnlyInsert]
        [Required]
        [Column("hr_inclusao")]
        public TimeSpan HoraInc { get; set; }

        [Required]
        [Column("id_usuario")]
        public long IdUsuario { get; set; }

        [Required]
        [Column("id_conta")]
        public long IdConta { get; set; }

        public Banco_Historico()
        {
            DataInc = Fuso.GetDateNow();
            HoraInc = Fuso.GetTimeNow();
            DsHistorico = null;
            var conta = new BaseUGrav();
            IdUsuario = conta.Usuario;
            IdConta = conta.IdConta;
        }

    }
}
