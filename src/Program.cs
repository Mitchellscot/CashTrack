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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Routing;
using CashTrack.Common;
using FluentValidation;
using CashTrack.Repositories.BudgetRepository;
using CashTrack.Services.BudgetService;
using CashTrack.Services.SummaryService;
using CashTrack.Common.Middleware;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;
using System.IO;
using CashTrack.Repositories.ImportRepository;
using CashTrack.Services.ImportProfileService;
using ElectronNET.API;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using ElectronNET.API.Entities;

namespace CashTrack
{
    public class Program
    {
        private static string _env { get; set; }
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            _env = builder.Environment.EnvironmentName;

            var connectionString = ConfigureConfiguration(builder);
            ConfigureServices(builder, connectionString);
            ConfigureAppServices(builder.Services);

            if (IsElectron())
            {
                builder.WebHost.UseElectron(args);
                builder.Services.AddElectron();
            }

            var app = builder.Build();

            ConfigureMiddleware(app);
            if (IsProduction())
                ConfigureProductionEndpoints(app);
            else
                ConfigureEndpoints(app);


            app.Logger.LogInformation($"Using environment: {_env}");
            app.Logger.LogInformation($"Listening on {string.Join(", ", app.Urls)}");

            if (IsElectron())
            {
                await app.StartAsync();
                var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
                {
                    Width = 1400,
                    Height = 895,
                    Center = true,
                    MinWidth = 375,
                    MinHeight = 1400,
                    MaxHeight = 1400,
                    MaxWidth = 1500,
                    Title = "CashTrack",
                    Icon = Path.Join(Directory.GetCurrentDirectory(), "wwwroot", "favicon", "favicon.ico"),
                    AutoHideMenuBar = true,
                    BackgroundColor = "#2b3c4c",
                    TitleBarStyle = TitleBarStyle.customButtonsOnHover,
                    WebPreferences = new WebPreferences()
                    {
                        DevTools = true
                    }
                });
                window.OnClosed += () =>
                {
                    Electron.App.Quit();
                };
                await window.WebContents.Session.ClearCacheAsync();
                window.OnReadyToShow += () => window.Show();
                app.WaitForShutdown();
            }
            else
                app.Run();
        }
        private static string ConfigureConfiguration(WebApplicationBuilder app)
        {
            app.Services.Configure<AppSettingsOptions>(app.Configuration.GetSection(AppSettingsOptions.AppSettings));

            return IsProduction() ? $"Data Source={Path.Join(Directory.GetCurrentDirectory(), app.Configuration[$"AppSettings:ConnectionStrings:{_env}"])}" : $"Data Source={Path.Join(Directory.GetCurrentDirectory(), "Data", app.Configuration[$"AppSettings:ConnectionStrings:{_env}"])}";
        }

        private static void ConfigureServices(WebApplicationBuilder app, string connectionString)
        {
            app.Services.AddRazorPages();
            app.Services.AddValidatorsFromAssemblyContaining<Program>();
            app.Services.AddAutoMapper(typeof(Program));

            app.Services.AddDbContext<AppDbContext>(o =>
            {
                o.UseSqlite(connectionString);
            });

            app.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();
            app.Services.ConfigureApplicationCookie(o => o.LoginPath = "/login");
            app.Services.AddIdentityCore<UserEntity>(o =>
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

            app.Services.Configure<RouteOptions>(o =>
            {
                o.LowercaseUrls = true;
                o.LowercaseQueryStrings = true;
            });
        }

        private static void ConfigureAppServices(IServiceCollection services)
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
            services.AddScoped<IImportProfileRepository, ImportProfileRepository>();
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
            services.AddScoped<IImportProfileService, ImportProfileService>();
            services.AddScoped<IExpenseReviewService, ExpenseReviewService>();
            services.AddScoped<IIncomeReviewService, IncomeReviewService>();
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<IBudgetService, BudgetService>();
            services.AddScoped<ISummaryService, SummaryService>();

            services.AddScoped<IpAddressMiddleware>();

        }
        private static void ConfigureMiddleware(IApplicationBuilder app)
        {
            //Forwards request headers from nginx proxy to the app
            if (IsDocker())
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,

                });
            }
            app.UseStaticFiles();
            if (IsProduction())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
                app.UseMiddleware<IpAddressMiddleware>();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
        }
        private static void ConfigureEndpoints(IEndpointRouteBuilder app)
        {
            app.MapRazorPages().RequireAuthorization();
            app.MapControllers().RequireAuthorization();
        }
        private static void ConfigureProductionEndpoints(IEndpointRouteBuilder app)
        {
            app.MapRazorPages();
            app.MapControllers();
        }

        private static bool UseHttp()
            => _env.Equals(CashTrackEnv.Development, StringComparison.CurrentCultureIgnoreCase) || _env.Equals(CashTrackEnv.Docker, StringComparison.CurrentCultureIgnoreCase);
        private static bool IsProduction()
            => _env.Equals(CashTrackEnv.Production, StringComparison.CurrentCultureIgnoreCase);
        private static bool IsDocker()
            => _env.Equals(CashTrackEnv.Docker, StringComparison.CurrentCultureIgnoreCase);
        private static bool IsElectron()
            => _env.Equals(CashTrackEnv.Electron, StringComparison.CurrentCultureIgnoreCase);
    }
}
