
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.model.generico;

namespace ConciliacaoModelo.model.conciliador.UseRede
{
    [Table("conciliador_userede_eevd_pontovenda")]
    public class ConciliacaoUseRedeEEVDTotalPontoVendaStruct : BaseUGrav
    {
        [Column("tipo_registro")]
        public int is_tipo_registro { get; set; }

        [Column("nm_tipo_registro")]
        public string nm_tipo_registro { get; set; }

        [Column("numero_ponto_venda")]
        public string is_numero_ponto_venda { get; set; }

        [Column("quantidade_resumo_vendas_acatados")]
        public decimal is_quantidade_resumo_vendas_acatados { get; set; }

        [Column("quantidade_comprovante_vendas")]
        public decimal is_quantidade_comprovante_vendas { get; set; }

        [Column("total_bruto")]
        public decimal is_total_bruto { get; set; }

        [Column("total_desconto")]
        public decimal is_total_desconto { get; set; }

        [Column("total_liquido")]
        public decimal is_total_liquido { get; set; }

        [Column("valor_bruto_predatado")]
        public decimal is_valor_bruto_predatado { get; set; }

        [Column("valor_desconto_predatado")]
        public decimal is_valor_desconto_predatado { get; set; }

        [Column("valor_liquido_predatado")]
        public decimal is_valor_liquido_predatado { get; set; }

        [Column("rede")]
        public int rede { get; set; }
        /*
        Construtor da classe.
        */
        public ConciliacaoUseRedeEEVDTotalPontoVendaStruct()
        {

        }

    }
}