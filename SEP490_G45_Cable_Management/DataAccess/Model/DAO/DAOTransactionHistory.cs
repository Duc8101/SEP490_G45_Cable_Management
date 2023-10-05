using DataAccess.Const;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.DAO
{
    public class DAOTransactionHistory : BaseDAO
    {
        private IQueryable<TransactionHistory> getQuery(string? filter, int? WareHouseID)
        {
            IQueryable<TransactionHistory> query = context.TransactionHistories.Where(t => t.IsDeleted == false);
            if (filter != null && filter.Trim().Length != 0)
            {
                query = query.Where(t => (t.Warehouse != null && t.Warehouse.WarehouseName != null && t.Warehouse.WarehouseName.ToLower().Contains(filter.ToLower().Trim()))
                || (t.TransactionCategoryName != null && t.TransactionCategoryName.Contains(filter.ToLower().Trim())));
            }
            if (WareHouseID != null)
            {
                query = query.Where(t => t.WarehouseId == WareHouseID);
            }
            return query;
        }
        public async Task<List<TransactionHistory>> getList(string? filter, int? WareHouseID, int page)
        {
            IQueryable<TransactionHistory> query = getQuery(filter, WareHouseID);
            query = query.OrderByDescending(t => t.CreatedAt).Skip(PageSizeConst.MAX_TRANSACTION_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_TRANSACTION_LIST_IN_PAGE);
            return await query.ToListAsync();
        }

        public async Task<int> getRowCount(string? filter, int? WareHouseID)
        {
            IQueryable<TransactionHistory> query = getQuery(filter, WareHouseID);
            return await query.CountAsync();
        }
        public async Task<TransactionHistory?> getTransactionHistory(Guid TransactionID)
        {
            return await context.TransactionHistories.SingleOrDefaultAsync(t => t.TransactionId == TransactionID);
        }
    }
}
