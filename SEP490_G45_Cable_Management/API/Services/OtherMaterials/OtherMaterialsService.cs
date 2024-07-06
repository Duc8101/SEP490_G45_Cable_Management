using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.DTO.OtherMaterialsDTO;
using Common.Entity;
using Common.Paginations;
using DataAccess.DAO;
using System.Net;

namespace API.Services.OtherMaterials
{
    public class OtherMaterialsService : BaseService, IOtherMaterialsService
    {
        private readonly DAOOtherMaterial _daoOtherMaterial;

        public OtherMaterialsService(IMapper mapper, DAOOtherMaterial daoOtherMaterial) : base(mapper)
        {
            _daoOtherMaterial = daoOtherMaterial;
        }

        public ResponseBase Create(OtherMaterialsCreateUpdateDTO DTO)
        {
            if (DTO.Code == null || DTO.Code.Trim().Length == 0)
            {
                return new ResponseBase(false, "Mã hàng không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.Unit.Trim().Length == 0)
            {
                return new ResponseBase(false, "Đơn vị không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.Status.Trim().Length == 0)
            {
                return new ResponseBase(false, "Trạng thái không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.WarehouseId == null)
            {
                return new ResponseBase(false, "Kho không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.Quantity < 0)
            {
                return new ResponseBase(false, "Số lượng không hợp lệ", (int)HttpStatusCode.Conflict);
            }

            try
            {
                OtherMaterial? material = _daoOtherMaterial.getOtherMaterial(DTO);
                // if not exist
                if (material == null)
                {
                    material = _mapper.Map<OtherMaterial>(DTO);
                    material.Code = DTO.Code.Trim();
                    material.SupplierId = 1;
                    material.CreatedAt = DateTime.Now;
                    material.UpdateAt = DateTime.Now;
                    material.IsDeleted = false;
                    _daoOtherMaterial.CreateOtherMaterial(material);
                }
                else
                {
                    material.Quantity = material.Quantity + DTO.Quantity;
                    material.UpdateAt = DateTime.Now;
                    _daoOtherMaterial.UpdateOtherMaterial(material);
                }
                return new ResponseBase(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Delete(int otherMaterialsId)
        {
            try
            {
                OtherMaterial? material = _daoOtherMaterial.getOtherMaterial(otherMaterialsId);
                // if not found
                if (material == null)
                {
                    return new ResponseBase(false, "Không tìm thấy vật liệu", (int)HttpStatusCode.NotFound);
                }
                _daoOtherMaterial.DeleteOtherMaterial(material);
                return new ResponseBase(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListAll(int? wareHouseId)
        {
            try
            {
                List<OtherMaterial> list = _daoOtherMaterial.getListOtherMaterial(wareHouseId);
                List<OtherMaterialsListDTO> data = _mapper.Map<List<OtherMaterialsListDTO>>(list);
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListPaged(string? filter, int? wareHouseId, Guid? wareHouseKeeperId, int page)
        {
            try
            {
                List<OtherMaterial> list = _daoOtherMaterial.getListOtherMaterial(filter, wareHouseId, wareHouseKeeperId, page);
                List<OtherMaterialsListDTO> DTOs = _mapper.Map<List<OtherMaterialsListDTO>>(list);
                int rowCount = _daoOtherMaterial.getRowCount(filter, wareHouseId, wareHouseKeeperId);
                int sum = _daoOtherMaterial.getSum(filter, wareHouseId, wareHouseKeeperId);
                Pagination<OtherMaterialsListDTO> data = new Pagination<OtherMaterialsListDTO>()
                {
                    CurrentPage = page,
                    RowCount = rowCount,
                    List = DTOs,
                    Sum = sum
                };
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Update(int otherMaterialsId, OtherMaterialsCreateUpdateDTO DTO)
        {
            try
            {
                OtherMaterial? material = _daoOtherMaterial.getOtherMaterial(otherMaterialsId);
                // if not found
                if (material == null)
                {
                    return new ResponseBase(false, "Không tìm thấy vật liệu", (int)HttpStatusCode.NotFound);
                }

                if (DTO.Code == null || DTO.Code.Trim().Length == 0)
                {
                    return new ResponseBase(false, "Mã hàng không được để trống", (int)HttpStatusCode.Conflict);
                }

                if (DTO.Unit.Trim().Length == 0)
                {
                    return new ResponseBase(false, "Đơn vị không được để trống", (int)HttpStatusCode.Conflict);
                }

                if (DTO.Status.Trim().Length == 0)
                {
                    return new ResponseBase(false, "Trạng thái không được để trống", (int)HttpStatusCode.Conflict);
                }

                if (DTO.WarehouseId == null)
                {
                    return new ResponseBase(false, "Kho không được để trống", (int)HttpStatusCode.Conflict);
                }

                if (DTO.Quantity < 0)
                {
                    return new ResponseBase(false, "Số lượng không hợp lệ", (int)HttpStatusCode.Conflict);
                }
                // if exist
                if (_daoOtherMaterial.isExist(otherMaterialsId, DTO))
                {
                    return new ResponseBase(false, "Vật liệu đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                material.Unit = DTO.Unit.Trim();
                material.Quantity = DTO.Quantity;
                material.Code = DTO.Code.Trim();
                material.WarehouseId = DTO.WarehouseId;
                material.Status = DTO.Status.Trim();
                material.OtherMaterialsCategoryId = DTO.OtherMaterialsCategoryId;
                material.UpdateAt = DateTime.Now;
                _daoOtherMaterial.UpdateOtherMaterial(material);
                return new ResponseBase(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
