
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;



// https://stackoverflow.com/questions/38247080/using-razor-outside-of-mvc-in-net-core
// https://github.com/toddams/RazorLight

// https://stackoverflow.com/questions/45755871/create-instance-using-ctor-injection-and-serviceprovider
// Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(app.ApplicationServices, typeof(myclass));

namespace RasterTileServer
{

    /*
HA-Proxy version 2.0.13-2ubuntu0.2 2021/08/16 - https://haproxy.org/

https://github.com/haproxytech/dataplaneapi

https://www.haproxy.com/blog/announcing-haproxy-dataplane-api-20/
https://www.haproxy.com/blog/announcing-haproxy-data-plane-api-2-2/
https://www.haproxy.com/blog/new-haproxy-data-plane-api/

https://stackoverflow.com/questions/29863064/haproxy-dynamic-acl

     */



    // https://mcguirev10.com/2020/01/12/logging-during-application-startup.html
    public class Startup
    {

        public IConfiguration Configuration { get; }



        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                // options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);


            services.Configure<Microsoft.AspNetCore.Routing.RouteOptions>(options =>
                { }
                
            );



            services.AddRazorPages();
        } // End Sub ConfigureServices 


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // string virtual_directory = "/Virt_DIR";
            string virtual_directory = "/";

            if (virtual_directory.EndsWith("/"))
                virtual_directory = virtual_directory.Substring(0, virtual_directory.Length - 1);

            // Don't map if you don't have to 
            // (wonder what the framework does or does not do for that case)
            if (string.IsNullOrWhiteSpace(virtual_directory))
                ConfigureVirtual(app, env, loggerFactory); 
            else
                app.Map(virtual_directory, 
                    delegate (IApplicationBuilder mappedApp)
                    {
                        ConfigureVirtual(mappedApp, env, loggerFactory);
                    }
                );
        } // End Sub Configure 


        public void ConfigureVirtual(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();




            DefaultFilesOptions dfo = new DefaultFilesOptions();
            dfo.DefaultFileNames.Clear();
            dfo.DefaultFileNames.Add("index.htm");
            dfo.DefaultFileNames.Add("index.html");

            app.UseDefaultFiles(dfo);


            app.UseStaticFiles(
                new Microsoft.AspNetCore.Builder.StaticFileOptions()
                {
                    ServeUnknownFileTypes = true,
                    DefaultContentType = "application/octet-stream",
                    ContentTypeProvider = new ExtensionContentTypeProvider(),

                    OnPrepareResponse = delegate (Microsoft.AspNetCore.StaticFiles.StaticFileResponseContext context)
                    {
                        // https://stackoverflow.com/questions/49547/how-do-we-control-web-page-caching-across-all-browsers

                        // The Cache-Control is per the HTTP 1.1 spec for clients and proxies
                        // If you don't care about IE6, then you could omit Cache-Control: no-cache.
                        // (some browsers observe no-store and some observe must-revalidate)
                        context.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate, max-age=0";
                        // Other Cache-Control parameters such as max-age are irrelevant 
                        // if the abovementioned Cache-Control parameters (no-cache,no-store,must-revalidate) are specified.


                        // Expires is per the HTTP 1.0 and 1.1 specs for clients and proxies. 
                        // In HTTP 1.1, the Cache-Control takes precedence over Expires, so it's after all for HTTP 1.0 proxies only.
                        // If you don't care about HTTP 1.0 proxies, then you could omit Expires.
                        context.Context.Response.Headers["Expires"] = "-1, 0, Tue, 01 Jan 1980 1:00:00 GMT";

                        // The Pragma is per the HTTP 1.0 spec for prehistoric clients, such as Java WebClient
                        // If you don't care about IE6 nor HTTP 1.0 clients 
                        // (HTTP 1.1 was introduced 1997), then you could omit Pragma.
                        context.Context.Response.Headers["pragma"] = "no-cache";


                        // On the other hand, if the server auto-includes a valid Date header, 
                        // then you could theoretically omit Cache-Control too and rely on Expires only.

                        // Date: Wed, 24 Aug 2016 18:32:02 GMT
                        // Expires: 0

                        // But that may fail if e.g. the end-user manipulates the operating system date 
                        // and the client software is relying on it.
                        // https://stackoverflow.com/questions/21120882/the-date-time-format-used-in-http-headers
                    } // End Sub OnPrepareResponse 

                }
            );



            // app.UseCookiePolicy();

            app.UseRouting();



            // https://stackoverflow.com/questions/60791843/changing-routedata-in-asp-net-core-3-1-in-middleware
            // Note: Sequence matters ! After app.UseRouting, but before app.UseEndpoints
            app.Use(
                async delegate (Microsoft.AspNetCore.Http.HttpContext context, System.Func<System.Threading.Tasks.Task> next)
                {
                    string url = context.Request.Headers["HOST"];
                    string[] splittedUrl = url.Split('.');

                    if (splittedUrl != null && (splittedUrl.Length > 0))
                    {
                        context.GetRouteData().Values.Add("Host", splittedUrl[0]);
                        context.Items["Host2"] = url;

                        //foreach (System.Collections.Generic.KeyValuePair<System.Type, object> kvp in context.Features)
                        //{
                        //    System.Console.WriteLine(kvp.Key.FullName);
                        //}

                    } // End if (splittedUrl != null && (splittedUrl.Length > 0)) 

                    // Call the next delegate/middleware in the pipeline
                    await next();
                }
            );


            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();


                // https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-controller?view=aspnetcore-5.0&tabs=visual-studio
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


        } // End Sub ConfigureVirtual 


    } // End Class Startup 


} // End Namespace RasterTileServer 
