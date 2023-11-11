using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Model.DAO
{
    public class DAORequestCable : BaseDAO
    {
        public async Task CreateRequestCable(RequestCable request)
        {
            context.RequestCables.Add(request);
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
