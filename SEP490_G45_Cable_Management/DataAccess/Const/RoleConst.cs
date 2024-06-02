using DataAccess.Enum;

namespace DataAccess.Const
{
    public class RoleConst
    {
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_LEADER = "Leader";
        public const string ROLE_STAFF = "Staff";
        public const string ROLE_WAREHOUSE_KEEPER = "WareHouse Keeper";

        public static Dictionary<int, string> getList()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic[(int)Role.Admin] = ROLE_ADMIN;
            dic[(int)Role.Leader] = ROLE_LEADER;
            dic[(int)Role.Staff] = ROLE_STAFF;
            dic[(int)Role.Warehouse_Keeper] = ROLE_WAREHOUSE_KEEPER;
            return dic;
        }
    }
}
