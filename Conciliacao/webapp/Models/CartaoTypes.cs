using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Conciliacao.Models
{
    public static class CartaoTypes
    {
        public static List<SelectListItem> getCartaoTypes(int selecionar = 1)
        {
            var data = new List<SelectListItem>
            {
                new SelectListItem() {Text = "CRÉDITO", Value = "0", Selected = 0 == selecionar},
                new SelectListItem() {Text = "DÉBITO", Value = "1", Selected = 1 == selecionar},
                new SelectListItem() {Text = "TODOS", Value = "3", Selected = 3 == selecionar}
            };
            return data;
        }

    }
}