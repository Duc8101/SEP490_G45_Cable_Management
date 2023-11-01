using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.StatisticDTO
{
    public class OtherMaterialCategoryStatistic
    {
        public int CategoryId { get; set; }
        //public string? CategoryName { get; set; }
        public string CategoryName { get; set; } = null!;
        public int SumOfQuantity { get; set; }
    }
}
