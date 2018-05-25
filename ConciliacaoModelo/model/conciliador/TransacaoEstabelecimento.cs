using System;
using System.ComponentModel.DataAnnotations.Schema;
using ConciliacaoModelo.classes;
using ConciliacaoModelo.model.generico;

namespace ConciliacaoModelo.model.conciliador
{
    [Table("conciliador_estabelecimento")]
    public class TransacaoEstabelecimento : BaseUGrav
    {
        [Column("id_conciliador")]
        public long id { get; set; }

        [Column("ds_rede")]
        public string ds_rede { get; set; }

        [Column("nm_estabelecimento")]
        public string nm_estabelecimento { get; set; }

        [Column("dt_transacao")]
        public DateTime dt_transacao { get; set; }

        [OnlySelect]
        public string data_string { get { return dt_transacao.ToString("dd/MM/yyyy"); } }

        [Column("vl_bruto")]
        public Decimal vl_bruto { get; set; }

        [Column("tot_parcela")]
        public int tot_parcela { get; set; }

        [Column("nsu_rede")]
        public string nsu_rede { get; set; }

        [OnlySelect]
        public long nsu_rede_long { get { return Convert.ToInt64(nsu_rede) ; } }

        [Column("nsu_tef")]
        public string nsu_tef { get; set; }

        [Column("is_autorizacao")]
        public string is_autorizacao { get; set; }

        [Column("nr_logico")]
        public string nr_logico { get; set; }

        [Column("cod_loja")]
        public int cod_loja { get; set; }

        [Column("operadora")]
        public string operadora { get; set; }

        [Column("bandeira")]
        public string bandeira { get; set; }

        [Column("operadora_desc")]
        public string operadora_desc { get; set; }

        [Column("bandeira_desc")]
        public string bandeira_desc { get; set; }

        [Column("caixa")]
        public int caixa { get; set; }

        [Column("nr_maquineta")]
        public string nr_maquineta { get; set; }

        [Column("area_cliente")]
        public string area_cliente { get; set; }

        [Column("reservado_cliente")]
        public string reservado_cliente { get; set; }

        [Column("produto")]
        public string produto { get; set; }

        public TransacaoEstabelecimento()
        {
            tot_parcela = 1;
        }
    }
}
