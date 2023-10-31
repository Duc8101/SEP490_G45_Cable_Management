using DataAccess.DTO.RequestDTO;
using DataAccess.DTO;
using DataAccess.Entity;
using DataAccess.Const;
using System.Net;
using DataAccess.DTO.CableDTO;
using API.Model.DAO;
using DataAccess.DTO.OtherMaterialsDTO;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using API.Services.IService;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Diagnostics;
using API.Model.Util;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API.Services.Service
{
    public class RequestService : IRequestService
    {
        private readonly DAORequest daoRequest = new DAORequest();
        private readonly DAOIssue daoIssue = new DAOIssue();
        private readonly DAORequestCable daoRequestCable = new DAORequestCable();
        private readonly DAOCable daoCable = new DAOCable();
        private readonly DAOOtherMaterial daoOtherMaterial = new DAOOtherMaterial();
        private readonly DAORequestOtherMaterial daoRequestMaterial = new DAORequestOtherMaterial();
        private readonly DAOTransactionCable daoTransactionCable = new DAOTransactionCable();
        private readonly DAOTransactionOtherMaterial daoTransactionMaterial = new DAOTransactionOtherMaterial();
        private readonly DAOTransactionHistory daoHistory = new DAOTransactionHistory();
        private readonly DAOUser daoUser = new DAOUser();
        private readonly DAOWarehouse daoWarehouse = new DAOWarehouse();
        private async Task<List<RequestListDTO>> getList(string? name, string? status, Guid? CreatorID, int page)
        {
            List<DataAccess.Entity.Request> list = await daoRequest.getList(name, status, CreatorID, page);
            List<RequestListDTO> result = new List<RequestListDTO>();
            foreach (DataAccess.Entity.Request item in list)
            {
                RequestListDTO DTO = new RequestListDTO()
                {
                    RequestId = item.RequestId,
                    RequestName = item.RequestName,
                    Content = item.Content,
                    CreatorName = item.Creator.Lastname + " " + item.Creator.Firstname,
                    ApproverName = item.Approver == null ? null : item.Approver.Lastname + " " + item.Approver.Firstname,
                    Status = item.Status,
                    RequestCategoryName = item.RequestCategory.RequestCategoryName
                };
                result.Add(DTO);
            }
            return result;
        }
        public async Task<ResponseDTO<PagedResultDTO<RequestListDTO>?>> List(string? name, string? status, Guid? CreatorID, int page)
        {
            try
            {
                List<RequestListDTO> list = await getList(name, status, CreatorID, page);
                int RowCount = await daoRequest.getRowCount(name, status, CreatorID);
                PagedResultDTO<RequestListDTO> result = new PagedResultDTO<RequestListDTO>(page, RowCount, PageSizeConst.MAX_REQUEST_LIST_IN_PAGE, list);
                return new ResponseDTO<PagedResultDTO<RequestListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<RequestListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        // check cable valid when create request export and deliver
        private async Task<ResponseDTO<bool>> isCableValidCreateExportDeliver(List<CableExportDeliverDTO> list)
        {
            if (list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach (CableExportDeliverDTO DTO in list)
            {
                Cable? cable = await daoCable.getCable(DTO.CableId);
                // if exist cable not found
                if (cable == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy cáp với ID " + DTO.CableId, (int)HttpStatusCode.NotFound);
                }
                // if invalid start point, end point or deleted
                if (DTO.StartPoint < cable.StartPoint || DTO.EndPoint > cable.EndPoint || DTO.StartPoint >= DTO.EndPoint || cable.IsDeleted || DTO.StartPoint < 0 || DTO.EndPoint < 0)
                {
                    return new ResponseDTO<bool>(false, "Cáp với ID: " + DTO.CableId + " có chỉ số đầu hoặc chỉ số cuối không hợp lệ hoặc đã bị hủy", (int)HttpStatusCode.Conflict);
                }
                // if cable in use
                if (cable.IsExportedToUse)
                {
                    return new ResponseDTO<bool>(false, "Cáp với ID: " + DTO.CableId + " đã được sử dụng", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseDTO<bool>(true, string.Empty);
        }
        // check material valid when create request export, deliver,cancel inside
        private async Task<ResponseDTO<bool>> isMaterialValidCreateExportDeliverCancelInside(List<OtherMaterialsExportDeliverCancelInsideDTO> list)
        {
            if (list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach (OtherMaterialsExportDeliverCancelInsideDTO DTO in list)
            {
                OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(DTO.OtherMaterialsId);
                // if not found
                if (material == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy vật liệu vs ID = " + DTO.OtherMaterialsId, (int)HttpStatusCode.NotFound);
                }
                // if not enough quantity
                if (material.Quantity < DTO.Quantity)
                {
                    return new ResponseDTO<bool>(false, "Vật liệu vs ID = " + DTO.OtherMaterialsId + " không có đủ số lượng", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseDTO<bool>(true, string.Empty);
        }
        // add request to db
        private Guid CreateRequest(string RequestName, string? content, Guid? IssueID, Guid CreatorID, int RequestCategoryID, int? DeliverWarehouseID)
        {
            DataAccess.Entity.Request request = new DataAccess.Entity.Request()
            {
                RequestId = Guid.NewGuid(),
                RequestName = RequestName.Trim(),
                Content = content == null || content.Trim().Length == 0 ? null : content.Trim(),
                IssueId = IssueID,
                CreatorId = CreatorID,
                CreatedAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                IsDeleted = false,
                RequestCategoryId = RequestCategoryID,
                Status = RequestConst.STATUS_PENDING,
                DeliverWarehouseId = DeliverWarehouseID
            };
            daoRequest.CreateRequest(request);
            return request.RequestId;
        }
        // create request cable when create request export, deliver
        private void CreateRequestCableExportDeliver(List<CableExportDeliverDTO> list, Guid RequestID)
        {
            if (list.Count > 0)
            {
                foreach (CableExportDeliverDTO item in list)
                {
                    RequestCable request = new RequestCable()
                    {
                        RequestId = RequestID,
                        CableId = item.CableId,
                        StartPoint = item.StartPoint,
                        EndPoint = item.EndPoint,
                        Length = item.EndPoint - item.StartPoint,
                        RecoveryDestWarehouseId = item.WarehouseId,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                    };
                    daoRequestCable.CreateRequestCable(request);
                }
            }
        }
        // create request material when create request export, deliver
        private void CreateRequestMaterialExportDeliver(List<OtherMaterialsExportDeliverCancelInsideDTO> list, Guid RequestID)
        {
            if (list.Count > 0)
            {
                foreach (OtherMaterialsExportDeliverCancelInsideDTO item in list)
                {
                    RequestOtherMaterial request = new RequestOtherMaterial()
                    {
                        RequestId = RequestID,
                        OtherMaterialsId = item.OtherMaterialsId,
                        Quantity = item.Quantity,
                        RecoveryDestWarehouseId = item.WarehouseId,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                    };
                    daoRequestMaterial.CreateRequestOtherMaterial(request);
                }
            }
        }
        // send email to admin after finish creating request
        private async Task sendEmailToAdmin(string RequestName, string RequestCategoryName, Issue? issue)
        {
            // get all email admin
            List<string> list = await daoUser.getEmailAdmins();
            string body = UserUtil.BodyEmailForAdminReceiveRequest(RequestName, RequestCategoryName, issue);
            foreach (string email in list)
            {
                await UserUtil.sendEmail("[FPT TELECOM CABLE MANAGEMENT] Thông báo có yêu cầu mới", body, email);
            }
        }
        public async Task<ResponseDTO<bool>> CreateRequestExport(RequestCreateExportDTO DTO, Guid CreatorID)
        {
            if (DTO.RequestName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên yêu cầu không được để trống", (int)HttpStatusCode.Conflict);
            }
            if (DTO.RequestCategoryId != RequestCategoryConst.CATEGORY_EXPORT)
            {
                return new ResponseDTO<bool>(false, "Không phải yêu cầu xuất kho", (int)HttpStatusCode.Conflict);
            }
            try
            {
                Issue? issue = await daoIssue.getIssue(DTO.IssueId);
                // if not found issue
                if (issue == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                // if issue done
                if (issue.Status.Equals(IssueConst.STATUS_DONE))
                {
                    return new ResponseDTO<bool>(false, "Sự vụ với mã " + issue.IssueCode + " đã được chấp thuận", (int)HttpStatusCode.NotAcceptable);
                }
                ResponseDTO<bool> response = await isCableValidCreateExportDeliver(DTO.CableExportDTOs);
                // if cable invalid
                if (response.Success == false)
                {
                    return response;
                }
                response = await isMaterialValidCreateExportDeliverCancelInside(DTO.OtherMaterialsExportDTOs);
                // if material invalid
                if (response.Success == false)
                {
                    return response;
                }
                //----------------------------- create request --------------------------------
                Guid RequestID = CreateRequest(DTO.RequestName.Trim(), DTO.Content, DTO.IssueId, CreatorID, DTO.RequestCategoryId, null);
                CreateRequestCableExportDeliver(DTO.CableExportDTOs, RequestID);
                CreateRequestMaterialExportDeliver(DTO.OtherMaterialsExportDTOs, RequestID);
                // ----------------------------- send email to admin ---------------------------
                await sendEmailToAdmin(DTO.RequestName.Trim(), "Xuất kho", issue);
                return new ResponseDTO<bool>(true, "Tạo yêu cầu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        // check cable valid when approve request export, deliver, cancel inside
        private async Task<ResponseDTO<bool>> isCableValidApproveExportDeliverCancelInside(List<RequestCable> list, bool action)
        {
            if (list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach (RequestCable item in list)
            {
                Cable? cable;
                // if cable deleted
                if (item.Cable.IsDeleted)
                {
                    cable = await daoCable.getCable(item.CableId, item.StartPoint, item.EndPoint);
                }
                else
                {
                    cable = item.Cable;
                }
                // if not found cable or start point, end point invalid
                if (cable == null || item.StartPoint < cable.StartPoint || item.EndPoint > cable.EndPoint)
                {
                    string mess = action ? "xuất kho" : "hủy";
                    return new ResponseDTO<bool>(false, "Không thể "+ mess + " " + item.Cable.CableCategory.CableCategoryName + " với ID: " + item.CableId
                               + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ")", (int)HttpStatusCode.Conflict);
                }
                // if cable exported
                if (cable.IsExportedToUse == true)
                {
                    return new ResponseDTO<bool>(false, item.Cable.CableCategory.CableCategoryName + " với ID: " + item.CableId
                        + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ") đã được sử dụng!", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseDTO<bool>(true, string.Empty);
        }
        // check material valid when approve request export, deliver, cancel inside
        private async Task<ResponseDTO<bool>> isMaterialValidApproveExportDeliverCancelInside(List<RequestOtherMaterial> list)
        {
            if (list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach (RequestOtherMaterial item in list)
            {
                OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(item.OtherMaterialsId);
                // if not found
                if(material == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy vật liệu " + item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName, (int)HttpStatusCode.NotFound);
                }
                // if not enough quantity
                if (material.Quantity < item.Quantity)
                {
                    return new ResponseDTO<bool>(false, "Không đủ số lượng " + item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName, (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseDTO<bool>(true, string.Empty);
        }
        private void UpdateRequest(DataAccess.Entity.Request request, Guid ID, string status)
        {
            request.ApproverId = ID;
            request.Status = status;
            request.UpdateAt = DateTime.Now;
            daoRequest.UpdateRequest(request);
        }
        // send email to creator after approve request
        private async Task sendEmailToCreator(DataAccess.Entity.Request request, string ApproverName)
        {
            string body = await UserUtil.BodyEmailForApproveRequest(request, ApproverName);
            await UserUtil.sendEmail("Thông báo yêu cầu được phê duyệt", body, request.Creator.Email);
        }
        private async Task CreateTransaction(DataAccess.Entity.Request request, List<int> listCableWarehouseID, List<Cable> listCable, List<int> listMaterialWarehouseID, List<int> listMaterialID, List<int> listQuantity, bool? action)
        {
            string CategoryName;
            int? FromWarehouseID = null;
            int? ToWarehouseID = null;
            if(action == null)
            {
                CategoryName = TransactionCategoryConst.CATEGORY_CANCEL;
            }
            else
            {
                CategoryName = action == true ? TransactionCategoryConst.CATEGORY_IMPORT : TransactionCategoryConst.CATEGORY_EXPORT;
            }
            // if transaction export and request deliver
            if(action == false && request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER)
            {
                ToWarehouseID = request.DeliverWarehouseId;
                // if transaction import and request deliver
            }else if(action == true && request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER)
            {
                FromWarehouseID = request.DeliverWarehouseId;
            }
            // ------------------------------- create transaction history -----------------------------
            // list contain all warehouse of cable and material
            List<int> listAllWarehouse = new List<int>();
            if (listCableWarehouseID.Count > 0)
            {
                foreach(int WarehouseID in listCableWarehouseID)
                {
                    // if list all warehouse not contain warehouse exist in list
                    if (!listAllWarehouse.Contains(WarehouseID))
                    {
                        listAllWarehouse.Add(WarehouseID);
                    }
                }
            }
            if (listMaterialWarehouseID.Count > 0)
            {
                foreach (int WarehouseID in listMaterialWarehouseID)
                {
                    // if list all warehouse not contain material warehouse
                    if (!listAllWarehouse.Contains(WarehouseID))
                    {
                        listAllWarehouse.Add(WarehouseID);
                    }
                }
            }
            // list key-value to store trasaction id and warehouse id
            Dictionary<int, Guid> dic = new Dictionary<int, Guid>();
            foreach (int WarehouseID in listAllWarehouse)
            {
                TransactionHistory history = new TransactionHistory()
                {
                    TransactionId = Guid.NewGuid(),
                    TransactionCategoryName = CategoryName,
                    CreatedDate = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false,
                    WarehouseId = WarehouseID,
                    RequestId = request.RequestId,
                    IssueId = request.IssueId,
                    FromWarehouseId = FromWarehouseID,
                    ToWarehouseId = ToWarehouseID
                };
                await daoHistory.CreateTransactionHistory(history);
                dic[WarehouseID] = history.TransactionId;
            }
            // ------------------------------- create transaction cable --------------------------------
            if (listCableWarehouseID.Count > 0)
            {
                for (int i = 0; i < listCableWarehouseID.Count; i++)
                {
                    TransactionCable transactionCable = new TransactionCable()
                    {
                        TransactionId = dic[listCableWarehouseID[i]],
                        CableId = listCable[i].CableId,
                        StartPoint = listCable[i].StartPoint,
                        EndPoint = listCable[i].EndPoint,
                        Length = listCable[i].Length,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false
                    };
                    await daoTransactionCable.CreateTransactionCable(transactionCable);
                }
            }
            // ------------------------------- create transaction material --------------------------------
            if (listMaterialWarehouseID.Count > 0)
            {
                for (int i = 0; i < listMaterialWarehouseID.Count; i++)
                {
                    TransactionOtherMaterial material = new TransactionOtherMaterial()
                    {
                        TransactionId = dic[listMaterialWarehouseID[i]],
                        OtherMaterialsId = listMaterialID[i],
                        Quantity = listQuantity[i],
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                    };
                    await daoTransactionMaterial.CreateTransactionMaterial(material);
                }
            }

        }
        private async Task<ResponseDTO<bool>> ApproveRequestExportDeliverCancelInside(Guid ApproverID, DataAccess.Entity.Request request, string ApproverName)
        {
            List<RequestCable> requestCables = await daoRequestCable.getList(request.RequestId);
            List<RequestOtherMaterial> requestMaterials = await daoRequestMaterial.getList(request.RequestId);
            // ------------------------------- check cable valid ------------------------------  
            ResponseDTO<bool> check = await isCableValidApproveExportDeliverCancelInside(requestCables, true);// true : xuat kho , false:huy
            // if exist cable not valid
            if (check.Success == false)
            {
                return check;
            }
            // ------------------------------- check material valid ------------------------------
            check = await isMaterialValidApproveExportDeliverCancelInside(requestMaterials);
            // if exist material not valid
            if (check.Success == false)
            {
                return check;
            }
            // ------------------------------- create transaction if request deliver --------------
            // list cable
            List<Cable> listCableExportedDeliverCancelInside = new List<Cable>();
            List<int> listCableWarehouseID = new List<int>();
            // list material
            List<int> listMaterialWarehouseID = new List<int>();
            List<int> listMaterialIDExportedDeliverCancelInside = new List<int>();
            List<int> listQuantity = new List<int>(); 
            if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER)
            {
                if(requestCables.Count > 0)
                {
                    foreach (RequestCable item in requestCables)
                    {
                        if (item.Cable.WarehouseId.HasValue)
                        {
                            listCableExportedDeliverCancelInside.Add(item.Cable);
                            listCableWarehouseID.Add(item.Cable.WarehouseId.Value);
                        }
                    }
                }
                if(requestMaterials.Count > 0)
                {
                    foreach(RequestOtherMaterial item in requestMaterials)
                    {
                        if (item.OtherMaterials.WarehouseId.HasValue)
                        {
                            listMaterialIDExportedDeliverCancelInside.Add(item.OtherMaterialsId);
                            listMaterialWarehouseID.Add(item.OtherMaterials.WarehouseId.Value);
                            listQuantity.Add(item.Quantity);
                        }
                    }
                }
                // create transaction
                await CreateTransaction(request, listCableWarehouseID,listCableExportedDeliverCancelInside,listMaterialWarehouseID,
                    listMaterialIDExportedDeliverCancelInside,listQuantity,false);
            }
            // ------------------------------- request cable ------------------------------
            listCableExportedDeliverCancelInside = new List<Cable>();
            listCableWarehouseID = new List<int>();
            if (requestCables.Count > 0)
            {
                foreach (RequestCable item in requestCables)
                {
                    Cable? cable;
                    if (item.Cable.IsDeleted)
                    {
                        cable = await daoCable.getCable(item.CableId, item.StartPoint, item.EndPoint);
                    }
                    else
                    {
                        cable = item.Cable;
                    }
                    // if cable valid
                    if (cable != null && cable.IsExportedToUse == false && item.StartPoint >= cable.StartPoint && item.EndPoint <= cable.EndPoint && cable.StartPoint >=0 && cable.EndPoint >=0)
                    {
                        if (item.StartPoint == cable.StartPoint && item.EndPoint == cable.EndPoint)
                        {
                            Cable? update = await daoCable.getCable(cable.CableId);
                            if (update != null)
                            {
                                // if exported
                                if(request.RequestCategoryId == RequestCategoryConst.CATEGORY_EXPORT)
                                {
                                    if (update.WarehouseId.HasValue)
                                    {
                                        listCableWarehouseID.Add(update.WarehouseId.Value);
                                        listCableExportedDeliverCancelInside.Add(update);
                                    }              
                                    // update cable
                                    update.IsExportedToUse = true;
                                    update.WarehouseId = null;
                                    // if deliver
                                }else if(request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER)
                                {
                                    if (request.DeliverWarehouseId.HasValue)
                                    {
                                        listCableWarehouseID.Add(request.DeliverWarehouseId.Value);
                                        listCableExportedDeliverCancelInside.Add(update);
                                    }
                                    update.WarehouseId = request.DeliverWarehouseId;
                                    // if cancel inside
                                }else if(request.RequestCategoryId == RequestCategoryConst.CATEGORY_CANCEL_INSIDE)
                                {
                                    if (update.WarehouseId.HasValue)
                                    {
                                        listCableWarehouseID.Add(update.WarehouseId.Value);
                                        listCableExportedDeliverCancelInside.Add(update);
                                    }
                                    update.IsDeleted = true;
                                }
                                update.UpdateAt = DateTime.Now;
                                await daoCable.UpdateCable(update);
                            }                     
                        }
                        else
                        {
                            List<Cable> listCut = new List<Cable>();
                            // if request export and deliver
                            if(request.RequestCategoryId == RequestCategoryConst.CATEGORY_EXPORT || request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER)
                            {
                                // get list cable cut request export and deliver
                                listCut = CableUtil.getListCableCut(item.StartPoint, item.EndPoint, cable, request.DeliverWarehouseId);
                            }// if request cancel inside
                            else if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_CANCEL_INSIDE)
                            {
                                // get list cable cut request cancel inside
                                listCut = CableUtil.getListCableCut(item.StartPoint, item.EndPoint, cable);
                            }
                            foreach (Cable cut in listCut)
                            {
                                // if cable cut request
                                if ((request.RequestCategoryId == RequestCategoryConst.CATEGORY_EXPORT && cut.IsExportedToUse) 
                                    || (request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER && cut.WarehouseId == request.DeliverWarehouseId)
                                    || (request.RequestCategoryId == RequestCategoryConst.CATEGORY_CANCEL_INSIDE && cut.IsDeleted))
                                {          
                                    // if request exported
                                    if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_EXPORT)
                                    {
                                        if (cable.WarehouseId.HasValue)
                                        {
                                            listCableWarehouseID.Add(cable.WarehouseId.Value);
                                            listCableExportedDeliverCancelInside.Add(cut);
                                        }
                                    }
                                    else if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER)
                                    {
                                        if (request.DeliverWarehouseId.HasValue)
                                        {
                                            listCableWarehouseID.Add(request.DeliverWarehouseId.Value);
                                            listCableExportedDeliverCancelInside.Add(cut);
                                        }
                                    }
                                    else
                                    {
                                        if (cut.WarehouseId.HasValue)
                                        {
                                            listCableWarehouseID.Add(cut.WarehouseId.Value);
                                            listCableExportedDeliverCancelInside.Add(cut);
                                        }
                                    }
                                    await daoCable.CreateCable(cut);
                                    // update request cable
                                    daoRequestCable.RemoveRequestCable(item);
                                    RequestCable create = new RequestCable()
                                    {
                                        RequestId = item.RequestId,
                                        CableId = cut.CableId,
                                        StartPoint = cut.StartPoint,
                                        EndPoint = cut.EndPoint,
                                        Length = cut.Length,
                                        RecoveryDestWarehouseId = item.RecoveryDestWarehouseId,
                                        CreatedAt = cut.CreatedAt,
                                        UpdateAt = DateTime.Now,
                                        IsDeleted = false
                                    };
                                    daoRequestCable.CreateRequestCable(create);
                                }
                                else
                                {
                                    await daoCable.CreateCable(cut);
                                }
                            }
                            // delete cable parent
                            Cable? cableParent = await daoCable.getCable(cable.CableId);
                            if(cableParent != null)
                            {
                               await daoCable.DeleteCable(cableParent);
                            }
                             
                        }
                    }
                }
            }
            // ------------------------------- request material ------------------------------
            listMaterialWarehouseID = new List<int>();
            listMaterialIDExportedDeliverCancelInside = new List<int>();
            listQuantity = new List<int>();
            if (requestMaterials.Count > 0)
            {
                foreach (RequestOtherMaterial item in requestMaterials)
                {
                    OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(item.OtherMaterialsId);
                    // if material valid
                    if(material != null && material.Quantity >= item.Quantity)
                    {
                        if (material.WarehouseId.HasValue)
                        {
                            // if not request deliver
                            if(request.RequestCategoryId != RequestCategoryConst.CATEGORY_DELIVER)
                            {
                                listMaterialWarehouseID.Add(material.WarehouseId.Value);
                                listMaterialIDExportedDeliverCancelInside.Add(material.OtherMaterialsId);
                                listQuantity.Add(item.Quantity);
                            }  
                        }
                        // update material quantity
                        material.Quantity = material.Quantity - item.Quantity;
                        material.UpdateAt = DateTime.Now;
                        await daoOtherMaterial.UpdateMaterial(material);
                        // if request deliver
                        if(request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER)
                        {
                            OtherMaterial? exist = await daoOtherMaterial.getOtherMaterial(request.DeliverWarehouseId, material.Code, material.Status, material.Unit);
                            // if material not exist in deliver warehouse
                            if (exist == null)
                            {
                                exist = new OtherMaterial()
                                {
                                    OtherMaterialsCategoryId = material.OtherMaterialsCategoryId,
                                    Code = material.Code,
                                    Unit = material.Unit,
                                    Quantity = item.Quantity,
                                    WarehouseId = request.DeliverWarehouseId,
                                    Status = material.Status,
                                    SupplierId = material.SupplierId
                                };
                                // create material
                                int MaterialID = await daoOtherMaterial.CreateMaterial(exist);
                                if (request.DeliverWarehouseId.HasValue)
                                {
                                    listMaterialIDExportedDeliverCancelInside.Add(MaterialID);
                                    listQuantity.Add(item.Quantity);
                                    listMaterialWarehouseID.Add(request.DeliverWarehouseId.Value);
                                }
                                // update request material
                                daoRequestMaterial.RemoveRequestMaterial(item);
                                RequestOtherMaterial requestMaterial = new RequestOtherMaterial()
                                {
                                    RequestId = item.RequestId,
                                    OtherMaterialsId = MaterialID,
                                    Quantity = item.Quantity,
                                    CreatedAt = item.CreatedAt,
                                    UpdateAt = item.UpdateAt,
                                    IsDeleted = false
                                };
                                daoRequestMaterial.CreateRequestOtherMaterial(requestMaterial);
                            }
                            else
                            {
                                if (request.DeliverWarehouseId.HasValue)
                                {
                                    listMaterialIDExportedDeliverCancelInside.Add(item.OtherMaterialsId);
                                    listQuantity.Add(item.Quantity);
                                    listMaterialWarehouseID.Add(request.DeliverWarehouseId.Value);
                                }     
                                // update material
                                exist.Quantity = exist.Quantity + item.Quantity;
                                exist.UpdateAt = DateTime.Now;
                                await daoOtherMaterial.UpdateMaterial(exist);
                            }
                        }
                    }
                }
            }
            // ------------------------------- update request approved --------------------------------
            UpdateRequest(request, ApproverID, RequestConst.STATUS_APPROVED);
            // ------------------------------- create transaction --------------------------------
            bool? action;
            // if request deliver
            if(request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER)
            {
                action = true;// import
            }else if(request.RequestCategoryId == RequestCategoryConst.CATEGORY_EXPORT)
            {
                action = false; // export
            }
            else
            {
                action = null; // cancel
            }
            await CreateTransaction(request, listCableWarehouseID, listCableExportedDeliverCancelInside, listMaterialWarehouseID, listMaterialIDExportedDeliverCancelInside, listQuantity, action);
            // ------------------------------- send email --------------------------------
            await sendEmailToCreator(request, ApproverName);
            return new ResponseDTO<bool>(true, "Yêu cầu được phê duyệt");
        }
        // create cable while creating request recovery
        private async Task<List<Cable>> CreateCable(List<CableCreateUpdateDTO> list, Guid CreatorID)
        {
            List<Cable> result = new List<Cable>();
            if (list.Count > 0)
            {
                foreach(CableCreateUpdateDTO DTO in list)
                {
                    Cable cable = new Cable()
                    {
                        CableId = Guid.NewGuid(),
                        WarehouseId = DTO.WarehouseId,
                        SupplierId = DTO.SupplierId,
                        StartPoint = DTO.StartPoint,
                        EndPoint = DTO.EndPoint,
                        Length = DTO.EndPoint - DTO.StartPoint,
                        YearOfManufacture = DTO.YearOfManufacture,
                        Code = DTO.Code.Trim(),
                        Status = DTO.Status.Trim(),
                        CreatorId = CreatorID,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        IsExportedToUse = false,
                        CableCategoryId = DTO.CableCategoryId,
                        IsInRequest = true,
                    };
                    await daoCable.CreateCable(cable);
                    result.Add(cable);
                }
            }
            return result;
        }
        // create material while creating request recovery
        private async Task<Dictionary<string, object>> CreateMaterial(List<OtherMaterialsRecoveryDTO> list)
        {
            // result to store list warehouse, list material id, list quantity, list status
            Dictionary<string, object> result = new Dictionary<string, object>();
            // list warehouse
            List<int?> listWareHouse = new List<int?>();
            // list quantity
            List<int> listQuantity = new List<int>();
            // list material id
            List<int> listID = new List<int>();
            // list status
            List<string> listStatus = new List<string>();
            if(list.Count > 0)
            {
                foreach(OtherMaterialsRecoveryDTO DTO in list)
                {
                    OtherMaterial material = new OtherMaterial()
                    {
                        Unit = DTO.Unit.Trim(),
                        Code = DTO.Code,
                        SupplierId = DTO.SupplierId,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        WarehouseId = DTO.WarehouseId,
                        Status = DTO.Status.Trim(),
                        OtherMaterialsCategoryId = DTO.OtherMaterialsCategoryId
                    };
                    int OtherMaterialID = await daoOtherMaterial.CreateMaterial(material);
                    listWareHouse.Add(DTO.WarehouseId);
                    listQuantity.Add(DTO.Quantity);
                    listID.Add(OtherMaterialID);
                    listStatus.Add(DTO.Status);
                }
                // ----------------------- update quantity -----------------
                foreach (int MaterialID in listID)
                {
                    OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(MaterialID);
                    if (material != null)
                    {
                        material.Quantity = 0;
                        await daoOtherMaterial.UpdateMaterial(material);
                    }
                }
            }
            result["Warehouse"] = listWareHouse;
            result["quantity"] = listQuantity;
            result["ID"] = listID;
            result["status"] = listStatus;
            return result;
        }
        // create request cable when create request recovery
        private void CreateRequestCableRecovery(List<Cable> list, Guid RequestID)
        {
            if(list.Count > 0)
            {
                foreach(Cable cable in list)
                {
                    RequestCable request = new RequestCable()
                    {
                        RequestId = RequestID,
                        CableId = cable.CableId,
                        StartPoint = cable.StartPoint,
                        EndPoint = cable.EndPoint,
                        Length = cable.EndPoint - cable.StartPoint,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        RecoveryDestWarehouseId = cable.WarehouseId,
                        Status = cable.Status,
                    };
                    daoRequestCable.CreateRequestCable(request);
                }
            }
        }
        // create request material when create request recovery
        private void CreateRequestMaterialRecovery(Dictionary<string, object> dic, Guid RequestID)
        {
            if(dic.Count > 0)
            {
                List<int?> listWareHouse = (List<int?>) dic["Warehouse"];
                List<int> listQuantity = (List<int>) dic["quantity"];
                List<int> listID = (List<int>) dic["ID"];
                List<string> listStatus = (List<string>) dic["status"];
                for(int i = 0; i < listID.Count; i++)
                {
                    RequestOtherMaterial request = new RequestOtherMaterial()
                    {
                        RequestId = RequestID,
                        OtherMaterialsId = listID[i],
                        Quantity = listQuantity[i],
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        RecoveryDestWarehouseId = listWareHouse[i],
                        Status = listStatus[i]
                    };
                    daoRequestMaterial.CreateRequestOtherMaterial(request);
                }
            }
        }
        public async Task<ResponseDTO<bool>> CreateRequestRecovery(RequestCreateRecoveryDTO DTO, Guid CreatorID)
        {
            if (DTO.RequestName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên yêu cầu không được để trống", (int)HttpStatusCode.Conflict);
            }
            if (DTO.RequestCategoryId != RequestCategoryConst.CATEGORY_RECOVERY)
            {
                return new ResponseDTO<bool>(false, "Không phải yêu cầu thu hồi", (int)HttpStatusCode.Conflict);
            }
            try
            {
                Issue? issue = await daoIssue.getIssue(DTO.IssueId);
                // if not found issue
                if (issue == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                // if issue done
                if (issue.Status.Equals(IssueConst.STATUS_DONE))
                {
                    return new ResponseDTO<bool>(false, "Sự vụ với mã " + issue.IssueCode + " đã được chấp thuận", (int)HttpStatusCode.NotAcceptable);
                }
                // create cable
                List<Cable> listCable = await CreateCable(DTO.CableRecoveryDTOs, CreatorID);
                // create material
                // result to store list warehouse, list material id, list quantity, list status
                Dictionary<string, object> dic  = await CreateMaterial(DTO.OtherMaterialsRecoveryDTOs);
                // ------------------------------ create request ----------------------
                Guid RequestID = CreateRequest(DTO.RequestName.Trim(), DTO.Content, DTO.IssueId, CreatorID, DTO.RequestCategoryId, null);
                // create request cable
                CreateRequestCableRecovery(listCable, RequestID);
                // create request material
                CreateRequestMaterialRecovery(dic, RequestID);
                // ----------------------------- send email to admin ---------------------------
                await sendEmailToAdmin(DTO.RequestName.Trim(), "Thu hồi", issue);
                return new ResponseDTO<bool>(true, "Tạo yêu cầu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        // check cable valid when approve request recovery
        private async Task<ResponseDTO<bool>> isCableValidApproveRecovery(List<RequestCable> list)
        {
            if(list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach(RequestCable request in list)
            {
                Cable? cable = await daoCable.getCable(request.CableId);
                if(cable == null)
                {
                    return new ResponseDTO<bool>(false, request.Cable.CableCategory.CableCategoryName + " với ID: " + request.CableId
                                + " không tồn tại " , (int) HttpStatusCode.NotFound);
                }

                if(cable.StartPoint < 0 || cable.EndPoint < 0 || cable.StartPoint >= cable.EndPoint)
                {
                    return new ResponseDTO<bool>(false, request.Cable.CableCategory.CableCategoryName + " với ID: " + request.CableId
                                + " có chỉ số đầu chỉ số cuối không hợp lệ ", (int)HttpStatusCode.NotFound);
                }
            }
            return new ResponseDTO<bool>(true, string.Empty);
        }
        // check material valid when approve request recovery
        private async Task<ResponseDTO<bool>> isMaterialValidApproveRecovery(List<RequestOtherMaterial> list)
        {
            if(list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach(RequestOtherMaterial request in list)
            {
                if(request.Quantity < 0)
                {
                    return new ResponseDTO<bool>(false, "Số lượng không hợp lệ", (int)HttpStatusCode.NotFound);
                }
                OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(request.OtherMaterialsId);
                if(material == null)
                {
                    return new ResponseDTO<bool>(false, request.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName + " với ID: " + request.OtherMaterialsId
                                + " không tồn tại ", (int)HttpStatusCode.NotFound);
                }
            }
            return new ResponseDTO<bool>(true, string.Empty);
        }
        private async Task<ResponseDTO<bool>> ApproveRequestRecovery(Guid ApproverID, DataAccess.Entity.Request request, string ApproverName)
        {
            List<RequestCable> requestCables = await daoRequestCable.getList(request.RequestId);
            List<RequestOtherMaterial> requestMaterials = await daoRequestMaterial.getList(request.RequestId);
            // ------------------------------- check cable valid ------------------------------  
            ResponseDTO<bool> check = await isCableValidApproveRecovery(requestCables);
            // if exist cable not valid
            if (check.Success == false)
            {
                return check;
            }
            // ------------------------------- check material valid ------------------------------
            check = await isMaterialValidApproveRecovery(requestMaterials);
            // if exist material not valid
            if (check.Success == false)
            {
                return check;
            }
            // ------------------------------- request cable ------------------------------
            List<Cable> listCableRecovery = new List<Cable>();
            List<int> listCableWarehouseID = new List<int>();
            if (requestCables.Count > 0)
            {
                foreach(RequestCable item in  requestCables)
                {
                    Cable? cable = await daoCable.getCable(item.CableId);
                    if(cable != null)
                    {
                        if (item.RecoveryDestWarehouseId.HasValue)
                        {
                            listCableRecovery.Add(cable);
                            listCableWarehouseID.Add(item.RecoveryDestWarehouseId.Value);
                        } 
                        // update cable
                        cable.IsInRequest = false;
                        cable.UpdateAt = DateTime.Now;
                        await daoCable.UpdateCable(cable);
                    }
                }
            }
            // ------------------------------- request material ------------------------------
            List<int> listMaterialWarehouseID = new List<int>();
            List<int> listMaterialIDRecovery= new List<int>();
            List<int> listQuantity = new List<int>();
            if(requestMaterials.Count > 0)
            {
                foreach(RequestOtherMaterial item in requestMaterials)
                {
                    OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(item.OtherMaterialsId);
                    if(material != null)
                    {
                        if (item.RecoveryDestWarehouseId.HasValue)
                        {
                            listMaterialIDRecovery.Add(material.OtherMaterialsId);
                            listMaterialWarehouseID.Add(item.RecoveryDestWarehouseId.Value);
                            listQuantity.Add(item.Quantity);
                        }
                        // update material
                        material.Quantity = material.Quantity + item.Quantity;
                        material.UpdateAt = DateTime.Now;
                        await daoOtherMaterial.UpdateMaterial(material);
                    }
                }
            }
            // ------------------------------- update request approved --------------------------------
            UpdateRequest(request, ApproverID, RequestConst.STATUS_APPROVED);
            // ------------------------------- create transaction ------------------------------
            await CreateTransaction(request, listCableWarehouseID, listCableRecovery, listMaterialWarehouseID, listMaterialIDRecovery, listQuantity, true);
            // ------------------------------- send email ------------------------------
            await sendEmailToCreator(request, ApproverName);
            return new ResponseDTO<bool>(true, "Yêu cầu được phê duyệt");
        }
        public async Task<ResponseDTO<bool>> Approve(Guid RequestID, Guid ApproverID, string ApproverName)
        {
            try
            {
                DataAccess.Entity.Request? request = await daoRequest.getRequest(RequestID);
                if (request == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy yêu cầu", (int)HttpStatusCode.NotFound);
                }
                if (!request.Status.Equals(RequestConst.STATUS_PENDING))
                {
                    return new ResponseDTO<bool>(false, "Yêu cầu đã được xác nhận chấp thuận hoặc bị từ chối", (int)HttpStatusCode.Conflict);
                }
                if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_EXPORT || request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER || request.RequestCategoryId == RequestCategoryConst.CATEGORY_CANCEL_INSIDE)
                {
                    return await ApproveRequestExportDeliverCancelInside(ApproverID, request, ApproverName);
                }
                if(request.RequestCategoryId == RequestCategoryConst.CATEGORY_RECOVERY)
                {
                    return await ApproveRequestRecovery(ApproverID, request, ApproverName);
                }
                if(request.RequestCategoryId == RequestCategoryConst.CATEGORY_CANCEL_OUTSIDE)
                {
                    return await ApproveRequestCancelOutside(ApproverID, request, ApproverName);
                }
                return new ResponseDTO<bool>(false, "Không hỗ trợ yêu cầu " + request.RequestCategory.RequestCategoryName, (int)HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Reject(Guid RequestID, Guid RejectorID)
        {
            try
            {
                DataAccess.Entity.Request? request = await daoRequest.getRequest(RequestID);
                if (request == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy yêu cầu", (int)HttpStatusCode.NotFound);
                }
                if (!request.Status.Equals(RequestConst.STATUS_PENDING))
                {
                    return new ResponseDTO<bool>(false, "Yêu cầu đã được xác nhận chấp thuận hoặc bị từ chối", (int)HttpStatusCode.Conflict);
                }
                // ------------------- update request ---------------
                UpdateRequest(request, RejectorID, RequestConst.STATUS_REJECTED);
                return new ResponseDTO<bool>(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> CreateRequestDeliver(RequestCreateDeliverDTO DTO, Guid CreatorID)
        {
            if (DTO.RequestName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên yêu cầu không được để trống", (int)HttpStatusCode.Conflict);
            }
            if (DTO.RequestCategoryId != RequestCategoryConst.CATEGORY_DELIVER)
            {
                return new ResponseDTO<bool>(false, "Không phải yêu cầu chuyển kho", (int)HttpStatusCode.Conflict);
            }
            try
            {
                Warehouse? ware = await daoWarehouse.getWarehouse(DTO.DeliverWareHouseID);
                if(ware == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy kho nhận", (int)HttpStatusCode.NotFound);
                }
                ResponseDTO<bool> response = await isCableValidCreateExportDeliver(DTO.CableDeliverDTOs);
                // if cable invalid
                if (response.Success == false)
                {
                    return response;
                }
                response = await isMaterialValidCreateExportDeliverCancelInside(DTO.OtherMaterialsDeliverDTOs);
                // if material invalid
                if (response.Success == false)
                {
                    return response;
                }
                //----------------------------- create request --------------------------------
                Guid RequestID = CreateRequest(DTO.RequestName.Trim(), DTO.Content, null, CreatorID, DTO.RequestCategoryId, DTO.DeliverWareHouseID);
                //----------------------------- create request cable --------------------------------
                CreateRequestCableExportDeliver(DTO.CableDeliverDTOs, RequestID);
                //----------------------------- create request material --------------------------------
                CreateRequestMaterialExportDeliver(DTO.OtherMaterialsDeliverDTOs, RequestID);
                // ----------------------------- send email to admin ---------------------------
                await sendEmailToAdmin(DTO.RequestName.Trim(), "Thu hồi", null);
                return new ResponseDTO<bool>(true, "Tạo yêu cầu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<List<CableListDTO>?>> SuggestionCable(SuggestionCableDTO suggestion)
        {
            try
            {
                List<Cable> list = await daoCable.getListAll();
                // if choose warehouse
                if(suggestion.WarehouseIds.Count > 0)
                {
                    list = list.Where(c => c.WarehouseId != null && suggestion.WarehouseIds.Contains(c.WarehouseId.Value)).ToList();
                }
                // if choose category
                if(suggestion.CableCategoryIds.Count > 0)
                {
                    list = list.Where(c => suggestion.CableCategoryIds.Contains(c.CableCategoryId)).ToList();
                }
                list = list.OrderByDescending(c => c.Length).ToList();
                List<CableListDTO> result = new List<CableListDTO>();
                int total = 0; // total length
                foreach (Cable item in list)
                {
                    // if total equal or greater than length request
                    if(total >= suggestion.Length)
                    {
                        break;
                    }
                    CableListDTO DTO = new CableListDTO()
                    {
                        CableId = item.CableId,
                        WarehouseId = item.WarehouseId,
                        WarehouseName = item.Warehouse == null ? null : item.Warehouse.WarehouseName,
                        SupplierId = item.SupplierId,
                        SupplierName = item.Supplier.SupplierName,
                        StartPoint = item.StartPoint,
                        EndPoint = item.EndPoint,
                        Length = item.Length,
                        YearOfManufacture = item.YearOfManufacture,
                        Code = item.Code,
                        Status = item.Status,
                        IsExportedToUse = item.IsExportedToUse,
                        IsInRequest = item.IsInRequest,
                        CableCategoryId = item.CableCategoryId,
                        CableCategoryName = item.CableCategory.CableCategoryName
                    };
                    result.Add(DTO);
                    total = total + item.Length;
                }
                return new ResponseDTO<List<CableListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<CableListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        // check cable valid when create request cancel inside 
        private async Task<ResponseDTO<bool>> isCableValidCreateCancelInside(List<CableCancelInsideDTO> list)
        {
            if (list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach (CableCancelInsideDTO DTO in list)
            {
                Cable? cable = await daoCable.getCable(DTO.CableId);
                // if exist cable not found
                if (cable == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy cáp với ID " + DTO.CableId, (int)HttpStatusCode.NotFound);
                }
                // if cable in use
                if (cable.IsExportedToUse)
                {
                    return new ResponseDTO<bool>(false, "Cáp với ID: " + DTO.CableId + " đã được sử dụng", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseDTO<bool>(true, string.Empty);
        }
        public async Task<ResponseDTO<bool>> CreateCancelInside(RequestCreateCancelInsideDTO DTO,Guid CreatorID)
        {
            if (DTO.RequestName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên yêu cầu không được để trống", (int)HttpStatusCode.Conflict);
            }
            if (DTO.RequestCategoryId != RequestCategoryConst.CATEGORY_CANCEL_INSIDE)
            {
                return new ResponseDTO<bool>(false, "Không phải yêu cầu hủy trong kho", (int)HttpStatusCode.Conflict);
            }
            try
            {
                Issue? issue = await daoIssue.getIssue(DTO.IssueId);
                // if not found issue
                if (issue == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                // if issue done
                if (issue.Status.Equals(IssueConst.STATUS_DONE))
                {
                    return new ResponseDTO<bool>(false, "Sự vụ với mã " + issue.IssueCode + " đã được chấp thuận", (int)HttpStatusCode.NotAcceptable);
                }
                // ----------------------- check cable valid --------------------------
                ResponseDTO<bool> check = await isCableValidCreateCancelInside(DTO.CableCancelInsideDTOs);
                // if exist cable invalid
                if(check.Success == false)
                {
                    return check;
                }
                // ----------------------- check material valid --------------------------
                check = await isMaterialValidCreateExportDeliverCancelInside(DTO.OtherMaterialsCancelInsideDTOs);
                // if exist material invalid
                if (check.Success == false)
                {
                    return check;
                }
                //----------------------------- create request --------------------------------
                Guid RequestID = CreateRequest(DTO.RequestName.Trim(), DTO.Content, DTO.IssueId, CreatorID, DTO.RequestCategoryId, null);
                //----------------------------- create request cable --------------------------------
                if(DTO.CableCancelInsideDTOs.Count > 0)
                {
                    foreach(CableCancelInsideDTO item in DTO.CableCancelInsideDTOs)
                    {
                        Cable? cable = await daoCable.getCable(item.CableId);
                        // if cable valid
                        if(cable != null && cable.IsExportedToUse == false)
                        {
                            RequestCable requestCable = new RequestCable()
                            {
                                RequestId = RequestID,
                                CableId = item.CableId,
                                StartPoint = cable.StartPoint,
                                EndPoint = cable.EndPoint,
                                Length = cable.Length,
                                CreatedAt = DateTime.Now,
                                UpdateAt = DateTime.Now,
                                IsDeleted = false,
                                RecoveryDestWarehouseId = item.WarehouseId
                            };
                            daoRequestCable.CreateRequestCable(requestCable);
                        }
                                  
                    }
                }
                //----------------------------- create request material --------------------------------
                if(DTO.OtherMaterialsCancelInsideDTOs.Count > 0)
                {
                    foreach(OtherMaterialsExportDeliverCancelInsideDTO item in DTO.OtherMaterialsCancelInsideDTOs)
                    {
                        RequestOtherMaterial requestMaterial = new RequestOtherMaterial()
                        {
                            RequestId = RequestID,
                            OtherMaterialsId = item.OtherMaterialsId,
                            Quantity = item.Quantity,
                            CreatedAt = DateTime.Now,
                            UpdateAt = DateTime.Now,
                            IsDeleted = false,
                            RecoveryDestWarehouseId = item.WarehouseId
                        };
                        daoRequestMaterial.CreateRequestOtherMaterial(requestMaterial);
                    }
                }
                await sendEmailToAdmin(DTO.RequestName.Trim(), "Hủy trong kho", issue);
                return new ResponseDTO<bool>(true, "Tạo yêu cầu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        // create cable while creating request cancel outside
        private async Task<List<Cable>> CreateCable(List<CableCancelOutsideDTO> list, Guid CreatorID)
        {
            List<Cable> result = new List<Cable>();
            if(list.Count > 0)
            {
                foreach(CableCancelOutsideDTO DTO in list)
                {
                    Cable cable = new Cable()
                    {
                        CableId = Guid.NewGuid(),
                        SupplierId = DTO.SupplierId,
                        StartPoint = DTO.StartPoint,
                        EndPoint = DTO.EndPoint,
                        Length = DTO.EndPoint - DTO.StartPoint,
                        YearOfManufacture = DTO.YearOfManufacture,
                        Code = DTO.Code,
                        Status = DTO.Status,
                        CreatorId = CreatorID,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsExportedToUse = false,
                        IsDeleted = false,
                        CableCategoryId = DTO.CableCategoryId,
                        IsInRequest = true
                    };
                    await daoCable.CreateCable(cable);
                    result.Add(cable);
                }
            }
            return result;
        }
        // create material while creating request cancel outside
        private async Task<Dictionary<int, int>> CreateMaterial(List<OtherMaterialsCancelOutsideDTO> list)
        {
            Dictionary<int,int> result = new Dictionary<int, int>();
            if(list.Count > 0)
            {
                foreach(OtherMaterialsCancelOutsideDTO DTO in list)
                {
                    OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(DTO);
                    if(material == null)
                    {
                        material = new OtherMaterial()
                        {
                            Unit = DTO.Unit.Trim(),
                            OtherMaterialsCategoryId = DTO.OtherMaterialsCategoryId,
                            Status = DTO.Status,
                            SupplierId = DTO.SupplierId,
                            IsDeleted = true
                        };
                        int MaterialID = await daoOtherMaterial.CreateMaterial(material);
                        result[MaterialID] = DTO.Quantity;
                    }
                    else
                    {
                        result[material.OtherMaterialsId] = DTO.Quantity;
                    }
                }
            }
            return result;
        }
        public async Task<ResponseDTO<bool>> CreateCancelOutside(RequestCreateCancelOutsideDTO DTO, Guid CreatorID)
        {
            if (DTO.RequestName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên yêu cầu không được để trống", (int)HttpStatusCode.Conflict);
            }
            if (DTO.RequestCategoryId != RequestCategoryConst.CATEGORY_CANCEL_OUTSIDE)
            {
                return new ResponseDTO<bool>(false, "Không phải yêu cầu hủy ngoài kho", (int)HttpStatusCode.Conflict);
            }
            try
            {
                Issue? issue = await daoIssue.getIssue(DTO.IssueId);
                // if not found issue
                if (issue == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                // if issue done
                if (issue.Status.Equals(IssueConst.STATUS_DONE))
                {
                    return new ResponseDTO<bool>(false, "Sự vụ với mã " + issue.IssueCode + " đã được chấp thuận", (int)HttpStatusCode.NotAcceptable);
                }
                // get list after create cable
                List<Cable> listCable = await CreateCable(DTO.CableCancelOutsideDTOs, CreatorID);
                // get list after create material
                // data store material id, quantity
                Dictionary<int, int> dic = await CreateMaterial(DTO.OtherMaterialsCancelOutsideDTOs);
                // --------------------------- create request ------------------------------------
                Guid RequestID = CreateRequest(DTO.RequestName.Trim(), DTO.Content,DTO.IssueId, CreatorID, DTO.RequestCategoryId, null);
                // --------------------------- create request cable ------------------------------------
                if(listCable.Count > 0)
                {
                    foreach(Cable item in listCable)
                    {
                        RequestCable requestCable = new RequestCable()
                        {
                            RequestId = RequestID,
                            CableId = item.CableId,
                            StartPoint = item.StartPoint,
                            EndPoint = item.EndPoint,
                            Length = item.Length,
                            CreatedAt = DateTime.Now,
                            UpdateAt = DateTime.Now,
                            IsDeleted = false
                        };
                        daoRequestCable.CreateRequestCable(requestCable);
                    }
                }
                // --------------------------- create request material ------------------------------------
                if(dic.Count > 0)
                {
                    foreach(int MaterialID in dic.Keys)
                    {
                        RequestOtherMaterial requestMaterial = new RequestOtherMaterial()
                        {
                            RequestId = RequestID,
                            OtherMaterialsId = MaterialID,
                            Quantity = dic[MaterialID],
                            CreatedAt = DateTime.Now,
                            UpdateAt = DateTime.Now,
                            IsDeleted = false
                        };
                        daoRequestMaterial.CreateRequestOtherMaterial(requestMaterial);
                    }
                }
                await sendEmailToAdmin(DTO.RequestName.Trim(), "Hủy ngoài kho", issue);
                return new ResponseDTO<bool>(true, "Tạo yêu cầu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        // check cable valid when approve request cancel outside
        private async Task<ResponseDTO<bool>> isCableValidApproveCancelOutside(List<RequestCable> list)
        {
            if(list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach(RequestCable item in list)
            {
                Cable? cable = await daoCable.getCable(item.CableId);
                if(cable == null || cable.IsInRequest == false)
                {
                    return new ResponseDTO<bool>(false, item.Cable.CableCategory.CableCategoryName + " với ID: " + item.CableId
                                + " không tồn tại ", (int) HttpStatusCode.NotFound);
                }
            }
            return new ResponseDTO<bool>(true, string.Empty);
        }
        public async Task<ResponseDTO<bool>> ApproveRequestCancelOutside(Guid ApproverID, DataAccess.Entity.Request request, string ApproverName)
        {
             List<RequestCable> requestCables = await daoRequestCable.getList(request.RequestId);
             ResponseDTO<bool> check = await isCableValidApproveCancelOutside(requestCables);
             if(check.Success == false)
             {
                return check;
             }
             if(requestCables.Count > 0)
             {
                foreach(RequestCable item in requestCables)
                {
                    Cable? cable = await daoCable.getCable(item.CableId);
                    if(cable != null && cable.IsInRequest == true)
                    {
                        cable.IsInRequest = false;
                        cable.IsDeleted = true;
                        cable.UpdateAt = DateTime.Now;
                        await daoCable.UpdateCable(cable);
                    }
                }
             }
             // -------------------------------update request approved --------------------------------
             UpdateRequest(request, ApproverID, RequestConst.STATUS_APPROVED);
             // ------------------------------- send email --------------------------------
             await sendEmailToCreator(request, ApproverName);
             return new ResponseDTO<bool>(true, "Yêu cầu được phê duyệt");
        }
        public async Task<ResponseDTO<bool>> Delete(Guid RequestID)
        {
            try
            {
                DataAccess.Entity.Request? request = await daoRequest.getRequest(RequestID);
                if (request == null || !request.Status.Equals(RequestConst.STATUS_PENDING))
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy yêu cầu", (int)HttpStatusCode.NotFound);
                }
                daoRequest.DeleteRequest(request);
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            catch(Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<RequestDetailDTO?>> Detail(Guid RequestID)
        {
            try
            {
                DataAccess.Entity.Request? request = await daoRequest.getRequest(RequestID);
                if (request == null)
                {
                    return new ResponseDTO<RequestDetailDTO?>(null, "Không tìm thấy yêu cầu", (int)HttpStatusCode.NotFound);
                }
                List<RequestCable> requestCables = await daoRequestCable.getList(RequestID);
                List<RequestCableDTO> RequestCableDTOs = new List<RequestCableDTO>();
                foreach(RequestCable item in requestCables)
                {
                    RequestCableDTO DTO = new RequestCableDTO()
                    {
                        CableCategoryName = item.Cable.CableCategory.CableCategoryName,
                        StartPoint = item.StartPoint,
                        EndPoint = item.EndPoint,
                        Length = item.Length,
                        RecoveryDestWarehouseName = item.RecoveryDestWarehouse == null ? null : item.RecoveryDestWarehouse.WarehouseName
                    };
                    RequestCableDTOs.Add(DTO);
                }
                List<RequestOtherMaterial> requestMaterials = await daoRequestMaterial.getList(RequestID);
                List<RequestOtherMaterialsDTO> RequestOtherMaterialsDTOs = new List<RequestOtherMaterialsDTO>();
                foreach(RequestOtherMaterial item in requestMaterials)
                {
                    RequestOtherMaterialsDTO DTO = new RequestOtherMaterialsDTO()
                    {
                        OtherMaterialsCategoryName = item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName,
                        Quantity = item.Quantity,
                        RecoveryDestWarehouseName = item.RecoveryDestWarehouse == null ? null : item.RecoveryDestWarehouse.WarehouseName
                    };
                    RequestOtherMaterialsDTOs.Add(DTO);
                }
                RequestDetailDTO detail = new RequestDetailDTO()
                {
                    RequestId = request.RequestId,
                    RequestName = request.RequestName,
                    Content = request.Content,
                    CreatorName = request.Creator.Lastname + " " + request.Creator.Firstname,
                    ApproverName = request.Approver == null ? null : request.Approver.Lastname + " " + request.Approver.Firstname,
                    Status = request.Status,
                    RequestCategoryName = request.RequestCategory.RequestCategoryName,
                    IssueName = request.Issue == null ? null : request.Issue.IssueName,
                    CableRoutingName = request.Issue == null ? null : request.Issue.CableRoutingName,
                    DeliverWarehouseName = request.DeliverWarehouse == null ? null : request.DeliverWarehouse.WarehouseName,
                    RequestCableDTOs = RequestCableDTOs,
                    RequestOtherMaterialsDTOs = RequestOtherMaterialsDTOs
                };
                return new ResponseDTO<RequestDetailDTO?>(detail, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<RequestDetailDTO?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
