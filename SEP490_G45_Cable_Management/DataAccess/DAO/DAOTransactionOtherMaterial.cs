using Common.Entity;
using Common.Enums;
using DataAccess.DBContext;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAOTransactionOtherMaterial : BaseDAO
    {
        public DAOTransactionOtherMaterial(CableManagementContext context) : base(context)
        {
        }

        public List<TransactionOtherMaterial> getListTransactionOtherMaterial(Guid transactionId)
        {
            return _context.TransactionOtherMaterials.Include(t => t.OtherMaterials).ThenInclude(t => t.OtherMaterialsCategory)
                .Where(t => t.TransactionId == transactionId).ToList();
        }

        private IQueryable<TransactionOtherMaterial> getQuery(int? otherMaterialCategoryId, int? warehouseId, int? year)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }
            IQueryable<TransactionOtherMaterial> query = _context.TransactionOtherMaterials.Where(t => t.Transaction.CreatedAt.Year == year);
            // if choose material category
            if (otherMaterialCategoryId.HasValue)
            {
                query = query.Where(t => t.OtherMaterials.OtherMaterialsCategoryId == otherMaterialCategoryId);
            }
            // if choose warehouse
            if (warehouseId.HasValue)
            {
                query = query.Where(t => t.Transaction.WarehouseId == warehouseId);
            }
            return query;
        }

        public int getQuantityPerMonth(int? otherMaterialCategoryId, int? warehouseId, int? year, int month)
        {
            IQueryable<TransactionOtherMaterial> query = getQuery(otherMaterialCategoryId, warehouseId, year);
            // get sum quantity of transaction import per month
            int sumIncrease = query.Where(t => t.Transaction.CreatedAt.Month == month
            && t.Transaction.TransactionCategoryName == TransactionCategories.Import.getDescription())
                .Sum(t => t.Quantity);
            // get sum quantity of transaction export and cancel per month
            int sumDecrease = query.Where(t => t.Transaction.CreatedAt.Month == month
            && (t.Transaction.TransactionCategoryName == TransactionCategories.Export.getDescription()
            || t.Transaction.TransactionCategoryName == TransactionCategories.Cancel.getDescription()))
                .Sum(t => t.Quantity);
            return sumIncrease - sumDecrease;
        }

        public void CreateTransactionMaterial(TransactionOtherMaterial transaction)
        {
            _context.TransactionOtherMaterials.Add(transaction);
            Save();
        }
    }
}
