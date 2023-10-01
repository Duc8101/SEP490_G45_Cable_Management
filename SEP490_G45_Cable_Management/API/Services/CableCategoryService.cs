using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.CableCategoryDTO;
using DataAccess.Entity;
using DataAccess.Model.DAO;
using System.Net;

namespace API.Services
{
    public class CableCategoryService
    {
        private readonly DAOCableCategory daoCableCategory = new DAOCableCategory();
        private async Task<List<CableCategoryListDTO>> getList(string? name, int page)
        {
            List<CableCategory> list = await daoCableCategory.getList(name, page);
            List<CableCategoryListDTO> result = new List<CableCategoryListDTO>();
            foreach(CableCategory cable in list)
            {
                CableCategoryListDTO DTO = new CableCategoryListDTO()
                {
                    CableCategoryId = cable.CableCategoryId,
                    CableCategoryName = cable.CableCategoryName,
                    CreatedAt = cable.CreatedAt,
                    UpdateAt = cable.UpdateAt
                };
                result.Add(DTO);
            }
            return result;
        }
        public async Task<PagedResultDTO<CableCategoryListDTO>> List(string? name, int page)
        {
            List<CableCategoryListDTO> list = await getList(name, page);
            int RowCount = await daoCableCategory.getRowCount(name);
            return new PagedResultDTO<CableCategoryListDTO>(page, PageSizeConst.MAX_CABLE_CATEGORY_LIST_IN_PAGE, RowCount, list);
        }

        public async Task<ResponseDTO<bool>> Create(CableCategoryCreateUpdateDTO DTO)
        {
             // if category already exist
             if (await daoCableCategory.isExist(DTO.CableCategoryName.Trim()))
             {
                return new ResponseDTO<bool>(false, "Loại cáp này đã tồn tại", (int) HttpStatusCode.NotAcceptable);
             }
             CableCategory cable = new CableCategory()
             {
                 CableCategoryName = DTO.CableCategoryName.Trim(),
                 CreatedAt = DateTime.Now,
                 IsDeleted = false,
             };
             int number = await daoCableCategory.CreateCableCategory(cable);
             // if create successful
             if(number > 0)
             {
                return new ResponseDTO<bool>(true);
             }
             return new ResponseDTO<bool>(false,"Tạo thất bại", (int) HttpStatusCode.Conflict);
        }

        public async Task<ResponseDTO<bool>> Update(int CableCategoryID, CableCategoryCreateUpdateDTO DTO)
        {
            CableCategory? cable = await daoCableCategory.getCableCategory(CableCategoryID);
            // if not found cable
            if (cable == null) {
                return new ResponseDTO<bool>(false, "Không tìm thấy cáp", (int) HttpStatusCode.NotFound);
            }
            // if cable already exist
            if(await daoCableCategory.isExist(CableCategoryID, DTO.CableCategoryName.Trim()))
            {
                return new ResponseDTO<bool>(false, "Loại cáp này đã tồn tại", (int) HttpStatusCode.NotAcceptable);
            }
            cable.CableCategoryName = DTO.CableCategoryName.Trim();
            cable.UpdateAt = DateTime.Now;
            int number = await daoCableCategory.UpdateCableCategory(cable);
            // if update successful
            if(number > 0) {  
                return new ResponseDTO<bool>(true); 
            }
            return new ResponseDTO<bool>(false, "Chỉnh sửa thất bại", (int) HttpStatusCode.Conflict);
        }
    }
}
