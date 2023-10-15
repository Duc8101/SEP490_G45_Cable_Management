using DataAccess.Entity;

namespace API.Model.DAO
{
    public class DAORequestOtherMaterial : BaseDAO
    {
        public async Task CreateRequestOtherMaterial(RequestOtherMaterial request)
        {
            await context.RequestOtherMaterials.AddAsync(request);
            await context.SaveChangesAsync();
        }
    }
}
