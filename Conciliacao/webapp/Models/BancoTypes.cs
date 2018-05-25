using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.banco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Conciliacao.Models
{
    public static class BancoTypes
    {
        public static List<SelectListItem> getBancoTypes(int selecionar = 1)
        {
            var data = new List<SelectListItem>
            {
                new SelectListItem() {Text = "TODOS", Value = "0", Selected = 0 == selecionar},
               
                new SelectListItem() {Text = "ITAÚ - 135929", Value = "341135929", Selected = 341135929 == selecionar},
                new SelectListItem() {Text = "ITAÚ - 226124", Value = "341226124", Selected = 341226124 == selecionar},
                new SelectListItem() {Text = "ITAÚ - 536571", Value = "341536571", Selected = 341536571 == selecionar}
            };
            return data;
        }

    }
}