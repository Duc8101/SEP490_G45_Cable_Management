using Common.Entity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAORequestCable : BaseDAO
    {
        public async Task CreateRequestCable(RequestCable request)
        {
            await context.RequestCables.AddAsync(request);
            await context.SaveChangesAsync();
        }
        public async Task<List<RequestCable>> getList(Guid RequestID)
        {
            return await context.RequestCables.Include(r => r.Cable).ThenInclude(r => r.CableCategory).Include(r => r.Cable)
                .ThenInclude(r => r.Warehouse).Include(r => r.RecoveryDestWarehouse).Where(r => r.RequestId == RequestID)
                .ToListAsync();
        }
    }
}
