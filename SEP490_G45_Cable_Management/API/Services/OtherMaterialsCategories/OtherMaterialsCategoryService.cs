using API.Services.Base;
using AutoMapper;
using Common.Base;
using Common.Const;
using Common.DTO.OtherMaterialsCategoryDTO;
using Common.Entity;
using Common.Pagination;
using DataAccess.DAO;
using System.Net;

namespace API.Services.OtherMaterialsCategories
{
    public class OtherMaterialsCategoryService : BaseService, IOtherMaterialsCategoryService
    {
        private readonly DAOOtherMaterialsCategory daoOtherMaterialsCategory = new DAOOtherMaterialsCategory();

        public OtherMaterialsCategoryService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseBase<Pagination<OtherMaterialsCategoryListDTO>?>> ListPaged(string? name, int page)
        {
            try
            {
                List<OtherMaterialsCategory> list = await daoOtherMaterialsCategory.getListPaged(name, page);
                List<OtherMaterialsCategoryListDTO> DTOs = _mapper.Map<List<OtherMaterialsCategoryListDTO>>(list);
                int RowCount = await daoOtherMaterialsCategory.getRowCount(name);
                Pagination<OtherMaterialsCategoryListDTO> result = new Pagination<OtherMaterialsCategoryListDTO>(page, RowCount, PageSizeConst.MAX_OTHER_MATERIAL_CATEGORY_LIST_IN_PAGE, DTOs);
                return new ResponseBase<Pagination<OtherMaterialsCategoryListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<Pagination<OtherMaterialsCategoryListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<List<OtherMaterialsCategoryListDTO>?>> ListAll()
        {
            try
            {
                List<OtherMaterialsCategory> list = await daoOtherMaterialsCategory.getListAll();
                List<OtherMaterialsCategoryListDTO> data = _mapper.Map<List<OtherMaterialsCategoryListDTO>>(list);
                return new ResponseBase<List<OtherMaterialsCategoryListDTO>?>(data, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<OtherMaterialsCategoryListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Create(OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            if (DTO.OtherMaterialsCategoryName.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Tên vật liệu không được để trống", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if name exist
                if (await daoOtherMaterialsCategory.isExist(DTO.OtherMaterialsCategoryName.Trim()))
                {
                    return new ResponseBase<bool>(false, "Loại vật liệu này đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                OtherMaterialsCategory category = new OtherMaterialsCategory()
                {
                    OtherMaterialsCategoryName = DTO.OtherMaterialsCategoryName.Trim(),
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false
                };
                await daoOtherMaterialsCategory.CreateOtherMaterialsCategory(category);
                return new ResponseBase<bool>(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Update(int OtherMaterialsCategoryID, OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            try
            {
                OtherMaterialsCategory? category = await daoOtherMaterialsCategory.getOtherMaterialsCategory(OtherMaterialsCategoryID);
                // if not found
                if (category == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy loại vật liệu này", (int)HttpStatusCode.NotFound);
                }

                if (DTO.OtherMaterialsCategoryName.Trim().Length == 0)
                {
                    return new ResponseBase<bool>(false, "Tên vật liệu không được để trống", (int)HttpStatusCode.Conflict);
                }
                // if already exist
                if (await daoOtherMaterialsCategory.isExist(OtherMaterialsCategoryID, DTO.OtherMaterialsCategoryName.Trim()))
                {
                    return new ResponseBase<bool>(false, "Loại vật liệu này đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                category.OtherMaterialsCategoryName = DTO.OtherMaterialsCategoryName.Trim();
                category.UpdateAt = DateTime.Now;
                await daoOtherMaterialsCategory.UpdateOtherMaterialsCategory(category);
                return new ResponseBase<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
