using DataAccess.DTO.RequestDTO;
using DataAccess.DTO;
using DataAccess.Entity;
using DataAccess.Const;
using System.Net;
using DataAccess.DTO.CableDTO;
using API.Model.DAO;
using DataAccess.DTO.OtherMaterialsDTO;
using Org.BouncyCastle.Asn1.Cmp;

namespace API.Services
{
    public class RequestService
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
        private async Task<List<RequestListDTO>> getList(string? name, string? status, Guid? CreatorID, int page)
        {
            List<Request> list = await daoRequest.getList(name, status, CreatorID, page);
            List<RequestListDTO> result = new List<RequestListDTO>();
            foreach (Request item in list)
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
            catch(Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<RequestListDTO>?>(null, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }
        private async Task<ResponseDTO<bool>> isCableValid(List<CableExportDTO> list)
        {
            if(list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach(CableExportDTO export in list)
            {
                Cable? cable = await daoCable.getCable(export.CableId);
                // if exist cable not found
                if(cable == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy cáp với ID " + export.CableId, (int) HttpStatusCode.NotFound);
                }
                // if invalid start point, end point or deleted
                if(export.StartPoint < cable.StartPoint || export.EndPoint > cable.EndPoint || export.StartPoint > export.EndPoint || cable.IsDeleted)
                {
                    return new ResponseDTO<bool>(false, "Cáp với ID: " + export.CableId + " có chỉ số đầu hoặc chỉ số cuối không hợp lệ hoặc đã bị hủy", (int) HttpStatusCode.Conflict);
                }
                // if cable in use
                if (cable.IsExportedToUse)
                {
                    return new ResponseDTO<bool>(false, "Cáp với ID: " + export.CableId + " đã được sử dụng", (int) HttpStatusCode.Conflict);
                }
            }
            return new ResponseDTO<bool>(true, string.Empty);
        }
        private async Task<ResponseDTO<bool>> isMaterialValid(List<OtherMaterialsExportDTO> list)
        {
            if (list.Count == 0)
            {
                return new ResponseDTO<bool>(true, string.Empty);
            }
            foreach (OtherMaterialsExportDTO export in list)
            {
                OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(export.OtherMaterialsId);
                // if not found
                if(material == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy vật liệu vs ID = " + export.OtherMaterialsId, (int) HttpStatusCode.NotFound);
                }
                // if not enough quantity
                if(material.Quantity < export.Quantity)
                {
                    return new ResponseDTO<bool>(false, "Vật liệu vs ID = " + export.OtherMaterialsId + " không có đủ số lượng", (int) HttpStatusCode.Conflict);
                }
            }
            return new ResponseDTO<bool>(true, string.Empty);
        }
        private async Task CreateRequestCable(List<CableExportDTO> list, Guid RequestID)
        {
            if (list.Count > 0)
            {
                foreach (CableExportDTO item in list)
                {
                    RequestCable request = new RequestCable()
                    {
                        RequestId = RequestID,
                        CableId = item.CableId,
                        StartPoint = item.StartPoint,
                        EndPoint = item.EndPoint,
                        Length = item.EndPoint - item.StartPoint,
                       // RecoveryDestWarehouseId = item.WarehouseId,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                    };
                    await daoRequestCable.CreateRequestCable(request);
                }
            }
        }
        private async Task CreateRequestMaterial(List<OtherMaterialsExportDTO> list, Guid RequestID)
        {
            if (list.Count > 0)
            {
                foreach(OtherMaterialsExportDTO item in list)
                {
                    RequestOtherMaterial request = new RequestOtherMaterial()
                    {
                        RequestId = RequestID,
                        OtherMaterialsId = item.OtherMaterialsId,
                        Quantity = item.Quantity,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        RecoveryDestWarehouseId = item.WarehouseId,
                    };
                    await daoRequestMaterial.CreateRequestOtherMaterial(request);
                }
            }
        }
        public async Task<ResponseDTO<bool>> CreateRequestExport(RequestCreateExportDTO DTO, Guid CreatorID)
        {
            if(DTO.RequestName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên yêu cầu không được để trống", (int) HttpStatusCode.NotAcceptable);
            }
            if(DTO.RequestCategoryId != RequestCategoryConst.CATEGORY_EXPORT)
            {
                return new ResponseDTO<bool>(false, "Không phải yêu cầu xuất kho", (int) HttpStatusCode.NotAcceptable);
            }
            try
            {
                Issue? issue = await daoIssue.getIssue(DTO.IssueId);
                // if not found issue
                if (issue == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int) HttpStatusCode.NotFound);
                }
                // if issue approved
                if (issue.Status.Equals(IssueConst.STATUS_DONE))
                {
                    return new ResponseDTO<bool>(false, "Sự vụ với mã " + issue.IssueCode + " đã được chấp thuận", (int) HttpStatusCode.NotAcceptable);
                }
                ResponseDTO<bool> response = await isCableValid(DTO.CableExportDTOs);
                // if cable invalid
                if(response.Success == false)
                {
                    return response;
                }
                response = await isMaterialValid(DTO.OtherMaterialsExportDTOs);
                // if material invalid
                if (response.Success == false)
                {
                    return response;
                }
                Request request = new Request()
                {
                    RequestId = Guid.NewGuid(),
                    RequestName = DTO.RequestName.Trim(),
                    Content = DTO.Content == null || DTO.Content.Trim().Length == 0 ? null : DTO.Content.Trim(),
                    IssueId = DTO.IssueId,
                    CreatorId = CreatorID,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false,
                    RequestCategoryId = DTO.RequestCategoryId,
                    Status = RequestConst.STATUS_PENDING,
                };
                await daoRequest.CreateRequest(request);
                await CreateRequestCable(DTO.CableExportDTOs, request.RequestId);
                await CreateRequestMaterial(DTO.OtherMaterialsExportDTOs, request.RequestId);
                return new ResponseDTO<bool>(true, "Tạo yêu cầu thành công");
            }
            catch(Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }  
        }
        private List<Cable> getListCableCut(int itemStartPoint, int itemEndPoint, Cable cable)
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
                    IsDeleted = false,
                    IsExportedToUse = true,
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
            }else if (itemStartPoint > cable.StartPoint && itemEndPoint == cable.EndPoint)
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
                    IsDeleted = false,
                    IsExportedToUse = true,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    CableCategoryId = cable.CableCategoryId,
                    IsInRequest = false,
                    Description = cable.Description,
                };
                list.Add(cableCut1);
                list.Add(cableCut2);
            }else if (itemStartPoint > cable.StartPoint && itemEndPoint < cable.EndPoint)
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
                    IsDeleted = false,
                    IsExportedToUse = true,
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
        private async Task<ResponseDTO<bool>> ApproveRequestExport(Guid RequestID, Guid ApproverID, Request request, string email)
        {
            List<RequestCable> requestCables = await daoRequestCable.getList(RequestID);
            List<Cable> listCableExported = new List<Cable>();
            if (requestCables.Count > 0)
            {
                foreach (RequestCable item in requestCables)
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
                    // if not found
                    if (cable == null)
                    {
                        return new ResponseDTO<bool>(false, "Không thể xuất kho " + item.Cable.CableCategory.CableCategoryName + " với ID: " + item.CableId
                                + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ")", (int) HttpStatusCode.Conflict);
                    }
                    // if exported to use
                    if (cable.IsExportedToUse)
                    {
                        return new ResponseDTO<bool>(false, item.Cable.CableCategory.CableCategoryName + " với ID: " + item.CableId
                                + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ") đã được sử dụng!", (int) HttpStatusCode.Conflict);
                    }

                    if (cable.StartPoint == item.StartPoint && cable.EndPoint == item.EndPoint)
                    {
                        cable.IsExportedToUse = true;
                        listCableExported.Add(cable);
                        continue;
                    }
                    // ------------------------------ cut cable -----------------------------------
                    List<Cable> cableCuts = getListCableCut(item.StartPoint, item.EndPoint, cable);
                    if(cableCuts.Count == 0)
                    {
                        return new ResponseDTO<bool>(false, "Có lỗi trong việc lấy list các cable cut", (int) HttpStatusCode.Conflict);
                    }
                    foreach(Cable cut in cableCuts)
                    {
                        // add cable 
                        await daoCable.CreateCable(cut);
                        // if cable cut exported to use
                        if(cut.IsExportedToUse)
                        {
                            listCableExported.Add(cut);
                            // ----------------update request cable -----------------
                            await daoRequestCable.DeleteRequestCable(item);
                            RequestCable update = new RequestCable()
                            {
                                RequestId = RequestID,
                                CableId = cut.CableId,
                                StartPoint = item.StartPoint,
                                EndPoint = item.EndPoint,
                                Length = item.EndPoint - item.StartPoint,
                                // RecoveryDestWarehouseId = item.WarehouseId,
                                CreatedAt = item.CreatedAt,
                                UpdateAt = DateTime.Now,
                                IsDeleted = false,
                            };
                            await daoRequestCable.CreateRequestCable(update);
                            // ------------------------------------------------------
                        }
                    }
                    // delete cable parent
                    await daoCable.DeleteCable(cable);
                }
            }
            // update request
            request.ApproverId = ApproverID;
            request.Status = RequestConst.STATUS_APPROVED;
            request.UpdateAt = DateTime.Now;
            await daoRequest.UpdateRequest(request);
            // ------------------------------- add transaction cable --------------------------------
            if(listCableExported.Count > 0)
            {
                foreach (Cable cableExported in listCableExported)
                {
                    TransactionHistory history = new TransactionHistory()
                    {
                        TransactionId = Guid.NewGuid(),
                        TransactionCategoryName = TransactionCategoryConst.CATEGORY_EXPORT,
                        CreatedDate = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        WarehouseId = cableExported.WarehouseId,
                        RequestId = RequestID,
                        IssueId = request.IssueId,
                    };
                    await daoHistory.CreateTransactionHistory(history);
                    TransactionCable transactionCable = new TransactionCable()
                    {
                        TransactionId = history.TransactionId,
                        CableId = cableExported.CableId,
                        StartPoint = cableExported.StartPoint,
                        EndPoint = cableExported.EndPoint,
                        Length = cableExported.Length,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false
                    };
                    await daoTransactionCable.CreateTransactionCable(transactionCable);
                    // update cable
                    cableExported.WarehouseId = null;
                    cableExported.UpdateAt = DateTime.Now;
                    await daoCable.UpdateCable(cableExported);
                }
            }
            // ------------------------------- add transaction material --------------------------------
            List<RequestOtherMaterial> requestMaterials = await daoRequestMaterial.getList(RequestID);
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
                        RequestId = RequestID,
                        IssueId = request.IssueId,
                    };
                    await daoHistory.CreateTransactionHistory(history);
                    TransactionOtherMaterial material = new TransactionOtherMaterial()
                    {
                        TransactionId = history.TransactionId,
                        OtherMaterialsId = item.Quantity,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                    };
                    await daoTransactionMaterial.CreateTransactionMaterial(material);
                    // update material quantity
                    item.OtherMaterials.Quantity = item.OtherMaterials.Quantity - item.Quantity;
                    await daoOtherMaterial.UpdateMaterial(item.OtherMaterials);
                }
            }
            return new ResponseDTO<bool>(true, "Yêu cầu được phê duyệt");
        }
        public async Task<ResponseDTO<bool>> Approve(Guid RequestID, Guid ApproverID, string email)
        {
            try
            {
                Request? request = await daoRequest.getRequest(RequestID);
                if(request == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy yêu cầu", (int) HttpStatusCode.NotFound);
                }
                if (!request.Status.Equals(RequestConst.STATUS_PENDING))
                {
                    return new ResponseDTO<bool>(false, "Yêu cầu đã được phê duyệt", (int) HttpStatusCode.Conflict);
                }
                if(request.RequestCategoryId == RequestCategoryConst.CATEGORY_EXPORT)
                {
                    return await ApproveRequestExport(RequestID, ApproverID, request, email);
                }
                return new ResponseDTO<bool>(false, string.Empty, (int) HttpStatusCode.Conflict);
            }catch(Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }


    }
}
