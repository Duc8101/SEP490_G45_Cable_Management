using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.DAO
{
    public class DAORole : BaseDAO
    {
        public async Task<Role?> getRole(int RoleID)
        {
            return await context.Roles.SingleOrDefaultAsync(r => r.RoleId == RoleID);
        }
    }
}
