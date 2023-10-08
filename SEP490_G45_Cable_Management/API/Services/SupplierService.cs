using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.SupplierDTO;
using DataAccess.Entity;
using API.Model.DAO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public async Task<PagedResultDTO<SupplierListDTO>> List(string? name, int page)
        {
            List<SupplierListDTO> list = await getList(name, page);
            int RowCount = await daoSupplier.getRowCount(name);
            PagedResultDTO<SupplierListDTO> pageResult = new PagedResultDTO<SupplierListDTO>(page, RowCount,PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE , list);
            return pageResult;
        }
        public async Task<ResponseDTO<bool>> Create(SupplierCreateUpdateDTO DTO, string CreatorID)
        {
            // if supplier already exist
            if (await daoSupplier.isExist(DTO.SupplierName.Trim()))
            {
                return new ResponseDTO<bool>(false, "Nhà cung cấp đã tồn tại", (int) HttpStatusCode.NotAcceptable);
            }
            Supplier supplier = new Supplier()
            {
                SupplierName = DTO.SupplierName.Trim(),
                Country = DTO.Country == null ? null : DTO.Country.Trim(),
                SupplierDescription = DTO.SupplierDescription == null || DTO.SupplierDescription.Trim().Length == 0 ? null : DTO.SupplierDescription.Trim(),
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                CreatorId = Guid.Parse(CreatorID)
            };
            // create supplier
            int number = await daoSupplier.CreateSupplier(supplier);
            // if create successful
            if(number > 0)
            {
                return new ResponseDTO<bool>(true, "Thêm thành công");
            }
            return new ResponseDTO<bool>(false,"Thêm thất bại", (int) HttpStatusCode.Conflict);
        }

        public async Task<ResponseDTO<bool>> Update(int SupplierID, SupplierCreateUpdateDTO DTO)
        {
            Supplier? supplier = await daoSupplier.getSupplier(SupplierID);
            // if supplier id not exist
            if (supplier == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy nhà cung cấp", (int) HttpStatusCode.NotFound);
            }
            // if supplier already exist
            if (await daoSupplier.isExist(SupplierID, DTO.SupplierName))
            {
                return new ResponseDTO<bool>(false, "Nhà cung cấp đã tồn tại", (int)HttpStatusCode.NotAcceptable);
            }
            supplier.SupplierName = DTO.SupplierName.Trim();
            supplier.Country = DTO.Country == null || DTO.Country.Trim().Length == 0 ? null : DTO.Country.Trim();
            supplier.SupplierDescription = DTO.SupplierDescription == null || DTO.SupplierDescription.Trim().Length == 0 ? null : DTO.SupplierDescription.Trim();
            supplier.UpdateAt = DateTime.Now;
            // update supplier
            int number = await daoSupplier.UpdateSupplier(supplier);
            // if update successful
            if (number > 0)
            {
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            return new ResponseDTO<bool>(false, "Chỉnh sửa thất bại", (int) HttpStatusCode.Conflict);
        }

        public async Task<ResponseDTO<bool>> Delete(int SupplierID)
        {
            Supplier? supplier = await daoSupplier.getSupplier(SupplierID); 
            // if supplier not exist
            if(supplier == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy nhà cung cấp", (int) HttpStatusCode.NotFound);
            }
            // delete supplier
            int number = await daoSupplier.DeleteSupplier(supplier); 
            // if delete successful
            if (number > 0) {
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            return new ResponseDTO<bool>(false, "Xóa thất bại", (int) HttpStatusCode.Conflict);
        }
    }
}
