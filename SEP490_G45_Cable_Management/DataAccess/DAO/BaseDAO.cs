using DataAccess.DBContext;

namespace DataAccess.DAO
{
    public class BaseDAO
    {
        private protected readonly CableManagementContext _context;
        public BaseDAO(CableManagementContext context)
        {
            _context = context;
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
