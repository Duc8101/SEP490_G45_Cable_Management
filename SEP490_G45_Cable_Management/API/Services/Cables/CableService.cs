using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.DTO.CableDTO;
using Common.Entity;
using Common.Paginations;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Cables
{
    public class CableService : BaseService, ICableService
    {
        private readonly DAOCable _daoCable;
        public CableService(IMapper mapper, DAOCable daoCable) : base(mapper)
        {
            _daoCable = daoCable;
        }

        public ResponseBase Create(CableCreateUpdateDTO DTO, Guid creatorId)
        {
            if (DTO.StartPoint < 0 || DTO.EndPoint < 0 || DTO.StartPoint >= DTO.EndPoint)
            {
                return new ResponseBase(false, "Chỉ số đầu hoặc chỉ số cuối không hợp lệ", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if cable exist
                if (_daoCable.isExist(DTO))
                {
                    return new ResponseBase(false, "Cáp đã tồn tại trong hệ thống", (int)HttpStatusCode.Conflict);
                }
                Cable cable = _mapper.Map<Cable>(DTO);
                cable.CableId = Guid.NewGuid();
                cable.CreatorId = creatorId;
                cable.CreatedAt = DateTime.Now;
                cable.UpdateAt = DateTime.Now;
                cable.IsDeleted = false;
                cable.IsExportedToUse = false;
                cable.IsInRequest = false;
                _daoCable.CreateCable(cable);
                return new ResponseBase(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Delete(Guid cableId)
        {
            try
            {
                Cable? cable = _daoCable.getCable(cableId);
                // if not found
                if (cable == null)
                {
                    return new ResponseBase(false, "Không tìm thấy cáp", (int)HttpStatusCode.NotFound);
                }
                _daoCable.DeleteCable(cable);
                return new ResponseBase(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListAll(int? warehouseId)
        {
            try
            {
                List<Cable> list = _daoCable.getListCable(warehouseId);
                List<CableListDTO> data = _mapper.Map<List<CableListDTO>>(list);
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListPaged(string? filter, int? warehouseId, bool isExportedToUse, int page)
        {
            try
            {
                List<Cable> list = _daoCable.getListCable(filter, warehouseId, isExportedToUse, page);
                List<CableListDTO> DTOs = _mapper.Map<List<CableListDTO>>(list);
                int rowCount = _daoCable.getRowCount(filter, warehouseId, isExportedToUse);
                int sum = _daoCable.getSum(filter, warehouseId, isExportedToUse);
                Pagination<CableListDTO> result = new Pagination<CableListDTO>()
                {
                    CurrentPage = page,
                    RowCount = rowCount,
                    Data = DTOs,
                    Sum = sum
                };
                return new ResponseBase(result);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Update(Guid cableId, CableCreateUpdateDTO DTO)
        {
            try
            {
                Cable? cable = _daoCable.getCable(cableId);
                // if not found
                if (cable == null)
                {
                    return new ResponseBase(false, "Không tìm thấy cáp", (int)HttpStatusCode.NotFound);
                }
                if (DTO.StartPoint < 0 || DTO.EndPoint < 0 || DTO.StartPoint >= DTO.EndPoint)
                {
                    return new ResponseBase(false, "Chỉ số đầu hoặc chỉ số cuối không hợp lệ", (int)HttpStatusCode.Conflict);
                }
                // if cable exist
                if (_daoCable.isExist(cableId, DTO))
                {
                    return new ResponseBase(false, "Cáp đã tồn tại trong hệ thống", (int)HttpStatusCode.Conflict);
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
                _daoCable.UpdateCable(cable);
                return new ResponseBase(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
