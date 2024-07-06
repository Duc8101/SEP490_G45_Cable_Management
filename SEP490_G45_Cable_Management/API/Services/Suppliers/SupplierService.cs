using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.DTO.SupplierDTO;
using Common.Entity;
using Common.Paginations;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Suppliers
{
    public class SupplierService : BaseService, ISupplierService
    {
        private readonly DAOSupplier _daoSupplier;
        public SupplierService(IMapper mapper, DAOSupplier daoSupplier) : base(mapper)
        {
            _daoSupplier = daoSupplier;
        }

        public ResponseBase Create(SupplierCreateUpdateDTO DTO, Guid creatorId)
        {
            if (DTO.SupplierName.Trim().Length == 0)
            {
                return new ResponseBase(false, "Tên nhà cung cấp không được để trống", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if supplier already exist
                if (_daoSupplier.isExist(DTO.SupplierName.Trim()))
                {
                    return new ResponseBase(false, "Nhà cung cấp đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                Supplier supplier = _mapper.Map<Supplier>(DTO);
                supplier.CreatedAt = DateTime.Now;
                supplier.UpdateAt = DateTime.Now;
                supplier.IsDeleted = false;
                supplier.CreatorId = creatorId;
                // create supplier
                _daoSupplier.CreateSupplier(supplier);
                return new ResponseBase(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Delete(int supplierId)
        {
            try
            {
                Supplier? supplier = _daoSupplier.getSupplier(supplierId);
                // if supplier not exist
                if (supplier == null)
                {
                    return new ResponseBase(false, "Không tìm thấy nhà cung cấp", (int)HttpStatusCode.NotFound);
                }
                // delete supplier
                _daoSupplier.DeleteSupplier(supplier);
                return new ResponseBase(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListAll()
        {
            try
            {
                List<Supplier> list = _daoSupplier.getListSupplier();
                List<SupplierListDTO> data = _mapper.Map<List<SupplierListDTO>>(list);
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase ListPaged(string? name, int page)
        {
            try
            {
                List<Supplier> list = _daoSupplier.getListSupplier(name, page);
                List<SupplierListDTO> DTOs = _mapper.Map<List<SupplierListDTO>>(list);
                int rowCount = _daoSupplier.getRowCount(name);
                Pagination<SupplierListDTO> data = new Pagination<SupplierListDTO>()
                {
                    CurrentPage = page,
                    RowCount = rowCount,
                    List = DTOs
                };
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Update(int supplierId, SupplierCreateUpdateDTO DTO)
        {
            try
            {
                Supplier? supplier = _daoSupplier.getSupplier(supplierId);
                // if supplier id not exist
                if (supplier == null)
                {
                    return new ResponseBase(false, "Không tìm thấy nhà cung cấp", (int)HttpStatusCode.NotFound);
                }
                if (DTO.SupplierName.Trim().Length == 0)
                {
                    return new ResponseBase(false, "Tên nhà cung cấp không được để trống", (int)HttpStatusCode.Conflict);
                }
                // if supplier already exist
                if (_daoSupplier.isExist(supplierId, DTO.SupplierName))
                {
                    return new ResponseBase(false, "Nhà cung cấp đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                supplier.SupplierName = DTO.SupplierName.Trim();
                supplier.Country = DTO.Country == null || DTO.Country.Trim().Length == 0 ? null : DTO.Country.Trim();
                supplier.SupplierDescription = DTO.SupplierDescription == null || DTO.SupplierDescription.Trim().Length == 0 ? null : DTO.SupplierDescription.Trim();
                supplier.UpdateAt = DateTime.Now;
                // update supplier
                _daoSupplier.UpdateSupplier(supplier);
                return new ResponseBase(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
