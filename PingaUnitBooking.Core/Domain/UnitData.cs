using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Core.Domain
{
    public class UnitData
    {
        public string towerName { get; set; }
        public string floorName { get; set; }
        public decimal floorID { get; set; }
        public int projectID { get; set; }
        public string projectName { get; set; }
        public string projectAddress { get; set; }
        public string locationName { get; set; }
        public decimal locationID { get; set; }
        public decimal companyID { get; set; }
        public decimal unitID { get; set; }
        public string unitNo { get; set; }
        public string categoryName { get; set; }
        public decimal netAmount { get; set; }
        public decimal area { get; set; }
        public decimal rate { get; set; }
        public decimal unitSuperArea { get; set; }
        public decimal unitSuperAreaRate { get; set; }
        public decimal unitBuiltUpArea { get; set; }
        public decimal unitBuiltUpAreaRate { get; set; }
        public decimal unitTerraceArea { get; set; }
        public decimal unitTerraceAreaRate { get; set; }
        public decimal unitBalconyArea { get; set; }
        public decimal unitBalconyAreaRate { get; set; }
        public decimal basicAmount { get; set; }
        public int unitStatus { get; set; }
        public decimal unitCarpetArea { get; set; }
        public decimal unitCarpetAreaRate { get; set; }
        public decimal additionalCharge { get; set; }

        public decimal discountAmount { get; set; }
        public decimal minSaleAmount { get; set; }
        public decimal maxSaleAmount { get; set; }
        public paymentPlan payment { get; set; }
        public intrestPlan intrest { get; set; }
        public int userID { get; set; }
        public decimal groupID { get; set; }
        public string roleName { get; set; }
        public int? payplanID { get; set; }
        public int? intPlanID { get; set; }
        public int? SchemeID { get; set; }
        public string SchemeName { get; set; }
        public string CreatedBy { get; set; }

    }

    public class paymentPlan
    {
      public decimal payplanID { get; set; }
      public string payplanName { get; set;}
    }
    public class intrestPlan
    {
        public decimal? intPlanID { get; set; }
        public string intPlanName { get; set; }
    }
}