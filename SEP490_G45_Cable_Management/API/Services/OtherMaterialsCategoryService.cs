using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.OtherMaterialsCategoryDTO;
using DataAccess.Entity;
using DataAccess.Model.DAO;
using System.Net;

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
                    OtherMaterialsCategoryName = item.OtherMaterialsCategoryName
                };
                result.Add(DTO);
            }
            return result;
        }

        public async Task<PagedResultDTO<OtherMaterialsCategoryListDTO>> List(int page)
        {
            List<OtherMaterialsCategoryListDTO> list = await getList(page);
            int RowCount = await daoOtherMaterialsCategory.getRowCount();
            return new PagedResultDTO<OtherMaterialsCategoryListDTO>(page, PageSizeConst.MAX_OTHER_MATERIAL_CATEGORY_LIST_IN_PAGE, RowCount, list);
        }

        public async Task<ResponseDTO<bool>> Create(OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            // if name exist
            if(await daoOtherMaterialsCategory.isExist(DTO.OtherMaterialsCategoryName.Trim()))
            {
                return new ResponseDTO<bool>(false, "Loại cáp này đã tồn tại" , (int) HttpStatusCode.NotAcceptable);
            }
            OtherMaterialsCategory category = new OtherMaterialsCategory()
            {
                OtherMaterialsCategoryName = DTO.OtherMaterialsCategoryName.Trim(),
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            int number = await daoOtherMaterialsCategory.CreateOtherMaterialsCategory(category);
            // if create successful
            if(number > 0)
            {
                return new ResponseDTO<bool>(true, "Tạo thành công");
            }
            return new ResponseDTO<bool>(false, "Tạo thất bại", (int) HttpStatusCode.Conflict);
        }

        public async Task<ResponseDTO<bool>> Update(int OtherMaterialsCategoryID, OtherMaterialsCategoryCreateUpdateDTO DTO)
        {
            OtherMaterialsCategory? category = await daoOtherMaterialsCategory.getOtherMaterialsCategory(OtherMaterialsCategoryID);
            // if not found
            if(category == null)
            {
                return new ResponseDTO<bool>(false, "Không tìm thấy loại cáp này" , (int) HttpStatusCode.NotFound);
            }
            // if already exist
            if(await daoOtherMaterialsCategory.isExist(OtherMaterialsCategoryID, DTO.OtherMaterialsCategoryName.Trim()))
            {
                return new ResponseDTO<bool>(false, "Loại cáp này đã tồn tại", (int) HttpStatusCode.NotFound);
            }
            category.OtherMaterialsCategoryName = DTO.OtherMaterialsCategoryName.Trim();
            category.UpdateAt = DateTime.Now;
            int number = await daoOtherMaterialsCategory.UpdateOtherMaterialsCategory(category);
            // if update successful
            if(number > 0)
            {
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            return new ResponseDTO<bool>(false, "Chỉnh sửa thất bại", (int) HttpStatusCode.Conflict);
        }
    }
}
