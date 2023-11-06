﻿using API.Services.Service;
using DataAccess.DTO;
using DataAccess.DTO.IssueDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit.Encodings;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class IssueController : BaseAPIController
    {
        private readonly IssueService service = new IssueService();
        [HttpGet("Paged/All")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<IssueListDTO>?>> List(string? filter, [Required] int page = 1)
        {
            // if admin, leader, staff
            if(isAdmin() || isLeader() || isStaff())
            {
                return await service.ListPagedAll(filter, page);
            }
            return new ResponseDTO<PagedResultDTO<IssueListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        [HttpGet("Paged/Doing")]
        [Authorize]
        public async Task<ResponseDTO<PagedResultDTO<IssueListDTO>?>> List([Required] int page = 1)
        {
            // if admin, leader, staff
            if (isAdmin() || isLeader() || isStaff())
            {
                return await service.ListPagedDoing(page);
            }
            return new ResponseDTO<PagedResultDTO<IssueListDTO>?>(null, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet("Doing")]
        [Authorize]
        public async Task<ResponseDTO<List<IssueListDTO>?>> List()
        {
            return await service.ListDoing();
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create(IssueCreateDTO DTO)
        {
            // if admin, leader, staff
            if (isAdmin() || isLeader() || isStaff())
            {
                string? CreatorID = getUserID();
                if(CreatorID == null)
                {
                    return new ResponseDTO<bool>(false, "Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập", (int) HttpStatusCode.NotFound);
                }
                return await service.Create(DTO, Guid.Parse(CreatorID));
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int) HttpStatusCode.Forbidden);
        }

        [HttpPut("{IssueID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update([Required] Guid IssueID, [Required] IssueUpdateDTO DTO)
        {
            // if admin, leader, staff
            if (isAdmin() || isLeader() || isStaff())
            {
                return await service.Update(IssueID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpDelete("{IssueID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Delete([Required] Guid IssueID)
        {
            // if admin, leader, staff
            if (isAdmin() || isLeader() || isStaff())
            {
                return await service.Delete(IssueID);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }

        [HttpGet("{IssueID}")]
        [Authorize]
        public async Task<ResponseDTO<List<IssueDetailDTO>?>> Detail([Required] Guid IssueID)
        {
            // if admin, leader, staff
            if (isAdmin() || isLeader() || isStaff())
            {
                return await service.Detail(IssueID);
            }
            return new ResponseDTO<List<IssueDetailDTO>?>(null, "Bạn không có quyền truy cập trang này", (int)HttpStatusCode.Forbidden);
        }
    }
}
