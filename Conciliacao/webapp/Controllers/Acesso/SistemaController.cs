#region Using

using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Conciliacao.Helper.Interfaces;
using Conciliacao.Models;
using Conciliacao.Helper.Rest;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System.Security.Principal;
using ConciliacaoModelo.model.adm;

#endregion

namespace Conciliacao.Controllers.Acesso
{
    [Authorize]
    public class SistemaController : Controller
    {

        static readonly IUsuariosRestClient _restClient = new UsuariosRestClient();

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return System.Web.HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        // TODO: This should be moved to the constructor of the controller in combination with a DependencyResolver setup
        // NOTE: You can use NuGet to find a strategy for the various IoC packages out there (i.e. StructureMap.MVC5)
        // GET: /account/forgotpassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();

            return View("~/views/acesso/passwordesquecido.cshtml");
        }

        // GET: /account/login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string returnUrl)
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();

            // Store the originating URL so we can attach it to a form field
            var viewModel = new AccountLoginModel { ReturnUrl = returnUrl };

            return View("~/views/acesso/login.cshtml",viewModel);
        }

        // POST: /account/login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(AccountLoginModel viewModel)
        {
            // Ensure we have a valid viewModel to work with
            if (!ModelState.IsValid)
                return View("~/views/acesso/login.cshtml", viewModel);

            Usuario usuario = _restClient.GetByEmailSenha(viewModel.Email,viewModel.Password);
            // Prepare the identity with the provided information
            if ((usuario == null) || ((usuario.Email==null)))
            {// No existing user was found that matched the given criteria
                ModelState.AddModelError("", @"Usuário ou senha inválidos.");
                return View("~/views/acesso/login.cshtml", viewModel);
            }

            var user = new UsuarioApp
            {
                UserName = usuario.NomeDoUsuario,
                Email = usuario.Email,
                Id = usuario.Id.ToString(CultureInfo.InvariantCulture),
                IdConta = usuario.IdConta,
                Perfil = usuario.perfil,
                SPRUsuario = usuario.sprusuario,
                ChaveConta = usuario.ds_chave ?? string.Empty
            };

            // Verify if a user exists with the provided identity information
            //var user = await _manager.FindByEmailAsync(viewModel.Email);

            // Then create an identity for it and sign it in
            await SignInAsync(user, viewModel.RememberMe);
            // If the user came from a specific page, redirect back to it
            
            return RedirectToLocal(viewModel.ReturnUrl);
        }

        // GET: /account/error
        [AllowAnonymous]
        public ActionResult Error()
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();

            return View();
        }

        // GET: /account/register
        [AllowAnonymous]
        public ActionResult Registro()
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();

            return View("~/views/acesso/registro.cshtml", new AccountRegistrationModel());
        }

        // POST: /account/register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Registro(AccountRegistrationModel viewModel)
        {

            Usuario getUsuario = _restClient.GetByEmailSenha(viewModel.Email, viewModel.Password);
            if (getUsuario.Id > 0)
            {
                ModelState.AddModelError("", @"Usuário já existe.");
                return View("~/views/acesso/registro.cshtml", viewModel);
            }

            if (!ModelState.IsValid)
                return View("~/views/acesso/registro.cshtml", viewModel);
            
            var conta = new Conta
            {
                DsLogin = viewModel.Email
            };

            // Try to create a user with the given identity
            try
            {
                long id = 1;// DAL.Gravar(conta);
                  var oUsuario = new Usuario
                  {
                     // IdConta = id,
                      Email = viewModel.Email,
                      Password = viewModel.Password,
                      NomeDoUsuario = viewModel.Username
                  };
                long idUsuario = 1;// DAL.Gravar(usuario);

                  var user = new UsuarioApp
                  {
                      Id = idUsuario.ToString(),
                      IdConta = id, 
                      UserName = viewModel.Email,
                      Email = viewModel.Email,
                      Perfil = viewModel.Perfil
                  };

                /* var result = await _manager.CreateAsync(user, viewModel.Password);*/

                // If the user could not be created
                /*  if (!result.Succeeded)
                  {
                      // Add all errors to the page so they can be used to display what went wrong
                      AddErrors(result);

                      return View(viewModel);
                  }*/

                // If the user was able to be created we can sign it in immediately
                // Note: Consider using the email verification proces
                await SignInAsync(user, false);

                return RedirectToLocal();
            }
            catch (System.Exception ex)
            {
                //Add all errors to the page so they can be used to display what went wrong
                AddErrors(ex);

                return View(viewModel);
            }
        }

        // POST: /account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);

            // Second we clear the principal to ensure the user does not retain any authentication
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

            return RedirectToLocal();
        }

        private ActionResult RedirectToLocal(string returnUrl = "")
        {
            // If the return url starts with a slash "/" we assume it belongs to our site
            // so we will redirect to this "action"
            if (!returnUrl.IsNullOrWhiteSpace() && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // If we cannot verify if the url is local to our host we redirect to a default location
            return RedirectToAction("index", "home");
        }

        private void AddErrors(System.Exception result)
        {
            // Add all errors that were returned to the page error collection
            ModelState.AddModelError("", result.ToString());
        }

        private void AddErrors(IdentityResult result)
        {
            // Add all errors that were returned to the page error collection
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action
            if (Request.IsAuthenticated)
                Logout();
        }

        private async Task SignInAsync(UsuarioApp user, bool isPersistent)
        {
            var claims = new List<Claim>();
            // create *required* claims
            claims.Add(new Claim(ClaimTypes.GroupSid, user.IdConta.ToString()));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Role, user.Perfil));
            claims.Add(new Claim(ClaimTypes.Sid, user.SPRUsuario.ToString()));
            claims.Add(new Claim(ClaimTypes.Authentication, user.ChaveConta));
            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            // add to user here!
            AuthenticationManager.SignIn(new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                ExpiresUtc = System.DateTime.UtcNow.AddDays(120)
            }, identity);
        
        }

        // GET: /account/lock
        [AllowAnonymous]
        public ActionResult Lock()
        {
            return View();
        }
    }
}