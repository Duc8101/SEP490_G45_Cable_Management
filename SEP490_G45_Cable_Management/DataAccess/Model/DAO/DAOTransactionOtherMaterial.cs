using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.DAO
{
    public class DAOTransactionOtherMaterial : BaseDAO
    {
        public async Task<List<TransactionOtherMaterial>> getList(Guid TransactionID)
        {
            return await context.TransactionOtherMaterials.Where(t => t.TransactionId == TransactionID).ToListAsync();
        }
    }
}
