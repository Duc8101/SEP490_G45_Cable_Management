﻿using API.Services.IService;
using AutoMapper;
using Common.Base;
using Common.Const;
using Common.DTO.CableCategoryDTO;
using Common.Entity;
using Common.Pagination;
using DataAccess.DAO;
using System.Net;

namespace API.Services.Service
{
    public class CableCategoryService : BaseService, ICableCategoryService
    {
        private readonly DAOCableCategory daoCableCategory = new DAOCableCategory();

        public CableCategoryService(IMapper mapper) : base(mapper)
        {
        }

        public async Task<ResponseBase<Pagination<CableCategoryListDTO>?>> ListPaged(string? name, int page)
        {
            try
            {
                List<CableCategory> list = await daoCableCategory.getListPaged(name, page);
                List<CableCategoryListDTO> DTOs = _mapper.Map<List<CableCategoryListDTO>>(list);
                int RowCount = await daoCableCategory.getRowCount(name);
                Pagination<CableCategoryListDTO> result = new Pagination<CableCategoryListDTO>(page, RowCount, PageSizeConst.MAX_CABLE_CATEGORY_LIST_IN_PAGE, DTOs);
                return new ResponseBase<Pagination<CableCategoryListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<Pagination<CableCategoryListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }

        }

        public async Task<ResponseBase<List<CableCategoryListDTO>?>> ListAll()
        {
            try
            {
                List<CableCategory> list = await daoCableCategory.getListAll();
                List<CableCategoryListDTO> data = _mapper.Map<List<CableCategoryListDTO>>(list);
                return new ResponseBase<List<CableCategoryListDTO>?>(data, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<CableCategoryListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ResponseBase<bool>> Create(CableCategoryCreateUpdateDTO DTO)
        {
            if (DTO.CableCategoryName.Trim().Length == 0)
            {
                return new ResponseBase<bool>(false, "Tên cáp không được để trống", (int)HttpStatusCode.Conflict);
            }
            try
            {
                // if category already exist
                if (await daoCableCategory.isExist(DTO.CableCategoryName.Trim()))
                {
                    return new ResponseBase<bool>(false, "Loại cáp này đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                CableCategory cable = new CableCategory()
                {
                    CableCategoryName = DTO.CableCategoryName.Trim(),
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    IsDeleted = false,
                };
                await daoCableCategory.CreateCableCategory(cable);
                return new ResponseBase<bool>(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseBase<bool>> Update(int CableCategoryID, CableCategoryCreateUpdateDTO DTO)
        {
            try
            {
                CableCategory? cable = await daoCableCategory.getCableCategory(CableCategoryID);
                // if not found cable
                if (cable == null)
                {
                    return new ResponseBase<bool>(false, "Không tìm thấy cáp", (int)HttpStatusCode.NotFound);
                }
                if (DTO.CableCategoryName.Trim().Length == 0)
                {
                    return new ResponseBase<bool>(false, "Tên cáp không được để trống", (int)HttpStatusCode.Conflict);
                }
                // if cable already exist
                if (await daoCableCategory.isExist(CableCategoryID, DTO.CableCategoryName.Trim()))
                {
                    return new ResponseBase<bool>(false, "Loại cáp này đã tồn tại", (int)HttpStatusCode.Conflict);
                }
                cable.CableCategoryName = DTO.CableCategoryName.Trim();
                cable.UpdateAt = DateTime.Now;
                await daoCableCategory.UpdateCableCategory(cable);
                return new ResponseBase<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }

        }
    }
}
