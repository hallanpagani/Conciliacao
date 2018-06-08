using Conciliacao.App_Helpers.Componentes;
using Conciliacao.Controllers.Generico;
using Conciliacao.Helper.Rest;
using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.banco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Conciliacao.Controllers.Cadastros
{
    [Authorize]
    public class EstabelecimentoController : AppController
    {
        static readonly EstabelecimentoRestClient _restClient = new EstabelecimentoRestClient();

        // GET: Estabelecimentos
        public ActionResult CadastrarEstabelecimento(long id = 0)
        {
            var model = id > 0 ? _restClient.GetEstabelecimentoPorId(id) : new Estabelecimento();
            model.EstabelecimentoRedeListar = _restClient.GetEstabelecimentoRedePorId(id) ?? new List<EstabelecimentoRedeListar>();
            return View(model);
        }

        [HttpPost]
        public ActionResult CadastrarEstabelecimento(Estabelecimento Estabelecimento, FormCollection form)
        {
            ViewBag.Notification = "";
            if (!ModelState.IsValid)
                return View(Estabelecimento);

            /* if (viewModel.Id == 0)
             {

                 Vans u = new Vans();
                 Filtros f = new Filtros(u);
                 f.Add(() => u.Email, viewModel.Email, FiltroExpressao.Igual);
                 u = DAL.GetObjeto<Vans>(f);
                 if (u != null)
                 {
                     this.AddNotification("Informação! Usuário já cadastrado.", NotificationType.Alerta);
                     return View("~/views/usuario/incluir.cshtml", viewModel);
                 }
             }*/

            Respostas resp;
            try
            {
                resp = _restClient.AddEstabelecimento(Estabelecimento);
            }
            catch (Exception ex)
            {
                AddErrors(ex);
                return View(Estabelecimento);
            }
            if (Estabelecimento.IdEstabelecimento > 0)
            {
                this.AddNotification("Estabelecimento alterado.", NotificationType.Sucesso);
                return View(Estabelecimento);
            }
            else
            {
                this.AddNotification("Estabelecimento incluído.", NotificationType.Sucesso);
            }



            return CadastrarEstabelecimento((long)resp.Dados);
        }

        // GET: Estabelecimentos
        public ActionResult ConsultarEstabelecimento()
        {
            var model = _restClient.GetEstabelecimentosAll("");
            return View(model);
        }


        public ActionResult DeletarEstabelecimento(int id)
        {
            Estabelecimento estabelecimento = DAL.GetObjeto<Estabelecimento>(string.Format("id_conta={0} and id={1}",UsuarioLogado.IdConta, id));


            EstabelecimentoRede estabelecimento_rede = DAL.GetObjeto<EstabelecimentoRede>(string.Format("id_conta ={0} and  id_estabelecimento={1}", UsuarioLogado.IdConta, id));
            DAL.Excluir(estabelecimento);
            this.AddNotification("Vínculo com administradora deletado.", NotificationType.Alerta);
            return RedirectToAction("CadastrarEstabelecimento", new { id = estabelecimento.IdEstabelecimento });
        }


        [HttpGet]
        [OutputCache(Duration = 30)]
        public JsonResult GetEstabelecimentos(string term)
        {
            List<Lista> list = _restClient.GetEstabelecimentosAll(term ?? "").Select(i => new Lista { id = i.id, text = i.nome.ToUpper() }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        class ItemEqualityComparer : IEqualityComparer<Lista>
        {
            public bool Equals(Lista x, Lista y)
            {
                return x.id == y.id && x.text.Equals(y.text) ; 
            }

            public int GetHashCode(Lista obj)
            {
                return obj.id.GetHashCode();
            }
        }

        [HttpGet]
        [OutputCache(Duration = 30)]
        public JsonResult GetEstabelecimentosRede(string term)
        {
            List<Lista> list = _restClient.GetEstabelecimentosRedeAll(term ?? "").Select(i => new Lista { id = i.CodigoEstabelecimento, text = i.CodigoEstabelecimento+" - "+i.NomeEstabelecimento.ToUpper() }).ToList();
            return Json(list.Distinct(new ItemEqualityComparer()).OrderBy(c => c.text).ThenBy(c => c.id).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CadastrarEstabelecimentoRede(EstabelecimentoRede EstabelecimentoRede, FormCollection form)
        {
            EstabelecimentoRede.IdEstabelecimento = Convert.ToInt32(form["IdEstabelecimento"]);
            EstabelecimentoRede.IdEstabelecimentoRede = form["idestabelecimentorede"];
            EstabelecimentoRede.CodigoEstabelecimento = Convert.ToInt32(form["CodigoEstabelecimento"] ?? "0");
            EstabelecimentoRede.NomeEstabelecimento = form["NomeEstabelecimento"];

            if (EstabelecimentoRede.IdEstabelecimento == 0)
            {
                ModelState.AddModelError("", "Campo \"Identificação\" deve ser preenchido!");
                return RedirectToAction("CadastrarEstabelecimento", new { id = EstabelecimentoRede.IdEstabelecimento });
            }
            if (string.IsNullOrEmpty(EstabelecimentoRede.IdEstabelecimentoRede))
            {
                ModelState.AddModelError("", "Campo \"Rede\" deve ser preenchido!");
                return RedirectToAction("CadastrarEstabelecimento", new { id = EstabelecimentoRede.IdEstabelecimento });
            }

            ViewBag.Notification = "";

            // if (!ModelState.IsValid)
            //    return RedirectToAction("CadastrarEstabelecimento", EstabelecimentoRede.IdEstabelecimento);

            try
            {
                var resp = _restClient.AddEstabelecimentoRede(EstabelecimentoRede);
            }
            catch (Exception ex)
            {
                AddErrors(ex);
                return RedirectToAction("CadastrarEstabelecimento", EstabelecimentoRede.IdEstabelecimento);
            }
            if (EstabelecimentoRede.IdEstabelecimento > 0)
            {
                this.AddNotification("Estabelecimento alterado.", NotificationType.Sucesso);
            }
            else
            {
                this.AddNotification("Estabelecimento incluído.", NotificationType.Sucesso);
            }

            return RedirectToAction("CadastrarEstabelecimento", new { id = EstabelecimentoRede.IdEstabelecimento });
        }

        private void AddErrors(Exception exc)
        {
            ModelState.AddModelError("", exc);
        }

        public ActionResult Deletar(int id)
        {
            EstabelecimentoRede estabelecimento = DAL.GetObjeto<EstabelecimentoRede>("id =" + id);
            DAL.Excluir(estabelecimento);
            this.AddNotification("Vínculo com administradora deletado.", NotificationType.Alerta);
            return RedirectToAction("CadastrarEstabelecimento", new { id = estabelecimento.IdEstabelecimento });
        }

    }
}