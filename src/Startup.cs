using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

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
                if (_env.IsDevelopment()) { options.EnableSensitiveDataLogging(true); }
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;

            }).AddIdentityCookies();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
            });

            //see what is in identity core vs add identity. Can also do adddefault identity https://stackoverflow.com/questions/55361533/addidentity-vs-addidentitycore
            services.AddIdentityCore<Users>(options =>
            {
                options.Stores.MaxLengthForKeys = 36;
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequiredLength = 4;
                options.Password.RequireDigit = false;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddSignInManager<SignInManager<Users>>();

            services.AddAutoMapper(typeof(Startup));

            services.AddRazorPages()
                    .AddFluentValidation(fv =>
                fv.RegisterValidatorsFromAssemblyContaining<Startup>()
            );

            services.AddTransient<ICurrentUserService, ContextUserService>();
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<IMerchantService, MerchantService>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IUserService, UserService>();
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