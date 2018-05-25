using System;
using ConciliacaoModelo.model.adm;
using RestSharp;
using Conciliacao.Helper.Interfaces;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Net;

namespace Conciliacao.Helper.Rest
{
    public class UsuariosRestClient : IUsuariosRestClient
    {
        private readonly RestClient _client;
        private readonly string _url = System.Configuration.ConfigurationManager.AppSettings["webapibaseurl"];

        public UsuariosRestClient()
        {
            _client = new RestClient(_url);
        }

        public void Add(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public void DeleteByContaId(int conta, int id)
        {
            throw new NotImplementedException();
        }

        public Usuario GetByContaId(int conta, int id)
        {
            throw new NotImplementedException();
        }

        public Usuario GetByEmailSenha(string email, string senha)
        {
            var request = new RestRequest("api/usuario/getbyemailsenha", Method.POST) { RequestFormat = DataFormat.Json };

            request.AddParameter("email", email, ParameterType.QueryString);
            request.AddParameter("senha", senha, ParameterType.QueryString);

            var response = _client.Execute<Usuario>(request);

            return response.Data;
        }

        public void Update(Usuario usuario)
        {
            throw new NotImplementedException();
        }

    }
}