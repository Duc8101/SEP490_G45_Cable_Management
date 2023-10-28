using DataAccess.DTO.RequestDTO;
using DataAccess.DTO;
using DataAccess.Entity;
using DataAccess.Const;
using System.Net;
using DataAccess.DTO.CableDTO;
using API.Model.DAO;
using DataAccess.DTO.OtherMaterialsDTO;
using API.Model;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using API.Services.IService;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

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
        // check material valid when create request export, deliver
        private async Task<ResponseDTO<bool>> isMaterialValidCreateExportDeliver(List<OtherMaterialsExportDeliverCancelInsideDTO> list)
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
                response = await isMaterialValidCreateExportDeliver(DTO.OtherMaterialsExportDTOs);
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
        private List<Cable> getListCableCut(int itemStartPoint, int itemEndPoint, Cable cable, int? DeliverWareHouseID)
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
                    WarehouseId = DeliverWareHouseID,
                    StartPoint = itemStartPoint,
                    EndPoint = itemEndPoint,
                    Length = itemEndPoint - itemStartPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = false,
                    IsExportedToUse = DeliverWareHouseID == null,
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
                    WarehouseId = DeliverWareHouseID,
                    StartPoint = itemStartPoint,
                    EndPoint = itemEndPoint,
                    Length = itemEndPoint - itemStartPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = false,
                    IsExportedToUse = DeliverWareHouseID == null,
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
                    WarehouseId = DeliverWareHouseID,
                    StartPoint = itemStartPoint,
                    EndPoint = itemEndPoint,
                    Length = itemEndPoint - itemStartPoint,
                    CreatorId = cable.CreatorId,
                    CableParentId = cable.CableId,
                    IsDeleted = false,
                    IsExportedToUse = DeliverWareHouseID == null,
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
        // check cable valid when approve request export
        private async Task<ResponseDTO<bool>> isCableValidApproveExportDeliver(List<RequestCable> list)
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
                    return new ResponseDTO<bool>(false, "Không thể xuất kho " + item.Cable.CableCategory.CableCategoryName + " với ID: " + item.CableId
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
        // check material valid when approve request export
        private ResponseDTO<bool> isMaterialValidApproveExportDeliver(List<RequestOtherMaterial> list)
        {
            if (list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach (RequestOtherMaterial item in list)
            {
                // if not enough quantity
                if (item.OtherMaterials.Quantity < item.Quantity)
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
        private async Task sendEmailToCreator(DataAccess.Entity.Request request, string ApproverName)
        {
            string body = await UserUtil.BodyEmailForApproveRequest(request, ApproverName);
            await UserUtil.sendEmail("Thông báo yêu cầu được phê duyệt", body, request.Creator.Email);
        }
        private async Task<ResponseDTO<bool>> ApproveRequestExport(Guid ApproverID, DataAccess.Entity.Request request, string ApproverName)
        {
            List<RequestCable> requestCables = await daoRequestCable.getList(request.RequestId);
            List<RequestOtherMaterial> requestMaterials = await daoRequestMaterial.getList(request.RequestId);
            // ------------------------------- check cable valid ------------------------------  
            ResponseDTO<bool> check = await isCableValidApproveExportDeliver(requestCables);
            // if exist cable not valid
            if (check.Success == false)
            {
                return check;
            }
            // ------------------------------- check material valid ------------------------------
            check = isMaterialValidApproveExportDeliver(requestMaterials);
            // if exist material not valid
            if (check.Success == false)
            {
                return check;
            }
            // ------------------------------- request cable ------------------------------
            List<Cable> listCableExported = new List<Cable>();
            List<int?> listWareHouseID = new List<int?>();
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
                                // update cable
                                update.IsExportedToUse = true;
                                update.WarehouseId = null;
                                update.UpdateAt = DateTime.Now;
                                daoCable.UpdateCable(update);
                                // update request cable
                                daoRequestCable.RemoveRequestCable(item);
                                RequestCable create = new RequestCable()
                                {
                                    RequestId = item.RequestId,
                                    CableId = update.CableId,
                                    StartPoint = update.StartPoint,
                                    EndPoint = update.EndPoint,
                                    Length = update.Length,
                                    CreatedAt = update.CreatedAt,
                                    UpdateAt = DateTime.Now,
                                    IsDeleted = false
                                };
                                daoRequestCable.CreateRequestCable(create);
                            }
                            listCableExported.Add(cable);
                            listWareHouseID.Add(cable.WarehouseId);
                        }
                        else
                        {
                            List<Cable> listCut = getListCableCut(item.StartPoint, item.EndPoint, cable,null);
                            foreach (Cable cut in listCut)
                            {
                                // if exported
                                if (cut.IsExportedToUse)
                                {
                                    listCableExported.Add(cut);
                                    listWareHouseID.Add(cable.WarehouseId);
                                    daoCable.CreateCable(cut);
                                    // update request cable
                                    daoRequestCable.RemoveRequestCable(item);
                                    RequestCable create = new RequestCable()
                                    {
                                        RequestId = item.RequestId,
                                        CableId = cut.CableId,
                                        StartPoint = cut.StartPoint,
                                        EndPoint = cut.EndPoint,
                                        Length = cut.Length,
                                        CreatedAt = cut.CreatedAt,
                                        UpdateAt = DateTime.Now,
                                        IsDeleted = false
                                    };
                                    daoRequestCable.CreateRequestCable(create);
                                }
                                else
                                {
                                    daoCable.CreateCable(cut);
                                }
                            }
                            // delete cable parent
                            Cable? cableParent = await daoCable.getCable(cable.CableId);
                            if(cableParent != null)
                            {
                               daoCable.DeleteCable(cableParent);
                            }
                             
                        }
                    }
                }
            }
            // ------------------------------- request material ------------------------------
            if (requestMaterials.Count > 0)
            {
                foreach (RequestOtherMaterial item in requestMaterials)
                {
                    OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(item.OtherMaterialsId);
                    // if material valid
                    if(material != null && material.Quantity >= item.Quantity)
                    {
                        // update material quantity
                        material.Quantity = material.Quantity - item.Quantity;
                        material.UpdateAt = DateTime.Now;
                        daoOtherMaterial.UpdateMaterial(material);
                    }
                }
            }
            // ------------------------------- update request approved --------------------------------
            UpdateRequest(request, ApproverID, RequestConst.STATUS_APPROVED);
            // ------------------------------- add transaction cable --------------------------------
            if (listCableExported.Count > 0)
            {
                for (int i = 0; i < listCableExported.Count; i++)
                {
                    TransactionHistory history = new TransactionHistory()
                    {
                        TransactionId = Guid.NewGuid(),
                        TransactionCategoryName = TransactionCategoryConst.CATEGORY_EXPORT,
                        CreatedDate = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        WarehouseId = listWareHouseID[i],
                        RequestId = request.RequestId,
                        IssueId = request.IssueId,
                    };
                    daoHistory.CreateTransactionHistory(history);
                    TransactionCable transactionCable = new TransactionCable()
                    {
                        TransactionId = history.TransactionId,
                        CableId = listCableExported[i].CableId,
                        StartPoint = listCableExported[i].StartPoint,
                        EndPoint = listCableExported[i].EndPoint,
                        Length = listCableExported[i].Length,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false
                    };
                    daoTransactionCable.CreateTransactionCable(transactionCable);
                }
            }
            // ------------------------------- add transaction material --------------------------------
            if (requestMaterials.Count > 0)
            {
                foreach (RequestOtherMaterial item in requestMaterials)
                {
                    TransactionHistory history = new TransactionHistory()
                    {
                        TransactionId = Guid.NewGuid(),
                        TransactionCategoryName = TransactionCategoryConst.CATEGORY_EXPORT,
                        CreatedDate = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        WarehouseId = item.OtherMaterials.WarehouseId,
                        RequestId = request.RequestId,
                        IssueId = request.IssueId,
                    };
                    daoHistory.CreateTransactionHistory(history);
                    TransactionOtherMaterial material = new TransactionOtherMaterial()
                    {
                        TransactionId = history.TransactionId,
                        OtherMaterialsId = item.OtherMaterialsId,
                        Quantity = item.Quantity,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                    };
                    daoTransactionMaterial.CreateTransactionMaterial(material);
                }
            }
            // ------------------------------- send email --------------------------------
            await sendEmailToCreator(request, ApproverName);
            return new ResponseDTO<bool>(true, "Yêu cầu được phê duyệt");
        }
        // check cable valid when create request recovery
        private ResponseDTO<bool> isCableValidCreateRecovery(List<CableCreateUpdateDTO> list)
        {
            if(list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach(CableCreateUpdateDTO DTO in list)
            {
                if (DTO.StartPoint < 0 || DTO.EndPoint < 0 || DTO.StartPoint >= DTO.EndPoint)
                {
                    return new ResponseDTO<bool>(false, "Chỉ số đầu hoặc chỉ số cuối không hợp lệ", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseDTO<bool>(true, string.Empty);
        }
        // check material valid when create request recovery
        private ResponseDTO<bool> isMaterialValidCreateRecovery(List<OtherMaterialsRecoveryDTO> list)
        {
            if (list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach(OtherMaterialsRecoveryDTO DTO in list)
            {
                if(DTO.Quantity <= 0)
                {
                    return new ResponseDTO<bool>(false, "Số lượng phải lớn hơn 0", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseDTO<bool>(true, string.Empty);
        }
        private List<Cable> CreateCable(List<CableCreateUpdateDTO> list, Guid CreatorID)
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
                    daoCable.CreateCable(cable);
                    result.Add(cable);
                }
            }
            return result;
        }
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
                        Code = DTO.Code.Trim(),
                        SupplierId = DTO.SupplierId,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        WarehouseId = DTO.WarehouseId,
                        Status = DTO.Status.Trim(),
                        OtherMaterialsCategoryId = DTO.OtherMaterialsCategoryId
                    };
                    daoOtherMaterial.CreateMaterial(material);
                    // get material just create
                    OtherMaterial? create = await daoOtherMaterial.getOtherMaterialCreate();
                    if(create != null)
                    {
                        listWareHouse.Add(DTO.WarehouseId);
                        listQuantity.Add(DTO.Quantity);
                        listID.Add(create.OtherMaterialsId);
                        listStatus.Add(DTO.Status);
                    }
                }
                // ----------------------- update quantity -----------------
                foreach (int MaterialID in listID)
                {
                    OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(MaterialID);
                    if (material != null)
                    {
                        material.Quantity = 0;
                        daoOtherMaterial.UpdateMaterial(material);
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
                ResponseDTO<bool> check = isCableValidCreateRecovery(DTO.CableRecoveryDTOs);
                // if exist cable invalid
                if(check.Success == false)
                {
                    return check;
                }
                check = isMaterialValidCreateRecovery(DTO.OtherMaterialsRecoveryDTOs);
                // if exist material invalid
                if (check.Success == false)
                {
                    return check;
                }
                // create cable
                List<Cable> listCable = CreateCable(DTO.CableRecoveryDTOs, CreatorID);
                // create material
                // result to store list warehouse, list material id, list quantity
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
            List<int?> listWareHouseCable = new List<int?>();
            if(requestCables.Count > 0)
            {
                foreach(RequestCable item in  requestCables)
                {
                    Cable? cable = await daoCable.getCable(item.CableId);
                    if(cable != null)
                    {
                        listCableRecovery.Add(cable);
                        listWareHouseCable.Add(item.RecoveryDestWarehouseId);
                        // update cable
                        cable.IsInRequest = false;
                        cable.UpdateAt = DateTime.Now;
                        daoCable.UpdateCable(cable);
                    }
                }
            }
            // ------------------------------- request material ------------------------------
            List<OtherMaterial> listMaterialRecovery = new List<OtherMaterial>();
            List<int?> listWareHouseMaterial = new List<int?>();
            List<int> listQuantity = new List<int>();
            if(requestMaterials.Count > 0)
            {
                foreach(RequestOtherMaterial item in requestMaterials)
                {
                    OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(item.OtherMaterialsId);
                    if(material != null)
                    {
                        listMaterialRecovery.Add(material);
                        listWareHouseMaterial.Add(item.RecoveryDestWarehouseId);
                        listQuantity.Add(item.Quantity);
                        // update material
                        material.Quantity = material.Quantity + item.Quantity;
                        material.UpdateAt = DateTime.Now;
                        daoOtherMaterial.UpdateMaterial(material);
                    }
                }
            }
            // ------------------------------- update request approved --------------------------------
            UpdateRequest(request, ApproverID, RequestConst.STATUS_APPROVED);
            // ------------------------------- create transaction cable ------------------------------
            if (listCableRecovery.Count > 0)
            {
                for(int i = 0; i < listCableRecovery.Count; i ++)
                {
                    TransactionHistory history = new TransactionHistory()
                    {
                        TransactionId = Guid.NewGuid(),
                        TransactionCategoryName = TransactionCategoryConst.CATEGORY_IMPORT,
                        CreatedDate = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        WarehouseId = listWareHouseCable[i],
                        RequestId = request.RequestId,
                        IssueId = request.IssueId,
                    };
                    daoHistory.CreateTransactionHistory(history);
                    TransactionCable transaction = new TransactionCable()
                    {
                        TransactionId = history.TransactionId,
                        CableId = listCableRecovery[i].CableId,
                        StartPoint = listCableRecovery[i].StartPoint,
                        EndPoint = listCableRecovery[i].EndPoint,
                        Length = listCableRecovery[i].EndPoint - listCableRecovery[i].StartPoint,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false
                    };
                    daoTransactionCable.CreateTransactionCable(transaction);
                }
            }
            // ------------------------------- create transaction material ------------------------------
            if(listMaterialRecovery.Count > 0)
            {
                for(int i = 0; i < listMaterialRecovery.Count; i ++)
                {
                    TransactionHistory history = new TransactionHistory()
                    {
                        TransactionId = Guid.NewGuid(),
                        TransactionCategoryName = TransactionCategoryConst.CATEGORY_IMPORT,
                        CreatedDate = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        WarehouseId = listWareHouseMaterial[i],
                        RequestId = request.RequestId,
                        IssueId = request.IssueId,
                    };
                    daoHistory.CreateTransactionHistory(history);
                    TransactionOtherMaterial transaction = new TransactionOtherMaterial()
                    {
                        TransactionId = history.TransactionId,
                        OtherMaterialsId = listMaterialRecovery[i].OtherMaterialsId,
                        Quantity = listQuantity[i],
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false
                    };
                    daoTransactionMaterial.CreateTransactionMaterial(transaction);
                }
            }
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
                    return new ResponseDTO<bool>(false, "Yêu cầu đã được xác nhận chấp thuận hoặc bị hủy", (int)HttpStatusCode.Conflict);
                }
                if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_EXPORT)
                {
                    return await ApproveRequestExport(ApproverID, request, ApproverName);
                }
                if(request.RequestCategoryId == RequestCategoryConst.CATEGORY_RECOVERY)
                {
                    return await ApproveRequestRecovery(ApproverID, request, ApproverName);
                }
                if(request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER)
                {
                    return await ApproveRequestDeliver(ApproverID, request, ApproverName);
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
                    return new ResponseDTO<bool>(false, "Yêu cầu đã được xác nhận chấp thuận hoặc bị hủy", (int)HttpStatusCode.Conflict);
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
                response = await isMaterialValidCreateExportDeliver(DTO.OtherMaterialsDeliverDTOs);
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
        private async Task<ResponseDTO<bool>> ApproveRequestDeliver(Guid ApproverID, DataAccess.Entity.Request request, string ApproverName)
        {
            List<RequestCable> requestCables = await daoRequestCable.getList(request.RequestId);
            List<RequestOtherMaterial> requestMaterials = await daoRequestMaterial.getList(request.RequestId);
            // ------------------------------- check cable valid ------------------------------  
            ResponseDTO<bool> check = await isCableValidApproveExportDeliver(requestCables);
            // if exist cable not valid
            if (check.Success == false)
            {
                return check;
            }
            // ------------------------------- check material valid ------------------------------
            check = isMaterialValidApproveExportDeliver(requestMaterials);
            // if exist material not valid
            if (check.Success == false)
            {
                return check;
            }
            // --------------------------- create transaction cable ------------------------------------------
            if (requestCables.Count > 0)
            {
                foreach(RequestCable item in requestCables)
                {
                    TransactionHistory history = new TransactionHistory()
                    {
                        TransactionId = Guid.NewGuid(),
                        TransactionCategoryName = TransactionCategoryConst.CATEGORY_EXPORT,
                        CreatedDate = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        RequestId = request.RequestId,
                        WarehouseId = item.Cable.WarehouseId,
                        ToWarehouseId = request.DeliverWarehouseId
                    };
                    daoHistory.CreateTransactionHistory(history);
                    TransactionCable transaction = new TransactionCable()
                    {
                        TransactionId = history.TransactionId,
                        CableId = item.Cable.CableId,
                        StartPoint = item.Cable.StartPoint,
                        EndPoint = item.Cable.EndPoint,
                        Length = item.Cable.Length,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                    };
                    daoTransactionCable.CreateTransactionCable(transaction);
                }
            }
            // --------------------------- create transaction material -----------------------------------------
            if(requestMaterials.Count > 0)
            {
                foreach(RequestOtherMaterial item in requestMaterials)
                {
                    TransactionHistory history = new TransactionHistory()
                    {
                        TransactionId = Guid.NewGuid(),
                        TransactionCategoryName = TransactionCategoryConst.CATEGORY_EXPORT,
                        CreatedDate = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        RequestId = request.RequestId,
                        WarehouseId = item.OtherMaterials.WarehouseId,
                        ToWarehouseId = request.DeliverWarehouseId
                    };
                    daoHistory.CreateTransactionHistory(history);
                    TransactionOtherMaterial transaction = new TransactionOtherMaterial()
                    {
                        TransactionId = history.TransactionId,
                        OtherMaterialsId = item.OtherMaterialsId,
                        Quantity = item.Quantity,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                    };
                    daoTransactionMaterial.CreateTransactionMaterial(transaction);
                }
            }
            // --------------------------- get list cable deliver and update cable ----------------------------------------------
            List<Cable> listCableDeliver = new List<Cable>();
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
                    if (cable != null && cable.IsExportedToUse == false && item.StartPoint >= cable.StartPoint && item.EndPoint <= cable.EndPoint)
                    {
                        if (item.StartPoint == cable.StartPoint && item.EndPoint == cable.EndPoint)
                        {
                            Cable? update = await daoCable.getCable(cable.CableId);
                            if (update != null)
                            {
                                update.WarehouseId = request.DeliverWarehouseId;
                                update.UpdateAt = DateTime.Now;
                                daoCable.UpdateCable(update);
                            }
                            listCableDeliver.Add(cable);
                        }
                        else
                        {
                            List<Cable> listCut = getListCableCut(item.StartPoint, item.EndPoint, cable, request.DeliverWarehouseId);
                            foreach (Cable cut in listCut)
                            {
                                // if cable deliver
                                if (cut.WarehouseId == request.DeliverWarehouseId)
                                {
                                    listCableDeliver.Add(cut);
                                    daoCable.CreateCable(cut);
                                    // update request cable
                                    daoRequestCable.RemoveRequestCable(item);
                                    RequestCable create = new RequestCable()
                                    {
                                        RequestId = item.RequestId,
                                        CableId = cut.CableId,
                                        StartPoint = cut.StartPoint,
                                        EndPoint = cut.EndPoint,
                                        Length = cut.Length,
                                        CreatedAt = cut.CreatedAt,
                                        UpdateAt = DateTime.Now,
                                        IsDeleted = false
                                    };
                                    daoRequestCable.CreateRequestCable(create);
                                }
                                else
                                {
                                    daoCable.CreateCable(cut);
                                }
                            }
                            // delete cable parent
                            Cable? cableParent = await daoCable.getCable(cable.CableId);
                            if (cableParent != null)
                            {
                                daoCable.DeleteCable(cableParent);
                            }

                        }
                    }
                }
            }
            // ------------------------------- update quantity and create or update material in deliver warehouse ------------------------------
            List<int> listMaterialID = new List<int>();
            List<int> listQuantity = new List<int>();
            List<int?> listWarehouseID = new List<int?>();
            if (requestMaterials.Count > 0)
            {
                foreach (RequestOtherMaterial item in requestMaterials)
                {
                    OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(item.OtherMaterialsId);
                    // if material valid
                    if (material != null && material.Quantity >= item.Quantity)
                    {
                        // update material quantity
                        material.Quantity = material.Quantity - item.Quantity;
                        material.UpdateAt = DateTime.Now;
                        daoOtherMaterial.UpdateMaterial(material);
                        OtherMaterial? exist = await daoOtherMaterial.getOtherMaterial(request.DeliverWarehouseId, material.Code, material.Status, material.Unit);
                        // if material not exist in deliver warehouse
                        if(exist == null)
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
                            daoOtherMaterial.CreateMaterial(exist);
                            // get material create
                            OtherMaterial? create = await daoOtherMaterial.getOtherMaterialCreate();
                            if(create != null)
                            {
                                listMaterialID.Add(create.OtherMaterialsId);
                                listQuantity.Add(item.Quantity);
                                listWarehouseID.Add(request.DeliverWarehouseId);
                                // update request material
                                daoRequestMaterial.RemoveRequestMaterial(item);
                                RequestOtherMaterial requestMaterial = new RequestOtherMaterial()
                                {
                                    RequestId = item.RequestId,
                                    OtherMaterialsId = create.OtherMaterialsId,
                                    Quantity = item.Quantity,
                                    CreatedAt = item.CreatedAt,
                                    UpdateAt = item.UpdateAt,
                                    IsDeleted = false
                                };
                                daoRequestMaterial.CreateRequestOtherMaterial(requestMaterial);
                            }
                        }
                        else
                        {
                            listMaterialID.Add(item.OtherMaterialsId);
                            listQuantity.Add(item.Quantity);
                            listWarehouseID.Add(material.WarehouseId);
                            // update material
                            exist.Quantity = exist.Quantity + item.Quantity;
                            exist.UpdateAt = DateTime.Now;
                            daoOtherMaterial.UpdateMaterial(exist);
                        }
                    }
                }
            }
            // ------------------------------- update request approved --------------------------------
            UpdateRequest(request, ApproverID, RequestConst.STATUS_APPROVED);          
            // --------------------------- create transaction cable ------------------------------------------
            if (listCableDeliver.Count > 0)
            {
                foreach (Cable item in listCableDeliver)
                {
                    TransactionHistory history = new TransactionHistory()
                    {
                        TransactionId = Guid.NewGuid(),
                        TransactionCategoryName = TransactionCategoryConst.CATEGORY_IMPORT,
                        CreatedDate = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        RequestId = request.RequestId,
                        WarehouseId = request.DeliverWarehouseId,
                        FromWarehouseId = request.DeliverWarehouseId
                    };
                    daoHistory.CreateTransactionHistory(history);
                    TransactionCable transaction = new TransactionCable()
                    {
                        TransactionId = history.TransactionId,
                        CableId = item.CableId,
                        StartPoint = item.StartPoint,
                        EndPoint = item.EndPoint,
                        Length = item.Length,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                    };
                    daoTransactionCable.CreateTransactionCable(transaction);
                }
            }
            // --------------------------- create transaction material -----------------------------------------
            if (listMaterialID.Count > 0)
            {
                for (int i = 0; i < listMaterialID.Count; i++)
                {
                    TransactionHistory history = new TransactionHistory()
                    {
                        TransactionId = Guid.NewGuid(),
                        TransactionCategoryName = TransactionCategoryConst.CATEGORY_IMPORT,
                        CreatedDate = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        RequestId = request.RequestId,
                        WarehouseId = listWarehouseID[i],
                        FromWarehouseId = listWarehouseID[i]
                    };
                    daoHistory.CreateTransactionHistory(history);
                    TransactionOtherMaterial transaction = new TransactionOtherMaterial()
                    {
                        TransactionId = history.TransactionId,
                        OtherMaterialsId = listMaterialID[i],
                        Quantity = listQuantity[i],
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                    };
                    daoTransactionMaterial.CreateTransactionMaterial(transaction);
                }
            }
            // ------------------------------- send email ------------------------------
            await sendEmailToCreator(request, ApproverName);
            return new ResponseDTO<bool>(true, "Yêu cầu được phê duyệt");
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
    }
}
