using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.Const;
using Common.DTO.OtherMaterialsDTO;
using Common.Entity;
using Common.Pagination;
using DataAccess.DAO;
using System.Net;

namespace API.Services.OtherMaterials
{
    public class OtherMaterialsService : BaseService, IOtherMaterialsService
    {
        private readonly DAOOtherMaterial daoOtherMaterials = new DAOOtherMaterial();

        public OtherMaterialsService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseBase<Pagination<OtherMaterialsListDTO>?>> ListPaged(string? filter, int? WareHouseID, Guid? WareHouseKeeperID, int page)
        {
            try
            {
                List<OtherMaterial> list = await daoOtherMaterials.getListPaged(filter, WareHouseID, WareHouseKeeperID, page);
                List<OtherMaterialsListDTO> DTOs = _mapper.Map<List<OtherMaterialsListDTO>>(list);
                int RowCount = await daoOtherMaterials.getRowCount(filter, WareHouseID, WareHouseKeeperID);
                int sum = await daoOtherMaterials.getSum(filter, WareHouseID, WareHouseKeeperID);
                Pagination<OtherMaterialsListDTO> result = new Pagination<OtherMaterialsListDTO>(page, RowCount, PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE, DTOs, sum);
                return new ResponseBase<Pagination<OtherMaterialsListDTO>?>(result, "Size : " + list.Count);
            }
            catch (Exception ex)
            {
                return new ResponseBase<Pagination<OtherMaterialsListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Create(OtherMaterialsCreateUpdateDTO DTO)
        {
            if (DTO.Code == null || DTO.Code.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Mã hàng không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.Unit.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Đơn vị không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.Status.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Trạng thái không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.WarehouseId == null)
            {
                return new ResponseBase<bool>(false, "Kho không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.Quantity < 0)
            {
                return new ResponseBase<bool>(false, "Số lượng không hợp lệ", (int)HttpStatusCode.Conflict);
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
                return new ResponseBase<bool>(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Update(int OtherMaterialsID, OtherMaterialsCreateUpdateDTO DTO)
        {
            try
            {
                OtherMaterial? material = await daoOtherMaterials.getOtherMaterial(OtherMaterialsID);
                // if not found
                if (material == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy vật liệu", (int)HttpStatusCode.NotFound);
                }

                if (DTO.Code == null || DTO.Code.Trim().Length == 0)
                {
                    return new ResponseBase<bool>(false, "Mã hàng không được để trống", (int)HttpStatusCode.Conflict);
                }

                if (DTO.Unit.Trim().Length == 0)
                {
                    return new ResponseBase<bool>(false, "Đơn vị không được để trống", (int)HttpStatusCode.Conflict);
                }

                if (DTO.Status.Trim().Length == 0)
                {
                    return new ResponseBase<bool>(false, "Trạng thái không được để trống", (int)HttpStatusCode.Conflict);
                }

                if (DTO.WarehouseId == null)
                {
                    return new ResponseBase<bool>(false, "Kho không được để trống", (int)HttpStatusCode.Conflict);
                }

                if (DTO.Quantity < 0)
                {
                    return new ResponseBase<bool>(false, "Số lượng không hợp lệ", (int)HttpStatusCode.Conflict);
                }
                // if exist
                if (await daoOtherMaterials.isExist(OtherMaterialsID, DTO))
                {
                    return new ResponseBase<bool>(false, "Vật liệu đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                material.Unit = DTO.Unit.Trim();
                material.Quantity = DTO.Quantity;
                material.Code = DTO.Code.Trim();
                material.WarehouseId = DTO.WarehouseId;
                material.Status = DTO.Status.Trim();
                material.OtherMaterialsCategoryId = DTO.OtherMaterialsCategoryId;
                material.UpdateAt = DateTime.Now;
                await daoOtherMaterials.UpdateMaterial(material);
                return new ResponseBase<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Delete(int OtherMaterialsID)
        {
            try
            {
                OtherMaterial? material = await daoOtherMaterials.getOtherMaterial(OtherMaterialsID);
                // if not found
                if (material == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy vật liệu", (int)HttpStatusCode.NotFound);
                }
                await daoOtherMaterials.DeleteMaterial(material);
                return new ResponseBase<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<List<OtherMaterialsListDTO>?>> ListAll(int? WareHouseID)
        {
            try
            {
                List<OtherMaterial> list = await daoOtherMaterials.getListAll(WareHouseID);
                List<OtherMaterialsListDTO> data = _mapper.Map<List<OtherMaterialsListDTO>>(list);
                return new ResponseBase<List<OtherMaterialsListDTO>?>(data, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<OtherMaterialsListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
