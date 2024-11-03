using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Core.Domain
{
  public class ApplicationDoc
   {
        public int DocumentID { get; set; }
        public int UbmKycid { get; set; }
        public string  DocumentName { get; set; }
        public string ApplicationType { get; set; }
        public string DocumentUrl { get; set; }
        public List<IFormFile> DocumentFile { get; set; }
        public int UserID { get; set; }
        public bool Mandatory { get; set; }
        public decimal GroupID { get; set; } 
        public decimal MobileNo { get; set; } 
        public int unitID { get; set; } 
        public int UbmID { get; set; } 

    }
 }


