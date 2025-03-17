using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elkollen.Models
{
    public class QuantityModel
    {
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public decimal amount { get; set; }

        // Formatted display properties
        public string FormattedStartDate => startTime.ToString("yyyy-MM-dd");
        public string FormattedEndDate => endTime.ToString("yyyy-MM-dd");

        public string FormattedStartTime => startTime.ToString("HH:mm");
        public string FormattedEndTime => endTime.ToString("HH:mm");
        public string FormattedQuantity => $"{(amount):F2} kWh";
    }
}