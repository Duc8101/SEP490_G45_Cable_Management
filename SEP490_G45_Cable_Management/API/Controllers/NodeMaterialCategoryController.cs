using API.Services.Service;
using DataAccess.DTO;
using DataAccess.DTO.CableCategoryDTO;
using DataAccess.DTO.NodeMaterialCategoryDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Xml.Linq;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class NodeMaterialCategoryController : BaseAPIController
    {
        private readonly NodeMaterialCategoryService service = new NodeMaterialCategoryService();
        [HttpPost]
        [Authorize]
        public async Task<ResponseDTO<bool>> Create([Required] NodeMaterialCategoryCreateDTO DTO)
        {
            // if admin or leader
            if (isAdmin() || isLeader())
            {
                return await service.Create(DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int) HttpStatusCode.Forbidden);
        }
    }
}
