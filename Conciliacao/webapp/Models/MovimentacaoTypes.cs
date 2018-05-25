using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Conciliacao.Models
{
    public static class MovimentacaoTypes
    {
        public static List<SelectListItem> getMovimentacaoTypes(int selecionar = 0)
        {
            var data = new List<SelectListItem>
            {
                new SelectListItem() {Text = "TODOS", Value = "0", Selected = 0 == selecionar},
                new SelectListItem() {Text = "CRÉDITOS", Value = "1", Selected = 1 == selecionar},
                new SelectListItem() {Text = "ANTECIPAÇÕES", Value = "2", Selected = 2 == selecionar}
            };
            return data;
        }

    }
}