using DataAccess.Const;
using DataAccess.DTO.CommonDTO;
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
        public async Task<BaseResponseDTO<bool>> Create(SupplierCreateUpdateDTO DTO, string CreatorID)
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
                        UpdateAt = DateTime.Now,
                        IsDeleted = false,
                        CreatorId = Guid.Parse(CreatorID)
                    };
                    // create supplier
                    int number = await daoSupplier.CreateSupplier(supplier);
                    // if create successful
                    if(number > 0)
                    {
                        return new BaseResponseDTO<bool>(true, string.Empty, (int) HttpStatusCode.OK);
                    }
                    return new BaseResponseDTO<bool>("Tạo thất bại", (int) HttpStatusCode.BadRequest);
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

        public async Task<BaseResponseDTO<PagedResultDTO<SupplierListDTO>>> List(string? filter, int pageSize/* number of row in a page*/, int currentPage)
        {
            List<Supplier> list = await daoSupplier.getList(filter, pageSize, currentPage);
            List<SupplierListDTO> result = getList(list);
            PagedResultDTO<SupplierListDTO> pageResult = new PagedResultDTO<SupplierListDTO>(currentPage, pageSize, result, 0);
            return new BaseResponseDTO<PagedResultDTO<SupplierListDTO>>(pageResult, string.Empty, (int) HttpStatusCode.OK);
        }

        public async Task<BaseResponseDTO<bool>> Update(int SupplierID, SupplierCreateUpdateDTO DTO)
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
            supplier.Country = DTO.Country == null ? null : DTO.Country.Trim();
            supplier.SupplierDescription = DTO.SupplierDescription == null ? null : DTO.SupplierDescription.Trim();
            supplier.UpdateAt = DateTime.Now;
            // update supplier
            int number = await daoSupplier.UpdateSupplier(supplier);
            // if update successful
            if (number > 0)
            {
                return new BaseResponseDTO<bool>(true, string.Empty, (int) HttpStatusCode.OK);
            }
            return new BaseResponseDTO<bool>("Chỉnh sửa thất bại", (int) HttpStatusCode.NotFound);
        }

        public async Task<BaseResponseDTO<bool>> Delete(int SupplierID)
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
                return new BaseResponseDTO<bool>(true, string.Empty, (int) HttpStatusCode.OK);
            }
            return new BaseResponseDTO<bool>("Xóa thất bại", (int)HttpStatusCode.NotFound);
        }
    }
}
