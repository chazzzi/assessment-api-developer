using System.Web.Http;
using System.Web.Http.Cors;

namespace assessment_platform_developer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            System.Diagnostics.Debug.WriteLine("Web API Routes Registered");
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }

    }
}
