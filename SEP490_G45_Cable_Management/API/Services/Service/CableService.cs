using DataAccess.DTO.CableDTO;
using DataAccess.DTO;
using DataAccess.Entity;
using DataAccess.Const;
using System.Net;
using API.Model.DAO;
using API.Services.IService;

namespace API.Services.Service
{
    public class CableService : ICableService
    {
        private readonly DAOCable daoCable = new DAOCable();
        private List<CableListDTO> getListDTO(List<Cable> list)
        {
            List<CableListDTO> result = new List<CableListDTO>();
            foreach (Cable item in list)
            {
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
            }
            return result;
        }
        private async Task<List<CableListDTO>> getListPaged(string? filter, int? WarehouseID, bool isExportedToUse, int page)
        {
            List<Cable> list = await daoCable.getListPaged(filter, WarehouseID, isExportedToUse, page);
            List<CableListDTO> result = getListDTO(list);
            return result;
        }
        public async Task<ResponseDTO<PagedResultDTO<CableListDTO>?>> ListPaged(string? filter, int? WarehouseID, bool isExportedToUse, int page)
        {
            try
            {
                List<CableListDTO> list = await getListPaged(filter, WarehouseID, isExportedToUse, page);
                int RowCount = await daoCable.getRowCount(filter, WarehouseID, isExportedToUse);
                int sum = await daoCable.getSum(filter, WarehouseID, isExportedToUse);
                PagedResultDTO<CableListDTO> result = new PagedResultDTO<CableListDTO>(page, RowCount, PageSizeConst.MAX_CABLE_LIST_IN_PAGE, list, sum);
                return new ResponseDTO<PagedResultDTO<CableListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<CableListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Create(CableCreateUpdateDTO DTO, Guid CreatorID)
        {
            if(DTO.StartPoint < 0 || DTO.EndPoint < 0 || DTO.StartPoint >= DTO.EndPoint)
            {
                return new ResponseDTO<bool>(false, "Chỉ số đầu hoặc chỉ số cuối không hợp lệ", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if cable exist
                if (await daoCable.isExist(DTO))
                {
                    return new ResponseDTO<bool>(false, "Cáp đã tồn tại trong hệ thống", (int)HttpStatusCode.Conflict);
                }
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
                    IsInRequest = false,
                };
                await daoCable.CreateCable(cable);
                return new ResponseDTO<bool>(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Update(Guid CableID, CableCreateUpdateDTO DTO)
        {
            try
            {
                Cable? cable = await daoCable.getCable(CableID);
                // if not found
                if (cable == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy cáp", (int)HttpStatusCode.NotFound);
                }
                if (DTO.StartPoint < 0 || DTO.EndPoint < 0 || DTO.StartPoint >= DTO.EndPoint)
                {
                    return new ResponseDTO<bool>(false, "Chỉ số đầu hoặc chỉ số cuối không hợp lệ", (int) HttpStatusCode.Conflict);
                }
                // if cable exist
                if (await daoCable.isExist(CableID, DTO))
                {
                    return new ResponseDTO<bool>(false, "Cáp đã tồn tại trong hệ thống", (int) HttpStatusCode.Conflict);
                }
                cable.WarehouseId = DTO.WarehouseId;
                cable.SupplierId = DTO.SupplierId;
                cable.StartPoint = DTO.StartPoint;
                cable.EndPoint = DTO.EndPoint;
                cable.Length = DTO.EndPoint - DTO.StartPoint;
                cable.YearOfManufacture = DTO.YearOfManufacture;
                cable.Code = DTO.Code.Trim();
                cable.Status = DTO.Status.Trim();
                cable.CableCategoryId = DTO.CableCategoryId;
                cable.UpdateAt = DateTime.Now;
                await daoCable.UpdateCable(cable);
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Delete(Guid CableID)
        {
            try
            {
                Cable? cable = await daoCable.getCable(CableID);
                // if not found
                if (cable == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy cáp", (int)HttpStatusCode.NotFound);
                }
                await daoCable.DeleteCable(cable);
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<List<CableListDTO>?>> ListAll(int? WarehouseID)
        {
            try
            {
                List<Cable> list = await daoCable.getListAll(WarehouseID);
                List<CableListDTO> result = getListDTO(list);
                return new ResponseDTO<List<CableListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<CableListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
