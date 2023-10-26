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
        public void CreateRequestCable(RequestCable request)
        {
            context.RequestCables.Add(request);
            context.SaveChanges();
        }

        public async Task<List<RequestCable>> getList(Guid RequestID)
        {
            return await context.RequestCables.Include(r => r.Cable).ThenInclude(r => r.CableCategory).Include(r => r.Cable)
                .ThenInclude(r => r.Warehouse).Where(r => r.RequestId == RequestID).ToListAsync();
        }
        public void RemoveRequestCable(RequestCable request)
        {
            context.RequestCables.Remove(request);
            context.SaveChanges();
        }
    }
}
