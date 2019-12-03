using AnkaCMS.Data.DataAccess.EntityFramework;
using AnkaCMS.Service;
using AnkaCMS.Service.Implementations;
using AnkaCMS.Service.Implementations.EmailMessaging.SystemNet;
using AnkaCMS.Core;
using AnkaCMS.Core.Caching;
using AnkaCMS.Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AnkaCMS.WebApi.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void ResolveDependency(this IServiceCollection services, IConfiguration configuration)
        {
            #region Veritabanı

            switch (configuration["DefaultConnectionString"])
            {

                case "MsSqlAzureConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("MsSqlAzureConnection")));
                    break;

                case "MsSqlConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("MsSqlConnection")));
                    break;

                case "MsSqlLocalDbConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("MsSqlLocalDbConnection")));
                    break;

                case "MySqlConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseMySQL(configuration.GetConnectionString("MySqlConnection")));
                    break;

                case "MariaDbConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseMySql(configuration.GetConnectionString("MariaDbConnection")));
                    break;

                case "PostgreSqlConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("PostgreSqlConnection")));
                    break;

                case "SqliteConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlite(configuration.GetConnectionString("SqliteConnection")));
                    break;

                default:
                    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqliteConnection")));
                    break;
            }

            services.AddScoped<IDbContext>(provider => provider.GetService<EfDbContext>());
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            #endregion

            #region Gerekli

            services.AddHttpContextAccessor();
            services.AddTransient<IMainService, MainService>();
            services.AddTransient<ISmtp, SystemNetSmtp>();
            services.AddTransient<ICacheService>(provider => new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()), configuration.GetSection("CacheTime").Value.ToInt()));
            //services.AddTransient<ICacheService>(s => new RedisCacheService(configuration.GetSection("RedisServer:Host").Value, configuration.GetSection("RedisServer:Port").Value.ToInt(), configuration.GetSection("CacheTime").Value.ToInt()));
            services.AddTransient<IIdentityService>(provider => new TokenBaseIdentityService(provider.GetService<IHttpContextAccessor>(), configuration.GetSection("JwtSecurityKey").Value));
            #endregion

            #region Servisler
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IParameterGroupService, ParameterGroupService>();
            services.AddTransient<IParameterService, ParameterService>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IMenuService, MenuService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IPartService, PartService>();
            services.AddTransient<IContentService, ContentService>();
            #endregion
        }
    }
}
