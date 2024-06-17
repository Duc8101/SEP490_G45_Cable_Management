using API.Services.IService;
using AutoMapper;
using Common.Base;
using Common.Const;
using Common.DTO.SupplierDTO;
using Common.Entity;
using Common.Pagination;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Service
{
    public class SupplierService : BaseService, ISupplierService
    {
        private readonly DAOSupplier daoSupplier = new DAOSupplier();

        public SupplierService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseBase<Pagination<SupplierListDTO>?>> ListPaged(string? name, int page)
        {
            try
            {
                List<Supplier> list = await daoSupplier.getListPaged(name, page);
                List<SupplierListDTO> DTOs = _mapper.Map<List<SupplierListDTO>>(list);
                int RowCount = await daoSupplier.getRowCount(name);
                Pagination<SupplierListDTO> result = new Pagination<SupplierListDTO>(page, RowCount, PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE, DTOs);
                return new ResponseBase<Pagination<SupplierListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<Pagination<SupplierListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseBase<List<SupplierListDTO>?>> ListAll()
        {
            try
            {
                List<Supplier> list = await daoSupplier.getListAll();
                List<SupplierListDTO> data = _mapper.Map<List<SupplierListDTO>>(list);
                return new ResponseBase<List<SupplierListDTO>?>(data, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<SupplierListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Create(SupplierCreateUpdateDTO DTO, Guid CreatorID)
        {
            if (DTO.SupplierName.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Tên nhà cung cấp không được để trống", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if supplier already exist
                if (await daoSupplier.isExist(DTO.SupplierName.Trim()))
                {
                    return new ResponseBase<bool>(false, "Nhà cung cấp đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                Supplier supplier = _mapper.Map<Supplier>(DTO);
                supplier.CreatedAt = DateTime.Now;
                supplier.UpdateAt = DateTime.Now;
                supplier.IsDeleted = false;
                supplier.CreatorId = CreatorID;
                // create supplier
                await daoSupplier.CreateSupplier(supplier);
                return new ResponseBase<bool>(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Update(int SupplierID, SupplierCreateUpdateDTO DTO)
        {
            try
            {
                Supplier? supplier = await daoSupplier.getSupplier(SupplierID);
                // if supplier id not exist
                if (supplier == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy nhà cung cấp", (int)HttpStatusCode.NotFound);
                }
                if (DTO.SupplierName.Trim().Length == 0)
                {
                    return new ResponseBase<bool>(false, "Tên nhà cung cấp không được để trống", (int)HttpStatusCode.Conflict);
                }
                // if supplier already exist
                if (await daoSupplier.isExist(SupplierID, DTO.SupplierName))
                {
                    return new ResponseBase<bool>(false, "Nhà cung cấp đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                supplier.SupplierName = DTO.SupplierName.Trim();
                supplier.Country = DTO.Country == null || DTO.Country.Trim().Length == 0 ? null : DTO.Country.Trim();
                supplier.SupplierDescription = DTO.SupplierDescription == null || DTO.SupplierDescription.Trim().Length == 0 ? null : DTO.SupplierDescription.Trim();
                supplier.UpdateAt = DateTime.Now;
                // update supplier
                await daoSupplier.UpdateSupplier(supplier);
                return new ResponseBase<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Delete(int SupplierID)
        {
            try
            {
                Supplier? supplier = await daoSupplier.getSupplier(SupplierID);
                // if supplier not exist
                if (supplier == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy nhà cung cấp", (int)HttpStatusCode.NotFound);
                }
                // delete supplier
                await daoSupplier.DeleteSupplier(supplier);
                return new ResponseBase<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
