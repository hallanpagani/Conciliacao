#region Using

using Conciliacao.App_Helpers;
using System;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Conciliacao.Controllers;

#endregion

namespace Conciliacao
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            IdentityConfig.RegisterIdentities();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Email;

            ModelBinders.Binders.Add(
                typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(
                typeof(decimal?), new DecimalModelBinder());
        }

        protected void Application_BeginRequest()
        {
           // if ((!Context.Request.IsSecureConnection) && (!Context.Request.IsLocal))
           //     Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var app = (MvcApplication)sender;
            var context = app.Context;
            var ex = app.Server.GetLastError();
            context.Response.Clear();
            context.ClearError();

            var httpException = ex as HttpException;

            var routeData = new RouteData();
            routeData.Values["controller"] = "errors";
            routeData.Values["exception"] = ex;
            routeData.Values["action"] = "http500";

            if (httpException != null)
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        routeData.Values["action"] = "http404";
                        break;
                    case 500:
                        routeData.Values["action"] = "http500";
                        break;
                }
            }

            IController controller = new ErrorsController();
            controller.Execute(new RequestContext(new HttpContextWrapper(context), routeData));
        }

    }
}