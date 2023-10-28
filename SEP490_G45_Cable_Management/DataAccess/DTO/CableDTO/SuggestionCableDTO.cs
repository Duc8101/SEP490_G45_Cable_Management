using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.CableDTO
{
    public class SuggestionCableDTO
    {
        public SuggestionCableDTO()
        {
            WarehouseIds = new List<int>();
            CableCategoryIds = new List<int>();
        }
        public int Length { get; set; }
        public List<int> WarehouseIds { get; set; }
        public List<int> CableCategoryIds { get; set; }
    }
}
