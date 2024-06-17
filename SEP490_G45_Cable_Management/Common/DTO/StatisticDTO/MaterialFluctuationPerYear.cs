using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.StatisticDTO
{
    public class MaterialFluctuationPerYear
    {
        public string? MaterialName { get; set; }
        public int? WarehouseId { get; set; }
        public int QuantityInJanuary { get; set; }
        public int QuantityInFebruary { get; set; }
        public int QuantityInMarch { get; set; }
        public int QuantityInApril { get; set; }
        public int QuantityInMay { get; set; }
        public int QuantityInJune { get; set; }
        public int QuantityInJuly { get; set; }
        public int QuantityInAugust { get; set; }
        public int QuantityInSeptember { get; set; }
        public int QuantityInOctober { get; set; }
        public int QuantityInNovember { get; set; }
        public int QuantityInDecember { get; set; }
    }
}
