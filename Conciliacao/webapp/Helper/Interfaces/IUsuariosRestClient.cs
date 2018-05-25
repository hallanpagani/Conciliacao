using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConciliacaoModelo.model.adm;


namespace Conciliacao.Helper.Interfaces
{
    public interface IUsuariosRestClient
    {
        void Add(Usuario usuario);
        void DeleteByContaId(int conta, int id);
        //IEnumerable<Usuario> GetAll();
        Usuario GetByContaId(int conta, int id);
        Usuario GetByEmailSenha(string email, string senha);
        void Update(Usuario usuario);
    }
}



