using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elkollen.Models
{
    public class QuantityPriceModel
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalCost => (Quantity * Price);

        public string FormattedPrice => $"{(Price * 100):F2} öre";
        public string FormattedQuantity => $"{(Quantity):F3} kWh";
        public string FormattedTotalcost => $"{(TotalCost):F2} kr";
    }
}