using ConciliacaoModelo.model.conciliador;
using ConciliacaoModelo.model.conciliador.UseRede;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conciliacao.Models.Relatorios
{
    public class TransacaoCreditosViewModel
    {
        public int? filtro_rede { get; set; }
        public string filtro_nm_rede { get; set; }
        public string filtro_resumo { get; set; }
        public int? filtro_banco { get; set; }
        public string filtro_nm_banco { get; set; }

        public int? filtro_agencia { get; set; }
        public string filtro_conta { get; set; }

        public string filtro_nm_agencia { get; set; }
        public string filtro_nm_conta { get; set; }

        public int tp_cartao { get; set; }
        public int tp_movimento { get; set; }

        public int? filtro_estabelecimento { get; set; }
        public string filtro_nm_estabelecimento { get; set; }

        public int tp_data { get; set; } // 0 - Emissao

        [DataType(DataType.Date, ErrorMessage = "Data inicial em formato inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataInicio { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Data final em formato inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataFinal { get; set; }

        public int tp_situacao { get; set; } // abertos, liquidados, todos.
        public string resumo { get; set; } // resumovenda



        public List<ConciliacaoTransacaoRede> ListDebitos { get; set; }

        public List<ConciliacaoUseRedeEEFICreditosStructListar> ListCreditos { get; set; }


        public TransacaoCreditosViewModel()
        {
            filtro_rede = null;
            filtro_agencia = null;
            filtro_resumo = "";
            filtro_nm_rede = "";
            filtro_banco = 0;
            filtro_nm_banco = "";
            filtro_conta = "";
            filtro_agencia = null;
            filtro_conta = null;
            filtro_estabelecimento = null;
            filtro_nm_estabelecimento = "";
            filtro_nm_agencia = "";
            filtro_nm_conta = "";
            var date = DateTime.Now; // inicio do mês até hoje
            DataInicio = new DateTime(date.Year, date.Month, 1);
            DataFinal = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
            tp_data = 0;
            tp_cartao = 0;
            tp_situacao = 2;
            ListCreditos = new List<ConciliacaoUseRedeEEFICreditosStructListar>();
            ListDebitos = new List<ConciliacaoTransacaoRede>();
        }
    }
}