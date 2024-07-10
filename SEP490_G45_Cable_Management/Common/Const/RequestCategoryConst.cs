using System.ComponentModel;

namespace Common.Const
{
    public enum RequestCategoryConst
    {
        [Description("Xuất kho")]
        Export = 1,
        [Description("Thu hồi")]
        Recovery = 3,
        [Description("Chuyển kho")]
        Deliver = 4,
        [Description("Hủy trong kho")]
        Cancel_Inside = 5,
        [Description("Hủy ngoài kho")]
        Cancel_Outside = 6,
    }
}
