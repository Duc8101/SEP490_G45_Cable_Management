using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.DAO
{
    public class DAORequestCategory : BaseDAO
    {
        public async Task<RequestCategory?> getRequestCategory(int ID)
        {
            return await context.RequestCategories.SingleOrDefaultAsync(r => r.RequestCategoryId == ID);
        }
    }
}
