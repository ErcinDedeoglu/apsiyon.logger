using Apsiyon.Logger.Helper;
using Microsoft.Extensions.Configuration;

namespace Apsiyon.Logger.Test.Helper
{
    public class ConfigurationHelper
    {
        public static IConfiguration Configuration()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            ConfigurationDto.Config = configuration.GetSection("ApsiyonLogger").Get<ConfigurationDto.Configuration>();
            return configuration;
        }
    }
}