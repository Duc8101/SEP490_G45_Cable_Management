using DataAccess.Const;
using DataAccess.DTO;
using DataAccess.DTO.CableCategoryDTO;
using DataAccess.Entity;
using System.Net;
using API.Model.DAO;

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
                };
                result.Add(DTO);
            }
            return result;
        }
        public async Task<ResponseDTO<PagedResultDTO<CableCategoryListDTO>?>> List(string? name, int page)
        {
            try
            {
                List<CableCategoryListDTO> list = await getList(name, page);
                int RowCount = await daoCableCategory.getRowCount(name);
                PagedResultDTO<CableCategoryListDTO> result =  new PagedResultDTO<CableCategoryListDTO>(page, RowCount,PageSizeConst.MAX_CABLE_CATEGORY_LIST_IN_PAGE , list);
                return new ResponseDTO<PagedResultDTO<CableCategoryListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<CableCategoryListDTO>?>(null, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
           
        }

        public async Task<ResponseDTO<bool>> Create(CableCategoryCreateUpdateDTO DTO)
        {
             if(DTO.CableCategoryName.Trim().Length == 0)
             {
                return new ResponseDTO<bool>(false, "Tên cáp không được để trống", (int) HttpStatusCode.Conflict);
             }
             try
             {
                // if category already exist
                if (await daoCableCategory.isExist(DTO.CableCategoryName.Trim()))
                {
                    return new ResponseDTO<bool>(false, "Loại cáp này đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                CableCategory cable = new CableCategory()
                {
                    CableCategoryName = DTO.CableCategoryName.Trim(),
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false,
                };
                await daoCableCategory.CreateCableCategory(cable);
                return new ResponseDTO<bool>(true, "Tạo thành công");
             }
             catch (Exception ex)
             {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseDTO<bool>> Update(int CableCategoryID, CableCategoryCreateUpdateDTO DTO)
        {
            try
            {
                CableCategory? cable = await daoCableCategory.getCableCategory(CableCategoryID);
                // if not found cable
                if (cable == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy cáp", (int) HttpStatusCode.NotFound);
                }
                if (DTO.CableCategoryName.Trim().Length == 0)
                {
                    return new ResponseDTO<bool>(false, "Tên cáp không được để trống", (int) HttpStatusCode.Conflict);
                }
                // if cable already exist
                if (await daoCableCategory.isExist(CableCategoryID, DTO.CableCategoryName.Trim()))
                {
                    return new ResponseDTO<bool>(false, "Loại cáp này đã tồn tại", (int) HttpStatusCode.Conflict);
                }
                cable.CableCategoryName = DTO.CableCategoryName.Trim();
                cable.UpdateAt = DateTime.Now;
                await daoCableCategory.UpdateCableCategory(cable);
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int) HttpStatusCode.InternalServerError);
            }
            
        }
    }
}
