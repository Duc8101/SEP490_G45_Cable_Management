using DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.DAO
{
    public class DAORequestCable : BaseDAO
    {
        public async Task<int> CreateRequestCable(RequestCable request)
        {
            await context.RequestCables.AddAsync(request);
            return await context.SaveChangesAsync();
        }
    }
}
