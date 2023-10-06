using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.OtherMaterialsDTO;
using DataAccess.Entity;
using DataAccess.Model.DAO;
using System.Net;

namespace API.Services
{
    public class OtherMaterialsService
    {
        private readonly DAOOtherMaterial daoOtherMaterials = new DAOOtherMaterial();
        private readonly DAOOtherMaterialsCategory daoCategory = new DAOOtherMaterialsCategory();
        private readonly DAOSupplier daoSupplier = new DAOSupplier();
        private readonly DAOWarehouse daoWarehouse = new DAOWarehouse();
        private async Task<List<OtherMaterialsListDTO>> getList(string? filter, int page)
        {
            List<OtherMaterial> list = await daoOtherMaterials.getList(filter, page);
            List<OtherMaterialsListDTO> result = new List<OtherMaterialsListDTO>();
            foreach (OtherMaterial item in list)
            {
                OtherMaterialsCategory? category = await daoCategory.getOtherMaterialsCategory(item.OtherMaterialsCategoryId);
                Supplier? supplier = item.SupplierId == null ? null : await daoSupplier.getSupplierIncludeDeleted(item.SupplierId.Value);
                Warehouse? ware = item.WarehouseId == null ? null : await daoWarehouse.getWarehouseIncludeDeleted(item.WarehouseId.Value);
                if (category != null && supplier != null && ware != null)
                {
                    OtherMaterialsListDTO DTO = new OtherMaterialsListDTO()
                    {
                        OtherMaterialsId = item.OtherMaterialsId,
                        Unit = item.Unit,
                        Quantity = item.Quantity,
                        Code = item.Code,
                        SupplierName = supplier.SupplierName,
                        WarehouseName = ware.WarehouseName,
                        OtherMaterialsCategoryName = category.OtherMaterialsCategoryName
                    };
                    result.Add(DTO);
                }
            }
            return result;
        }
        private async Task<bool> isSuccessfulGetList(string? filter, int page)
        {
            List<OtherMaterial> list = await daoOtherMaterials.getList(filter, page);
            List<OtherMaterialsListDTO> result = await getList(filter, page);
            return list.Count == result.Count;
        }
        public async Task<ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>> List(string? filter, int page)
        {
            // if get list successful
            if(await isSuccessfulGetList(filter, page))
            {
                List<OtherMaterialsListDTO> list = await getList(filter, page);
                int RowCount = await daoOtherMaterials.getRowCount(filter);
                int sum = await daoOtherMaterials.getSum(filter);
                PagedResultDTO<OtherMaterialsListDTO> result = new PagedResultDTO<OtherMaterialsListDTO>(page, RowCount, PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE, list, sum);
                return new ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>(result, "");
            }
            return new ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>(null, "Có lỗi khi lấy dữ liệu", (int) HttpStatusCode.NotAcceptable);
        }

        public async Task<ResponseDTO<bool>> Create(OtherMaterialsCreateUpdateDTO DTO)
        {
            if(DTO.Code == null || DTO.Code.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Mã hàng không được để trống", (int) HttpStatusCode.NotAcceptable);
            }

            if (DTO.Unit == null || DTO.Unit.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Đơn vị không được để trống", (int)HttpStatusCode.NotAcceptable);
            }

            if (DTO.Quantity == null)
            {
                return new ResponseDTO<bool>(false, "Số lượng không được để trống", (int) HttpStatusCode.NotAcceptable);
            }

            if (DTO.Status == null || DTO.Status.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Trạng thái không được để trống", (int) HttpStatusCode.NotAcceptable);
            }

            if(DTO.WarehouseId == null)
            {
                return new ResponseDTO<bool>(false, "Kho không được để trống", (int) HttpStatusCode.NotAcceptable);
            }

            OtherMaterial? material = await daoOtherMaterials.getOtherMaterial(DTO);
            int number;
            // if not exist
            if(material == null)
            {
                material = new OtherMaterial()
                {
                    Unit = DTO.Unit.Trim(),
                    Quantity = DTO.Quantity,
                    Code = DTO.Code.Trim(),
                    SupplierId = DTO.SupplierId,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false,
                    WarehouseId = DTO.WarehouseId,
                    Status = DTO.Status.Trim(),
                    OtherMaterialsCategoryId = DTO.OtherMaterialsCategoryId
                };
                number = await daoOtherMaterials.CreateMaterial(material);
            }
            else
            {
                material.Quantity = material.Quantity + DTO.Quantity;
                material.UpdateAt = DateTime.Now;
                number = await daoOtherMaterials.UpdateMaterial(material);
            }
            if(number > 0)
            {
                return new ResponseDTO<bool>(true, "Tạo thành công");
            }
            return new ResponseDTO<bool>(false, "Tạo thất bại", (int) HttpStatusCode.Conflict);
        }

        public async Task<ResponseDTO<bool>> Update(int OtherMaterialsID, OtherMaterialsCreateUpdateDTO DTO)
        {
            OtherMaterial? material = await daoOtherMaterials.getOtherMaterial(OtherMaterialsID);
            // if not found
            if (material == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy vật liệu", (int) HttpStatusCode.NotFound);
            }

            if (DTO.Code == null || DTO.Code.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Mã hàng không được để trống", (int)HttpStatusCode.NotAcceptable);
            }

            if (DTO.Unit == null || DTO.Unit.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Đơn vị không được để trống", (int)HttpStatusCode.NotAcceptable);
            }

            if (DTO.Quantity == null)
            {
                return new ResponseDTO<bool>(false, "Số lượng không được để trống", (int)HttpStatusCode.NotAcceptable);
            }

            if (DTO.Status == null || DTO.Status.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Trạng thái không được để trống", (int)HttpStatusCode.NotAcceptable);
            }

            if (DTO.WarehouseId == null)
            {
                return new ResponseDTO<bool>(false, "Kho không được để trống", (int)HttpStatusCode.NotAcceptable);
            }
            // if exist
            if (await daoOtherMaterials.isExist(DTO))
            {
                return new ResponseDTO<bool>(false, "Vật liệu đã tồn tại", (int) HttpStatusCode.NotAcceptable);
            }
            material.Unit = DTO.Unit.Trim();
            material.Quantity = DTO.Quantity;
            material.Code = DTO.Code.Trim();
            material.SupplierId = DTO.SupplierId;
            material.WarehouseId = DTO.WarehouseId;
            material.Status = DTO.Status.Trim();
            material.OtherMaterialsCategoryId = DTO.OtherMaterialsCategoryId;
            material.UpdateAt = DateTime.Now;
            int number = await daoOtherMaterials.UpdateMaterial(material);
            // if update successful
            if (number > 0)
            {
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            return new ResponseDTO<bool>(false, "Chỉnh sửa thất bại", (int) HttpStatusCode.Conflict);
        }

        public async Task<ResponseDTO<bool>> Delete(int OtherMaterialsID)
        {
            OtherMaterial? material = await daoOtherMaterials.getOtherMaterial(OtherMaterialsID);
            // if not found
            if (material == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy vật liệu", (int) HttpStatusCode.NotFound);
            }
            int number = await daoOtherMaterials.DeleteMaterial(material);
            // if delete successful
            if (number > 0)
            {
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            return new ResponseDTO<bool>(false, "Xóa thất bại", (int) HttpStatusCode.Conflict);
        }
    }
}
