using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.DTO.CableCategoryDTO;
using Common.Entity;
using Common.Paginations;
using DataAccess.DAO;
using System.Net;

namespace API.Services.CableCategories
{
    public class CableCategoryService : BaseService, ICableCategoryService
    {
        private readonly DAOCableCategory _daoCableCategory;
        public CableCategoryService(IMapper mapper, DAOCableCategory daoCableCategory) : base(mapper)
        {
            _daoCableCategory = daoCableCategory;
        }

        public ResponseBase Create(CableCategoryCreateUpdateDTO DTO)
        {
            if (DTO.CableCategoryName.Trim().Length == 0)
            {
                return new ResponseBase(false, "Tên cáp không được để trống", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if category already exist
                if (_daoCableCategory.isExist(DTO.CableCategoryName.Trim()))
                {
                    return new ResponseBase(false, "Loại cáp này đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                CableCategory cable = new CableCategory()
                {
                    CableCategoryName = DTO.CableCategoryName.Trim(),
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false,
                };
                _daoCableCategory.CreateCableCategory(cable);
                return new ResponseBase(true, "Tạo thành công");
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
                List<CableCategory> list = _daoCableCategory.getListCableCategoryAll();
                List<CableCategoryListDTO> data = _mapper.Map<List<CableCategoryListDTO>>(list);
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
                List<CableCategory> list = _daoCableCategory.getListCableCategoryPaged(name, page);
                List<CableCategoryListDTO> DTOs = _mapper.Map<List<CableCategoryListDTO>>(list);
                int rowCount = _daoCableCategory.getRowCount(name);
                Pagination<CableCategoryListDTO> result = new Pagination<CableCategoryListDTO>
                {
                    List = DTOs,
                    CurrentPage = page,
                    RowCount = rowCount
                };
                return new ResponseBase(result);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Update(int cableCategoryId, CableCategoryCreateUpdateDTO DTO)
        {
            try
            {
                CableCategory? cable = _daoCableCategory.getCableCategory(cableCategoryId);
                // if not found cable
                if (cable == null)
                {
                    return new ResponseBase(false, "Không tìm thấy cáp", (int)HttpStatusCode.NotFound);
                }
                if (DTO.CableCategoryName.Trim().Length == 0)
                {
                    return new ResponseBase(false, "Tên cáp không được để trống", (int)HttpStatusCode.Conflict);
                }
                // if cable already exist
                if (_daoCableCategory.isExist(cableCategoryId, DTO.CableCategoryName.Trim()))
                {
                    return new ResponseBase(false, "Loại cáp này đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                cable.CableCategoryName = DTO.CableCategoryName.Trim();
                cable.UpdateAt = DateTime.Now;
                _daoCableCategory.UpdateCableCategory(cable);
                return new ResponseBase(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
