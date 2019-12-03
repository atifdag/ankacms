using AnkaCMS.Data.DataAccess.EntityFramework;
using AnkaCMS.SetupConsoleApp.Installation;
using AnkaCMS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace AnkaCMS.SetupConsoleApp
{
    internal class Program
    {
        private static IConfiguration Configuration
        {
            get
            {

                var path = AppDomain.CurrentDomain.BaseDirectory;
                if (AppDomain.CurrentDomain.BaseDirectory.Contains("bin"))
                {
                    path = AppDomain.CurrentDomain.BaseDirectory.Split(new[] { @"bin\" }, StringSplitOptions.None)[0];
                }

                var appsettingFile = "appsettings.json";
#if DEBUG
                appsettingFile = "appsettings.Development.json";
#endif
                return new ConfigurationBuilder().SetBasePath(path).AddJsonFile(appsettingFile, false, true).Build();
            }
        }

        private static void Main()
        {

            var services = new ServiceCollection();

            switch (Configuration["DefaultConnectionString"])
            {

                case "MsSqlAzureConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MsSqlAzureConnection")));
                    break;

                case "MsSqlConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnection")));
                    break;

                case "MsSqlLocalDbConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MsSqlLocalDbConnection")));
                    break;

                case "MySqlConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("MySqlConnection")));
                    break;
                case "MariaDbConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MariaDbConnection")));
                    break;

                case "PostgreSqlConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("PostgreSqlConnection")));
                    break;

                case "SqliteConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqliteConnection")));
                    break;

                default:
                    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SqliteConnection")));
                    break;
            }



            services.AddScoped(typeof(IUnitOfWork<EfDbContext>), typeof(UnitOfWork));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            var provider = services.BuildServiceProvider();

            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();

            var dbName = "";
            var dbServer = "";

            foreach (var s in Configuration.GetSection("ConnectionStrings:" + Configuration["DefaultConnectionString"]).Value.Split(";"))
            {
                if (s.Contains("Data Source"))
                {
                    dbServer = s.Replace("Data Source=", "");
                }

                if (s.Contains("Server"))
                {
                    dbServer = s.Replace("Server=", "");
                }

                if (s.Contains("Host"))
                {
                    dbServer = s.Replace("Host=", "");
                }

                if (s.Contains("Database"))
                {
                    dbName = s.Replace("Database=", "");
                }

                if (s.Contains("Initial Catalog"))
                {
                    dbName = s.Replace("Initial Catalog=", "");
                }
            }

            Console.WriteLine(@"Kurulum başlatılıyor...");
            Console.WriteLine(@"Başlama Zamanı: " + DateTime.Now);

            Console.WriteLine(@"Veritabanı Türü: " + Configuration["DefaultConnectionString"].Replace("Connection", ""));
            Console.WriteLine(@"Veritabanı Sunucusu: " + dbServer);
            Console.WriteLine(@"Veritabanı Adı: " + dbName);
            Console.WriteLine(@"");
            Console.WriteLine(@"Mevcut veritabanı kaldırılıyor...");

            if (unitOfWork.Context.Database.GetService<IRelationalDatabaseCreator>().Exists())
            {
                if (unitOfWork.Context.Database.EnsureDeleted())
                {
                    Console.WriteLine(@"Mevcut veritabanı kaldırıldı.");
                    Console.WriteLine(@"");
                }
                else
                {
                    Console.WriteLine(@"Hata: Mevcut veritabanı kaldırılamadı!");
                    Console.WriteLine(@"");
                    return;
                }
            }

            Console.WriteLine(@"Yeni veritabanı oluşturuluyor...");

            if (unitOfWork.Context.Database.EnsureCreated())
            {

                Console.WriteLine(@"Yeni veritabanı oluşturuldu.");
                Console.WriteLine(@"");

                try
                {
                    UserInstallation.Install(provider);
                    Console.WriteLine(@"Kullanıcılar oluşturuldu.");
                    Console.WriteLine(@"");

                    ParameterInstallation.Install(provider);
                    Console.WriteLine(@"Parametreler oluşturuldu.");
                    Console.WriteLine(@"");

                    RoleInstallation.Install(provider);
                    Console.WriteLine(@"Roller oluşturuldu.");
                    Console.WriteLine(@"");

                    PermissionInstallation.Install(provider);
                    Console.WriteLine(@"Yetkiler oluşturuldu.");
                    Console.WriteLine(@"");

                    MenuInstallation.Install(provider);
                    Console.WriteLine(@"Menüler oluşturuldu.");
                    Console.WriteLine(@"");

                    LanguageInstallation.Install(provider);
                    Console.WriteLine(@"Diller oluşturuldu.");
                    Console.WriteLine(@"");

                    CategoryInstallation.Install(provider);
                    Console.WriteLine(@"Kategoriler oluşturuldu.");
                    Console.WriteLine(@"");

                    ContentInstallation.Install(provider);
                    Console.WriteLine(@"İçerikler oluşturuldu.");
                    Console.WriteLine(@"");

                    PartInstallation.Install(provider);
                    Console.WriteLine(@"Bölümler oluşturuldu.");
                    Console.WriteLine(@"");

                    PartInstallation.SetContents(provider);
                    Console.WriteLine(@"Bölüm içerikleri ayarlandı.");
                    Console.WriteLine(@"");

                    Console.WriteLine(@"Kurulum Tamamlandı.");
                    Console.WriteLine(@"Bitiş Zamanı: " + DateTime.Now);
                    Console.WriteLine(@"Programı kapatabilirsiniz.");
                    Console.WriteLine(@"--------------------------");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine(@"");
                    Console.WriteLine(@"Hata oluştu.");
                    Console.WriteLine(@"--------------------------");
                }

            }
            else
            {
                Console.WriteLine(@"Hata: Yeni veritabanı oluşturulamadı!");
                Console.WriteLine(@"");
            }

            Console.ReadLine();
        }

    }
}
