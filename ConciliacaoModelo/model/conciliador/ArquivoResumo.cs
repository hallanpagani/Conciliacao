namespace ConciliacaoModelo.model.conciliador
{
    public class ArquivoResumo
    {
        public string resumo { get; set; }
        public string estabelecimento { get; set; }
        public string parcela { get; set; }
        public string transacao { get; set; }
        public string prev_pagamento { get; set; }
        public decimal valor_bruto { get; set; }
        public string taxa_comissao { get; set; }
        public decimal vl_comissao { get; set; }
        public string vl_rejeitado { get; set; }
        public decimal vl_liquido { get; set; }
        public string situacao { get; set; }
        public string produto { get; set; }
        public string terminal { get; set; }
        public string chave { get; set; }
    }
}
