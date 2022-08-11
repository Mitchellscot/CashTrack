using CashTrack.Data;
using CashTrack.Data.Entities;
using CashTrack.Repositories.Common;
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
using CashTrack.Repositories.TagRepository;
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
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Routing;
using CashTrack.Common;
using System.Diagnostics;
using System.Collections.Generic;

namespace CashTrack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //basically .NET 6 with in a Main method with a startup class
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = ConfigureConfiguration(builder, builder.Environment);
            ConfigureThirdPartyServices(builder.Services, connectionString);
            ConfigureServices(builder.Services);

            var app = builder.Build();
            app.Logger.LogInformation($"Using connection string: {connectionString}");

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
            app.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());
            app.AddAutoMapper(typeof(Program));
            app.AddDbContext<AppDbContext>(o =>
            {
                o.UseSqlServer(connectionString);
                o.EnableSensitiveDataLogging(true);
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
            services.AddTransient<IExpenseRepository, ExpenseRepository>();
            services.AddTransient<IExpenseService, ExpenseService>();
            services.AddTransient<IMerchantRepository, MerchantRepository>();
            services.AddTransient<IMerchantService, MerchantService>();
            services.AddTransient<ITagRepository, TagRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ISubCategoryRepository, SubCategoryRepository>();
            services.AddTransient<ISubCategoryService, SubCategoryService>();
            services.AddTransient<IMainCategoriesService, MainCategoriesService>();
            services.AddTransient<IMainCategoriesRepository, MainCategoriesRepository>();
            services.AddTransient<IIncomeCategoryService, IncomeCategoryService>();
            services.AddTransient<IIncomeCategoryRepository, IncomeCategoryRepository>();
            services.AddTransient<IIncomeSourceRepository, IncomeSourceRepository>();
            services.AddTransient<IIncomeSourceService, IncomeSourceService>();
            services.AddTransient<IIncomeRepository, IncomeRepository>();
            services.AddTransient<IIncomeService, IncomeService>();
            services.AddTransient<IImportRulesRepository, ImportRulesRepository>();
            services.AddTransient<IRepository<ExpenseReviewEntity>, ExpenseReviewRepository>();
            services.AddTransient<IExpenseReviewService, ExpenseReviewService>();
            services.AddTransient<IIncomeReviewService, IncomeReviewService>();
            services.AddTransient<IIncomeReviewRepository, IncomeReviewRepository>();
            services.AddTransient<IExpenseReviewRepository, ExpenseReviewRepository>();
            services.AddTransient<IImportService, ImportService>();
            services.AddTransient<IExportService, ExportService>();
            services.AddTransient<IExportRepository, ExportRepository>();
        }
        private static void ConfigureMiddleware(IApplicationBuilder app, IServiceProvider services, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
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
