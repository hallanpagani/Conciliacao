using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;

namespace ConciliacaoModelo.model.entrada
{
    [Table("tb_entrada")]
    public class Entrada
    {
        public static class EntradaStatus
        {
            public const string ABERTO = "A";
            public const string FINALIZADO = "F";
        }

        [Key]
        [AutoInc]
        [Required]
        [Column("id_entrada")]
        public int Id { get; set; }

        [Required]
        [OnlyInsert]
        [Column("data_inc")]
        public DateTime DataInc { get; set; }

        [Required]
        [OnlyInsert]
        [Column("hora_inc")]
        public TimeSpan HoraInc { get; set; }

        [Required]
        [Column("status")]
        public string Status { get; set; }  // F = finalizado  A = aberto

        [Column("obs")]
        public string Obs { get; set; }

        [Column("data_fecha")]
        public DateTime? DataFecha { get; set; }

        [Column("hora_fecha")]
        public TimeSpan? HoraFecha { get; set; }

        [Required]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        public Entrada()
        {
            this.DataInc = Fuso.GetDateNow();
            this.HoraInc = Fuso.GetTimeNow();
            this.Status = EntradaStatus.ABERTO;
        }
    }
}