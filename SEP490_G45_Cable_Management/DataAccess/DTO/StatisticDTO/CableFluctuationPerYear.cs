using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.StatisticDTO
{
    public class CableFluctuationPerYear
    {
        public string? CableName { get; set; }
        public int? WarehouseId { get; set; }
        public int LengthInJanuary { get; set; }
        public int LengthInFebruary { get; set; }
        public int LengthInMarch { get; set; }
        public int LengthInApril { get; set; }
        public int LengthInMay { get; set; }
        public int LengthInJune { get; set; }
        public int LengthInJuly { get; set; }
        public int LengthInAugust { get; set; }
        public int LengthInSeptember { get; set; }
        public int LengthInOctober { get; set; }
        public int LengthInNovember { get; set; }
        public int LengthInDecember { get; set; }
    }
}
