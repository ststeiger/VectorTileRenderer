
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace RasterTileServer
{


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

            app.UseStaticFiles();
            // app.UseCookiePolicy();

            app.UseRouting();

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
