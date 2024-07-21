using System.ComponentModel;

namespace Common.Enums
{
    public enum TransactionCategories
    {
        [Description("Nhập Kho")]
        Import,
        [Description("Xuất Kho")]
        Export,
        [Description("Hủy")]
        Cancel
    }
}
