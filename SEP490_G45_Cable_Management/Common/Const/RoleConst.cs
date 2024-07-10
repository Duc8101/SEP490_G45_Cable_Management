using System.ComponentModel;

namespace Common.Const
{
    public enum RoleConst
    {
        [Description("")]
        None = 0,
        [Description("Admin")]
        Admin = 1,
        [Description("Leader")]
        Leader = 2,
        [Description("Staff")]
        Staff = 3,
        [Description("WareHouse Keeper")]
        Warehouse_Keeper = 4,
    }
}
