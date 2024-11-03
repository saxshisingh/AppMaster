using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Core.Domain
{
   
        // Company DTO
        public class mstCompany
        {
            public decimal companyID { get; set; }
           
            public string companyName { get; set; }
                 }
        // Location DTO
        public class mstLocation
        {
            public decimal locationID { get; set; }
            public string locationName { get; set; }
          
        }
        // Project DTO
        public class mstProject
        {
            public decimal projectID { get; set; }
            public string projectName { get; set; }
          
        }

    // Tower DTO
    public class mstTower
    {
        public decimal towerID { get; set; }
        public string towerName { get; set; }

    }
    // FLOOR DTO
    public class mstFloor
    {
        public decimal floorID { get; set; }
        public string floorName { get; set; }

    }
    // FLOOR DTO
    public class mstunit
    {
        public decimal unitID { get; set; }
        public string unitName { get; set; }

    }

    public class ProjectAllData 
        {
           public List<mstCompany> company { get; set; }
           public List<mstLocation> location { get; set;}
           public List<mstProject> project { get; set;}
           public List<mstTower> tower { get; set;}
           public List<mstFloor> floor { get; set;}
           public List<mstunit> unitData { get; set;}
           
        }
    // Search DTO
    public class SearchData
    {
        public string search { get; set; }
        public string type { get; set; }
        public ProjectAllData projectAllData { get; set; }
        public string? getDataType { get; set; }
        public string searchType { get; set; }
        public int userID { get; set; }
        public decimal groupID { get; set; }
    }

    public class BindProjectPermissionDTO
    {
        public List<int> useridList { get; set; }
        public int? ProjectPermissionID { get; set; }
        public int userID { get; set; }
        public int companyid { get; set; }
        public string companyName { get; set; }
        public int locationid { get; set; }
        public string locationName { get; set; }
        public List<int> towerList { get; set; }
        public int towerID { get; set; }
        public string towerName { get; set; }
        public int projectid { get; set; }
        public string projectName { get; set; }
        public int CreatedBy { get; set; }
        public decimal GroupID { get; set; }
    }
}
