using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace ConciliacaoAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Id_String",
                routeTemplate: "api/{controller}/{id}/{lsString}",
                defaults: new { id = RouteParameter.Optional, lsString = RouteParameter.Optional }
            );

            /*config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.Indent = true;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();*/
        }
    }
}
