using ConciliacaoModelo.model.generico;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.conciliador.UseRede
{
    [Table("conciliador_userede_eevd_resumooperacao")]
    public class ConciliacaoUseRedeEEVDResumoOperacaoStruct : BaseUGrav
    {
        [Column("tipo_registro")]
        public int is_tipo_registro { get; set; }

        [Column("nm_tipo_registro")]
        public string nm_tipo_registro { get; set; }

        [Column("numero_filiacao_pv")]
        public decimal
            is_numero_filiacao_pv { get; set; }

        [Column("data_credito")]
        public DateTime is_data_credito { get; set; }

        public string data_credito { get { return is_data_credito.ToString("dd/MM/yyyy"); } }

        [Column("data_resumo_venda")]
        public DateTime is_data_resumo_venda { get; set; }

        public string data_resumo_venda { get { return is_data_resumo_venda.ToString("dd/MM/yyyy"); } }

        [Column("numero_resumo_venda")]
        public string
            is_numero_resumo_venda { get; set; }

        public long is_numero_resumo_venda_long { get { return Convert.ToInt64(is_numero_resumo_venda); } }

        [Column("quantidade_resumo_vendas")]
        public string
            is_quantidade_resumo_vendas { get; set; }

        [Column("valor_bruto")]
        public decimal
            is_valor_bruto { get; set; }

        [Column("valor_desconto")]
        public decimal
            is_valor_desconto { get; set; }

        [Column("valor_liquido")]
        public decimal
            is_valor_liquido { get; set; }

        [Column("tipo_resumo")]
        public string
            is_tipo_resumo { get; set; }

        [Column("banco")]
        public string is_banco { get; set; }

        [Column("agencia")]
        public string is_agencia { get; set; }

        [Column("conta_corrente")]
        public string is_conta_corrente { get; set; }

        [Column("bandeira")]
        public string is_bandeira { get; set; }

        [Column("rede")]
        public int rede { get; set; }
        /*
        Construtor da classe.
        */
        public ConciliacaoUseRedeEEVDResumoOperacaoStruct()
        {

        }

    }
}