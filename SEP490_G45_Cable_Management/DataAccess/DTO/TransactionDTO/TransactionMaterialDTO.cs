using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.TransactionDTO
{
    public class TransactionMaterialDTO
    {
        public Guid TransactionId { get; set; }
        public int OtherMaterialsId { get; set; }
        public string? OtherMaterialsCategoryName { get; set; }
        public string? Code { get; set; }
        public string? Status { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}
