using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.TransactionDTO
{
    public class TransactionDetailDTO : TransactionHistoryDTO
    {
        public TransactionDetailDTO()
        {
            CableTransactions = new List<TransactionCableDTO>();
            MaterialsTransaction = new List<TransactionMaterialDTO>();
        }
        public List<TransactionCableDTO> CableTransactions { get; set; }
        public List<TransactionMaterialDTO> MaterialsTransaction { get; set; }
    }
}
