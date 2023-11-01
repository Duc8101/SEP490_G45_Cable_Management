using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.StatisticDTO
{
    public class RouteStatistic
    {
        public int OtherMaterialsCategoryId { get; set; }
        public string OtherMaterialsCategoryName { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
