using Common.Entity;
using DataAccess.DBContext;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAORequestOtherMaterial : BaseDAO
    {
        public DAORequestOtherMaterial(CableManagementContext context) : base(context)
        {
        }

        public void CreateRequestOtherMaterial(RequestOtherMaterial request)
        {
            _context.RequestOtherMaterials.Add(request);
            Save();
        }

        public List<RequestOtherMaterial> getListRequestOtherMaterial(Guid requestId)
        {
            return _context.RequestOtherMaterials.Include(r => r.OtherMaterials).ThenInclude(r => r.OtherMaterialsCategory)
                .Include(r => r.OtherMaterials).ThenInclude(r => r.Warehouse).Include(r => r.RecoveryDestWarehouse)
                .Where(r => r.RequestId == requestId).ToList();
        }
    }
}
