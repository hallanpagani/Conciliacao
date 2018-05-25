using ConciliacaoModelo.model.generico;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.conciliador.UseRede
{
    [Table("conciliador_userede_eevc_comprovantevenda_parcelas")]
    public class ConciliacaoUseRedeParcelasStructListar
    {
        [Column("tipo_registro")]
        public int is_tipo_registro { get; set; }

        [Column("numero_filiacao_pv")]
        public decimal is_numero_filiacao_pv { get; set; }

        [Column("numero_resumo_venda")]
        public long is_numero_resumo_vendas { get; set; }

        public long is_numero_resumo_vendas_long { get { return is_numero_resumo_vendas; } }

        [Column("data_rv")]
        public DateTime is_data_rv { get; set; }

        [Column("numero_parcela")]
        public long numero_parcela { get; set; }

        [Column("valor_parcela_bruto")]
        public decimal valor_parcela_bruto { get; set; }

        [Column("valor_parcela_desconto")]
        public decimal valor_desconto_parcela { get; set; }

        [Column("valor_parcela_liquida")]
        public decimal valor_parcela_liquida { get; set; }

        [Column("data_credito")]
        public DateTime data_credito { get; set; }

        [Column("rede")]
        public int rede { get; set; }

        /*
        Construtor da classe.
        */
        public ConciliacaoUseRedeParcelasStructListar()
        {

        }

    }
}