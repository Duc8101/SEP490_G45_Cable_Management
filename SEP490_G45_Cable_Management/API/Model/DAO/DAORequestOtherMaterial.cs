using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Model.DAO
{
    public class DAORequestOtherMaterial : BaseDAO
    {
        public async Task CreateRequestOtherMaterial(RequestOtherMaterial request)
        {
            await context.RequestOtherMaterials.AddAsync(request);
            await context.SaveChangesAsync();
        }

        public async Task<List<RequestOtherMaterial>> getList(Guid RequestID)
        {
            return await context.RequestOtherMaterials.Include(r => r.OtherMaterials).ThenInclude(r => r.OtherMaterialsCategory)
                .Include(r => r.OtherMaterials).ThenInclude(r => r.Warehouse).Where(r => r.RequestId == RequestID).ToListAsync();
        }
    }
}
