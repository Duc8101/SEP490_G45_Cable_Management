using Common.Const;
using Common.Entity;
using DataAccess.DBContext;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAOTransactionCable : BaseDAO
    {
        public DAOTransactionCable(CableManagementContext context) : base(context)
        {
        }

        public List<TransactionCable> getListTransactionCable(Guid transactionId)
        {
            return _context.TransactionCables.Include(t => t.Cable).ThenInclude(t => t.CableCategory)
                .Where(t => t.TransactionId == transactionId).ToList();
        }

        private IQueryable<TransactionCable> getQuery(int? cableCategoryId, int? warehouseId, int? year)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            IQueryable<TransactionCable> query = _context.TransactionCables.Where(t => t.Transaction.CreatedAt.Year == year);
            // if choose category
            if (cableCategoryId.HasValue)
            {
                query = query.Where(t => t.Cable.CableCategoryId == cableCategoryId);
            }
            // if choose warehouse
            if (warehouseId.HasValue)
            {
                query = query.Where(t => t.Transaction.WarehouseId == warehouseId);
            }
            return query;
        }

        public int getLengthPerMonth(int? cableCategoryId, int? warehouseId, int? year, int month)
        {
            IQueryable<TransactionCable> query = getQuery(cableCategoryId, warehouseId, year);
            int sumIncrease = query.Where(t => t.Transaction.CreatedAt.Month == month
            && t.Transaction.TransactionCategoryName == TransactionCategoryConst.CATEGORY_IMPORT)
                .Sum(t => t.Length);
            int sumDecrease = query.Where(t => t.Transaction.CreatedAt.Month == month
           && (t.Transaction.TransactionCategoryName == TransactionCategoryConst.CATEGORY_EXPORT
           || t.Transaction.TransactionCategoryName == TransactionCategoryConst.CATEGORY_CANCEL))
               .Sum(t => t.Length);
            return sumIncrease - sumDecrease;
        }

        public void CreateTransactionCable(TransactionCable transaction)
        {
            _context.TransactionCables.Add(transaction);
            Save();
        }
    }
}
