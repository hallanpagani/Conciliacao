using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.generico;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.banco
{
    [Table("banco_conta")]
    public class Banco_Conta
    {
        [Key]
        [AutoInc]
        [Required]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("ds_banco_conta")]
        [Display(Name = "Descrição")]
        public string NmBancoConta { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column("vl_saldo_inicial")]
        [Display(Name = "Saldo Inicial")]
        public decimal? ValorSaldoInicial { get; set; }

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

        public Banco_Conta()
        {
            DataInc = Fuso.GetDateNow();
            HoraInc = Fuso.GetTimeNow();
            var conta = new BaseUGrav();
            IdUsuario = conta.Usuario;
            IdConta = conta.IdConta;
        }

    }
}
