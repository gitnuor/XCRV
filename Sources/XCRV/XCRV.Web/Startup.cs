using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;
using XCRV.Infrastructure;
using XCRV.Web.Helpers;

namespace XCRV.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //string connectionString = Configuration.GetConnectionString("SqlCRMConnection");
            //var columnOptions = new ColumnOptions
            //{
            //    AdditionalColumns = new System.Collections.ObjectModel.Collection<SqlColumn>
            //   {
            //       new SqlColumn("UserName", System.Data.SqlDbType.NVarChar)
            //     }
            //};

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                // .WriteTo.MSSqlServer(connectionString, sinkOptions: new MSSqlServerSinkOptions { TableName = "Xcrv_Log" }
                //, null, null, Serilog.Events.LogEventLevel.Information, null, columnOptions: columnOptions, null, null)
                .CreateLogger();

            Log.Information("Starting up");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDetection();

            services.AddInfrastructure();
            services.AddControllersWithViews();


            services.AddSession(options => {
                options.IdleTimeout = System.TimeSpan.FromSeconds(3600);
            });

            services.AddMvc(o =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                o.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = PathString.FromUriComponent("/Home/Index");
                        options.LogoutPath = PathString.FromUriComponent("/Home/Logout");
                    }
                    );

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();


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

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                await next();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseDetection();

            app.UseRouting();

            //app.Use(async (httpContext, next) =>
            //{
            //    var userName = httpContext.User.Identity.IsAuthenticated ? httpContext.User.Identity.Name : "Guest"; //Gets user Name from user Identity  
            //    Serilog.Context.LogContext.PushProperty("Username", userName); //Push user in LogContext;  
            //    await next.Invoke();
            //}
            //);


            app.UseMiddleware<Middleware.SerilogMiddleware>();


            app.UseAuthentication();
            app.UseAuthorization();

            var antiforgery = app.ApplicationServices.GetRequiredService<IAntiforgery>();//app.Services.GetRequiredService<IAntiforgery>();

            app.UseStatusCodePages(context => {
                var response = context.HttpContext.Response;
                if (response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == (int)System.Net.HttpStatusCode.Forbidden)
                    response.Redirect("/Error/UnauthorizedPage");
                return Task.CompletedTask;
            });

            app.Use((context, next) =>
            {
                var requestPath = context.Request.Path.Value;

                if (string.Equals(requestPath, "/", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(requestPath, "/Home/Index", StringComparison.OrdinalIgnoreCase))
                {
                    var tokenSet = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN", tokenSet.RequestToken!,
                        new CookieOptions { HttpOnly = true, Secure = true });
                }

                return next(context);
            });

           
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
                HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.Always
            };
            app.UseCookiePolicy(cookiePolicyOptions);
            //app.UseMiddleware<ValidateAntiForgeryTokenMiddleware>();
        }
    }
}
