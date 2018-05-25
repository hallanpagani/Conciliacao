using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Conciliacao.Models
{
    public static class DataTypes
    {
        public static List<SelectListItem> getDataTypes()
        {
            List<SelectListItem> data = new List<SelectListItem>();
            data.Add(new SelectListItem() { Text = "TRANSAÇÃO", Value = "0" });
            //data.Add(new SelectListItem() { Text = "VENCIMENTO", Value = "1" });
            return data;
        }


    }
}