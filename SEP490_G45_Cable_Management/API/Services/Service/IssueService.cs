﻿using DataAccess.DTO.IssueDTO;
using DataAccess.DTO;
using DataAccess.Entity;
using DataAccess.Const;
using System.Net;
using System.Text.RegularExpressions;
using API.Model.DAO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using API.Services.IService;
using DataAccess.DTO.RequestDTO;
using Org.BouncyCastle.Asn1.Ocsp;

namespace API.Services.Service
{
    public class IssueService : IIssueService
    {
        private readonly DAOIssue daoIssue = new DAOIssue();
        private readonly DAORequest daoRequest = new DAORequest();
        private readonly DAORequestCable daoRequestCable = new DAORequestCable();
        private readonly DAORequestOtherMaterial daoRequestMaterial = new DAORequestOtherMaterial();
        private List<IssueListDTO> getListDTO(List<Issue> list)
        {
            List<IssueListDTO> result = new List<IssueListDTO>();
            foreach (Issue issue in list)
            {
                IssueListDTO DTO = new IssueListDTO()
                {
                    IssueId = issue.IssueId,
                    IssueName = issue.IssueName,
                    IssueCode = issue.IssueCode,
                    Description = issue.Description == null ? null : issue.Description,
                    CreatedDate = issue.CreatedDate,
                    CableRoutingName = issue.CableRoutingName,
                    Group = issue.Group,
                    Status = issue.Status,
                };
                result.Add(DTO);
            }
            return result;
        }
        private async Task<List<IssueListDTO>> getListPagedAll(string? filter, int page)
        {
            List<Issue> list = await daoIssue.getListPagedAll(filter, page);
            List<IssueListDTO> result = getListDTO(list);
            return result;
        }
        private async Task<List<IssueListDTO>> getListPagedDoing(int page)
        {
            List<Issue> list = await daoIssue.getListPagedDoing(page);
            List<IssueListDTO> result = getListDTO(list);
            return result;
        }
        public async Task<ResponseDTO<PagedResultDTO<IssueListDTO>?>> ListPagedAll(string? filter, int page)
        {
            try
            {
                List<IssueListDTO> list = await getListPagedAll(filter, page);
                int RowCount = await daoIssue.getRowCount(filter);
                PagedResultDTO<IssueListDTO> result = new PagedResultDTO<IssueListDTO>(page, RowCount, PageSizeConst.MAX_ISSUE_LIST_IN_PAGE, list);
                return new ResponseDTO<PagedResultDTO<IssueListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<IssueListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<PagedResultDTO<IssueListDTO>?>> ListPagedDoing(int page)
        {
            try
            {
                List<IssueListDTO> list = await getListPagedDoing(page);
                int RowCount = await daoIssue.getRowCount();
                PagedResultDTO<IssueListDTO> result = new PagedResultDTO<IssueListDTO>(page, RowCount, PageSizeConst.MAX_ISSUE_LIST_IN_PAGE, list);
                return new ResponseDTO<PagedResultDTO<IssueListDTO>?>(result, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<PagedResultDTO<IssueListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        private async Task<List<IssueListDTO>> getListDoing()
        {
            List<Issue> list = await daoIssue.getListDoing();
            List<IssueListDTO> result = getListDTO(list);
            return result;
        }
        public async Task<ResponseDTO<List<IssueListDTO>?>> ListDoing()
        {
            try
            {
                List<IssueListDTO> list = await getListDoing();
                return new ResponseDTO<List<IssueListDTO>?>(list, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<IssueListDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Create(IssueCreateDTO DTO, Guid CreatorID)
        {
            if (DTO.IssueName.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Tên sự vụ không được để trống", (int)HttpStatusCode.Conflict);
            }

            if (DTO.IssueCode.Trim().Length == 0)
            {
                return new ResponseDTO<bool>(false, "Mã sự vụ không được để trống", (int)HttpStatusCode.Conflict);
            }

            Issue issue = new Issue()
            {
                IssueId = Guid.NewGuid(),
                IssueName = DTO.IssueName.Trim(),
                IssueCode = DTO.IssueCode.Trim(),
                Description = DTO.Description == null || DTO.Description.Trim().Length == 0 ? null : DTO.Description.Trim(),
                CreatedAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                CreatedDate = DTO.CreatedDate,
                CableRoutingName = DTO.CableRoutingName == null || DTO.CableRoutingName.Trim().Length == 0 ? null : DTO.CableRoutingName.Trim(),
                Group = DTO.Group == null || DTO.Group.Trim().Length == 0 ? null : DTO.Group.Trim(),
                CreatorId = CreatorID,
                Status = IssueConst.STATUS_DOING,
                IsDeleted = false,
            };
            try
            {
                await daoIssue.CreateIssue(issue);
                return new ResponseDTO<bool>(true, "Tạo thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Update(Guid IssueID, IssueUpdateDTO DTO)
        {
            try
            {
                Issue? issue = await daoIssue.getIssue(IssueID);
                // if not found
                if (issue == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                if (DTO.IssueName.Trim().Length == 0)
                {
                    return new ResponseDTO<bool>(false, "Tên sự vụ không được để trống", (int)HttpStatusCode.Conflict);
                }
                if (DTO.IssueCode.Trim().Length == 0)
                {
                    return new ResponseDTO<bool>(false, "Mã sự vụ không được để trống", (int)HttpStatusCode.Conflict);
                }
                if(issue.Status == IssueConst.STATUS_DONE)
                {
                    return new ResponseDTO<bool>(false, "Sự vụ đã được xử lý", (int)HttpStatusCode.Conflict);
                }
                issue.IssueName = DTO.IssueName.Trim();
                issue.IssueCode = DTO.IssueCode.Trim();
                issue.Description = DTO.Description == null || DTO.Description.Trim().Length == 0 ? null : DTO.Description.Trim();
                issue.CreatedDate = DTO.CreatedDate;
                issue.CableRoutingName = DTO.CableRoutingName == null || DTO.CableRoutingName.Trim().Length == 0 ? null : DTO.CableRoutingName.Trim();
                issue.Group = DTO.Group == null || DTO.Group.Trim().Length == 0 ? null : DTO.Group.Trim();
                issue.Status = DTO.Status;
                issue.UpdateAt = DateTime.Now;
                await daoIssue.UpdateIssue(issue);
                return new ResponseDTO<bool>(true, "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<bool>> Delete(Guid IssueID)
        {
            try
            {
                Issue? issue = await daoIssue.getIssue(IssueID);
                // if not found
                if (issue == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                await daoIssue.DeleteIssue(issue);
                return new ResponseDTO<bool>(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(false, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<ResponseDTO<List<IssueDetailDTO>?>> Detail(Guid IssueID)
        {
            try
            {
                Issue? issue = await daoIssue.getIssue(IssueID);
                // if not found
                if (issue == null)
                {
                    return new ResponseDTO<List<IssueDetailDTO>?>(null, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
                }
                List<DataAccess.Entity.Request> requests = await daoRequest.getListByIssue(IssueID);
                List<IssueDetailDTO> details = new List<IssueDetailDTO>();
                foreach(DataAccess.Entity.Request request in requests)
                {
                    List<RequestCable> requestCables = await daoRequestCable.getList(request.RequestId);
                    List<RequestCableByIssueDTO> requestCableDTOs = new List<RequestCableByIssueDTO>();
                    foreach (RequestCable item in requestCables)
                    {
                        RequestCableByIssueDTO DTO = new RequestCableByIssueDTO()
                        {
                            CableCategoryName = item.Cable.CableCategory.CableCategoryName,
                            StartPoint = item.StartPoint,
                            EndPoint = item.EndPoint,
                            Length = item.Length,
                        };
                        requestCableDTOs.Add(DTO);
                    }
                    List<RequestOtherMaterial> requestMaterials = await daoRequestMaterial.getList(request.RequestId);
                    List<RequestOtherMaterialsByIssueDTO> requestOtherMaterialsDTOs = new List<RequestOtherMaterialsByIssueDTO>();
                    foreach (RequestOtherMaterial item in requestMaterials)
                    {
                        RequestOtherMaterialsByIssueDTO DTO = new RequestOtherMaterialsByIssueDTO()
                        {
                            OtherMaterialsCategoryName = item.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryName,
                            Quantity = item.Quantity,
                        };
                        requestOtherMaterialsDTOs.Add(DTO);
                    }
                    IssueDetailDTO detail = new IssueDetailDTO()
                    {
                        RequestName = request.RequestName,
                        ApproverName = request.Approver == null ? null : request.Approver.Lastname + " " + request.Approver.Firstname,
                        CableRoutingName = issue.CableRoutingName,
                        Group = issue.Group,
                        RequestCableDTOs = requestCableDTOs,
                        RequestOtherMaterialsDTOs = requestOtherMaterialsDTOs
                    };
                    details.Add(detail);
                }
                return new ResponseDTO<List<IssueDetailDTO>?>(details, string.Empty);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<IssueDetailDTO>?>(null, ex.Message + " " + ex, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
