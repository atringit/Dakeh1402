using Dake.Service;
using Dake.Service.Interface;
using Jumbula.WebSite.Utilities.Captcha;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Pushe.co;
using System;

namespace Dake
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
            CaptchaConstant.SecretKey = Configuration.GetSection("GoogleRecaptcha:Secret_Key").Value;

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            #region Authentication

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddCookie(options =>
            {
                options.LoginPath = "/User/Login";
                options.LogoutPath = "/Logout";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(43200);

            });

            ///services.AddHttpContextAccessor();


            #endregion
            services.AddMvc().AddJsonOptions(opt =>
                                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            //.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            #region DataBaseContext
            services.AddDbContext<Dake.DAL.Context>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DakeConnection"));
            }

            );
            #endregion
            #region IOc
            services.AddTransient<INotice, noticeService>();
            services.AddTransient<ISlider, sliderService>();
            services.AddTransient<IUser, userService>();
            services.AddTransient<IFactor, factorService>();
            services.AddTransient<IInformation, informationService>();
            services.AddTransient<Icategory, categoryService>();
            services.AddTransient<ICity, cityService>();
            services.AddTransient<IReportNotice, reportNoticeService>();
            services.AddTransient<IStaticPrice, staticPriceService>();
            services.AddTransient<IDiscountCode, discountCodeService>();
            services.AddTransient<IMessage, messageService>();
            services.AddTransient<IPushNotificationService, PushNotificationService>();

            services.AddScoped<IBannerSevice, BannerSevice>();
            services.AddScoped<IPaymentService, PaymentService>();
            #endregion

            services.AddCors(opt => opt.AddPolicy(name: "AllowOrigin",
                builder => builder.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin()));


            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = false;
            });

            //services.AddPushe(options =>
            //{
            //    options.AccessToken = "60692d4b8a2da08e82da03e598f40d26c71986a7";
            //});

            // services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = long.MaxValue);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            FileExtensionContentTypeProvider contentTypes = new FileExtensionContentTypeProvider();
            contentTypes.Mappings[".apk"] = "application/vnd.android.package-archive";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = contentTypes
            });
            app.UseHttpsRedirection();
            app.UseHsts();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                System.IO.Path.Combine(env.WebRootPath, "react")),
                RequestPath = ""
            });
            //app.UseDefaultFiles();
            app.UseAuthentication();
            //app.UseMvcWithDefaultRoute();
            app.UseSession();

          app.UseCors(bulider=> bulider.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "areas",
                //    template: "{area:exists}/{controller=User}/{action=Login}/{id?}"

                //);
                //routes.MapRoute("Default", "{controller=User}/{action=Login}/{id?}");
                routes.MapRoute("Default", "{controller=Home2}/{action=Index}/{id?}");
                routes.MapRoute("ActionApi", "api/{controller}/{name?}");

                //routes.MapSpaFallbackRoute(
                //    name: "spa-fallback",
                //    defaults: new { controller = "Home", action = "Index" });
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Error!!");
            });
        }
    }
}
