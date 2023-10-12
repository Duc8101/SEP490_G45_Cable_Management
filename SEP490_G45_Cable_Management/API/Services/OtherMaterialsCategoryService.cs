using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.OtherMaterialsCategoryDTO;
using DataAccess.Entity;
using System.Net;
using API.Model.DAO;

namespace API.Services
{
    public class OtherMaterialsCategoryService
    {
        private readonly DAOOtherMaterialsCategory daoOtherMaterialsCategory = new DAOOtherMaterialsCategory();

        private async Task<List<OtherMaterialsCategoryListDTO>> getList(string? name, int page)
        {
            List<OtherMaterialsCategory> list = await daoOtherMaterialsCategory.getList(name,page);
            List<OtherMaterialsCategoryListDTO> result = new List<OtherMaterialsCategoryListDTO>();
            foreach (OtherMaterialsCategory item in list)
            {
                OtherMaterialsCategoryListDTO DTO = new OtherMaterialsCategoryListDTO()
                {
                    OtherMaterialsCategoryId = item.OtherMaterialsCategoryId,
                    OtherMaterialsCategoryName = item.OtherMaterialsCategoryName
                };
                result.Add(DTO);
            }
            return result;
        }

        public async Task<ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?>> List(string? name, int page)
        {
            try
            {
                List<OtherMaterialsCategoryListDTO> list = await getList(name, page);
                int RowCount = await daoOtherMaterialsCategory.getRowCount(name);
                PagedResultDTO<OtherMaterialsCategoryListDTO> result = new PagedResultDTO<OtherMaterialsCategoryListDTO>(page, RowCount, PageSizeConst.MAX_OTHER_MATERIAL_CATEGORY_LIST_IN_PAGE, list);
                return new ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>?>(null, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseDTO<bool>> Create(OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            if(DTO.OtherMaterialsCategoryName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên vật liệu không được để trống", (int)HttpStatusCode.NotAcceptable);
            }
            try
            {
                // if name exist
                if (await daoOtherMaterialsCategory.isExist(DTO.OtherMaterialsCategoryName.Trim()))
                {
                    return new ResponseDTO<bool>(false, "Loại vật liệu này đã tồn tại", (int)HttpStatusCode.NotAcceptable);
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
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
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
                    return new ResponseDTO<bool>(false, "Không tìm thấy loại vật liệu này", (int) HttpStatusCode.NotFound);
                }

                if (DTO.OtherMaterialsCategoryName.Trim().Length == 0)
                {
                    return new ResponseDTO<bool>(false, "Tên vật liệu không được để trống", (int) HttpStatusCode.NotAcceptable);
                }
                // if already exist
                if (await daoOtherMaterialsCategory.isExist(OtherMaterialsCategoryID, DTO.OtherMaterialsCategoryName.Trim()))
                {
                    return new ResponseDTO<bool>(false, "Loại vật liệu này đã tồn tại", (int) HttpStatusCode.NotFound);
                }
                category.OtherMaterialsCategoryName = DTO.OtherMaterialsCategoryName.Trim();
                category.UpdateAt = DateTime.Now;
                await daoOtherMaterialsCategory.UpdateOtherMaterialsCategory(category);
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }
    }
}
