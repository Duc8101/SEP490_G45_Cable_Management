using API.Model.DAO;
using API.Services.IService;
using AutoMapper;
using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.SupplierDTO;
using DataAccess.Entity;
using System.Net;

namespace API.Services.Service
{
    public class SupplierService : BaseService, ISupplierService
    {
        private readonly DAOSupplier daoSupplier = new DAOSupplier();

        public SupplierService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseDTO<PagedResultDTO<SupplierListDTO>?>> ListPaged(string? name, int page)
        {
            try
            {
                List<Supplier> list = await daoSupplier.getListPaged(name, page);
                List<SupplierListDTO> DTOs = _mapper.Map<List<SupplierListDTO>>(list);
                int RowCount = await daoSupplier.getRowCount(name);
                PagedResultDTO<SupplierListDTO> result = new PagedResultDTO<SupplierListDTO>(page, RowCount, PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE, DTOs);
                return new ResponseDTO<PagedResultDTO<SupplierListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<SupplierListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseDTO<List<SupplierListDTO>?>> ListAll()
        {
            try
            {
                List<Supplier> list = await daoSupplier.getListAll();
                List<SupplierListDTO> data = _mapper.Map<List<SupplierListDTO>>(list);
                return new ResponseDTO<List<SupplierListDTO>?>(data, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<SupplierListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Create(SupplierCreateUpdateDTO DTO, Guid CreatorID)
        {
            if (DTO.SupplierName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên nhà cung cấp không được để trống", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if supplier already exist
                if (await daoSupplier.isExist(DTO.SupplierName.Trim()))
                {
                    return new ResponseDTO<bool>(false, "Nhà cung cấp đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                Supplier supplier = _mapper.Map<Supplier>(DTO);
                supplier.CreatedAt = DateTime.Now;
                supplier.UpdateAt = DateTime.Now;
                supplier.IsDeleted = false;
                supplier.CreatorId = CreatorID;
                // create supplier
                await daoSupplier.CreateSupplier(supplier);
                return new ResponseDTO<bool>(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Update(int SupplierID, SupplierCreateUpdateDTO DTO)
        {
            try
            {
                Supplier? supplier = await daoSupplier.getSupplier(SupplierID);
                // if supplier id not exist
                if (supplier == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy nhà cung cấp", (int)HttpStatusCode.NotFound);
                }
                if (DTO.SupplierName.Trim().Length == 0)
                {
                    return new ResponseDTO<bool>(false, "Tên nhà cung cấp không được để trống", (int)HttpStatusCode.Conflict);
                }
                // if supplier already exist
                if (await daoSupplier.isExist(SupplierID, DTO.SupplierName))
                {
                    return new ResponseDTO<bool>(false, "Nhà cung cấp đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                supplier.SupplierName = DTO.SupplierName.Trim();
                supplier.Country = DTO.Country == null || DTO.Country.Trim().Length == 0 ? null : DTO.Country.Trim();
                supplier.SupplierDescription = DTO.SupplierDescription == null || DTO.SupplierDescription.Trim().Length == 0 ? null : DTO.SupplierDescription.Trim();
                supplier.UpdateAt = DateTime.Now;
                // update supplier
                await daoSupplier.UpdateSupplier(supplier);
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Delete(int SupplierID)
        {
            try
            {
                Supplier? supplier = await daoSupplier.getSupplier(SupplierID);
                // if supplier not exist
                if (supplier == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy nhà cung cấp", (int)HttpStatusCode.NotFound);
                }
                // delete supplier
                await daoSupplier.DeleteSupplier(supplier);
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
