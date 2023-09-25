using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace DataAccess.Model.Util
{
    public class UserUtil
    {
        private const int MAX_SIZE = 8; // randow password 8 characters
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string RandomPassword()
        {
            Random random = new Random();
            // string that contain both alphabets and numbers
            string str = "abcdefghijklmnopqrstuvwxyz0123456789QWERTYUIOPASDFGHJKLZXCVBNM";
            string result = "";
            for (int i = 0; i < MAX_SIZE; i++)
            {
                // get random index character
                int index = random.Next(str.Length);
                result = result + str[index];
            }
            return result;
        }

        public static string BodyEmailForRegister(string password)
        {
            string body = "<h1>Mật khẩu cho tài khoản mới</h1>\n" +
                            "<p>Mật khẩu của bạn là: " + password + "</p>\n";
            return body;
        }

        public static Task sendEmail(string subject, string body, string to)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            IConfigurationRoot config = builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
            IConfigurationSection mail = config.GetSection("MailAddress");
            MimeMessage mime = new MimeMessage();
            mime.From.Add(MailboxAddress.Parse(mail["Username"]));
            mime.To.Add(MailboxAddress.Parse(to));
            mime.Subject = subject;
            mime.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };
            using var stmp = new SmtpClient();
            stmp.Connect(mail["Host"], 587, MailKit.Security.SecureSocketOptions.StartTls);
            stmp.Authenticate(mail["Username"], mail["Password"]);
            stmp.Send(mime);
            stmp.Disconnect(true);
            return Task.CompletedTask;
        }
    }
}
