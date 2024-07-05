using Microsoft.Extensions.Configuration;

namespace DataAccess.Configuration
{
    public class ConfigData
    {
        public static string SqlConnection { get; set; } = string.Empty;
        public static string JwtKey { get; set; } = string.Empty;
        public static string JwtIssuer { get; set; } = string.Empty;
        public static string JwtAudience { get; set; } = string.Empty;
        public static string MailHost { get; set; } = string.Empty;
        public static string MailUser { get; set; } = string.Empty;
        public static string MailPassword { get; set; } = string.Empty;
        public static void LoadAll()
        {
            SqlConnection = getConfigValue("ConnectionStrings:DefaultConnection");
            JwtKey = getConfigValue("Jwt:Key");
            JwtIssuer = getConfigValue("Jwt:Issuer");
            JwtAudience = getConfigValue("Jwt:Audience");
            MailHost = getConfigValue("MailAddress:Host");
            MailUser = getConfigValue("MailAddress:Username");
            MailPassword = getConfigValue("MailAddress:Password");
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
