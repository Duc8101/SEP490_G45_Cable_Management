using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.SupplierDTO;
using DataAccess.Entity;
using DataAccess.Model.DAO;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API.Services
{
    public class SupplierService
    {
        private readonly DAOSupplier daoSupplier = new DAOSupplier();
        public async Task<ResponseDTO<bool>> Create(SupplierCreateUpdateDTO DTO, string CreatorID)
        {
            try
            {
                Supplier? supplier = await daoSupplier.getSupplier(DTO.SupplierName);
                // if supplier not exist
                if (supplier == null)
                {
                    supplier = new Supplier()
                    {
                        SupplierName = DTO.SupplierName.Trim(),
                        Country = DTO.Country == null ? null : DTO.Country.Trim(),
                        SupplierDescription = DTO.SupplierDescription == null || DTO.SupplierDescription.Trim().Length == 0 ? null : DTO.SupplierDescription.Trim(),
                        CreatedAt = DateTime.Now,
                        UpdateAt = null,
                        IsDeleted = false,
                        CreatorId = Guid.Parse(CreatorID)
                    };
                    // create supplier
                    int number = await daoSupplier.CreateSupplier(supplier);
                    // if create successful
                    if(number > 0)
                    {
                        return new ResponseDTO<bool>(true);
                    }
                    return new ResponseDTO<bool>(false,"Tạo thất bại", (int) HttpStatusCode.Conflict);
                }
                throw new ApplicationException("Nhà cung cấp này đã tồn tại");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private List<SupplierListDTO> getList(List<Supplier> list)
        {
            List<SupplierListDTO> result = new List<SupplierListDTO>();
            foreach (Supplier supplier in list)
            {
                SupplierListDTO DTO = new SupplierListDTO()
                {
                    SupplierId = supplier.SupplierId,
                    SupplierName = supplier.SupplierName,
                    Country = supplier.Country,
                    SupplierDescription = supplier.SupplierDescription,
                    CreatedAt = supplier.CreatedAt,
                    UpdateAt = supplier.UpdateAt
                };
                result.Add(DTO);
            }
            return result;
        }

        public async Task<PagedResultDTO<SupplierListDTO>> List(string? filter, int page)
        {
            List<Supplier> list = await daoSupplier.getList(filter, page);
            List<SupplierListDTO> result = getList(list);
            PagedResultDTO<SupplierListDTO> pageResult = new PagedResultDTO<SupplierListDTO>(page, PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE, result, 0);
            return pageResult;
        }

        public async Task<ResponseDTO<bool>> Update(int SupplierID, SupplierCreateUpdateDTO DTO)
        {
            Supplier? supplier = await daoSupplier.getSupplier(SupplierID);
            // if supplier id not exist
            if (supplier == null)
            {
                throw new ApplicationException(MessageConst.SUPPLIER_NOT_FOUND);
            }
            // if supplier already exist
            if( await daoSupplier.isSupplierExist(SupplierID, DTO.SupplierName))
            {
                throw new ApplicationException("Nhà cung cấp đã tồn tại");
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
                return new ResponseDTO<bool>(true);
            }
            return new ResponseDTO<bool>(false, "Chỉnh sửa thất bại", (int) HttpStatusCode.Conflict);
        }

        public async Task<ResponseDTO<bool>> Delete(int SupplierID)
        {
            Supplier? supplier = await daoSupplier.getSupplier(SupplierID); 
            // if supplier not exist
            if(supplier == null)
            {
                throw new ApplicationException(MessageConst.SUPPLIER_NOT_FOUND);
            }
            // delete supplier
            int number = await daoSupplier.DeleteSupplier(supplier); 
            // if delete successful
            if (number > 0) {
                return new ResponseDTO<bool>(true);
            }
            return new ResponseDTO<bool>(false, "Xóa thất bại", (int) HttpStatusCode.Conflict);
        }
    }
}
