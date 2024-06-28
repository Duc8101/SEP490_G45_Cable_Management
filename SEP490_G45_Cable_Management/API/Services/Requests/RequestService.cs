using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.Const;
using Common.DTO.CableDTO;
using Common.DTO.OtherMaterialsDTO;
using Common.DTO.RequestDTO;
using Common.Entity;
using Common.Pagination;
using DataAccess.DAO;
using DataAccess.Util;
using System.Net;

namespace API.Services.Requests
{
    public class RequestService : BaseService, IRequestService
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

        public RequestService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseBase<Pagination<RequestListDTO>?>> List(string? name, int? RequestCategoryID, string? status, Guid? CreatorID, int page)
        {
            try
            {
                List<Request> list = await daoRequest.getListAll(name, RequestCategoryID, status, CreatorID, page);
                List<RequestListDTO> DTOs = _mapper.Map<List<RequestListDTO>>(list);
                int RowCount = await daoRequest.getRowCount(name, RequestCategoryID, status, CreatorID);
                Pagination<RequestListDTO> result = new Pagination<RequestListDTO>(page, RowCount, PageSizeConst.MAX_REQUEST_LIST_IN_PAGE, DTOs);
                return new ResponseBase<Pagination<RequestListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<Pagination<RequestListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        // check cable valid when create request export and deliver
        private async Task<ResponseBase<bool>> isCableValidCreateExportDeliver(List<CableExportDeliverDTO> list)
        {
            if (list.Count == 0)
            {
                return new ResponseBase<bool>(true, string.Empty);
            }
            foreach (CableExportDeliverDTO DTO in list)
            {
                Cable? cable = await daoCable.getCable(DTO.CableId);
                // if exist cable not found
                if (cable == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy cáp với ID " + DTO.CableId, (int)HttpStatusCode.NotFound);
                }
                // if invalid start point, end point or deleted
                if (DTO.StartPoint < cable.StartPoint || DTO.EndPoint > cable.EndPoint || DTO.StartPoint >= DTO.EndPoint || cable.IsDeleted || DTO.StartPoint < 0 || DTO.EndPoint < 0)
                {
                    return new ResponseBase<bool>(false, "Cáp với ID: " + DTO.CableId + " có chỉ số đầu hoặc chỉ số cuối không hợp lệ hoặc đã bị hủy", (int)HttpStatusCode.Conflict);
                }
                // if cable in use
                if (cable.IsExportedToUse)
                {
                    return new ResponseBase<bool>(false, "Cáp với ID: " + DTO.CableId + " đã được sử dụng", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseBase<bool>(true, string.Empty);
        }
        // check material valid when create request export, deliver,cancel inside
        private async Task<ResponseBase<bool>> isMaterialValidCreateExportDeliverCancelInside(List<OtherMaterialsExportDeliverCancelInsideDTO> list)
        {
            if (list.Count == 0)
            {
                return new ResponseBase<bool>(true, string.Empty);
            }
            foreach (OtherMaterialsExportDeliverCancelInsideDTO DTO in list)
            {
                OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(DTO.OtherMaterialsId);
                // if not found
                if (material == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy vật liệu vs ID = " + DTO.OtherMaterialsId, (int)HttpStatusCode.NotFound);
                }
                // if not enough quantity
                if (material.Quantity < DTO.Quantity)
                {
                    return new ResponseBase<bool>(false, "Vật liệu vs ID = " + DTO.OtherMaterialsId + " không có đủ số lượng", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseBase<bool>(true, string.Empty);
        }
        // add request to db
        private async Task<Guid> CreateRequest(string RequestName, string? content, Guid? IssueID, Guid CreatorID, int RequestCategoryID, int? DeliverWarehouseID)
        {
            Request request = new Request()
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
            await daoRequest.CreateRequest(request);
            return request.RequestId;
        }
        // create request cable when create request export, deliver
        private async Task CreateRequestCableExportDeliver(List<CableExportDeliverDTO> list, Guid RequestID)
        {
            if (list.Count > 0)
            {
                foreach (CableExportDeliverDTO item in list)
                {
                    RequestCable request = _mapper.Map<RequestCable>(item);
                    request.RequestId = RequestID;
                    request.CreatedAt = DateTime.Now;
                    request.UpdateAt = DateTime.Now;
                    request.IsDeleted = false;
                    await daoRequestCable.CreateRequestCable(request);
                }
            }
        }
        // create request material when create request export, deliver
        private async Task CreateRequestMaterialExportDeliver(List<OtherMaterialsExportDeliverCancelInsideDTO> list, Guid RequestID)
        {
            if (list.Count > 0)
            {
                foreach (OtherMaterialsExportDeliverCancelInsideDTO item in list)
                {
                    RequestOtherMaterial request = _mapper.Map<RequestOtherMaterial>(item);
                    request.RequestId = RequestID;
                    request.CreatedAt = DateTime.Now;
                    request.UpdateAt = DateTime.Now;
                    request.IsDeleted = false;
                    await daoRequestMaterial.CreateRequestOtherMaterial(request);
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
        public async Task<ResponseBase<bool>> CreateRequestExport(RequestCreateExportDTO DTO, Guid CreatorID)
        {
            if (DTO.RequestName.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Tên yêu cầu không được để trống", (int)HttpStatusCode.Conflict);
            }
            if (DTO.RequestCategoryId != RequestCategoryConst.CATEGORY_EXPORT)
            {
                return new ResponseBase<bool>(false, "Không phải yêu cầu xuất kho", (int)HttpStatusCode.Conflict);
            }
            try
            {
                Issue? issue = await daoIssue.getIssue(DTO.IssueId);
                // if not found issue
                if (issue == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                // if issue done
                if (issue.Status == IssueConst.STATUS_DONE)
                {
                    return new ResponseBase<bool>(false, "Sự vụ với mã " + issue.IssueCode + " đã được chấp thuận", (int)HttpStatusCode.NotAcceptable);
                }
                ResponseBase<bool> response = await isCableValidCreateExportDeliver(DTO.CableExportDTOs);
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
                Guid RequestID = await CreateRequest(DTO.RequestName.Trim(), DTO.Content, DTO.IssueId, CreatorID, DTO.RequestCategoryId, null);
                await CreateRequestCableExportDeliver(DTO.CableExportDTOs, RequestID);
                await CreateRequestMaterialExportDeliver(DTO.OtherMaterialsExportDTOs, RequestID);
                // ----------------------------- send email to admin ---------------------------
                await sendEmailToAdmin(DTO.RequestName.Trim(), "Xuất kho", issue);
                return new ResponseBase<bool>(true, "Tạo yêu cầu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        // check cable valid when approve request export, deliver, cancel inside
        private async Task<ResponseBase<bool>> isCableValidApproveExportDeliverCancelInside(List<RequestCable> list, bool action)
        {
            if (list.Count == 0)
            {
                return new ResponseBase<bool>(true, string.Empty);
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
                    return new ResponseBase<bool>(false, "Không thể " + mess + " " + item.Cable.CableCategory.CableCategoryName + " với ID: " + item.CableId
                               + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ")", (int)HttpStatusCode.Conflict);
                }
                // if cable exported
                if (cable.IsExportedToUse == true)
                {
                    return new ResponseBase<bool>(false, item.Cable.CableCategory.CableCategoryName + " với ID: " + item.CableId
                        + " (chỉ số đầu: " + item.StartPoint + ", chỉ số cuối: " + item.EndPoint + ") đã được sử dụng!", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseBase<bool>(true, string.Empty);
        }
        // check material valid when approve request export, deliver, cancel inside
        private async Task<ResponseBase<bool>> isMaterialValidApproveExportDeliverCancelInside(List<RequestOtherMaterial> list)
        {
            if (list.Count == 0)
            {
                return new ResponseBase<bool>(true, string.Empty);
            }
            foreach (RequestOtherMaterial item in list)
            {
                OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(item.OtherMaterialsId);
                // if not found
                if (material == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy vật liệu " + item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName, (int)HttpStatusCode.NotFound);
                }
                // if not enough quantity
                if (material.Quantity < item.Quantity)
                {
                    return new ResponseBase<bool>(false, "Không đủ số lượng " + item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName, (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseBase<bool>(true, string.Empty);
        }
        private async Task UpdateRequest(Request request, Guid ID, string status)
        {
            request.ApproverId = ID;
            request.Status = status;
            request.UpdateAt = DateTime.Now;
            await daoRequest.UpdateRequest(request);
        }
        // send email to creator after approve request
        private async Task sendEmailToCreator(Request request, string ApproverName)
        {
            string body = await UserUtil.BodyEmailForApproveRequest(request, ApproverName);
            await UserUtil.sendEmail("Thông báo yêu cầu được phê duyệt", body, request.Creator.Email);
        }
        private async Task CreateTransaction(Request request, List<int> listCableWarehouseID, List<Cable> listCable, List<int> listMaterialWarehouseID, List<int> listMaterialID, List<int> listQuantity, bool? action)
        {
            string CategoryName;
            int? FromWarehouseID = null;
            int? ToWarehouseID = null;
            if (action == null)
            {
                CategoryName = TransactionCategoryConst.CATEGORY_CANCEL;
            }
            else
            {
                CategoryName = action == true ? TransactionCategoryConst.CATEGORY_IMPORT : TransactionCategoryConst.CATEGORY_EXPORT;
            }
            // if transaction export and request deliver
            if (action == false && request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER)
            {
                ToWarehouseID = request.DeliverWarehouseId;
                // if transaction import and request deliver
            }
            else if (action == true && request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER)
            {
                FromWarehouseID = request.DeliverWarehouseId;
            }
            // ------------------------------- create transaction history -----------------------------
            // list contain all warehouse of cable and material
            List<int> listAllWarehouse = new List<int>();
            if (listCableWarehouseID.Count > 0)
            {
                foreach (int WarehouseID in listCableWarehouseID)
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
        private async Task<ResponseBase<bool>> ApproveRequestExportDeliverCancelInside(Guid ApproverID, Request request, string ApproverName)
        {
            List<RequestCable> requestCables = await daoRequestCable.getList(request.RequestId);
            List<RequestOtherMaterial> requestMaterials = await daoRequestMaterial.getList(request.RequestId);
            // ------------------------------- check cable valid ------------------------------  
            ResponseBase<bool> check = await isCableValidApproveExportDeliverCancelInside(requestCables, true);// true : xuat kho , false:huy
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
            // if request export
            if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_EXPORT)
            {
                Dictionary<string, object> result = await ApproveRequestExport(request, requestCables, requestMaterials);
                List<Cable> listCableExported = (List<Cable>)result["listCableExported"];
                List<int> listCableWarehouseID = (List<int>)result["listCableWarehouseID"];
                List<int> listMaterialWarehouseID = (List<int>)result["listMaterialWarehouseID"];
                List<int> listMaterialIDExported = (List<int>)result["listMaterialIDExported"];
                List<int> listQuantity = (List<int>)result["listQuantity"];
                await CreateTransaction(request, listCableWarehouseID, listCableExported, listMaterialWarehouseID, listMaterialIDExported, listQuantity, false);
                // if request deliver
            }
            else if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER)
            {
                Dictionary<string, object> result = await ApproveRequestDeliver(request, requestCables, requestMaterials);
                List<Cable> listCableDeliver = (List<Cable>)result["listCableDeliver"];
                List<int> listCableWarehouseID = (List<int>)result["listCableWarehouseID"];
                List<int> listMaterialWarehouseID = (List<int>)result["listMaterialWarehouseID"];
                List<int> listMaterialIDDeliver = (List<int>)result["listMaterialIDDeliver"];
                List<int> listQuantity = (List<int>)result["listQuantity"];
                await CreateTransaction(request, listCableWarehouseID, listCableDeliver, listMaterialWarehouseID, listMaterialIDDeliver, listQuantity, true);
            }
            else
            {
                Dictionary<string, object> result = await ApproveRequestCancelInside(requestCables, requestMaterials);
                List<Cable> listCableCancelInside = (List<Cable>)result["listCableCancelInside"];
                List<int> listCableWarehouseID = (List<int>)result["listCableWarehouseID"];
                List<int> listMaterialWarehouseID = (List<int>)result["listMaterialWarehouseID"];
                List<int> listMaterialIDCancelInside = (List<int>)result["listMaterialIDCancelInside"];
                List<int> listQuantity = (List<int>)result["listQuantity"];
                await CreateTransaction(request, listCableWarehouseID, listCableCancelInside, listMaterialWarehouseID, listMaterialIDCancelInside, listQuantity, null);
            }
            // ------------------------------- update request approved --------------------------------
            await UpdateRequest(request, ApproverID, RequestConst.STATUS_APPROVED);
            // ------------------------------- send email --------------------------------
            await sendEmailToCreator(request, ApproverName);
            return new ResponseBase<bool>(true, "Yêu cầu được phê duyệt");
        }
        private async Task<Dictionary<string, object>> ApproveRequestExport(Request request, List<RequestCable> requestCables, List<RequestOtherMaterial> requestMaterials)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            // list cable
            List<Cable> listCableExported = new List<Cable>();
            List<int> listCableWarehouseID = new List<int>();
            // list material
            List<int> listMaterialWarehouseID = new List<int>();
            List<int> listMaterialIDExported = new List<int>();
            List<int> listQuantity = new List<int>();
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
                    if (cable != null)
                    {
                        if (item.StartPoint == cable.StartPoint && item.EndPoint == cable.EndPoint)
                        {
                            Cable? update = await daoCable.getCable(cable.CableId);
                            if (update != null)
                            {
                                if (update.WarehouseId.HasValue)
                                {
                                    listCableWarehouseID.Add(update.WarehouseId.Value);
                                    listCableExported.Add(update);
                                }
                                // update cable
                                update.IsExportedToUse = true;
                                update.WarehouseId = null;
                                update.UpdateAt = DateTime.Now;
                                await daoCable.UpdateCable(update);
                            }
                        }
                        else
                        {
                            List<Cable> listCut = CableUtil.getListCableCut(item.StartPoint, item.EndPoint, cable, request.DeliverWarehouseId);
                            foreach (Cable cut in listCut)
                            {
                                // if cable cut exported
                                if (cut.IsExportedToUse)
                                {
                                    // if warehouse cable not null
                                    if (cable.WarehouseId.HasValue)
                                    {
                                        listCableWarehouseID.Add(cable.WarehouseId.Value);
                                        listCableExported.Add(cut);
                                    }
                                    await daoCable.CreateCable(cut);
                                }
                                else
                                {
                                    await daoCable.CreateCable(cut);
                                }
                            }
                            // delete cable parent
                            Cable? cableParent = await daoCable.getCable(cable.CableId);
                            if (cableParent != null)
                            {
                                await daoCable.DeleteCable(cableParent);
                            }
                        }
                    }
                }
            }

            if (requestMaterials.Count > 0)
            {
                foreach (RequestOtherMaterial item in requestMaterials)
                {
                    OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(item.OtherMaterialsId);
                    // if material valid
                    if (material != null)
                    {
                        // if material warehouse id not null
                        if (material.WarehouseId.HasValue)
                        {
                            listMaterialWarehouseID.Add(material.WarehouseId.Value);
                            listMaterialIDExported.Add(material.OtherMaterialsId);
                            listQuantity.Add(item.Quantity);
                        }
                        // update material quantity
                        material.Quantity = material.Quantity - item.Quantity;
                        material.UpdateAt = DateTime.Now;
                        await daoOtherMaterial.UpdateMaterial(material);
                    }
                }
            }
            result["listCableExported"] = listCableExported;
            result["listCableWarehouseID"] = listCableWarehouseID;
            result["listMaterialWarehouseID"] = listMaterialWarehouseID;
            result["listMaterialIDExported"] = listMaterialIDExported;
            result["listQuantity"] = listQuantity;
            return result;

        }
        private async Task<Dictionary<string, object>> ApproveRequestDeliver(Request request, List<RequestCable> requestCables, List<RequestOtherMaterial> requestMaterials)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            // list cable
            List<Cable> listCableDeliver = new List<Cable>();
            List<int> listCableWarehouseID = new List<int>();
            // list material
            List<int> listMaterialWarehouseID = new List<int>();
            List<int> listMaterialIDDeliver = new List<int>();
            List<int> listQuantity = new List<int>();
            //-------------------- create transaction export first, then create transaction import -------------------
            if (requestCables.Count > 0)
            {
                foreach (RequestCable item in requestCables)
                {
                    if (item.Cable.WarehouseId.HasValue)
                    {
                        Cable cable = new Cable()
                        {
                            CableId = item.CableId,
                            StartPoint = item.StartPoint,
                            EndPoint = item.EndPoint,
                            Length = item.Length,
                        };
                        listCableDeliver.Add(cable);
                        listCableWarehouseID.Add(item.Cable.WarehouseId.Value);
                    }
                }
            }
            if (requestMaterials.Count > 0)
            {
                foreach (RequestOtherMaterial item in requestMaterials)
                {
                    if (item.OtherMaterials.WarehouseId.HasValue)
                    {
                        listMaterialIDDeliver.Add(item.OtherMaterialsId);
                        listMaterialWarehouseID.Add(item.OtherMaterials.WarehouseId.Value);
                        listQuantity.Add(item.Quantity);
                    }
                }
            }
            await CreateTransaction(request, listCableWarehouseID, listCableDeliver, listMaterialWarehouseID, listMaterialIDDeliver, listQuantity, false);
            // ------------------------------- request cable ------------------------------
            listCableDeliver = new List<Cable>();
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
                    if (cable != null)
                    {
                        if (item.StartPoint == cable.StartPoint && item.EndPoint == cable.EndPoint)
                        {
                            Cable? update = await daoCable.getCable(cable.CableId);
                            if (update != null)
                            {
                                if (request.DeliverWarehouseId.HasValue)
                                {
                                    listCableWarehouseID.Add(request.DeliverWarehouseId.Value);
                                    listCableDeliver.Add(update);
                                }
                                // update cable
                                update.WarehouseId = request.DeliverWarehouseId;
                                update.UpdateAt = DateTime.Now;
                                await daoCable.UpdateCable(update);
                            }
                        }
                        else
                        {
                            List<Cable> listCut = CableUtil.getListCableCut(item.StartPoint, item.EndPoint, cable, request.DeliverWarehouseId);
                            foreach (Cable cut in listCut)
                            {
                                // if cable deliver
                                if (cut.WarehouseId == request.DeliverWarehouseId)
                                {
                                    // if warehouse deliver not null
                                    if (request.DeliverWarehouseId.HasValue)
                                    {
                                        listCableWarehouseID.Add(request.DeliverWarehouseId.Value);
                                        listCableDeliver.Add(cut);
                                    }
                                    await daoCable.CreateCable(cut);
                                }
                                else
                                {
                                    await daoCable.CreateCable(cut);
                                }
                            }
                            // delete cable parent
                            Cable? cableParent = await daoCable.getCable(cable.CableId);
                            if (cableParent != null)
                            {
                                await daoCable.DeleteCable(cableParent);
                            }
                        }

                    }
                }

            }
            // ------------------------------- request material ------------------------------
            listMaterialWarehouseID = new List<int>();
            listMaterialIDDeliver = new List<int>();
            listQuantity = new List<int>();
            if (requestMaterials.Count > 0)
            {
                foreach (RequestOtherMaterial item in requestMaterials)
                {
                    OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(item.OtherMaterialsId);
                    // if material valid
                    if (material != null)
                    {
                        // update quantity
                        material.Quantity = material.Quantity - item.Quantity;
                        material.UpdateAt = DateTime.Now;
                        await daoOtherMaterial.UpdateMaterial(material);
                        // check material exist in deliver warehouse
                        OtherMaterial? exist = await daoOtherMaterial.getOtherMaterial(request.DeliverWarehouseId, material.Code, material.Status, material.Unit);
                        // if material not exist in deliver warehouse
                        if (exist == null)
                        {
                            // create new other material in deliver warehouse
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
                                listMaterialIDDeliver.Add(MaterialID);
                                listQuantity.Add(item.Quantity);
                                listMaterialWarehouseID.Add(request.DeliverWarehouseId.Value);
                            }
                        }
                        else
                        {
                            if (request.DeliverWarehouseId.HasValue)
                            {
                                listMaterialIDDeliver.Add(item.OtherMaterialsId);
                                listQuantity.Add(item.Quantity);
                                listMaterialWarehouseID.Add(request.DeliverWarehouseId.Value);
                            }
                            // update material quantity
                            exist.Quantity = exist.Quantity + item.Quantity;
                            exist.UpdateAt = DateTime.Now;
                            await daoOtherMaterial.UpdateMaterial(exist);
                        }
                    }
                }
            }
            result["listCableDeliver"] = listCableDeliver;
            result["listCableWarehouseID"] = listCableWarehouseID;
            result["listMaterialWarehouseID"] = listMaterialWarehouseID;
            result["listMaterialIDDeliver"] = listMaterialIDDeliver;
            result["listQuantity"] = listQuantity;
            return result;
        }
        private async Task<Dictionary<string, object>> ApproveRequestCancelInside(List<RequestCable> requestCables, List<RequestOtherMaterial> requestMaterials)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            // list cable
            List<Cable> listCableCancelInside = new List<Cable>();
            List<int> listCableWarehouseID = new List<int>();
            // list material
            List<int> listMaterialWarehouseID = new List<int>();
            List<int> listMaterialIDCancelInside = new List<int>();
            List<int> listQuantity = new List<int>();
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
                    if (cable != null)
                    {
                        if (item.StartPoint == cable.StartPoint && item.EndPoint == cable.EndPoint)
                        {
                            Cable? update = await daoCable.getCable(cable.CableId);
                            if (update != null)
                            {
                                if (update.WarehouseId.HasValue)
                                {
                                    listCableWarehouseID.Add(update.WarehouseId.Value);
                                    listCableCancelInside.Add(update);
                                }
                                // update cable
                                update.IsDeleted = true;
                                update.UpdateAt = DateTime.Now;
                                await daoCable.UpdateCable(update);
                            }
                        }
                        else
                        {
                            List<Cable> listCut = CableUtil.getListCableCut(item.StartPoint, item.EndPoint, cable);
                            foreach (Cable cut in listCut)
                            {
                                // if cable cancel
                                if (cut.IsDeleted)
                                {
                                    // if warehouse cancel not null
                                    if (cut.WarehouseId.HasValue)
                                    {
                                        listCableWarehouseID.Add(cut.WarehouseId.Value);
                                        listCableCancelInside.Add(cut);
                                    }
                                    await daoCable.CreateCable(cut);
                                }
                                else
                                {
                                    await daoCable.CreateCable(cut);
                                }
                            }
                            // delete cable parent
                            Cable? cableParent = await daoCable.getCable(cable.CableId);
                            if (cableParent != null)
                            {
                                await daoCable.DeleteCable(cableParent);
                            }
                        }

                    }
                }

            }

            if (requestMaterials.Count > 0)
            {
                foreach (RequestOtherMaterial item in requestMaterials)
                {
                    OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(item.OtherMaterialsId);
                    // if material valid
                    if (material != null)
                    {
                        if (material.WarehouseId.HasValue)
                        {
                            listMaterialWarehouseID.Add(material.WarehouseId.Value);
                            listMaterialIDCancelInside.Add(material.OtherMaterialsId);
                            listQuantity.Add(item.Quantity);
                        }
                        // update material quantity
                        material.Quantity = material.Quantity - item.Quantity;
                        material.UpdateAt = DateTime.Now;
                        await daoOtherMaterial.UpdateMaterial(material);
                    }
                }
            }

            result["listCableCancelInside"] = listCableCancelInside;
            result["listCableWarehouseID"] = listCableWarehouseID;
            result["listMaterialWarehouseID"] = listMaterialWarehouseID;
            result["listMaterialIDCancelInside"] = listMaterialIDCancelInside;
            result["listQuantity"] = listQuantity;
            return result;
        }
        public async Task<ResponseBase<bool>> CreateRequestRecovery(RequestCreateRecoveryDTO DTO, Guid CreatorID)
        {
            if (DTO.RequestName.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Tên yêu cầu không được để trống", (int)HttpStatusCode.Conflict);
            }
            if (DTO.RequestCategoryId != RequestCategoryConst.CATEGORY_RECOVERY)
            {
                return new ResponseBase<bool>(false, "Không phải yêu cầu thu hồi", (int)HttpStatusCode.Conflict);
            }
            try
            {
                Issue? issue = await daoIssue.getIssue(DTO.IssueId);
                // if not found issue
                if (issue == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                // if issue done
                if (issue.Status == IssueConst.STATUS_DONE)
                {
                    return new ResponseBase<bool>(false, "Sự vụ với mã " + issue.IssueCode + " đã được chấp thuận", (int)HttpStatusCode.NotAcceptable);
                }
                // ------------------------------- create cable --------------------------
                List<Cable> listCable = new List<Cable>();
                if (DTO.CableRecoveryDTOs.Count > 0)
                {
                    foreach (CableCreateUpdateDTO item in DTO.CableRecoveryDTOs)
                    {
                        Cable cable = _mapper.Map<Cable>(DTO);
                        cable.CableId = Guid.NewGuid();
                        cable.CreatorId = CreatorID;
                        cable.CreatedAt = DateTime.Now;
                        cable.UpdateAt = DateTime.Now;
                        cable.IsDeleted = false;
                        cable.IsExportedToUse = false;
                        cable.IsInRequest = true;
                        await daoCable.CreateCable(cable);
                        listCable.Add(cable);
                    }
                }
                // ------------------------------- create material --------------------------
                // list warehouse
                List<int?> listWareHouse = new List<int?>();
                // list quantity
                List<int> listQuantity = new List<int>();
                // list material id
                List<int> listID = new List<int>();
                // list status
                List<string?> listStatus = new List<string?>();
                if (DTO.OtherMaterialsRecoveryDTOs.Count > 0)
                {
                    foreach (OtherMaterialsRecoveryDTO item in DTO.OtherMaterialsRecoveryDTOs)
                    {
                        OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(item);
                        // if material not exist
                        if (material == null)
                        {
                            material = _mapper.Map<OtherMaterial>(DTO);
                            material.Quantity = 0;
                            material.CreatedAt = DateTime.Now;
                            material.UpdateAt = DateTime.Now;
                            material.IsDeleted = false;
                            int OtherMaterialID = await daoOtherMaterial.CreateMaterial(material);
                            listWareHouse.Add(item.WarehouseId);
                            listQuantity.Add(item.Quantity);
                            listID.Add(OtherMaterialID);
                            listStatus.Add(item.Status.Trim());
                        }
                        else
                        {
                            listWareHouse.Add(item.WarehouseId);
                            listQuantity.Add(item.Quantity);
                            listID.Add(material.OtherMaterialsId);
                            listStatus.Add(null);
                            // update material
                            material.UpdateAt = DateTime.Now;
                            await daoOtherMaterial.UpdateMaterial(material);
                        }
                    }
                }
                //----------------------------- create request --------------------------------
                Guid RequestID = await CreateRequest(DTO.RequestName.Trim(), DTO.Content, DTO.IssueId, CreatorID, DTO.RequestCategoryId, null);
                //----------------------------- create request cable --------------------------------
                if (listCable.Count > 0)
                {
                    foreach (Cable cable in listCable)
                    {
                        RequestCable request = _mapper.Map<RequestCable>(cable);
                        request.RequestId = RequestID;
                        request.CreatedAt = DateTime.Now;
                        request.UpdateAt = DateTime.Now;
                        request.IsDeleted = false;
                        await daoRequestCable.CreateRequestCable(request);
                    }
                }
                //----------------------------- create request material --------------------------------
                if (listID.Count > 0)
                {
                    for (int i = 0; i < listID.Count; i++)
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
                        await daoRequestMaterial.CreateRequestOtherMaterial(request);
                    }
                }
                // ----------------------------- send email to admin ---------------------------
                await sendEmailToAdmin(DTO.RequestName.Trim(), "Thu hồi", issue);
                return new ResponseBase<bool>(true, "Tạo yêu cầu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        // check cable valid when approve request recovery
        private async Task<ResponseBase<bool>> isCableValidApproveRecovery(List<RequestCable> list)
        {
            if (list.Count == 0)
            {
                return new ResponseBase<bool>(true, string.Empty);
            }
            foreach (RequestCable request in list)
            {
                Cable? cable = await daoCable.getCable(request.CableId);
                if (cable == null)
                {
                    return new ResponseBase<bool>(false, request.Cable.CableCategory.CableCategoryName + " với ID: " + request.CableId
                                + " không tồn tại ", (int)HttpStatusCode.NotFound);
                }

                if (cable.StartPoint < 0 || cable.EndPoint < 0 || cable.StartPoint >= cable.EndPoint)
                {
                    return new ResponseBase<bool>(false, request.Cable.CableCategory.CableCategoryName + " với ID: " + request.CableId
                                + " có chỉ số đầu chỉ số cuối không hợp lệ ", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseBase<bool>(true, string.Empty);
        }
        // check material valid when approve request recovery
        private async Task<ResponseBase<bool>> isMaterialValidApproveRecovery(List<RequestOtherMaterial> list)
        {
            if (list.Count == 0)
            {
                return new ResponseBase<bool>(true, string.Empty);
            }
            foreach (RequestOtherMaterial request in list)
            {
                if (request.Quantity < 0)
                {
                    return new ResponseBase<bool>(false, "Số lượng không hợp lệ", (int)HttpStatusCode.NotFound);
                }
                OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(request.OtherMaterialsId);
                if (material == null)
                {
                    return new ResponseBase<bool>(false, request.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName + " với ID: " + request.OtherMaterialsId
                                + " không tồn tại ", (int)HttpStatusCode.NotFound);
                }
            }
            return new ResponseBase<bool>(true, string.Empty);
        }
        private async Task<ResponseBase<bool>> ApproveRequestRecovery(Guid ApproverID, Request request, string ApproverName)
        {
            List<RequestCable> requestCables = await daoRequestCable.getList(request.RequestId);
            List<RequestOtherMaterial> requestMaterials = await daoRequestMaterial.getList(request.RequestId);
            // ------------------------------- check cable valid ------------------------------  
            ResponseBase<bool> check = await isCableValidApproveRecovery(requestCables);
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
                foreach (RequestCable item in requestCables)
                {
                    Cable? cable = await daoCable.getCable(item.CableId);
                    if (cable != null)
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
            List<int> listMaterialIDRecovery = new List<int>();
            List<int> listQuantity = new List<int>();
            if (requestMaterials.Count > 0)
            {
                foreach (RequestOtherMaterial item in requestMaterials)
                {
                    OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(item.OtherMaterialsId);
                    if (material != null)
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
            await UpdateRequest(request, ApproverID, RequestConst.STATUS_APPROVED);
            // ------------------------------- create transaction ------------------------------
            await CreateTransaction(request, listCableWarehouseID, listCableRecovery, listMaterialWarehouseID, listMaterialIDRecovery, listQuantity, true);
            // ------------------------------- send email ------------------------------
            await sendEmailToCreator(request, ApproverName);
            return new ResponseBase<bool>(true, "Yêu cầu được phê duyệt");
        }
        public async Task<ResponseBase<bool>> Approve(Guid RequestID, Guid ApproverID, string ApproverName)
        {
            try
            {
                Request? request = await daoRequest.getRequest(RequestID);
                if (request == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy yêu cầu", (int)HttpStatusCode.NotFound);
                }
                if (request.Status != RequestConst.STATUS_PENDING)
                {
                    return new ResponseBase<bool>(false, "Yêu cầu đã được xác nhận chấp thuận hoặc bị từ chối", (int)HttpStatusCode.Conflict);
                }
                if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_EXPORT || request.RequestCategoryId == RequestCategoryConst.CATEGORY_DELIVER || request.RequestCategoryId == RequestCategoryConst.CATEGORY_CANCEL_INSIDE)
                {
                    return await ApproveRequestExportDeliverCancelInside(ApproverID, request, ApproverName);
                }
                if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_RECOVERY)
                {
                    return await ApproveRequestRecovery(ApproverID, request, ApproverName);
                }
                if (request.RequestCategoryId == RequestCategoryConst.CATEGORY_CANCEL_OUTSIDE)
                {
                    return await ApproveRequestCancelOutside(ApproverID, request, ApproverName);
                }
                return new ResponseBase<bool>(false, "Không hỗ trợ yêu cầu " + request.RequestCategory.RequestCategoryName, (int)HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Reject(Guid RequestID, Guid RejectorID)
        {
            try
            {
                Request? request = await daoRequest.getRequest(RequestID);
                if (request == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy yêu cầu", (int)HttpStatusCode.NotFound);
                }
                if (request.Status != RequestConst.STATUS_PENDING)
                {
                    return new ResponseBase<bool>(false, "Yêu cầu đã được xác nhận chấp thuận hoặc bị từ chối", (int)HttpStatusCode.Conflict);
                }
                // ------------------- update request ---------------
                await UpdateRequest(request, RejectorID, RequestConst.STATUS_REJECTED);
                return new ResponseBase<bool>(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> CreateRequestDeliver(RequestCreateDeliverDTO DTO, Guid CreatorID)
        {
            if (DTO.RequestName.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Tên yêu cầu không được để trống", (int)HttpStatusCode.Conflict);
            }
            if (DTO.RequestCategoryId != RequestCategoryConst.CATEGORY_DELIVER)
            {
                return new ResponseBase<bool>(false, "Không phải yêu cầu chuyển kho", (int)HttpStatusCode.Conflict);
            }
            try
            {
                Warehouse? ware = await daoWarehouse.getWarehouse(DTO.DeliverWareHouseID);
                if (ware == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy kho nhận", (int)HttpStatusCode.NotFound);
                }
                ResponseBase<bool> response = await isCableValidCreateExportDeliver(DTO.CableDeliverDTOs);
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
                Guid RequestID = await CreateRequest(DTO.RequestName.Trim(), DTO.Content, null, CreatorID, DTO.RequestCategoryId, DTO.DeliverWareHouseID);
                //----------------------------- create request cable --------------------------------
                await CreateRequestCableExportDeliver(DTO.CableDeliverDTOs, RequestID);
                //----------------------------- create request material --------------------------------
                await CreateRequestMaterialExportDeliver(DTO.OtherMaterialsDeliverDTOs, RequestID);
                // ----------------------------- send email to admin ---------------------------
                await sendEmailToAdmin(DTO.RequestName.Trim(), "Thu hồi", null);
                return new ResponseBase<bool>(true, "Tạo yêu cầu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        private async Task<ResponseBase<bool>> isCableValidCreateCancelInside(List<CableCancelInsideDTO> list)
        {
            if (list.Count == 0)
            {
                return new ResponseBase<bool>(true, string.Empty);
            }
            foreach (CableCancelInsideDTO DTO in list)
            {
                Cable? cable = await daoCable.getCable(DTO.CableId);
                // if exist cable not found
                if (cable == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy cáp với ID " + DTO.CableId, (int)HttpStatusCode.NotFound);
                }
                // if cable in use
                if (cable.IsExportedToUse)
                {
                    return new ResponseBase<bool>(false, "Cáp với ID: " + DTO.CableId + " đã được sử dụng", (int)HttpStatusCode.Conflict);
                }
            }
            return new ResponseBase<bool>(true, string.Empty);
        }
        public async Task<ResponseBase<bool>> CreateRequestCancelInside(RequestCreateCancelInsideDTO DTO, Guid CreatorID)
        {
            if (DTO.RequestName.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Tên yêu cầu không được để trống", (int)HttpStatusCode.Conflict);
            }
            if (DTO.RequestCategoryId != RequestCategoryConst.CATEGORY_CANCEL_INSIDE)
            {
                return new ResponseBase<bool>(false, "Không phải yêu cầu hủy trong kho", (int)HttpStatusCode.Conflict);
            }
            try
            {
                Issue? issue = await daoIssue.getIssue(DTO.IssueId);
                // if not found issue
                if (issue == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                // if issue done
                if (issue.Status == IssueConst.STATUS_DONE)
                {
                    return new ResponseBase<bool>(false, "Sự vụ với mã " + issue.IssueCode + " đã được chấp thuận", (int)HttpStatusCode.NotAcceptable);
                }
                // ----------------------- check cable valid --------------------------
                ResponseBase<bool> check = await isCableValidCreateCancelInside(DTO.CableCancelInsideDTOs);
                // if exist cable invalid
                if (check.Success == false)
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
                Guid RequestID = await CreateRequest(DTO.RequestName.Trim(), DTO.Content, DTO.IssueId, CreatorID, DTO.RequestCategoryId, null);
                //----------------------------- create request cable --------------------------------
                if (DTO.CableCancelInsideDTOs.Count > 0)
                {
                    foreach (CableCancelInsideDTO item in DTO.CableCancelInsideDTOs)
                    {
                        Cable? cable = await daoCable.getCable(item.CableId);
                        // if cable valid
                        if (cable != null && cable.IsExportedToUse == false)
                        {
                            RequestCable requestCable = _mapper.Map<RequestCable>(cable);
                            requestCable.RequestId = RequestID;
                            requestCable.CreatedAt = DateTime.Now;
                            requestCable.UpdateAt = DateTime.Now;
                            requestCable.IsDeleted = false;
                            await daoRequestCable.CreateRequestCable(requestCable);
                        }
                    }
                }
                //----------------------------- create request material --------------------------------
                if (DTO.OtherMaterialsCancelInsideDTOs.Count > 0)
                {
                    foreach (OtherMaterialsExportDeliverCancelInsideDTO item in DTO.OtherMaterialsCancelInsideDTOs)
                    {
                        RequestOtherMaterial requestMaterial = _mapper.Map<RequestOtherMaterial>(item);
                        requestMaterial.RequestId = RequestID;
                        requestMaterial.CreatedAt = DateTime.Now;
                        requestMaterial.UpdateAt = DateTime.Now;
                        requestMaterial.IsDeleted = false;
                        await daoRequestMaterial.CreateRequestOtherMaterial(requestMaterial);
                    }
                }
                await sendEmailToAdmin(DTO.RequestName.Trim(), "Hủy trong kho", issue);
                return new ResponseBase<bool>(true, "Tạo yêu cầu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        // create cable while creating request cancel outside
        private async Task<List<Cable>> CreateCable(List<CableCancelOutsideDTO> list, Guid CreatorID)
        {
            List<Cable> result = new List<Cable>();
            if (list.Count > 0)
            {
                foreach (CableCancelOutsideDTO DTO in list)
                {
                    Cable cable = _mapper.Map<Cable>(DTO);
                    cable.CableId = Guid.NewGuid();
                    cable.CreatorId = CreatorID;
                    cable.CreatedAt = DateTime.Now;
                    cable.UpdateAt = DateTime.Now;
                    cable.IsDeleted = false;
                    cable.IsExportedToUse = false;
                    cable.IsInRequest = true;
                    cable.WarehouseId = null;
                    await daoCable.CreateCable(cable);
                    result.Add(cable);
                }
            }
            return result;
        }
        // create material while creating request cancel outside
        private async Task<Dictionary<int, int>> CreateMaterial(List<OtherMaterialsCancelOutsideDTO> list)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            if (list.Count > 0)
            {
                foreach (OtherMaterialsCancelOutsideDTO DTO in list)
                {
                    OtherMaterial? material = await daoOtherMaterial.getOtherMaterial(DTO);
                    if (material == null)
                    {
                        material = _mapper.Map<OtherMaterial>(DTO);
                        material.CreatedAt = DateTime.Now;
                        material.UpdateAt = DateTime.Now;
                        material.WarehouseId = null;
                        material.IsDeleted = true;
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
        public async Task<ResponseBase<bool>> CreateRequestCancelOutside(RequestCreateCancelOutsideDTO DTO, Guid CreatorID)
        {
            if (DTO.RequestName.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Tên yêu cầu không được để trống", (int)HttpStatusCode.Conflict);
            }
            if (DTO.RequestCategoryId != RequestCategoryConst.CATEGORY_CANCEL_OUTSIDE)
            {
                return new ResponseBase<bool>(false, "Không phải yêu cầu hủy ngoài kho", (int)HttpStatusCode.Conflict);
            }
            try
            {
                Issue? issue = await daoIssue.getIssue(DTO.IssueId);
                // if not found issue
                if (issue == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                // if issue done
                if (issue.Status == IssueConst.STATUS_DONE)
                {
                    return new ResponseBase<bool>(false, "Sự vụ với mã " + issue.IssueCode + " đã được chấp thuận", (int)HttpStatusCode.NotAcceptable);
                }
                // get list after create cable
                List<Cable> listCable = await CreateCable(DTO.CableCancelOutsideDTOs, CreatorID);
                // get list after create material
                // data store material id, quantity
                Dictionary<int, int> dic = await CreateMaterial(DTO.OtherMaterialsCancelOutsideDTOs);
                // --------------------------- create request ------------------------------------
                Guid RequestID = await CreateRequest(DTO.RequestName.Trim(), DTO.Content, DTO.IssueId, CreatorID, DTO.RequestCategoryId, null);
                // --------------------------- create request cable ------------------------------------
                if (listCable.Count > 0)
                {
                    foreach (Cable item in listCable)
                    {
                        RequestCable requestCable = _mapper.Map<RequestCable>(item);
                        requestCable.RequestId = RequestID;
                        requestCable.CreatedAt = DateTime.Now;
                        requestCable.UpdateAt = DateTime.Now;
                        requestCable.IsDeleted = false;
                        await daoRequestCable.CreateRequestCable(requestCable);
                    }
                }
                // --------------------------- create request material ------------------------------------
                if (dic.Count > 0)
                {
                    foreach (int MaterialID in dic.Keys)
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
                        await daoRequestMaterial.CreateRequestOtherMaterial(requestMaterial);
                    }
                }
                await sendEmailToAdmin(DTO.RequestName.Trim(), "Hủy ngoài kho", issue);
                return new ResponseBase<bool>(true, "Tạo yêu cầu thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        private async Task<ResponseBase<bool>> ApproveRequestCancelOutside(Guid ApproverID, Request request, string ApproverName)
        {
            List<RequestCable> requestCables = await daoRequestCable.getList(request.RequestId);
            if (requestCables.Count > 0)
            {
                foreach (RequestCable item in requestCables)
                {
                    Cable? cable = await daoCable.getCable(item.CableId);
                    if (cable != null && cable.IsInRequest == true)
                    {
                        cable.IsInRequest = false;
                        cable.IsDeleted = true;
                        cable.UpdateAt = DateTime.Now;
                        await daoCable.UpdateCable(cable);
                    }
                }
            }
            // -------------------------------update request approved --------------------------------
            await UpdateRequest(request, ApproverID, RequestConst.STATUS_APPROVED);
            // ------------------------------- send email --------------------------------
            await sendEmailToCreator(request, ApproverName);
            return new ResponseBase<bool>(true, "Yêu cầu được phê duyệt");
        }
        public async Task<ResponseBase<bool>> Delete(Guid RequestID)
        {
            try
            {
                Request? request = await daoRequest.getRequest(RequestID);
                if (request == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy yêu cầu", (int)HttpStatusCode.NotFound);
                }
                if (!request.Status.Equals(RequestConst.STATUS_PENDING))
                {
                    return new ResponseBase<bool>(false, "Yêu cầu đã được chấp thuận hoặc bị từ chối", (int)HttpStatusCode.Conflict);
                }
                await daoRequest.DeleteRequest(request);
                return new ResponseBase<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<RequestDetailDTO?>> Detail(Guid RequestID)
        {
            try
            {
                Request? request = await daoRequest.getRequest(RequestID);
                if (request == null)
                {
                    return new ResponseBase<RequestDetailDTO?>(null, "Không tìm thấy yêu cầu", (int)HttpStatusCode.NotFound);
                }
                List<RequestCable> requestCables = await daoRequestCable.getList(RequestID);
                List<RequestCableListDTO> RequestCableDTOs = _mapper.Map<List<RequestCableListDTO>>(requestCables);
                List<RequestOtherMaterial> requestMaterials = await daoRequestMaterial.getList(RequestID);
                List<RequestOtherMaterialsListDTO> RequestOtherMaterialsDTOs = _mapper.Map<List<RequestOtherMaterialsListDTO>>(requestMaterials);
                RequestDetailDTO data = _mapper.Map<RequestDetailDTO>(request);
                data.RequestCableDTOs = RequestCableDTOs;
                data.RequestOtherMaterialsDTOs = RequestOtherMaterialsDTOs;
                return new ResponseBase<RequestDetailDTO?>(data, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<RequestDetailDTO?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
