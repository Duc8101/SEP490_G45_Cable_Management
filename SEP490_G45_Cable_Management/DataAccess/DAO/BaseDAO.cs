using DataAccess.DBContext;

namespace DataAccess.DAO
{
    public class BaseDAO
    {
        internal readonly CableManagementContext context = new CableManagementContext();
    }
}
