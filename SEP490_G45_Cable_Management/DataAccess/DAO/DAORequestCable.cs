using Common.Entity;
using DataAccess.DBContext;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAORequestCable : BaseDAO
    {
        public DAORequestCable(CableManagementContext context) : base(context)
        {
        }

        public void CreateRequestCable(RequestCable request)
        {
            _context.RequestCables.Add(request);
            Save();
        }
        public List<RequestCable> getListRequestCable(Guid requestId)
        {
            return _context.RequestCables.Include(r => r.Cable).ThenInclude(r => r.CableCategory).Include(r => r.Cable)
                .ThenInclude(r => r.Warehouse).Include(r => r.RecoveryDestWarehouse).Where(r => r.RequestId == requestId)
                .ToList();
        }
    }
}
