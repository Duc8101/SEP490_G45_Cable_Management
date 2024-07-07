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
        public static ResponseBase CheckRequestNameAndRequestCategoryValidWhenCreateRequest(string requestName
            , int requestCategoryId, RequestCategories category)
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

        public static ResponseBase CheckMaterialValidCreateExportDeliverCancelInside(DAOOtherMaterial daoOtherMaterial
            , List<OtherMaterialsExportDeliverCancelInsideDTO> list)
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

        public static Guid CreateRequest(DAORequest daoRequest, string requestName, string? content, Guid? issueId
            , Guid creatorId, int requestCategoryId, int? deliverWarehouseId)
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

        public static void CreateRequestCableExportDeliver(DAORequestCable daoRequestCable, List<CableExportDeliverDTO> list
            , Guid requestId)
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

        public static void CreateRequestMaterialExportDeliver(DAORequestOtherMaterial daoRequestOtherMaterial
            , List<OtherMaterialsExportDeliverCancelInsideDTO> list, Guid requestId)
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

        public static async Task sendEmailToAdmin(DAOUser daoUser, string requestName, string requestCategoryName
            , string? issueCode)
        {
            // get all email admin
            List<string> list = daoUser.getEmailAdmins();
            string body = UserHelper.BodyEmailForAdminReceiveRequest(requestName, requestCategoryName, issueCode);
            foreach (string email in list)
            {
                await UserHelper.sendEmail("[FPT TELECOM CABLE MANAGEMENT] Thông báo có yêu cầu mới", body, email);
            }
        }

        public static void UpdateRequest(DAORequest daoRequest, Request request, Guid id, string status)
        {
            request.ApproverId = id;
            request.Status = status;
            request.UpdateAt = DateTime.Now;
            daoRequest.UpdateRequest(request);
        }

        public static ResponseBase CheckCableValidCreateCancelInside(DAOCable daoCable, List<CableCancelInsideDTO> list)
        {
            if (list.Count == 0)
            {
                return new ResponseBase(true);
            }
            foreach (CableCancelInsideDTO DTO in list)
            {
                Cable? cable = daoCable.getCable(DTO.CableId);
                // if exist cable not found
                if (cable == null)
                {
                    return new ResponseBase(false, "Không tìm thấy cáp với ID " + DTO.CableId, (int)HttpStatusCode.NotFound);
                }
                // if cable in use
                if (cable.IsExportedToUse)
                {
                    return new ResponseBase(false, "Cáp với ID: " + DTO.CableId + " đã được sử dụng", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseBase(true);
        }

        private static async Task sendEmailToCreator(DAORequestCable daoRequestCable, DAORequestOtherMaterial daoRequestMaterial
            , Request request, string approverName)
        {
            string body = UserHelper.BodyEmailForApproveRequest(daoRequestCable, daoRequestMaterial, request, approverName);
            await UserHelper.sendEmail("Thông báo yêu cầu được phê duyệt", body, request.Creator.Email);
        }

        private static void CreateTransaction(DAOTransactionHistory daoHistory, DAOTransactionCable daoTransactionCable
            , DAOTransactionOtherMaterial daoTransactionOtherMaterial, Request request, List<int> listCableWarehouseId
            , List<Cable> listCable, List<int> listOtherMaterialWarehouseId, List<int> listOtherMaterialId, List<int> listOtherMaterialQuantity, bool? action)
        {
            string CategoryName;
            int? fromWarehouseId = null;
            int? toWarehouseId = null;
            if (action == null)
            {
                CategoryName = TransactionCategoryConst.CATEGORY_CANCEL;
            }
            else
            {
                CategoryName = action == true ? TransactionCategoryConst.CATEGORY_IMPORT : TransactionCategoryConst.CATEGORY_EXPORT;
            }
            // if transaction export and request deliver
            if (action == false && request.RequestCategoryId == (int)RequestCategories.Deliver)
            {
                toWarehouseId = request.DeliverWarehouseId;
                // if transaction import and request deliver
            }
            else if (action == true && request.RequestCategoryId == (int)RequestCategories.Deliver)
            {
                fromWarehouseId = request.DeliverWarehouseId;
            }
            // ------------------------------- create transaction history -----------------------------
            // list contain all warehouse of cable and material
            List<int> listAllWarehouseId = new List<int>();
            if (listCableWarehouseId.Count > 0)
            {
                foreach (int warehouseId in listCableWarehouseId)
                {
                    // if list all warehouse not contain warehouse exist in list
                    if (!listAllWarehouseId.Contains(warehouseId))
                    {
                        listAllWarehouseId.Add(warehouseId);
                    }
                }
            }
            if (listOtherMaterialWarehouseId.Count > 0)
            {
                foreach (int warehouseId in listAllWarehouseId)
                {
                    // if list all warehouse not contain material warehouse
                    if (!listAllWarehouseId.Contains(warehouseId))
                    {
                        listAllWarehouseId.Add(warehouseId);
                    }
                }
            }
            // list key-value to store trasaction id and warehouse id
            Dictionary<int, Guid> dic = new Dictionary<int, Guid>();
            foreach (int warehouseId in listAllWarehouseId)
            {
                TransactionHistory history = new TransactionHistory()
                {
                    TransactionId = Guid.NewGuid(),
                    TransactionCategoryName = CategoryName,
                    CreatedDate = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false,
                    WarehouseId = warehouseId,
                    RequestId = request.RequestId,
                    IssueId = request.IssueId,
                    FromWarehouseId = fromWarehouseId,
                    ToWarehouseId = toWarehouseId
                };
                daoHistory.CreateTransactionHistory(history);
                dic[warehouseId] = history.TransactionId;
            }
            // ------------------------------- create transaction cable --------------------------------
            if (listCableWarehouseId.Count > 0)
            {
                for (int i = 0; i < listCableWarehouseId.Count; i++)
                {
                    TransactionCable transactionCable = new TransactionCable()
                    {
                        TransactionId = dic[listCableWarehouseId[i]],
                        CableId = listCable[i].CableId,
                        StartPoint = listCable[i].StartPoint,
                        EndPoint = listCable[i].EndPoint,
                        Length = listCable[i].Length,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false
                    };
                    daoTransactionCable.CreateTransactionCable(transactionCable);
                }
            }
            // ------------------------------- create transaction material --------------------------------
            if (listOtherMaterialWarehouseId.Count > 0)
            {
                for (int i = 0; i < listOtherMaterialWarehouseId.Count; i++)
                {
                    TransactionOtherMaterial material = new TransactionOtherMaterial()
                    {
                        TransactionId = dic[listOtherMaterialWarehouseId[i]],
                        OtherMaterialsId = listOtherMaterialId[i],
                        Quantity = listOtherMaterialQuantity[i],
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                    };
                    daoTransactionOtherMaterial.CreateTransactionMaterial(material);
                }
            }
        }


        public static async Task<ResponseBase> ApproveRequestExportDeliverCancelInside(DAORequestCable daoRequestCable
            , DAORequestOtherMaterial daoRequestOtherMaterial, DAORequest daoRequest, DAOCable daoCable, Guid approverId
            , DAOOtherMaterial daoOtherMaterial, DAOTransactionHistory daoHistory, DAOTransactionCable daoTransactionCable
            , DAOTransactionOtherMaterial daoTransactionOtherMaterial, Request request, string approverName)
        {
            List<RequestCable> requestCables = daoRequestCable.getListRequestCable(request.RequestId);
            List<RequestOtherMaterial> requestMaterials = daoRequestOtherMaterial.getListRequestOtherMaterial(request.RequestId);
            // ------------------------------- check cable valid ------------------------------  
            ResponseBase response = CheckCableValidApproveExportDeliverCancelInside(daoCable, requestCables, true);// true : xuat kho , false:huy
            // if exist cable not valid
            if (response.Success == false)
            {
                return response;
            }
            // ------------------------------- check material valid ------------------------------
            response = CheckMaterialValidApproveExportDeliverCancelInside(daoOtherMaterial, requestMaterials);
            // if exist material not valid
            if (response.Success == false)
            {
                return response;
            }
            // if request export
            if (request.RequestCategoryId == (int)RequestCategories.Export)
            {
                Dictionary<string, object> result = ApproveRequestExport(daoCable, daoOtherMaterial,request, requestCables, requestMaterials);
                List<Cable> listCableExported = (List<Cable>)result["listCableExported"];
                List<int> listCableWarehouseId = (List<int>)result["listCableWarehouseId"];
                List<int> listOtherMaterialWarehouseId = (List<int>)result["listOtherMaterialWarehouseId"];
                List<int> listOtherMaterialIdExported = (List<int>)result["listOtherMaterialIdExported"];
                List<int> listOtherMaterialQuantity = (List<int>)result["listOtherMaterialQuantity"];
                CreateTransaction(daoHistory, daoTransactionCable, daoTransactionOtherMaterial, request, listCableWarehouseId
                    , listCableExported, listOtherMaterialWarehouseId, listOtherMaterialIdExported, listOtherMaterialQuantity
                    , false);
                // if request deliver
            }
            else if (request.RequestCategoryId == (int)RequestCategories.Deliver)
            {
/*                Dictionary<string, object> result = ApproveRequestDeliver(request, requestCables, requestMaterials);
                List<Cable> listCableDeliver = (List<Cable>)result["listCableDeliver"];
                List<int> listCableWarehouseId = (List<int>)result["listCableWarehouseId"];
                List<int> listOtherMaterialWarehouseId = (List<int>)result["listOtherMaterialWarehouseId"];
                List<int> listOtherMaterialIdDeliver = (List<int>)result["listOtherMaterialIdDeliver"];
                List<int> listOtherMaterialQuantity = (List<int>)result["listOtherMaterialQuantity"];
                CreateTransaction(daoHistory, daoTransactionCable, daoTransactionOtherMaterial, request
                    , listCableWarehouseId, listCableDeliver, listOtherMaterialWarehouseId, listOtherMaterialIdDeliver
                    , listOtherMaterialQuantity, true);
*/            }
            else
            {
/*                Dictionary<string, object> result = ApproveRequestCancelInside(requestCables, requestMaterials);
                List<Cable> listCableCancelInside = (List<Cable>)result["listCableCancelInside"];
                List<int> listCableWarehouseId = (List<int>)result["listCableWarehouseId"];
                List<int> listOtherMaterialWarehouseId = (List<int>)result["listOtherMaterialWarehouseId"];
                List<int> listOtherMaterialIdCancelInside = (List<int>)result["listOtherMaterialIdCancelInside"];
                List<int> listOtherMaterialQuantity = (List<int>)result["listOtherMaterialQuantity"];
                CreateTransaction(daoHistory, daoTransactionCable, daoTransactionOtherMaterial, request
                    , listCableWarehouseId, listCableCancelInside, listOtherMaterialWarehouseId, listOtherMaterialIdCancelInside
                    , listOtherMaterialQuantity, null);
*/            }
            // ------------------------------- update request approved --------------------------------
            UpdateRequest(daoRequest, request, approverId, RequestConst.STATUS_APPROVED);
            // ------------------------------- send email --------------------------------
            await sendEmailToCreator(daoRequestCable, daoRequestOtherMaterial, request, approverName);
            return new ResponseBase(true, "Yêu cầu được phê duyệt");
        }

        private static ResponseBase CheckCableValidApproveExportDeliverCancelInside(DAOCable daoCable, List<RequestCable> list, bool action)
        {
            if (list.Count == 0)
            {
                return new ResponseBase(true);
            }
            foreach (RequestCable item in list)
            {
                Cable? cable;
                // if cable deleted
                if (item.Cable.IsDeleted)
                {
                    cable = daoCable.getCable(item.CableId, item.StartPoint, item.EndPoint);
                }
                else
                {
                    cable = item.Cable;
                }
                // if not found cable or start point, end point invalid
                if (cable == null || item.StartPoint < cable.StartPoint || item.EndPoint > cable.EndPoint)
                {
                    string mess = action ? "xuất kho" : "hủy";
                    return new ResponseBase(false, "Không thể " + mess + " " + item.Cable.CableCategory.CableCategoryName + " với ID: " + item.CableId
                               + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ")", (int)HttpStatusCode.Conflict);
                }
                // if cable exported
                if (cable.IsExportedToUse == true)
                {
                    return new ResponseBase(false, item.Cable.CableCategory.CableCategoryName + " với ID: " + item.CableId
                        + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ") đã được sử dụng!", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseBase(true);
        }

        private static ResponseBase CheckMaterialValidApproveExportDeliverCancelInside(DAOOtherMaterial daoOtherMaterial, List<RequestOtherMaterial> list)
        {
            if (list.Count == 0)
            {
                return new ResponseBase(true);
            }
            foreach (RequestOtherMaterial item in list)
            {
                OtherMaterial? material = daoOtherMaterial.getOtherMaterial(item.OtherMaterialsId);
                // if not found
                if (material == null)
                {
                    return new ResponseBase(false, "Không tìm thấy vật liệu " + item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName, (int)HttpStatusCode.NotFound);
                }
                // if not enough quantity
                if (material.Quantity < item.Quantity)
                {
                    return new ResponseBase(false, "Không đủ số lượng " + item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName, (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseBase(true);
        }

        // get list cable cut when approve request deliver, export
        private static List<Cable> getListCableCut(int itemStartPoint, int itemEndPoint, Cable cable, int? deliverWareHouseId)
        {
            List<Cable> list = new List<Cable>();
            if (itemStartPoint == cable.StartPoint && itemEndPoint < cable.EndPoint)
            {
                Cable cableCut1 = new Cable()
                {
                    CableId = Guid.NewGuid(),
                    SupplierId = cable.SupplierId,
                    YearOfManufacture = cable.YearOfManufacture,
                    Code = cable.Code,
                    Status = cable.Status,
                    WarehouseId = deliverWareHouseId,
                    StartPoint = itemStartPoint,
                    EndPoint = itemEndPoint,
                    Length = itemEndPoint - itemStartPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = false,
                    IsExportedToUse = deliverWareHouseId == null,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                Cable cableCut2 = new Cable()
                {
                    CableId = Guid.NewGuid(),
                    SupplierId = cable.SupplierId,
                    YearOfManufacture = cable.YearOfManufacture,
                    Code = cable.Code,
                    Status = cable.Status,
                    WarehouseId = cable.WarehouseId,
                    StartPoint = itemEndPoint,
                    EndPoint = cable.EndPoint,
                    Length = cable.EndPoint - itemEndPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = false,
                    IsExportedToUse = false,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                list.Add(cableCut1);
                list.Add(cableCut2);
            }
            else if (itemStartPoint > cable.StartPoint && itemEndPoint == cable.EndPoint)
            {
                Cable cableCut1 = new Cable()
                {
                    CableId = Guid.NewGuid(),
                    SupplierId = cable.SupplierId,
                    YearOfManufacture = cable.YearOfManufacture,
                    Code = cable.Code,
                    Status = cable.Status,
                    WarehouseId = cable.WarehouseId,
                    StartPoint = cable.StartPoint,
                    EndPoint = itemStartPoint,
                    Length = itemStartPoint - cable.StartPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = false,
                    IsExportedToUse = false,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                Cable cableCut2 = new Cable()
                {
                    CableId = Guid.NewGuid(),
                    SupplierId = cable.SupplierId,
                    YearOfManufacture = cable.YearOfManufacture,
                    Code = cable.Code,
                    Status = cable.Status,
                    WarehouseId = deliverWareHouseId,
                    StartPoint = itemStartPoint,
                    EndPoint = itemEndPoint,
                    Length = itemEndPoint - itemStartPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = false,
                    IsExportedToUse = deliverWareHouseId == null,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                list.Add(cableCut1);
                list.Add(cableCut2);
            }
            else if (itemStartPoint > cable.StartPoint && itemEndPoint < cable.EndPoint)
            {
                Cable cableCut1 = new Cable()
                {
                    CableId = Guid.NewGuid(),
                    SupplierId = cable.SupplierId,
                    YearOfManufacture = cable.YearOfManufacture,
                    Code = cable.Code,
                    Status = cable.Status,
                    WarehouseId = cable.WarehouseId,
                    StartPoint = cable.StartPoint,
                    EndPoint = itemStartPoint,
                    Length = itemStartPoint - cable.StartPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = false,
                    IsExportedToUse = false,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                Cable cableCut2 = new Cable()
                {
                    CableId = Guid.NewGuid(),
                    SupplierId = cable.SupplierId,
                    YearOfManufacture = cable.YearOfManufacture,
                    Code = cable.Code,
                    Status = cable.Status,
                    WarehouseId = deliverWareHouseId,
                    StartPoint = itemStartPoint,
                    EndPoint = itemEndPoint,
                    Length = itemEndPoint - itemStartPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = false,
                    IsExportedToUse = deliverWareHouseId == null,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                Cable cableCut3 = new Cable()
                {
                    CableId = Guid.NewGuid(),
                    SupplierId = cable.SupplierId,
                    YearOfManufacture = cable.YearOfManufacture,
                    Code = cable.Code,
                    Status = cable.Status,
                    WarehouseId = cable.WarehouseId,
                    StartPoint = itemEndPoint,
                    EndPoint = cable.EndPoint,
                    Length = cable.EndPoint - itemEndPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = false,
                    IsExportedToUse = false,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                list.Add(cableCut1);
                list.Add(cableCut2);
                list.Add(cableCut3);
            }
            return list;
        }

        // get list cable cut when approve request cancel inside
        private static List<Cable> getListCableCut(int itemStartPoint, int itemEndPoint, Cable cable)
        {
            List<Cable> list = new List<Cable>();
            if (itemStartPoint == cable.StartPoint && itemEndPoint < cable.EndPoint)
            {
                Cable cableCut1 = new Cable()
                {
                    CableId = Guid.NewGuid(),
                    SupplierId = cable.SupplierId,
                    YearOfManufacture = cable.YearOfManufacture,
                    Code = cable.Code,
                    Status = cable.Status,
                    WarehouseId = cable.WarehouseId,
                    StartPoint = itemStartPoint,
                    EndPoint = itemEndPoint,
                    Length = itemEndPoint - itemStartPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = true,
                    IsExportedToUse = false,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                Cable cableCut2 = new Cable()
                {
                    CableId = Guid.NewGuid(),
                    SupplierId = cable.SupplierId,
                    YearOfManufacture = cable.YearOfManufacture,
                    Code = cable.Code,
                    Status = cable.Status,
                    WarehouseId = cable.WarehouseId,
                    StartPoint = itemEndPoint,
                    EndPoint = cable.EndPoint,
                    Length = cable.EndPoint - itemEndPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = false,
                    IsExportedToUse = false,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                list.Add(cableCut1);
                list.Add(cableCut2);
            }
            else if (itemStartPoint > cable.StartPoint && itemEndPoint == cable.EndPoint)
            {
                Cable cableCut1 = new Cable()
                {
                    CableId = Guid.NewGuid(),
                    SupplierId = cable.SupplierId,
                    YearOfManufacture = cable.YearOfManufacture,
                    Code = cable.Code,
                    Status = cable.Status,
                    WarehouseId = cable.WarehouseId,
                    StartPoint = cable.StartPoint,
                    EndPoint = itemStartPoint,
                    Length = itemStartPoint - cable.StartPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = false,
                    IsExportedToUse = false,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                Cable cableCut2 = new Cable()
                {
                    CableId = Guid.NewGuid(),
                    SupplierId = cable.SupplierId,
                    YearOfManufacture = cable.YearOfManufacture,
                    Code = cable.Code,
                    Status = cable.Status,
                    WarehouseId = cable.WarehouseId,
                    StartPoint = itemStartPoint,
                    EndPoint = itemEndPoint,
                    Length = itemEndPoint - itemStartPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = true,
                    IsExportedToUse = false,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                list.Add(cableCut1);
                list.Add(cableCut2);
            }
            else if (itemStartPoint > cable.StartPoint && itemEndPoint < cable.EndPoint)
            {
                Cable cableCut1 = new Cable()
                {
                    CableId = Guid.NewGuid(),
                    SupplierId = cable.SupplierId,
                    YearOfManufacture = cable.YearOfManufacture,
                    Code = cable.Code,
                    Status = cable.Status,
                    WarehouseId = cable.WarehouseId,
                    StartPoint = cable.StartPoint,
                    EndPoint = itemStartPoint,
                    Length = itemStartPoint - cable.StartPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = false,
                    IsExportedToUse = false,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                Cable cableCut2 = new Cable()
                {
                    CableId = Guid.NewGuid(),
                    SupplierId = cable.SupplierId,
                    YearOfManufacture = cable.YearOfManufacture,
                    Code = cable.Code,
                    Status = cable.Status,
                    WarehouseId = cable.WarehouseId,
                    StartPoint = itemStartPoint,
                    EndPoint = itemEndPoint,
                    Length = itemEndPoint - itemStartPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = true,
                    IsExportedToUse = false,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                Cable cableCut3 = new Cable()
                {
                    CableId = Guid.NewGuid(),
                    SupplierId = cable.SupplierId,
                    YearOfManufacture = cable.YearOfManufacture,
                    Code = cable.Code,
                    Status = cable.Status,
                    WarehouseId = cable.WarehouseId,
                    StartPoint = itemEndPoint,
                    EndPoint = cable.EndPoint,
                    Length = cable.EndPoint - itemEndPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = false,
                    IsExportedToUse = false,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                list.Add(cableCut1);
                list.Add(cableCut2);
                list.Add(cableCut3);
            }
            return list;
        }

        private static Dictionary<string, object> ApproveRequestExport(DAOCable daoCable, DAOOtherMaterial daoOtherMaterial
            , Request request, List<RequestCable> requestCables, List<RequestOtherMaterial> requestMaterials)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            // list cable
            List<Cable> listCableExported = new List<Cable>();
            List<int> listCableWarehouseId = new List<int>();
            // list material
            List<int> listOtherMaterialWarehouseId = new List<int>();
            List<int> listOtherMaterialIdExported = new List<int>();
            List<int> listOtherMaterialQuantity = new List<int>();
            if (requestCables.Count > 0)
            {
                foreach (RequestCable item in requestCables)
                {
                    Cable? cable;
                    if (item.Cable.IsDeleted)
                    {
                        cable = daoCable.getCable(item.CableId, item.StartPoint, item.EndPoint);
                    }
                    else
                    {
                        cable = item.Cable;
                    }
                    if (cable != null)
                    {
                        if (item.StartPoint == cable.StartPoint && item.EndPoint == cable.EndPoint)
                        {
                            Cable? update = daoCable.getCable(cable.CableId);
                            if (update != null)
                            {
                                if (update.WarehouseId.HasValue)
                                {
                                    listCableWarehouseId.Add(update.WarehouseId.Value);
                                    listCableExported.Add(update);
                                }
                                // update cable
                                update.IsExportedToUse = true;
                                update.WarehouseId = null;
                                update.UpdateAt = DateTime.Now;
                                daoCable.UpdateCable(update);
                            }
                        }
                        else
                        {
                            List<Cable> listCut = getListCableCut(item.StartPoint, item.EndPoint, cable, request.DeliverWarehouseId);
                            foreach (Cable cut in listCut)
                            {
                                // if cable cut exported
                                if (cut.IsExportedToUse)
                                {
                                    // if warehouse cable not null
                                    if (cable.WarehouseId.HasValue)
                                    {
                                        listCableWarehouseId.Add(cable.WarehouseId.Value);
                                        listCableExported.Add(cut);
                                    }
                                    daoCable.CreateCable(cut);
                                }
                                else
                                {
                                    daoCable.CreateCable(cut);
                                }
                            }
                            // delete cable parent
                            Cable? cableParent = daoCable.getCable(cable.CableId);
                            if (cableParent != null)
                            {
                                daoCable.DeleteCable(cableParent);
                            }
                        }
                    }

                }
            }

            if (requestMaterials.Count > 0)
            {
                foreach (RequestOtherMaterial item in requestMaterials)
                {
                    if (item.OtherMaterials.WarehouseId.HasValue)
                    {
                        listOtherMaterialWarehouseId.Add(item.OtherMaterials.WarehouseId.Value);
                        listOtherMaterialIdExported.Add(item.OtherMaterialsId);
                        listOtherMaterialQuantity.Add(item.Quantity);
                    }
                    // update material quantity
                    item.OtherMaterials.Quantity = item.OtherMaterials.Quantity - item.Quantity;
                    item.OtherMaterials.UpdateAt = DateTime.Now;
                    daoOtherMaterial.Save();
                }
            }
            result["listCableExported"] = listCableExported;
            result["listCableWarehouseId"] = listCableWarehouseId;
            result["listOtherMaterialWarehouseId"] = listOtherMaterialWarehouseId;
            result["listOtherMaterialIdExported"] = listOtherMaterialIdExported;
            result["listOtherMaterialQuantity"] = listOtherMaterialQuantity;
            return result;
        }


    }
}
