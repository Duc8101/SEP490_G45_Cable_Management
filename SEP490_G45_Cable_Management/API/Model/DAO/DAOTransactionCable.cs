using DataAccess.Const;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Model.DAO
{
    public class DAOTransactionCable : BaseDAO
    {
        public async Task<List<TransactionCable>> getList(Guid TransactionID)
        {
            return await context.TransactionCables.Include(t => t.Cable).ThenInclude(t => t.CableCategory)
                .Where(t => t.TransactionId == TransactionID).ToListAsync();
        }

        private IQueryable<TransactionCable> getQuery(int? CableCategoryID, int? WarehouseID, int? year)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            IQueryable<TransactionCable> query = context.TransactionCables.Where(t => t.Transaction.CreatedAt.Year == year);
            // if choose category
            if (CableCategoryID != null)
            {
                query = query.Where(t => t.Cable.CableCategory.CableCategoryId == CableCategoryID);
            }
            // if choose warehouse
            if (WarehouseID != null)
            {
                query = query.Where(t => t.Transaction.WarehouseId == WarehouseID);
            }
            return query;
        }

        public async Task<int> getLengthPerMonth(int? CableCategoryID, int? WarehouseID, int? year, int month)
        {
            IQueryable<TransactionCable> query = getQuery(CableCategoryID, WarehouseID, year);
            int sumIncrease = await query.Where(t => t.Transaction.CreatedAt.Month == month
            && t.Transaction.TransactionCategoryName == TransactionCategoryConst.CATEGORY_IMPORT)
                .SumAsync(t => t.Length);
            int sumDecrease = await query.Where(t => t.Transaction.CreatedAt.Month == month
           && (t.Transaction.TransactionCategoryName == TransactionCategoryConst.CATEGORY_EXPORT
           || t.Transaction.TransactionCategoryName == TransactionCategoryConst.CATEGORY_CANCEL))
               .SumAsync(t => t.Length);
            return sumIncrease - sumDecrease;
        }

        public void CreateTransactionCable(TransactionCable transaction)
        {
            context.TransactionCables.Add(transaction);
            context.SaveChanges();
        }
    }
}
