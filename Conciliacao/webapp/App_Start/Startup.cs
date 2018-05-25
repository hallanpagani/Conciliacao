using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
//using Microsoft.Owin.Security.Google;
using Owin;

[assembly: OwinStartup("ProducaoOwin", typeof(Conciliacao.Startup))]

namespace Conciliacao
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void Configuration(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request

            //app.CreatePerOwinContext(ApplicationDbContext.Create);

            // app.CreatePerOwinContext<UsuarioManager>(UsuarioManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Sistema/Login")//,
                                                            //CookieSecure = CookieSecureOption.Never

                /*Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UsuarioManager, Usuario>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
                */
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            /*app.UseFacebookAuthentication(
               appId: "602838406535923",
               appSecret: "366d2f2f2b07b0c6a5602b9b6b1b9ea4");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "922096522548-9pkl7etndh9njnqhvcki844imt4a6gcv.apps.googleusercontent.com",
                ClientSecret = "ieMj4pW2Xde51Qak-XmdwmXa",
                Provider = new GoogleOAuth2AuthenticationProvider()
            });

            */
        }
    }
}