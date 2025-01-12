using SimpleInjector;
using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using SimpleInjector.Diagnostics;
using System.Web.Compilation;
using System.Web.UI;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using assessment_platform_developer.Services;
using SimpleInjector.Integration.Web;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Http;
using assessment_platform_developer.Models;

namespace assessment_platform_developer
{
    public sealed class PageInitializerModule : IHttpModule
    {
        public static void Initialize()
        {
            DynamicModuleUtility.RegisterModule(typeof(PageInitializerModule));
        }

        void IHttpModule.Init(HttpApplication app)
        {
            app.PreRequestHandlerExecute += (sender, e) =>
            {
                var handler = app.Context.CurrentHandler;
                if (handler != null)
                {
                    string name = handler.GetType().Assembly.FullName;
                    if (!name.StartsWith("System.Web") &&
                        !name.StartsWith("Microsoft"))
                    {
                        Global.InitializeHandler(handler);
                    }
                }
            };
        }

        void IHttpModule.Dispose() { }
    }

    public class Global : HttpApplication
    {
        private static Container container;

        public static void InitializeHandler(IHttpHandler handler)
        {
            var handlerType = handler is Page
                ? handler.GetType().BaseType
                : handler.GetType();
            container.GetRegistration(handlerType, true).Registration
                .InitializeInstance(handler);
        }

        void Application_Start(object sender, EventArgs e)
        {
            // Load environment variables
            string envPath = Server.MapPath("~/.env");
            DotNetEnv.Env.Load(envPath);

            string dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            string dbInstance = Environment.GetEnvironmentVariable("DB_INSTANCE");
            string dbSchema = Environment.GetEnvironmentVariable("DB_SCHEMA");
            string dbUser = Environment.GetEnvironmentVariable("DB_USER");
            string dbPass = Environment.GetEnvironmentVariable("DB_PASS");

            string connectionString = $"Server={dbHost}\\{dbInstance};Database={dbSchema};User Id={dbUser};Password={dbPass};";

            var config = WebConfigurationManager.OpenWebConfiguration("~");
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");

            if (connectionStringsSection.ConnectionStrings["AssessmentDbContext"] == null)
            {
                connectionStringsSection.ConnectionStrings.Add(
                    new ConnectionStringSettings
                    {
                        Name = "AssessmentDbContext",
                        ConnectionString = connectionString,
                        ProviderName = "System.Data.SqlClient"
                    });
            }
            else
            {
                connectionStringsSection.ConnectionStrings["AssessmentDbContext"].ConnectionString = connectionString;
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");

            // Register routes, bundles, and APIs
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configure(WebApiConfig.Register);

            EnableCors();
            Bootstrap();
        }

        private static void Bootstrap()
        {
            // Create the Simple Injector container
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            // Register DbContext and repository
            container.Register<AssessmentDbContext>(Lifestyle.Scoped);
            container.Register<ICustomerRepository, CustomerRepository>(Lifestyle.Scoped);
            container.Register<ICustomerService, CustomerService>(Lifestyle.Scoped); // Added service registration

            // Register Web Pages
            RegisterWebPages(container);

            container.Options.ResolveUnregisteredConcreteTypes = true;

            // Verify container
            container.Verify();

            Global.container = container;

            // Store in application
            HttpContext.Current.Application["DIContainer"] = container;
        }
        private static void EnableCors()
        {
            var config = GlobalConfiguration.Configuration;

            // Enable CORS for all origins, methods, and headers
            config.EnableCors(new System.Web.Http.Cors.EnableCorsAttribute("*", "*", "*"));
        }
        private static void RegisterWebPages(Container container)
        {
            var pageTypes =
                from assembly in BuildManager.GetReferencedAssemblies().Cast<Assembly>()
                where !assembly.IsDynamic
                where !assembly.GlobalAssemblyCache
                from type in assembly.GetExportedTypes()
                where type.IsSubclassOf(typeof(Page))
                where !type.IsAbstract && !type.IsGenericType
                select type;

            foreach (var type in pageTypes)
            {
                var reg = Lifestyle.Transient.CreateRegistration(type, container);
                reg.SuppressDiagnosticWarning(
                    DiagnosticType.DisposableTransientComponent,
                    "ASP.NET creates and disposes page classes for us.");
                container.AddRegistration(type, reg);
            }
        }

    }
}