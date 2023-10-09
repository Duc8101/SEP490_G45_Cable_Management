using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Model.DAO
{
    public class BaseDAO
    {
        protected readonly CableManagementContext context = new CableManagementContext();
    }
}
