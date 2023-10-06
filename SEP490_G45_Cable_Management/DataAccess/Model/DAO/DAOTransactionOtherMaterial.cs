using DataAccess.Const;
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

        private IQueryable<TransactionOtherMaterial> getQuery(int? MaterialCategoryID, int? WarehouseID, int? year)
        {
            if(year == null)
            {
                year = DateTime.Now.Year;
            }
            IQueryable<TransactionOtherMaterial> query = context.TransactionOtherMaterials.Where(t => t.Transaction.CreatedAt.Year <= year);
            // if choose category
            if(MaterialCategoryID != null)
            {
                query = query.Where(t => t.OtherMaterials.OtherMaterialsCategory.OtherMaterialsCategoryId == MaterialCategoryID);
            }
            // if choose warehouse
            if(WarehouseID != null)
            {
                query = query.Where(t => t.Transaction.WarehouseId == WarehouseID);
            }
            return query;
        }

        public async Task<int> getQuantityPerMonth(int? MaterialCategoryID, int? WarehouseID, int? year,int month)
        {
            IQueryable<TransactionOtherMaterial> query = getQuery(MaterialCategoryID, WarehouseID, year);
            int SumIncrease = await query.Where(t => t.Transaction.CreatedAt.Month == month
            && t.Transaction.TransactionCategoryName == TransactionCategoryConst.CATEGORY_IMPORT)
                .SumAsync(t => t.Quantity);
            int SumDecrease = await query.Where(t => t.Transaction.CreatedAt.Month == month
            && (t.Transaction.TransactionCategoryName == TransactionCategoryConst.CATEGORY_EXPORT 
            || t.Transaction.TransactionCategoryName == TransactionCategoryConst.CATEGORY_CANCEL))
                .SumAsync(t => t.Quantity);
            return SumIncrease - SumDecrease;
        }
    }
}
