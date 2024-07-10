using Microsoft.Extensions.Configuration;

namespace DataAccess.Configuration
{
    public class ConfigData
    {
        public static string SqlConnection
        {
            get => getConfigValue("ConnectionStrings:DefaultConnection");
        }

        public static string JwtKey
        {
            get => getConfigValue("Jwt:Key");
        }

        public static string JwtIssuer
        {
            get => getConfigValue("Jwt:Issuer");
        }

        public static string JwtAudience
        {
            get => getConfigValue("Jwt:Audience");
        }

        public static string MailHost
        {
            get => getConfigValue("MailAddress:Host");
        }

        public static string MailUser
        {
            get => getConfigValue("MailAddress:Username");
        }

        public static string MailPassword
        {
            get => getConfigValue("MailAddress:Password");
        }

        private static string getConfigValue(string configPath)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            IConfigurationRoot config = builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
            return config.GetSection(configPath).Value;
        }
    }
}
