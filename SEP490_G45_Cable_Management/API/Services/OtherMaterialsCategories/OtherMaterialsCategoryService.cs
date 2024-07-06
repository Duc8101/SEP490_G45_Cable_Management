using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.DTO.OtherMaterialsCategoryDTO;
using Common.Entity;
using Common.Paginations;
using DataAccess.DAO;
using System.Net;

namespace API.Services.OtherMaterialsCategories
{
    public class OtherMaterialsCategoryService : BaseService, IOtherMaterialsCategoryService
    {
        private readonly DAOOtherMaterialsCategory _daoOtherMaterialCategory;
        public OtherMaterialsCategoryService(IMapper mapper, DAOOtherMaterialsCategory daoOtherMaterialCategory) : base(mapper)
        {
            _daoOtherMaterialCategory = daoOtherMaterialCategory;
        }

        public ResponseBase Create(OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            if (DTO.OtherMaterialsCategoryName.Trim().Length == 0)
            {
                return new ResponseBase(false, "Tên vật liệu không được để trống", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if name exist
                if (_daoOtherMaterialCategory.isExist(DTO.OtherMaterialsCategoryName.Trim()))
                {
                    return new ResponseBase(false, "Loại vật liệu này đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                OtherMaterialsCategory category = new OtherMaterialsCategory()
                {
                    OtherMaterialsCategoryName = DTO.OtherMaterialsCategoryName.Trim(),
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false
                };
                _daoOtherMaterialCategory.CreateOtherMaterialsCategory(category);
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
                List<OtherMaterialsCategory> list = _daoOtherMaterialCategory.getListOtherMaterialsCategory();
                List<OtherMaterialsCategoryListDTO> data = _mapper.Map<List<OtherMaterialsCategoryListDTO>>(list);
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
                List<OtherMaterialsCategory> list = _daoOtherMaterialCategory.getListOtherMaterialsCategory(name, page);
                List<OtherMaterialsCategoryListDTO> DTOs = _mapper.Map<List<OtherMaterialsCategoryListDTO>>(list);
                int rowCount = _daoOtherMaterialCategory.getRowCount(name);
                Pagination<OtherMaterialsCategoryListDTO> data = new Pagination<OtherMaterialsCategoryListDTO>()
                {
                    CurrentPage = page,
                    RowCount = rowCount,
                    Data = DTOs
                };
                return new ResponseBase(data);
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public ResponseBase Update(int otherMaterialsCategoryId, OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            try
            {
                OtherMaterialsCategory? category = _daoOtherMaterialCategory.getOtherMaterialsCategory(otherMaterialsCategoryId);
                // if not found
                if (category == null)
                {
                    return new ResponseBase(false, "Không tìm thấy loại vật liệu này", (int)HttpStatusCode.NotFound);
                }

                if (DTO.OtherMaterialsCategoryName.Trim().Length == 0)
                {
                    return new ResponseBase(false, "Tên vật liệu không được để trống", (int)HttpStatusCode.Conflict);
                }

                // if already exist
                if (_daoOtherMaterialCategory.isExist(otherMaterialsCategoryId, DTO.OtherMaterialsCategoryName.Trim()))
                {
                    return new ResponseBase(false, "Loại vật liệu này đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                category.OtherMaterialsCategoryName = DTO.OtherMaterialsCategoryName.Trim();
                category.UpdateAt = DateTime.Now;
                _daoOtherMaterialCategory.UpdateOtherMaterialsCategory(category);
                return new ResponseBase(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase(ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
