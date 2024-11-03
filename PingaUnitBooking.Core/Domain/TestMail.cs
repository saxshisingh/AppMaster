using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Core.Domain
{
    public class TestMail
    {
        public int? MailConfigureID { get; set; }
        public decimal GroupID { get; set; }
        public int UserID { get; set; }
        public int UbmID { get; set; }
        public string Type { get; set; }
        public string ToEmail { get; set; }
        public string ToMobileNo { get; set; }
        public string TemplateID { get; set; }
        public string Message { get; set; }
        public string? Subject { get; set; }

    }

    public class MailHistory
    {
        public int MailConfigureID { get; set; }
        public decimal GroupID { get; set; }
        public int? UbmID { get; set; } 
        public string ToEmail { get; set; }
        public string SendTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int? CreatedBy { get; set; }

    }
}
