using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using CashTrack.Data;
using FluentValidation.AspNetCore;
using CashTrack.Repositories.ExpenseRepository;
using CashTrack.Repositories.UserRepository;
using CashTrack.Repositories.TagRepository;
using Microsoft.Extensions.Logging;
using CashTrack.Repositories.MerchantRepository;
using CashTrack.Services.MerchantService;
using CashTrack.Services.ExpenseService;
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
using Microsoft.AspNetCore.Identity;
using CashTrack.Services.UserService;
using CashTrack.Repositories.ImportRuleRepository;
using CashTrack.Services.ImportService;
using CashTrack.Services.ExportService;
using CashTrack.Repositories.ExportRepository;

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

            if (_env.IsDevelopment())
            {
                //for ef core logging
                services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConsole()
                        .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
                    loggingBuilder.AddDebug();
                });

            }
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
                if (_env.IsDevelopment())
                    options.EnableSensitiveDataLogging(true);
            });

            services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();
            services.ConfigureApplicationCookie(o => o.LoginPath = "/login");

            services.AddIdentityCore<UserEntity>(options =>
            {
                options.Stores.MaxLengthForKeys = 36;
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequiredLength = 4;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddSignInManager<SignInManager<UserEntity>>();

            services.AddAutoMapper(typeof(Startup));

            services.AddRazorPages()
                    .AddFluentValidation(fv =>
                fv.RegisterValidatorsFromAssemblyContaining<Startup>()
            );

            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<IMerchantService, MerchantService>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
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
            services.AddScoped<IImportRulesRepository, ImportRulesRepository>();
            services.AddScoped<IRepository<ExpenseReviewEntity>, ExpenseReviewRepository>();
            services.AddScoped<IExpenseReviewService, ExpenseReviewService>();
            services.AddScoped<IIncomeReviewService, IncomeReviewService>();
            services.AddScoped<IIncomeReviewRepository, IncomeReviewRepository>();
            services.AddScoped<IExpenseReviewRepository, ExpenseReviewRepository>();
            services.AddScoped<IImportService, ImportService>();
            services.AddTransient<IExportService, ExportService>();
            services.AddTransient<IExportRepository, ExportRepository>();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //both are for https
                app.UseExceptionHandler("/Error");
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages().RequireAuthorization();
                endpoints.MapControllers().RequireAuthorization();
            });
        }
    }
}