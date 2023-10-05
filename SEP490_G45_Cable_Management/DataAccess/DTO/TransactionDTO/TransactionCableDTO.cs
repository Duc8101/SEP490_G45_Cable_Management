using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.TransactionDTO
{
    public class TransactionCableDTO
    {
        public Guid TransactionId { get; set; }
        public Guid CableId { get; set; }
        public string CableCategoryName { get; set; } = null!;
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public int Length { get; set; }
        public string? Note { get; set; }
    }
}
