using Common.Base;
using Common.Const;
using Common.DTO.CableDTO;
using Common.DTO.OtherMaterialsDTO;
using Common.Entity;
using Common.Enum;
using DataAccess.DAO;
using System.Net;

namespace DataAccess.Helper
{
    public class RequestHelper
    {
        public static ResponseBase CheckRequestNameAndRequestCategoryValidWhenCreateRequest(string requestName, int requestCategoryId, RequestCategories category)
        {
            if (requestName.Trim().Length == 0)
            {
                return new ResponseBase(false, "Tên yêu cầu không được để trống", (int)HttpStatusCode.Conflict);
            }
            if (requestCategoryId != (int)category)
            {
                return new ResponseBase(false, "Không phải yêu cầu " + category.ToString().ToLower(), (int)HttpStatusCode.Conflict);
            }
            return new ResponseBase(true);
        }

        public static ResponseBase CheckIssueValidWhenCreateRequest(DAOIssue daoIssue, Guid issueId)
        {
            Issue? issue = daoIssue.getIssue(issueId);
            // if not found issue
            if (issue == null)
            {
                return new ResponseBase(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
            }
            // if issue done
            if (issue.Status == IssueConst.STATUS_DONE)
            {
                return new ResponseBase(false, "Sự vụ với mã " + issue.IssueCode + " đã được chấp thuận", (int)HttpStatusCode.Conflict);
            }
            return new ResponseBase(true, issue.IssueCode);
        }

        public static ResponseBase CheckCableValidWhenCreateRequestExportDeliver(DAOCable daoCable, List<CableExportDeliverDTO> list)
        {
            if (list.Count == 0)
            {
                return new ResponseBase(true);
            }
            foreach (CableExportDeliverDTO DTO in list)
            {
                Cable? cable = daoCable.getCable(DTO.CableId);
                // if exist cable not found
                if (cable == null)
                {
                    return new ResponseBase(false, "Không tìm thấy cáp với ID " + DTO.CableId, (int)HttpStatusCode.NotFound);
                }
                // if invalid start point, end point or deleted
                if (DTO.StartPoint < cable.StartPoint || DTO.EndPoint > cable.EndPoint || DTO.StartPoint >= DTO.EndPoint || cable.IsDeleted || DTO.StartPoint < 0 || DTO.EndPoint < 0)
                {
                    return new ResponseBase(false, "Cáp với ID: " + DTO.CableId + " có chỉ số đầu hoặc chỉ số cuối không hợp lệ hoặc đã bị hủy", (int)HttpStatusCode.Conflict);
                }
                // if cable in use
                if (cable.IsExportedToUse)
                {
                    return new ResponseBase(false, "Cáp với ID: " + DTO.CableId + " đã được sử dụng", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseBase(true);
        }

        public static ResponseBase CheckMaterialValidCreateExportDeliverCancelInside(DAOOtherMaterial daoOtherMaterial, List<OtherMaterialsExportDeliverCancelInsideDTO> list)
        {
            if (list.Count == 0)
            {
                return new ResponseBase(true);
            }
            foreach (OtherMaterialsExportDeliverCancelInsideDTO DTO in list)
            {
                OtherMaterial? material = daoOtherMaterial.getOtherMaterial(DTO.OtherMaterialsId);
                // if not found
                if (material == null)
                {
                    return new ResponseBase(false, "Không tìm thấy vật liệu vs ID = " + DTO.OtherMaterialsId, (int)HttpStatusCode.NotFound);
                }
                // if not enough quantity
                if (material.Quantity < DTO.Quantity)
                {
                    return new ResponseBase(false, "Vật liệu vs ID = " + DTO.OtherMaterialsId + " không có đủ số lượng", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseBase(true);
        }

        public static Guid CreateRequest(DAORequest daoRequest, string requestName, string? content, Guid? issueId, Guid creatorId, int requestCategoryId, int? deliverWarehouseId)
        {
            Request request = new Request()
            {
                RequestId = Guid.NewGuid(),
                RequestName = requestName.Trim(),
                Content = content == null || content.Trim().Length == 0 ? null : content.Trim(),
                IssueId = issueId,
                CreatorId = creatorId,
                CreatedAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                IsDeleted = false,
                RequestCategoryId = requestCategoryId,
                Status = RequestConst.STATUS_PENDING,
                DeliverWarehouseId = deliverWarehouseId
            };
            daoRequest.CreateRequest(request);
            return request.RequestId;
        }

        public static void CreateRequestCableExportDeliver(DAORequestCable daoRequestCable, List<CableExportDeliverDTO> list, Guid requestId)
        {
            if (list.Count > 0)
            {
                foreach (CableExportDeliverDTO item in list)
                {
                    RequestCable request = new RequestCable()
                    {
                        RequestId = requestId,
                        CableId = item.CableId,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        StartPoint = item.StartPoint,
                        EndPoint = item.EndPoint,
                        Length = item.EndPoint - item.StartPoint,
                        RecoveryDestWarehouseId = item.WarehouseId,
                    };
                    daoRequestCable.CreateRequestCable(request);
                }
            }
        }

        public static void CreateRequestMaterialExportDeliver(DAORequestOtherMaterial daoRequestOtherMaterial, List<OtherMaterialsExportDeliverCancelInsideDTO> list, Guid requestId)
        {
            if (list.Count > 0)
            {
                foreach (OtherMaterialsExportDeliverCancelInsideDTO item in list)
                {
                    RequestOtherMaterial request = new RequestOtherMaterial()
                    {
                        RequestId = requestId,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        OtherMaterialsId = item.OtherMaterialsId,
                        Quantity = item.Quantity,
                        RecoveryDestWarehouseId = item.WarehouseId,
                    };
                    daoRequestOtherMaterial.CreateRequestOtherMaterial(request);
                }
            }
        }

        public static async Task sendEmailToAdmin(DAOUser daoUser, string requestName, string requestCategoryName, string? issueCode)
        {
            // get all email admin
            List<string> list = daoUser.getEmailAdmins();
            string body = UserHelper.BodyEmailForAdminReceiveRequest(requestName, requestCategoryName, issueCode);
            foreach (string email in list)
            {
                await UserHelper.sendEmail("[FPT TELECOM CABLE MANAGEMENT] Thông báo có yêu cầu mới", body, email);
            }
        }

        public static void UpdateRequest(DAORequest daoRequest,  Request request, Guid id, string status)
        {
            request.ApproverId = id;
            request.Status = status;
            request.UpdateAt = DateTime.Now;
            daoRequest.UpdateRequest(request);
        }
    }
}
