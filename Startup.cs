using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetOAuthLinkedIn
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options => {
                options.LoginPath = "/signin";
                options.LogoutPath = "/signout";
            })
            .AddLinkedIn(options =>
            {
                options.ClientId = Configuration["LinkedIn:ClientId"];
                options.ClientSecret = Configuration["LinkedIn:ClientSecret"];
                // your scopes may vary, see https://developer.linkedin.com/docs/oauth2#scopes
                options.Scope.Add("r_liteprofile");
                options.Scope.Add("r_emailaddress");
                options.Scope.Add("r_organization_social");
                options.Scope.Add("w_organization_social");
                options.Scope.Add("rw_organization_admin");
                options.SaveTokens = true; //<-- this is the important line if you wat to use the tokens
            })
            .AddGitHub(options =>
            {
                options.ClientId = Configuration["GitHub:ClientId"];
                options.ClientSecret = Configuration["GitHub:ClientSecret"];
                options.EnterpriseDomain = Configuration["GitHub:EnterpriseDomain"];
                options.Scope.Add("user:email"); // --> we just want to log these in as a test
                options.SaveTokens = true;  //<-- this is the important line if you wat to use the tokens
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
