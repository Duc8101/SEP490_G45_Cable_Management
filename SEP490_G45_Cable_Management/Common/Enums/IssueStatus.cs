using System.ComponentModel;

namespace Common.Enums
{
    public enum IssueStatus
    {
        [Description("Đang xử lý")]
        Doing,
        [Description("Đã xử lý")]
        Done
    }
}
