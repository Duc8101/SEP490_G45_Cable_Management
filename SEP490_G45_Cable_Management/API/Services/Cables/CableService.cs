using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.Const;
using Common.DTO.CableDTO;
using Common.Entity;
using Common.Pagination;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Cables
{
    public class CableService : BaseService, ICableService
    {
        private readonly DAOCable daoCable = new DAOCable();

        public CableService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseBase<Pagination<CableListDTO>?>> ListPaged(string? filter, int? WarehouseID, bool isExportedToUse, int page)
        {
            try
            {
                List<Cable> list = await daoCable.getListPaged(filter, WarehouseID, isExportedToUse, page);
                List<CableListDTO> DTOs = _mapper.Map<List<CableListDTO>>(list);
                int RowCount = await daoCable.getRowCount(filter, WarehouseID, isExportedToUse);
                int sum = await daoCable.getSum(filter, WarehouseID, isExportedToUse);
                Pagination<CableListDTO> result = new Pagination<CableListDTO>(page, RowCount, PageSizeConst.MAX_CABLE_LIST_IN_PAGE, DTOs, sum);
                return new ResponseBase<Pagination<CableListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<Pagination<CableListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Create(CableCreateUpdateDTO DTO, Guid CreatorID)
        {
            if (DTO.StartPoint < 0 || DTO.EndPoint < 0 || DTO.StartPoint >= DTO.EndPoint)
            {
                return new ResponseBase<bool>(false, "Chỉ số đầu hoặc chỉ số cuối không hợp lệ", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if cable exist
                if (await daoCable.isExist(DTO))
                {
                    return new ResponseBase<bool>(false, "Cáp đã tồn tại trong hệ thống", (int)HttpStatusCode.Conflict);
                }
                Cable cable = _mapper.Map<Cable>(DTO);
                cable.CableId = Guid.NewGuid();
                cable.CreatorId = CreatorID;
                cable.CreatedAt = DateTime.Now;
                cable.UpdateAt = DateTime.Now;
                cable.IsDeleted = false;
                cable.IsExportedToUse = false;
                cable.IsInRequest = false;
                await daoCable.CreateCable(cable);
                return new ResponseBase<bool>(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Update(Guid CableID, CableCreateUpdateDTO DTO)
        {
            try
            {
                Cable? cable = await daoCable.getCable(CableID);
                // if not found
                if (cable == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy cáp", (int)HttpStatusCode.NotFound);
                }
                if (DTO.StartPoint < 0 || DTO.EndPoint < 0 || DTO.StartPoint >= DTO.EndPoint)
                {
                    return new ResponseBase<bool>(false, "Chỉ số đầu hoặc chỉ số cuối không hợp lệ", (int)HttpStatusCode.Conflict);
                }
                // if cable exist
                if (await daoCable.isExist(CableID, DTO))
                {
                    return new ResponseBase<bool>(false, "Cáp đã tồn tại trong hệ thống", (int)HttpStatusCode.Conflict);
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
                return new ResponseBase<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Delete(Guid CableID)
        {
            try
            {
                Cable? cable = await daoCable.getCable(CableID);
                // if not found
                if (cable == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy cáp", (int)HttpStatusCode.NotFound);
                }
                await daoCable.DeleteCable(cable);
                return new ResponseBase<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<List<CableListDTO>?>> ListAll(int? WarehouseID)
        {
            try
            {
                List<Cable> list = await daoCable.getListAll(WarehouseID);
                List<CableListDTO> result = _mapper.Map<List<CableListDTO>>(list);
                return new ResponseBase<List<CableListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<CableListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
