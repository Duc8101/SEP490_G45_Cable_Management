using DataAccess.DTO.CableDTO;
using DataAccess.DTO;
using API.Model.DAO;
using DataAccess.Entity;
using DataAccess.Const;
using System.Net;

namespace API.Services
{
    public class CableService
    {
        private readonly DAOCable daoCable = new DAOCable();
        private async Task<List<CableListDTO>> getList(string? filter, int? WarehouseID, bool isExportedToUse, int page)
        {
            List<Cable> list = await daoCable.getList(filter, WarehouseID, isExportedToUse, page);
            List<CableListDTO> result = new List<CableListDTO>();
            foreach (Cable item in list)
            {
                CableListDTO DTO = new CableListDTO()
                {
                    CableId = item.CableId,
                    WarehouseName = item.Warehouse == null ? null : item.Warehouse.WarehouseName,
                    SupplierName = item.Supplier == null ? null : item.Supplier.SupplierName,
                    StartPoint = item.StartPoint,
                    EndPoint = item.EndPoint,
                    Length = item.Length,
                    YearOfManufacture = item.YearOfManufacture,
                    Code = item.Code,
                    Status = item.Status,
                    CableCategoryName = item.CableCategory.CableCategoryName
                };
                result.Add(DTO);
            }
            return result;
        }
        public async Task<PagedResultDTO<CableListDTO>> List(string? filter, int? WarehouseID, bool isExportedToUse, int page)
        {
            List<CableListDTO> list = await getList(filter, WarehouseID, isExportedToUse, page);
            int RowCount = await daoCable.getRowCount(filter, WarehouseID, isExportedToUse);
            int sum = await daoCable.getSum(filter, WarehouseID, isExportedToUse);
            return new PagedResultDTO<CableListDTO>(page, RowCount, PageSizeConst.MAX_CABLE_LIST_IN_PAGE ,list,sum);
        }

        public async Task<ResponseDTO<bool>> Create(CableCreateUpdateDTO DTO, Guid CreatorID)
        {
            // if cable exist
            if(await daoCable.isExist(DTO))
            {
                return new ResponseDTO<bool>(false, "Cáp đã tồn tại trong hệ thống", (int) HttpStatusCode.NotAcceptable);
            }
            Cable cable = new Cable()
            {
                CableId = Guid.NewGuid(),
                WarehouseId = DTO.WarehouseId,
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
            int number = await daoCable.CreateCable(cable);
            // if create successful
            if(number > 0)
            {
                return new ResponseDTO<bool>(true, "Thêm thành công");
            }
            return new ResponseDTO<bool>(false, "Thêm thất bại", (int) HttpStatusCode.Conflict);
        }

        public async Task<ResponseDTO<bool>> Update(Guid CableID, CableCreateUpdateDTO DTO)
        {
            Cable? cable = await daoCable.getCable(CableID);
            // if not found
            if(cable == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy cáp", (int) HttpStatusCode.NotFound);
            }
            cable.WarehouseId = DTO.WarehouseId;
            cable.StartPoint = DTO.StartPoint;
            cable.EndPoint = DTO.EndPoint;
            cable.Length = DTO.EndPoint - DTO.StartPoint;
            cable.YearOfManufacture = DTO.YearOfManufacture;
            cable.Code = DTO.Code.Trim();
            cable.Status = DTO.Status.Trim();
            cable.CableCategoryId = DTO.CableCategoryId;
            cable.UpdateAt = DateTime.Now;
            int number = await daoCable.UpdateCable(cable);
            // if update successful
            if(number > 0)
            {
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            return new ResponseDTO<bool>(false, "Chỉnh sửa thất bại", (int) HttpStatusCode.Conflict); 
        }

        public async Task<ResponseDTO<bool>> Delete(Guid CableID)
        {
            Cable? cable = await daoCable.getCable(CableID);
            // if not found
            if (cable == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy cáp", (int) HttpStatusCode.NotFound);
            }
            int number = await daoCable.DeleteCable(cable);
            // if delete successful
            if (number > 0)
            {
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            return new ResponseDTO<bool>(false, "Xóa thất bại", (int) HttpStatusCode.Conflict);
        }
    }
}
