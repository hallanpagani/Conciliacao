using ConciliacaoModelo.model.conciliador;
using ConciliacaoModelo.model.relatorio;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conciliacao.Models.Relatorios
{
    public class RelatorioEstabelecimentoViewModel
    {
        public int? filtro_rede { get; set; }
        public string filtro_nm_rede { get; set; }
        public string filtro_resumo { get; set; }

        public int tp_data { get; set; } // 0 - Emissao

        [DataType(DataType.Date, ErrorMessage = "Data inicial em formato inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataInicio { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Data final em formato inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataFinal { get; set; }

        public int tp_situacao { get; set; } // abertos, liquidados, todos.

        public List<TransacaoEstabelecimentoListar> ListaEstabelecimento { get; set; }

        public RelatorioEstabelecimentoViewModel()
        {
            filtro_rede = null;
            filtro_nm_rede = "";
            filtro_resumo = "";
            var date = DateTime.Now; // inicio do mês até hoje
            DataInicio = new DateTime(date.Year, date.Month, 1);
            DataFinal = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
            ListaEstabelecimento = new List<TransacaoEstabelecimentoListar>();
            tp_data = 0;
            tp_situacao = 2;
        }
    }
}