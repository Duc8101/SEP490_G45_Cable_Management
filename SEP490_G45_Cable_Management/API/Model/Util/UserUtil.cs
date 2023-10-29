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
using DataAccess.Entity;
using Org.BouncyCastle.Asn1.Ocsp;
using API.Model.DAO;
using DataAccess.Const;

namespace API.Model.Util
{
    public class UserUtil
    {
        private const int MAX_SIZE = 8; // randow password 8 characters
        public static string HashPassword(string password)
        {
            // using SHA256 for hash password
            byte[] hashPw = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
            string result = "";
            for (int i = 0; i < hashPw.Length; i++)
            {
                // convert into hexadecimal
                result = result + hashPw[i].ToString("x2");
            }
            return result;
        }
        public static string RandomPassword()
        {
            Random random = new Random();
            // password contain both alphabets and numbers
            string format = "abcdefghijklmnopqrstuvwxyz0123456789QWERTYUIOPASDFGHJKLZXCVBNM";
            string result = "";
            for (int i = 0; i < MAX_SIZE; i++)
            {
                // get random index character
                int index = random.Next(format.Length);
                result = result + format[index];
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
            // get information of mail address
            ConfigurationBuilder builder = new ConfigurationBuilder();
            IConfigurationRoot config = builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
            IConfigurationSection mail = config.GetSection("MailAddress");
            // create message to send
            MimeMessage mime = new MimeMessage();
            MailboxAddress mailFrom = MailboxAddress.Parse(mail["Username"]);
            MailboxAddress mailTo = MailboxAddress.Parse(to);
            mime.From.Add(mailFrom);
            mime.To.Add(mailTo);
            mime.Subject = subject;
            mime.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };
            // send message
            SmtpClient smtp = new SmtpClient();
            smtp.Connect(mail["Host"]);
            smtp.Authenticate(mail["Username"], mail["Password"]);
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
        public static string BodyEmailForAdminReceiveRequest(string RequestName, string RequestCategoryName, Issue? issue)
        {
            string body = "<h1>Yêu cầu với tên \"" + RequestName + "\"</h1>\n" +
                            "<p>Loại yêu cầu: " + RequestCategoryName + "</p>\n";
            if (issue != null)
            {
                body = body + "<p>Mã sụ vụ: " + issue.IssueCode + "</p>\n";
            }
            body = body + "<p>Vui lòng kiểm tra chi tiết yêu cầu</p>\n";
            return body;
        }
        public static async Task<string> BodyEmailForApproveRequest(DataAccess.Entity.Request request, string ApproverName)
        {
            DAORequestCable daoRequestCable = new DAORequestCable();
            DAORequestOtherMaterial daoRequestMaterial = new DAORequestOtherMaterial();
            List<RequestCable> requestCables = await daoRequestCable.getList(request.RequestId);
            List<RequestOtherMaterial> requestMaterials = await daoRequestMaterial.getList(request.RequestId);
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
                    if (item.Cable.Warehouse != null)
                    {
                        if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_EXPORT || request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER)
                        {
                            builder.AppendLine("<p> - " + item.Cable.CableCategory.CableCategoryName + " xuất từ kho " + item.Cable.Warehouse.WarehouseName
                                + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ", độ dài: " + item.Length + ")</p>");
                        }
                        else if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_RECOVERY)
                        {
                            builder.AppendLine("<p> - " + item.Cable.CableCategory.CableCategoryName + " được thu hồi về kho " + item.Cable.Warehouse.WarehouseName
                                + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ", độ dài: " + item.Length + ")</p>");
                        }
                        else if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_CANCEL_INSIDE)
                        {
                            builder.AppendLine("<p> - " + item.Cable.CableCategory.CableCategoryName + " được hủy trong kho " + item.Cable.Warehouse.WarehouseName
                                + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ", độ dài: " + item.Length + ")</p>");
                        }
                    }else if(request.RequestCategoryId == RequestCategoryConst.CATEGORY_CANCEL_OUTSIDE)
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
                    if (item.OtherMaterials.Warehouse != null)
                    {
                        if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_EXPORT || request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER)
                        {
                            builder.AppendLine("<p> - " + item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName + " xuất từ trong kho " +
                                item.OtherMaterials.Warehouse.WarehouseName + ", số lượng: " + item.Quantity + "</p>");
                        }
                        else if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_RECOVERY)
                        {
                            builder.AppendLine("<p> - " + item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName + " được thu hồi về kho " +
                                item.OtherMaterials.Warehouse.WarehouseName + ", số lượng: " + item.Quantity + "</p>");
                        }
                        else if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_CANCEL_INSIDE)
                        {
                            builder.AppendLine("<p> - " + item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName + " được hủy trong kho " +
                                item.OtherMaterials.Warehouse.WarehouseName + ", số lượng: " + item.Quantity + "</p>");
                        }
                    }else if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_CANCEL_OUTSIDE)
                    {
                        builder.AppendLine("<p> - " + item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName + ", số lượng: " + item.Quantity + "</p>");
                    }
                }
            }
            builder.AppendLine("<p>Người duyệt</p>");
            builder.AppendLine("<p>" + ApproverName + "</p>");
            return builder.ToString();
        }
    }
}
