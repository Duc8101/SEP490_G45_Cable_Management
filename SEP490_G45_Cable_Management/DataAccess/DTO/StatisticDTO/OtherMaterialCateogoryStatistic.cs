using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.StatisticDTO
{
    public class OtherMaterialCateogoryStatistic
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int SumOfQuantity { get; set; }
    }
}
