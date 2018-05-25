namespace ConciliacaoModelo.model.flags
{
    public class Financeiro_Flags
    {
        public enum Tipo
        {
            Receita,
            Despesa,
            Tranferencia
        };

        public enum Status {
            Aberto,
            Finalizado,
            Cancelado
        };

        public enum Baixa {
            BaixaParcial,
            BaixaCompleta
        };

    }
}