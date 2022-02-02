using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CashTrack.Common;
using Microsoft.EntityFrameworkCore;
using CashTrack.Data;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.UserRepository;
using CashTrack.Repositories.TagRepository;
using CashTrack.Services.AuthenticationService;
using Microsoft.Extensions.Logging;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Services.MerchantService;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.UserService;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.SubCategoryService;
using CashTrack.Services.MainCategoriesService;
using CashTrack.Repositories.MainCategoriesRepository;
using CashTrack.Services.IncomeCategoryService;
using CashTrack.Repositories.IncomeCategoryRepository;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Services.IncomeSourceService;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Services.IncomeService;
using CashTrack.Repositories.Common;
using CashTrack.Data.Entities;
using CashTrack.Repositories.ExpenseReviewRepository;
using CashTrack.Services.ExpenseReviewService;
using CashTrack.Services.IncomeReviewService;
using CashTrack.Repositories.IncomeReviewRepository;

namespace CashTrack
{
    public class Startup
    {
        public readonly IConfiguration _config;

        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _config = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string db = "DefaultConnection";
            if (_env.IsEnvironment("Test"))
                db = "TestDb";

            string connectionString = _config.GetConnectionString(db);
            Console.WriteLine($"Using connection string: {connectionString}");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.EnableSensitiveDataLogging(true);
            });

            //for ef core logging
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole()
                    .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
                loggingBuilder.AddDebug();
            });

            //needed for webpack proxy - remove in prod
            services.AddCors();

            services.AddAutoMapper(typeof(Startup));

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(CustomValidationFilter));
            })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<IMerchantService, MerchantService>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<ISubCategoryService, SubCategoryService>();
            services.AddScoped<IMainCategoriesService, MainCategoriesService>();
            services.AddScoped<IMainCategoriesRepository, MainCategoriesRepository>();
            services.AddScoped<IIncomeCategoryService, IncomeCategoryService>();
            services.AddScoped<IIncomeCategoryRepository, IncomeCategoryRepository>();
            services.AddScoped<IIncomeSourceRepository, IncomeSourceRepository>();
            services.AddScoped<IIncomeSourceService, IncomeSourceService>();
            services.AddScoped<IIncomeRepository, IncomeRepository>();
            services.AddScoped<IIncomeService, IncomeService>();
            services.AddScoped<IRepository<ExpenseReview>, ExpenseReviewRepository>();
            services.AddScoped<IExpenseReviewService, ExpenseReviewService>();
            services.AddScoped<IIncomeReviewService, IncomeReviewService>();
            services.AddScoped<IRepository<IncomeReview>, IncomeReviewRepository>();

            services.Configure<AppSettings>(_config.GetSection("AppSettings"));

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
           {
               configuration.RootPath = "./ClientApp/build";
           });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader());

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
           {
               spa.Options.SourcePath = "ClientApp";

               if (_env.IsDevelopment())
               {
                   spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                   spa.UseReactDevelopmentServer(npmScript: "start");
               }
           });
        }
    }
}
