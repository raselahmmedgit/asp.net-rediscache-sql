using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GCSideLoading.Web.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GCSideLoading.Core;
using AspNetCore.Identity.DocumentDb;
using Microsoft.AspNetCore.HttpOverrides;
using GCSideLoading.Services;
using GCSideLoading.Core.BLL;
using Microsoft.Azure.Documents;
using GCSideLoading.Core.EntityModel;
using GCSideLoading.Core.DAL;
using GCSideLoading.Core.Util;
using GCSideLoading.Core.Helper.CronJobHelper;
using GCSideLoading.Core.BLL.Redis;

namespace GCSideLoading.Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            // Add framework services.

            services.AddAuthentication();

            services.AddIdentity<ApplicationUser, DocumentDbIdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = true;
            }).AddRoles<DocumentDbIdentityRole>()
            .AddDocumentDbStores(options =>
            {
                options.Database = Configuration["GCSideLoadingDbConnection:DatabaseId"];
                options.UserStoreDocumentCollection = "AspNetIdentityUsers";
                //options.RoleStoreDocumentCollection = "AspNetIdentityRoles";
            })
            .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            // Add our Config object so it can be injected
            services.Configure<GCSideLoadingDbConnection>(Configuration.GetSection("GCSideLoadingDbConnection"));
            services.Configure<GCSideLoadingConfig>(Configuration.GetSection("GCSideLoadingConfig"));
            services.Configure<GCSideLoadingEmailConfig>(Configuration.GetSection("GCSideLoadingEmailConfig"));
            services.Configure<GCSideLoadingSmsConfig>(Configuration.GetSection("GCSideLoadingSmsConfig"));

            // Add DocumentDb client singleton instance (it's recommended to use a singleton instance for it)
            new GCSideLoadingDbRepository(Configuration);
            var documentClient = GCSideLoadingDbRepository.GetDocumentClient();
            services.AddSingleton<IDocumentClient>(documentClient);

            //AutoMapper registration
            AutoMapperConfiguration.RegisterMapper();

            ////Email Sender
            //new EmailSenderManager(Configuration);

            ////Sms Sender
            //new SmsSenderManager(Configuration);

            ////Google Login
            //services.AddAuthentication().AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = Configuration["GoogleLoginConfig:Authentication:Google:ClientId"];
            //    googleOptions.ClientSecret = Configuration["GoogleLoginConfig:Authentication:Google:ClientSecret"];
            //});

            ////Facebook Login
            //services.AddAuthentication().AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = Configuration["FacebookLoginConfig:Authentication:Facebook:AppId"];
            //    facebookOptions.AppSecret = Configuration["FacebookLoginConfig:Authentication:Facebook:AppSecret"];
            //});

            AppConstants.IsDevelopmentMode = Configuration["GCSideLoadingConfig:IsDevelopmentMode"] == null ? true : bool.Parse(Configuration["GCSideLoadingConfig:IsDevelopmentMode"].ToString());
            if (AppConstants.IsDevelopmentMode)
            {
                AppConstants.BaseUrl = Configuration["GCSideLoadingConfig:DevelopmentBaseUrl"] == null ? "http://localhost:44379/" : Configuration["GCSideLoadingConfig:DevelopmentBaseUrl"].ToString();
            }
            else
            {
                AppConstants.BaseUrl = Configuration["GCSideLoadingConfig:ProductionBaseUrl"] == null ? "http://plugnplay.eastus.cloudapp.azure.com/" : Configuration["ConferenceGamingConfig:ProductionBaseUrl"].ToString();
            }
            //RedisDataReaderManager redisDataReaderManager = new RedisDataReaderManager();
            //redisDataReaderManager.readRedisDataAndStoreCosmosDB();
            AutomaticSideLoadingManager automaticSideLoadingManager = new AutomaticSideLoadingManager();
            automaticSideLoadingManager.RunAutomaticRedisDataReader();
            //services.AddCronJob<AutomaticGameStartCronJobService>(c =>
            //{
            //    c.TimeZoneInfo = TimeZoneInfo.Local;
            //    c.CronExpression = @"* * * * *";
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
