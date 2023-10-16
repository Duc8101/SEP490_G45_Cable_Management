using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.SupplierDTO;
using DataAccess.Entity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Net;
using API.Model.DAO;

namespace API.Services
{
    public class SupplierService
    {
        private readonly DAOSupplier daoSupplier = new DAOSupplier();
        private async Task<List<SupplierListDTO>> getList(string? name, int page)
        {
            List<Supplier> list = await daoSupplier.getList(name, page);
            List<SupplierListDTO> result = new List<SupplierListDTO>();
            foreach (Supplier supplier in list)
            {
                SupplierListDTO DTO = new SupplierListDTO()
                {
                    SupplierId = supplier.SupplierId,
                    SupplierName = supplier.SupplierName,
                    Country = supplier.Country,
                    SupplierDescription = supplier.SupplierDescription
                };
                result.Add(DTO);
            }
            return result;
        }
        public async Task<ResponseDTO<PagedResultDTO<SupplierListDTO>?>> List(string? name, int page)
        {
            try
            {
                List<SupplierListDTO> list = await getList(name, page);
                int RowCount = await daoSupplier.getRowCount(name);
                PagedResultDTO<SupplierListDTO> result = new PagedResultDTO<SupplierListDTO>(page, RowCount, PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE, list);
                return new ResponseDTO<PagedResultDTO<SupplierListDTO>?>(result, string.Empty);
            }
            catch(Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<SupplierListDTO>?>(null, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Create(SupplierCreateUpdateDTO DTO, string CreatorID)
        {
            if(DTO.SupplierName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên nhà cung cấp không được để trống", (int) HttpStatusCode.Conflict);
            }
            try
            {
                // if supplier already exist
                if (await daoSupplier.isExist(DTO.SupplierName.Trim()))
                {
                    return new ResponseDTO<bool>(false, "Nhà cung cấp đã tồn tại", (int) HttpStatusCode.Conflict);
                }
                Supplier supplier = new Supplier()
                {
                    SupplierName = DTO.SupplierName.Trim(),
                    Country = DTO.Country == null || DTO.Country.Trim().Length == 0 ? null : DTO.Country.Trim(),
                    SupplierDescription = DTO.SupplierDescription == null || DTO.SupplierDescription.Trim().Length == 0 ? null : DTO.SupplierDescription.Trim(),
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false,
                    CreatorId = Guid.Parse(CreatorID)
                };
                // create supplier
                await daoSupplier.CreateSupplier(supplier);
                return new ResponseDTO<bool>(true, "Thêm thành công");
            }
            catch(Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
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
                    return new ResponseDTO<bool>(false, "Tên nhà cung cấp không được để trống", (int) HttpStatusCode.Conflict);
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
            catch(Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
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
                    return new ResponseDTO<bool>(false, "Không tìm thấy nhà cung cấp", (int) HttpStatusCode.NotFound);
                }
                // delete supplier
                await daoSupplier.DeleteSupplier(supplier);
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            catch(Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }
    }
}
