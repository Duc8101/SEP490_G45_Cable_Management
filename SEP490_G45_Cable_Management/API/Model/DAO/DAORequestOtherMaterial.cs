using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Model.DAO
{
    public class DAORequestOtherMaterial : BaseDAO
    {
        public void CreateRequestOtherMaterial(RequestOtherMaterial request)
        {
            context.RequestOtherMaterials.Add(request);
            context.SaveChanges();
        }

        public async Task<List<RequestOtherMaterial>> getList(Guid RequestID)
        {
            return await context.RequestOtherMaterials.Include(r => r.OtherMaterials).ThenInclude(r => r.OtherMaterialsCategory)
                .Include(r => r.OtherMaterials).ThenInclude(r => r.Warehouse).Include(r => r.RecoveryDestWarehouse).Where(r => r.RequestId == RequestID).ToListAsync();
        }

        public void RemoveRequestMaterial(RequestOtherMaterial request)
        {
            context.RequestOtherMaterials.Remove(request);
            context.SaveChanges();
        }
    }
}
