using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.OtherMaterialsDTO;
using DataAccess.Entity;
using System.Net;
using API.Model.DAO;
using API.Services.IService;
using AutoMapper;

namespace API.Services.Service
{
    public class OtherMaterialsService : BaseService, IOtherMaterialsService
    {
        private readonly DAOOtherMaterial daoOtherMaterials = new DAOOtherMaterial();

        public OtherMaterialsService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>> ListPaged(string? filter, int? WareHouseID, Guid? WareHouseKeeperID, int page)
        {
            try
            {
                List<OtherMaterial> list = await daoOtherMaterials.getListPaged(filter, WareHouseID, WareHouseKeeperID, page);
                List<OtherMaterialsListDTO> DTOs = _mapper.Map<List<OtherMaterialsListDTO>>(list);
                int RowCount = await daoOtherMaterials.getRowCount(filter, WareHouseID, WareHouseKeeperID);
                int sum = await daoOtherMaterials.getSum(filter, WareHouseID, WareHouseKeeperID);
                PagedResultDTO<OtherMaterialsListDTO> result = new PagedResultDTO<OtherMaterialsListDTO>(page, RowCount, PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE, DTOs, sum);
                return new ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>(result, "Size : " + list.Count);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Create(OtherMaterialsCreateUpdateDTO DTO)
        {
            if (DTO.Code == null || DTO.Code.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Mã hàng không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.Unit.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Đơn vị không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.Status.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Trạng thái không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.WarehouseId == null)
            {
                return new ResponseDTO<bool>(false, "Kho không được để trống", (int)HttpStatusCode.Conflict);
            }

            if(DTO.Quantity < 0)
            {
                return new ResponseDTO<bool>(false, "Số lượng không hợp lệ", (int)HttpStatusCode.Conflict);
            }

            try
            {
                OtherMaterial? material = await daoOtherMaterials.getOtherMaterial(DTO);
                // if not exist
                if (material == null)
                {
                    material = _mapper.Map<OtherMaterial>(DTO);
                    material.Code = DTO.Code.Trim();
                    material.SupplierId = 1;
                    material.CreatedAt = DateTime.Now;
                    material.UpdateAt = DateTime.Now;
                    material.IsDeleted = false;
                    await daoOtherMaterials.CreateMaterial(material);
                }
                else
                {
                    material.Quantity = material.Quantity + DTO.Quantity;
                    material.UpdateAt = DateTime.Now;
                    await daoOtherMaterials.UpdateMaterial(material);
                }
                return new ResponseDTO<bool>(true, "Tạo thành công " + material.OtherMaterialsId);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Update(int OtherMaterialsID, OtherMaterialsCreateUpdateDTO DTO)
        {
            try
            {
                OtherMaterial? material = await daoOtherMaterials.getOtherMaterial(OtherMaterialsID);
                // if not found
                if (material == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy vật liệu", (int)HttpStatusCode.NotFound);
                }

                if (DTO.Code == null || DTO.Code.Trim().Length == 0)
                {
                    return new ResponseDTO<bool>(false, "Mã hàng không được để trống", (int)HttpStatusCode.Conflict);
                }

                if (DTO.Unit.Trim().Length == 0)
                {
                    return new ResponseDTO<bool>(false, "Đơn vị không được để trống", (int)HttpStatusCode.Conflict);
                }

                if (DTO.Status.Trim().Length == 0)
                {
                    return new ResponseDTO<bool>(false, "Trạng thái không được để trống", (int)HttpStatusCode.Conflict);
                }

                if (DTO.WarehouseId == null)
                {
                    return new ResponseDTO<bool>(false, "Kho không được để trống", (int)HttpStatusCode.Conflict);
                }

                if (DTO.Quantity < 0)
                {
                    return new ResponseDTO<bool>(false, "Số lượng không hợp lệ", (int)HttpStatusCode.Conflict);
                }
                // if exist
                if (await daoOtherMaterials.isExist(OtherMaterialsID, DTO))
                {
                    return new ResponseDTO<bool>(false, "Vật liệu đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                material.Unit = DTO.Unit.Trim();
                material.Quantity = DTO.Quantity;
                material.Code = DTO.Code.Trim();
                material.WarehouseId = DTO.WarehouseId;
                material.Status = DTO.Status.Trim();
                material.OtherMaterialsCategoryId = DTO.OtherMaterialsCategoryId;
                material.UpdateAt = DateTime.Now;
                await daoOtherMaterials.UpdateMaterial(material);
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Delete(int OtherMaterialsID)
        {
            try
            {
                OtherMaterial? material = await daoOtherMaterials.getOtherMaterial(OtherMaterialsID);
                // if not found
                if (material == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy vật liệu", (int)HttpStatusCode.NotFound);
                }
                await daoOtherMaterials.DeleteMaterial(material);
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<List<OtherMaterialsListDTO>?>> ListAll(int? WareHouseID)
        {
            try
            {
                List<OtherMaterial> list = await daoOtherMaterials.getListAll(WareHouseID);
                List<OtherMaterialsListDTO> data = _mapper.Map<List<OtherMaterialsListDTO>>(list);
                return new ResponseDTO<List<OtherMaterialsListDTO>?>(data, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<OtherMaterialsListDTO>?>(null, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }
    }
}
