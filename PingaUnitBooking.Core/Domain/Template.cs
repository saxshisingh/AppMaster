using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Core.Domain
{
    public class Template
    {
        public int TemplateID { get; set; }
        public decimal GroupID { get; set; }
        public string ProcessType { get; set; }
        public int? ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string TemplateType { get; set; }
        public string VendorTemplateID { get; set; }
        public string TemplateMsg { get; set; }
        public int? IsActive { get; set; }
        public int CreatedBy { get; set; }
    }
 }
