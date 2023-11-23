using API.Services.IService;
using DataAccess.DTO;
using DataAccess.DTO.NodeMaterialCategoryDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class NodeMaterialCategoryController : BaseAPIController
    {
        private readonly INodeMaterialCategoryService service;

        public NodeMaterialCategoryController(INodeMaterialCategoryService service)
        {
            this.service = service;
        }

        [HttpPut("{NodeID}")]
        [Authorize]
        public async Task<ResponseDTO<bool>> Update([Required] Guid NodeID, [Required] NodeMaterialCategoryUpdateDTO DTO)
        {
            // if admin
            if (isAdmin())
            {
                return await service.Update(NodeID, DTO);
            }
            return new ResponseDTO<bool>(false, "Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
        }
    }
}
