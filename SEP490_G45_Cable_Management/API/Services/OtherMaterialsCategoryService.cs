using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.OtherMaterialsCategoryDTO;
using DataAccess.Entity;
using DataAccess.Model.DAO;

namespace API.Services
{
    public class OtherMaterialsCategoryService
    {
        private readonly DAOOtherMaterialsCategory daoOtherMaterialsCategory = new DAOOtherMaterialsCategory();

        private async Task<List<OtherMaterialsCategoryListDTO>> getList(int page)
        {
            List<OtherMaterialsCategory> list = await daoOtherMaterialsCategory.getList(page);
            List<OtherMaterialsCategoryListDTO> result = new List<OtherMaterialsCategoryListDTO>();
            foreach (OtherMaterialsCategory item in list)
            {
                OtherMaterialsCategoryListDTO DTO = new OtherMaterialsCategoryListDTO()
                {
                    OtherMaterialsCategoryId = item.OtherMaterialsCategoryId,
                    OtherMaterialsCategoryName = item.OtherMaterialsCategoryName,
                    CreatedAt = item.CreatedAt,
                    UpdateAt = item.UpdateAt,
                };
                result.Add(DTO);
            }
            return result;
        }

        public async Task<PagedResultDTO<OtherMaterialsCategoryListDTO>> List(int page)
        {
            List<OtherMaterialsCategoryListDTO> list = await getList(page);
            PagedResultDTO<OtherMaterialsCategoryListDTO> result = new PagedResultDTO<OtherMaterialsCategoryListDTO>(page, PageSizeConst.MAX_OTHER_MATERIAL_CATEGORY_LIST_IN_PAGE, list, 0);
            return result;
        }
    }
}
