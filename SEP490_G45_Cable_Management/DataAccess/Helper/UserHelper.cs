using Common.Const;
using Common.Entity;
using DataAccess.Configuration;
using DataAccess.DAO;
using MailKit.Net.Smtp;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DataAccess.Helper
{
    public class UserHelper
    {
        private const int MAX_SIZE = 8; // randow password 8 characters

        public static string GenerateToken(User user)
        {
            // get credential
            byte[] key = Encoding.UTF8.GetBytes(ConfigData.JwtKey);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //  create list claim  to store user's information
            List<Claim> list = new List<Claim>()
            {
                new Claim("id", user.UserId.ToString()),
                new Claim(ClaimTypes.Email , user.Email),
                new Claim(ClaimTypes.Role, user.Role.Rolename),
                new Claim("FirstName", user.Firstname),
                new Claim("LastName", user.Lastname),
            };
            JwtSecurityToken token = new JwtSecurityToken(ConfigData.JwtIssuer,
                ConfigData.JwtAudience, list, expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            // get access token
            return handler.WriteToken(token);
        }

        public static string HashPassword(string password)
        {
            // using SHA256 for hash password
            byte[] hashPw = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashPw.Length; i++)
            {
                // convert into hexadecimal
                builder.Append(hashPw[i].ToString("x2"));
            }
            return builder.ToString();
        }
        public static string RandomPassword()
        {
            Random random = new Random();
            // password contain both alphabets and numbers
            string format = "abcdefghijklmnopqrstuvwxyz0123456789QWERTYUIOPASDFGHJKLZXCVBNM";
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < MAX_SIZE; i++)
            {
                // get random index character
                int index = random.Next(format.Length);
                builder.Append(format[index]);
            }
            return builder.ToString();
        }
        public static string BodyEmailForRegister(string password)
        {
            string body = "<h1>Mật khẩu cho tài khoản mới</h1>\n" +
                            "<p>Mật khẩu của bạn là: " + password + "</p>\n";
            return body;
        }
        public static Task sendEmail(string subject, string body, string to)
        {
            /*using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.APSoutheast1))
            {
                SendEmailRequest sendRequest = new SendEmailRequest
                {
                    Source = SENDER_EMAIL,
                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { to }
                    },
                    Message = new Message
                    {
                        Subject = new Content(subject),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = "UTF-8",
                                Data = body
                            },
                        }
                    },
                };
                await client.SendEmailAsync(sendRequest);
            }*/
            // create message to send
            MimeMessage mime = new MimeMessage();
            MailboxAddress mailFrom = MailboxAddress.Parse(ConfigData.MailUser);
            MailboxAddress mailTo = MailboxAddress.Parse(to);
            mime.From.Add(mailFrom);
            mime.To.Add(mailTo);
            mime.Subject = subject;
            mime.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };
            // send message
            SmtpClient smtp = new SmtpClient();
            smtp.Connect(ConfigData.MailHost);
            smtp.Authenticate(ConfigData.MailUser, ConfigData.MailPassword);
            smtp.Send(mime);
            smtp.Disconnect(true);
            return Task.CompletedTask;
        }

        public static string BodyEmailForForgetPassword(string password)
        {
            string body = "<h1>Mật khẩu mới</h1>\n" +
                            "<p>Mật khẩu mới là: " + password + "</p>\n" +
                            "<p>Không nên chia sẻ mật khẩu của bạn với người khác.</p>";
            return body;
        }
        public static string BodyEmailForAdminReceiveRequest(string requestName, string requestCategoryName, string? issueCode)
        {
            string body = "<h1>Yêu cầu với tên \"" + requestName + "\"</h1>\n" +
                            "<p>Loại yêu cầu: " + requestCategoryName + "</p>\n";
            if (issueCode != null)
            {
                body = body + "<p>Mã sụ vụ: " + issueCode + "</p>\n";
            }
            body = body + "<p>Vui lòng kiểm tra chi tiết yêu cầu</p>\n";
            return body;
        }

        public static string BodyEmailForApproveRequest(DAORequestCable daoRequestCable, DAORequestOtherMaterial daoRequestMaterial, Request request, string approverName)
        {
            List<RequestCable> requestCables = daoRequestCable.getListRequestCable(request.RequestId);
            List<RequestOtherMaterial> requestMaterials = daoRequestMaterial.getListRequestOtherMaterial(request.RequestId);
            StringBuilder builder = new StringBuilder("<h1>Yêu cầu với tên \"" + request.RequestName + "\" đã được duyệt</h1>\n" +
                            "<p>Loại yêu cầu: " + request.RequestCategory.RequestCategoryName + "</p>\n");
            if (request.Issue != null)
            {
                builder.Append("<p>Mã sụ vụ: " + request.Issue.IssueCode + "</p>");
            }
            if (request.DeliverWarehouse != null)
            {
                builder.Append("<p> Vật liệu được chuyển đến kho " + request.DeliverWarehouse.WarehouseName + "</p>");
            }
            builder.Append("<p>Thông tin chi tiết của yêu cầu:</p>\n");
            if (requestCables.Count > 0)
            {
                foreach (RequestCable item in requestCables)
                {
                    if (item.RecoveryDestWarehouse != null)
                    {
                        if (request.RequestCategoryId == (int)RequestCategoryConst.Export || request.RequestCategoryId == (int)RequestCategoryConst.Deliver)
                        {
                            builder.AppendLine("<p> - " + item.Cable.CableCategory.CableCategoryName + " xuất từ kho " + item.RecoveryDestWarehouse.WarehouseName
                                + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ", độ dài: " + item.Length + ")</p>");
                        }
                        else if (request.RequestCategoryId == (int)RequestCategoryConst.Recovery)
                        {
                            builder.AppendLine("<p> - " + item.Cable.CableCategory.CableCategoryName + " được thu hồi về kho " + item.RecoveryDestWarehouse.WarehouseName
                                + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ", độ dài: " + item.Length + ")</p>");
                        }
                        else if (request.RequestCategoryId == (int)RequestCategoryConst.Cancel_Inside)
                        {
                            builder.AppendLine("<p> - " + item.Cable.CableCategory.CableCategoryName + " được hủy trong kho " + item.RecoveryDestWarehouse.WarehouseName
                                + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ", độ dài: " + item.Length + ")</p>");
                        }
                    }
                    else if (request.RequestCategoryId == (int)RequestCategoryConst.Cancel_Outside)
                    {
                        builder.AppendLine("<p> - " + item.Cable.CableCategory.CableCategoryName
                           + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ", độ dài: " + item.Length + ")</p>");
                    }

                }
            }
            if (requestMaterials.Count > 0)
            {
                foreach (RequestOtherMaterial item in requestMaterials)
                {
                    if (item.RecoveryDestWarehouse != null)
                    {
                        if (request.RequestCategoryId == (int)RequestCategoryConst.Export || request.RequestCategoryId == (int)RequestCategoryConst.Deliver)
                        {
                            builder.AppendLine("<p> - " + item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName + " xuất từ trong kho " +
                                item.RecoveryDestWarehouse.WarehouseName + ", số lượng: " + item.Quantity + "</p>");
                        }
                        else if (request.RequestCategoryId == (int)RequestCategoryConst.Recovery)
                        {
                            builder.AppendLine("<p> - " + item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName + " được thu hồi về kho " +
                                item.RecoveryDestWarehouse.WarehouseName + ", số lượng: " + item.Quantity + "</p>");
                        }
                        else if (request.RequestCategoryId == (int)RequestCategoryConst.Cancel_Inside)
                        {
                            builder.AppendLine("<p> - " + item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName + " được hủy trong kho " +
                                item.RecoveryDestWarehouse.WarehouseName + ", số lượng: " + item.Quantity + "</p>");
                        }
                    }
                    else if (request.RequestCategoryId == (int)RequestCategoryConst.Cancel_Outside)
                    {
                        builder.AppendLine("<p> - " + item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName + ", số lượng: " + item.Quantity + "</p>");
                    }
                }
            }
            builder.AppendLine("<p>Người duyệt</p>");
            builder.AppendLine("<p>" + approverName + "</p>");
            return builder.ToString();
        }

    }
}

