using lab.RedisCacheSql.Config;
using lab.RedisCacheSql.Repository;
using lab.RedisCacheSql.Managers;
using DataTables.AspNet.AspNetCore;
using lab.RedisCacheSql.Extensions;
using lab.RedisCacheSql.Services;
using lab.RedisCacheSql.RedisManagers;

namespace lab.RedisCacheSql
{
    public static class BootStrapper
    {
        public static void Run(IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                services.RegisterAutoMapper();
                services.RegisterDataTables();

                // Add our Config object so it can be injected
                services.Configure<AppConfig>(configuration.GetSection(AppConfig.Name));
                services.Configure<AppDbConnectionConfig>(configuration.GetSection(AppDbConnectionConfig.Name));
                services.Configure<AppEmailConfig>(configuration.GetSection(AppEmailConfig.Name));
                services.Configure<AppSmsConfig>(configuration.GetSection(AppSmsConfig.Name));
                services.Configure<RedisConfig>(configuration.GetSection(RedisConfig.Name));

                services.AddStackExchangeRedisCache(options =>
                {
                    //options.Configuration = "localhost:6379";
                    options.Configuration = configuration.GetValue<string>("RedisConfig:Host");
                    //options.InstanceName = "TestDbRedisCache";
                    options.InstanceName = configuration.GetValue<string>("RedisConfig:InstanceName");
                });

                services.AddScoped<IPatientProfileRepository, PatientProfileRepository>();
                services.AddScoped<IPatientProfileManager, PatientProfileManager>();
                services.AddScoped<IEmployeeManager, EmployeeManager>();

                services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();

                services.AddScoped<IAppBackgroundService, AppBackgroundService>();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
