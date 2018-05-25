using System.Collections.Generic;
using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.generico;

namespace Conciliacao.Helper.Interfaces
{
    interface ICadastrosRestClient
    {
        Respostas AddColaborador(Estabelecimento base_Colaborador);
        Respostas DelColaboradorPorId(int id);
        IEnumerable<EstabelecimentoListar> GetColaboradorAll(string term);
        List<EstabelecimentoListar> GetListaAniversariosMes();

        Estabelecimento GetColaboradorPorId(int id);

        Respostas AddCliente(Estabelecimento base_cliente);
        Respostas DelClientePorId(int id);
        IEnumerable<EstabelecimentoListar> GetClienteAll(string term);
        Estabelecimento GetClientePorId(int id);

    }
}
