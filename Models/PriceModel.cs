using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elkollen.Models
{
    public class PriceModel
    {
        public DateTime startDate { get; set; }
        public decimal price { get; set; }
        public bool complete { get; set; }

        // formaterar datumen och priset för bättre läsbarhet
        public string FormattedStartDate => startDate.ToString("yyyy-MM-dd");
        public string FormattedStartTime => startDate.ToString("HH:mm");
        public string FormattedPrice => $"{(price * 100):F2} öre";
    }
}