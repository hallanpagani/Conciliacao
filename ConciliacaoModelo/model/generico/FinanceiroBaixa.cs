﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ConciliacaoModelo.model.generico
{
    public class FinanceiroBaixa
    {
        public long Id { get; set; }

        [DataType(DataType.Currency)]
        public decimal? ValorSaldo { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Valor")]
        public decimal? ValorBaixa { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        public DateTime DataBaixa { get; set; }

        public FinanceiroBaixa()
        {
            DataBaixa = DateTime.Now;
        }

    }
}
