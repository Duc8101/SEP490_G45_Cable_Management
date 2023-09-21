using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.CommonDTO
{
    public class MetaDataDTO
    {
        public int CurrentPage { get; set; }
        // number of row in a page
        public int PageSize { get; set; }
        // number of all rows
        public int RowCount { get; set; }
    }
}
