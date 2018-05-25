using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Conciliacao.Models.conciliacao
{
    public class ConciliacaoTrailerStruct : Controller
    {
        /*
        Variaveis de trabalho.
        */
        public string
        is_tipo_registro,
        is_total_registro;

        /*
        Construtor da classe.
        */
        public ConciliacaoTrailerStruct()
        {

        }

        // GET: ConciliacaoTrailerStruct
        public ActionResult Index()
        {
            return View();
        }
    }
}