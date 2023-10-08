using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Model
{
    public class BaseDAO
    {
        protected readonly CableManagementContext context = new CableManagementContext();
    }
}
