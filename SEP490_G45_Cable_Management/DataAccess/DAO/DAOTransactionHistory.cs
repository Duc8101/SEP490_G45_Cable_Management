using Common.Entity;
using Common.Enums;
using DataAccess.DBContext;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAOTransactionHistory : BaseDAO
    {
        public DAOTransactionHistory(CableManagementContext context) : base(context)
        {
        }

        private IQueryable<TransactionHistory> getQuery(string? filter, int? wareHouseId)
        {
            IQueryable<TransactionHistory> query = _context.TransactionHistories.Include(w => w.Issue).Include(w => w.FromWarehouse)
                .Include(w => w.ToWarehouse).Where(t => t.IsDeleted == false);
            if (filter != null && filter.Trim().Length > 0)
            {
                query = query.Where(t => t.Warehouse != null && t.Warehouse.WarehouseName.ToLower().Contains(filter.ToLower().Trim())
                || t.TransactionCategoryName.Contains(filter.ToLower().Trim()));
            }
            if (wareHouseId.HasValue)
            {
                query = query.Where(t => t.WarehouseId == wareHouseId);
            }
            return query;
        }
        public List<TransactionHistory> getListTransactionHistory(string? filter, int? wareHouseId, int page)
        {
            IQueryable<TransactionHistory> query = getQuery(filter, wareHouseId);
            return query.OrderByDescending(t => t.CreatedAt).Skip((int) PageSize.Size * (page - 1)).Take((int)PageSize.Size)
                .ToList();
        }

        public int getRowCount(string? filter, int? wareHouseId)
        {
            IQueryable<TransactionHistory> query = getQuery(filter, wareHouseId);
            return query.Count();
        }

        public TransactionHistory? getTransactionHistory(Guid transactionId)
        {
            return _context.TransactionHistories.Include(w => w.Issue).Include(w => w.FromWarehouse)
                .Include(w => w.ToWarehouse).SingleOrDefault(t => t.TransactionId == transactionId);
        }

        public void CreateTransactionHistory(TransactionHistory history)
        {
            _context.TransactionHistories.AddAsync(history);
            Save();
        }
    }
}
