using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConciliacaoFuncoes.classes;

namespace Conciliacao.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult Http404(Exception exception)
        {
            Response.StatusCode = 404;
            Response.ContentType = "text/html";
            return View(exception);
        }

        public ActionResult Http500(Exception exception)
        {
            Response.StatusCode = 500;
            Response.ContentType = "text/html";

            string msgErro = " <p> <strong> Mensagem </strong>: " + exception.Message + " </p>" +
                               " <p> <strong> Source </strong>: " + exception.Source + " </p>" +
                               " <p> <strong> StackTrace </strong>: " + exception.StackTrace + " </p>";

            //  " <p> <strong> StackTrace </strong>: " + exception.StackTrace + " </p>";

            ViewData["Erro"] = msgErro;

            AppUtil.EnviarEmail("hallanpagani@gmail.com", "Hallan", "Ops! Ocorreu um erro ", msgErro);

            return View(exception);
        }
    }
}