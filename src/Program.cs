using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.ExpenseReviewRepository;
using CashTrack.Repositories.ExportRepository;
using CashTrack.Repositories.ImportRuleRepository;
using CashTrack.Repositories.IncomeCategoryRepository;
using CashTrack.Repositories.IncomeRepository;
using CashTrack.Repositories.IncomeReviewRepository;
using CashTrack.Repositories.IncomeSourceRepository;
using CashTrack.Repositories.MainCategoriesRepository;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Repositories.SubCategoriesRepository;
using CashTrack.Services.ImportRulesService;
using CashTrack.Repositories.UserRepository;
using CashTrack.Services.ExpenseReviewService;
using CashTrack.Services.ExpenseService;
using CashTrack.Services.ExportService;
using CashTrack.Services.ImportService;
using CashTrack.Services.IncomeCategoryService;
using CashTrack.Services.IncomeReviewService;
using CashTrack.Services.IncomeService;
using CashTrack.Services.IncomeSourceService;
using CashTrack.Services.MainCategoriesService;
using CashTrack.Services.MerchantService;
using CashTrack.Services.SubCategoryService;
using CashTrack.Services.UserService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Routing;
using CashTrack.Common;
using FluentValidation;
using CashTrack.Repositories.BudgetRepository;
using CashTrack.Services.BudgetService;
using CashTrack.Services.SummaryService;

namespace CashTrack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = ConfigureConfiguration(builder, builder.Environment);
            ConfigureThirdPartyServices(builder.Services, connectionString);
            ConfigureServices(builder.Services);

            var app = builder.Build();
            app.Logger.LogInformation($"Using connection string: {connectionString} and environment {builder.Environment.EnvironmentName}");

            ConfigureMiddleware(app, app.Services, app.Environment);
            ConfigureEndpoints(app, app.Services);

            app.Run();

        }
        private static string ConfigureConfiguration(WebApplicationBuilder app, IWebHostEnvironment env)
        {
            //binds appsettings so I can use with IOptions in other parts of the application
            app.Services.Configure<AppSettingsOptions>(app.Configuration.GetSection(AppSettingsOptions.AppSettings));
            return app.Configuration[$"AppSettings:ConnectionStrings:{env.EnvironmentName}"];
        }

        private static void ConfigureThirdPartyServices(IServiceCollection app, string connectionString)
        {
            app.AddRazorPages();
            app.AddValidatorsFromAssemblyContaining<Program>();
            app.AddAutoMapper(typeof(Program));
            app.AddDbContext<AppDbContext>(o =>
            {
                o.UseSqlServer(connectionString);
                //o.EnableSensitiveDataLogging(true);
            });
            app.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();
            app.ConfigureApplicationCookie(o => o.LoginPath = "/login");
            app.AddIdentityCore<UserEntity>(o =>
            {
                o.Stores.MaxLengthForKeys = 36;
                o.SignIn.RequireConfirmedAccount = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireLowercase = false;
                o.Password.RequiredUniqueChars = 0;
                o.Password.RequiredUniqueChars = 0;
                o.Password.RequiredLength = 4;
                o.Password.RequireDigit = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Lockout.MaxFailedAccessAttempts = 3;
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddSignInManager<SignInManager<UserEntity>>();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IMainCategoriesRepository, MainCategoriesRepository>();
            services.AddScoped<IIncomeCategoryRepository, IncomeCategoryRepository>();
            services.AddScoped<IIncomeSourceRepository, IncomeSourceRepository>();
            services.AddScoped<IIncomeRepository, IncomeRepository>();
            services.AddScoped<IImportRulesRepository, ImportRulesRepository>();
            services.AddScoped<IIncomeReviewRepository, IncomeReviewRepository>();
            services.AddScoped<IExpenseReviewRepository, ExpenseReviewRepository>();
            services.AddScoped<IExportRepository, ExportRepository>();
            services.AddScoped<IBudgetRepository, BudgetRepository>();

            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IMerchantService, MerchantService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISubCategoryService, SubCategoryService>();
            services.AddScoped<IMainCategoriesService, MainCategoriesService>();
            services.AddScoped<IIncomeCategoryService, IncomeCategoryService>();
            services.AddScoped<IIncomeSourceService, IncomeSourceService>();
            services.AddScoped<IIncomeService, IncomeService>();
            services.AddScoped<IImportService, ImportService>();
            services.AddScoped<IImportRulesService, ImportRulesService>();
            services.AddScoped<IExpenseReviewService, ExpenseReviewService>();
            services.AddScoped<IIncomeReviewService, IncomeReviewService>();
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<IBudgetService, BudgetService>();
            services.AddScoped<ISummaryService, SummaryService>();

        }
        private static void ConfigureMiddleware(IApplicationBuilder app, IServiceProvider services, IWebHostEnvironment env)
        {
            if (env.IsProduction())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
        }
        private static void ConfigureEndpoints(IEndpointRouteBuilder app, IServiceProvider services)
        {
            app.MapRazorPages().RequireAuthorization();
            app.MapControllers().RequireAuthorization();
        }
    }
}
