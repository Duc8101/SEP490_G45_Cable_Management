using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.TransactionDTO
{
    public class TransactionHistoryDTO
    {

        public Guid TransactionId { get; set; }
        public string? TransactionCategoryName { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? WarehouseId { get; set; }
        public string? IssueCode { get; set; }
        public string? FromWarehouseName { get; set; }
        public string? ToWarehouseName { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }
}
