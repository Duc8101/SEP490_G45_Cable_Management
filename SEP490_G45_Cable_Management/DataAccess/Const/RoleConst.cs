using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Const
{
    public class RoleConst
    {
        public const int INT_ROLE_NONE = 0;
        public const int INT_ROLE_ADMIN = 1;
        public const int INT_ROLE_LEADER = 2;
        public const int INT_ROLE_STAFF = 3;
        public const int INT_ROLE_WAREHOUSE_KEEPER = 4;
        public const string STRING_ROLE_ADMIN = "Admin";
        public const string STRING_ROLE_LEADER = "Leader";
        public const string STRING_ROLE_STAFF = "Staff";
        public const string STRING_ROLE_WAREHOUSE_KEEPER = "WareHouse Keeper";

        public static Dictionary<int, string> getList()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic[INT_ROLE_ADMIN] = STRING_ROLE_ADMIN;
            dic[INT_ROLE_LEADER] = STRING_ROLE_LEADER;
            dic[INT_ROLE_STAFF] = STRING_ROLE_STAFF;
            dic[INT_ROLE_WAREHOUSE_KEEPER] = STRING_ROLE_WAREHOUSE_KEEPER;
            return dic;
        }
    }
}
