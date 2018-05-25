using ConciliacaoModelo.model.generico;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.conciliador.UseRede
{
    [Table("conciliador_userede_eevd_comprovantevenda")]
    public class ConciliacaoUseRedeEEVDComprovanteVendaStruct : BaseUGrav
    {
        [Column("tipo_registro")]
        public int is_tipo_registro { get; set; }

        [Column("nm_tipo_registro")]
        public string nm_tipo_registro { get; set; }

        [Column("numero_filiacao_pv")]
        public decimal is_numero_filiacao_pv { get; set; }

        [Column("numero_resumo_venda")]
        public long is_numero_resumo_vendas { get; set; }

        public long is_numero_resumo_vendas_long { get { return is_numero_resumo_vendas; } }

        [Column("data_cv")]
        public DateTime is_data_cv { get; set; }

        public string data_cv { get { return is_data_cv.ToString("dd/MM/yyyy"); } }

        [Column("valor_bruto")]
        public decimal is_valor_bruto { get; set; }

        [Column("valor_desconto")]
        public decimal is_valor_desconto { get; set; }

        [Column("valor_liquido")]
        public decimal is_valor_liquido { get; set; }

        [Column("numero_cartao")]
        public string is_numero_cartao { get; set; }

        [Column("tipo_transacao")]
        public string is_tipo_transacao { get; set; }

        [Column("numero_cv")] /**/ 
        public decimal is_numero_cv { get; set; }

        public long is_numero_cv_long { get { return Convert.ToInt64(is_numero_cv); } }

        [Column("data_credito")]
        public DateTime is_data_credito { get; set; }

        public string data_credito { get { return is_data_credito.ToString("dd/MM/yyyy"); } }

        [Column("status_transacao")]
        public string is_status_transacao { get; set; }

        [Column("hora_transacao")]
        public string is_hora_transacao { get; set; }

        [Column("numero_terminal")]
        public string is_numero_terminal { get; set; }

        [Column("tipo_captura")]
        public string is_tipo_captura { get; set; }

        [Column("reservado")]
        public string is_reservado { get; set; }

        [Column("valor_compra")]
        public decimal is_valor_compra { get; set; }

        [Column("valor_saque")]
        public decimal is_valor_saque { get; set; }

        [Column("bandeira")]
        public string is_bandeira { get; set; }

        [Column("codigo_tef")]
        public string is_codigo_tef { get; set; }

        [Column("nsu_tef")]
        public string is_nsu_tef { get; set; }

        [Column("taxa_cobrada")]
        public decimal taxa_cobrada { get; set; }

        [Column("numero_autorizacao")]
        public string is_numero_autorizacao { get; set; }

        [Column("numero_nota_fiscal")]
        public string numero_nota_fiscal { get; set; }

        [Column("numero_unico_transacao")]
        public string numero_unico_transacao { get; set; }

        [Column("valor_liquido_primeira_parc")]
        public decimal
            is_valor_liquido_primeira_parc
        { get; set; }

        [Column("valor_liquido_demais_parc")]
        public decimal
            is_valor_liquido_demais_parc
        { get; set; }

        [Column("parcela")]
        public string
          is_parcela
        { get; set; }

        [Column("numero_parcelas")]
        public int
            is_numero_parcelas
        { get; set; }

        [Column("rede")]
        public int rede { get; set; }
        /*
        Construtor da classe.
        */
        public ConciliacaoUseRedeEEVDComprovanteVendaStruct()
        {

        }

    }
}