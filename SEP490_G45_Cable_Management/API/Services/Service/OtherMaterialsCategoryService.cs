using API.Model.DAO;
using API.Services.IService;
using AutoMapper;
using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.OtherMaterialsCategoryDTO;
using DataAccess.Entity;
using System.Net;

namespace API.Services.Service
{
    public class OtherMaterialsCategoryService : BaseService, IOtherMaterialsCategoryService
    {
        private readonly DAOOtherMaterialsCategory daoOtherMaterialsCategory = new DAOOtherMaterialsCategory();

        public OtherMaterialsCategoryService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?>> ListPaged(string? name, int page)
        {
            try
            {
                List<OtherMaterialsCategory> list = await daoOtherMaterialsCategory.getListPaged(name, page);
                List<OtherMaterialsCategoryListDTO> DTOs = mapper.Map<List<OtherMaterialsCategoryListDTO>>(list);
                int RowCount = await daoOtherMaterialsCategory.getRowCount(name);
                PagedResultDTO<OtherMaterialsCategoryListDTO> result = new PagedResultDTO<OtherMaterialsCategoryListDTO>(page, RowCount, PageSizeConst.MAX_OTHER_MATERIAL_CATEGORY_LIST_IN_PAGE, DTOs);
                return new ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<List<OtherMaterialsCategoryListDTO>?>> ListAll()
        {
            try
            {
                List<OtherMaterialsCategory> list = await daoOtherMaterialsCategory.getListAll();
                List<OtherMaterialsCategoryListDTO> data = mapper.Map<List<OtherMaterialsCategoryListDTO>>(list);
                return new ResponseDTO<List<OtherMaterialsCategoryListDTO>?>(data, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<OtherMaterialsCategoryListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Create(OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            if (DTO.OtherMaterialsCategoryName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên vật liệu không được để trống", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if name exist
                if (await daoOtherMaterialsCategory.isExist(DTO.OtherMaterialsCategoryName.Trim()))
                {
                    return new ResponseDTO<bool>(false, "Loại vật liệu này đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                OtherMaterialsCategory category = new OtherMaterialsCategory()
                {
                    OtherMaterialsCategoryName = DTO.OtherMaterialsCategoryName.Trim(),
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false
                };
                await daoOtherMaterialsCategory.CreateOtherMaterialsCategory(category);
                return new ResponseDTO<bool>(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Update(int OtherMaterialsCategoryID, OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            try
            {
                OtherMaterialsCategory? category = await daoOtherMaterialsCategory.getOtherMaterialsCategory(OtherMaterialsCategoryID);
                // if not found
                if (category == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy loại vật liệu này", (int)HttpStatusCode.NotFound);
                }

                if (DTO.OtherMaterialsCategoryName.Trim().Length == 0)
                {
                    return new ResponseDTO<bool>(false, "Tên vật liệu không được để trống", (int)HttpStatusCode.Conflict);
                }
                // if already exist
                if (await daoOtherMaterialsCategory.isExist(OtherMaterialsCategoryID, DTO.OtherMaterialsCategoryName.Trim()))
                {
                    return new ResponseDTO<bool>(false, "Loại vật liệu này đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                category.OtherMaterialsCategoryName = DTO.OtherMaterialsCategoryName.Trim();
                category.UpdateAt = DateTime.Now;
                await daoOtherMaterialsCategory.UpdateOtherMaterialsCategory(category);
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
