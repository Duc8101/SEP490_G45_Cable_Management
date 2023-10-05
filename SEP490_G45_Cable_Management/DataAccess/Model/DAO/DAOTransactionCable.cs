using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.DAO
{
    public class DAOTransactionCable : BaseDAO
    {
        public async Task<List<TransactionCable>> getList(Guid TransactionID)
        {
            return await context.TransactionCables.Where(t => t.TransactionId == TransactionID).ToListAsync();
        }
    }
}
