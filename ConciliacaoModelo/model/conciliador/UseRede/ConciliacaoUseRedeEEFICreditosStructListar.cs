using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.conciliador.UseRede
{
    [Table("conciliador_userede_eefi_credito")]
    public class ConciliacaoUseRedeEEFICreditosStructListar 
    {
       // [Column("id")]
       // public long id { get; set; }

        [Column("numero_pv_centralizador")]
        public string numero_pv_centralizador { get; set; }

        [Column("(select codigo_estabelecimento from cadastro_estabelecimento_rede er where er.id_estabelecimento_rede = cast(a.numero_pv_centralizador as decimal(20,0)) and(er.id_rede = coalesce(a.rede, 1))) as codigo_estabelecimento")]
        public long codigo_estabelecimento { get; set; }

        [Column("(select nome_estabelecimento from cadastro_estabelecimento_rede er where er.id_estabelecimento_rede = cast(a.numero_pv_centralizador as decimal(20,0)) and(er.id_rede = coalesce(a.rede, 1))) as nome_estabelecimento")]
        public string nome_estabelecimento { get; set; }

        [Column("numero_documento")]
        public string numero_documento { get; set; }

        [Column("data_lancamento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_lancamento { get; set; }

        [Column("valor_lancamento")]
        public decimal valor_lancamento { get; set; }

        [Column("banco")]
        public int banco { get; set; }

        [Column("agencia")]
        public string agencia { get; set; }

        [Column("conta_corrente")]
        public long conta_corrente { get; set; }

        public string banco_trim
        {
            get
            {
                return banco.ToString().TrimStart('0');
            }
        }

        public string agencia_trim
        {
            get
            {
                return (agencia ?? "0").ToString().TrimStart('0');
            }
        }

        public string conta_corrente_trim
        {
            get
            {
                return conta_corrente.ToString().TrimStart('0');
            }
        }

        [Column("data_movimento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_movimento { get; set; }

        [Column("numero_rv")]
        public long numero_rv { get; set; }

        [Column("data_rv")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_rv { get; set; }

        [Column("bandeira")]
        public string bandeira { get; set; }

        [Column("tipo_transacao")]
        public string tipo_transacao { get; set; }

        [Column("valor_bruto_rv")]
        public decimal valor_bruto_rv { get; set; }

        [Column("valor_taxa_desconto")]
        public decimal valor_taxa_desconto { get; set; }

        [Column("numero_parcela")]
        public string numero_parcela { get; set; }

        [Column("situacao")]
        public string situacao { get; set; }

        [Column("numero_pv_original")]
        public long numero_pv_original { get; set; }

        [Column("rede")]
        public int rede { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Data inicial em formato inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataInicio { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Data final em formato inválido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataFinal { get; set; }

    }
}
