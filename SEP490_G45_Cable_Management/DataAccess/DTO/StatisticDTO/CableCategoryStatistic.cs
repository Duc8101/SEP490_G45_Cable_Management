using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.StatisticDTO
{
    public class CableCategoryStatistic
    {
        public int CableCategoryId { get; set; }
        public string CableCategoryName { get; set; } = null!;

        public int SumOfLength { get; set; }
    }
}
