using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.CommonDTO
{
    public abstract class PagedBaseDTO : MetaDataDTO
    {
        // number of page
        public int PageCount { get; set; }

        public int FirstRowOnPage
        {
            get { return Math.Min(RowCount, (CurrentPage - 1) * PageSize + 1); }
        }

        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }
    }
}
