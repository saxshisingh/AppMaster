using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Core.Domain
{
    public class Dashboard
    {
        public int? TotalUnit { get; set; }
        public int? SoldUnit { get; set; }
        public int? ProgressUnit { get; set; }
        public int? UnsoldUnit { get; set; }
        public List<SaleSummary> SaleSummary { get; set; }
        public List<BookingAmount> BookingAmount { get; set; }
        public List<UnitSaleProgress> UnitSaleProgress { get; set; }
    }
    public class SaleSummary
    {
        public string  SaleMonth    { get; set; }
        public int? TotalUnit { get; set; }
      
    }
    public class BookingAmount
    {
        public string CollectionMonth { get; set; }
        public double Amount { get; set; }

    }
    public class UnitSaleProgress
    {
        public int UbmID { get; set; }
        public string BookingType { get; set; }
        public string ProjectName { get; set; }
        public string UnitNo { get; set; }
        public string SalesPersonName { get; set; } 
        public string StatusDate { get; set; }
        public string StatusName { get; set; } 
        public int MailConfigureID { get; set; }

    }
}
