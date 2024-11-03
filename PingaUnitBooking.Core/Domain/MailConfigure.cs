using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Core.Domain
{
    public class MailConfigure
    {
        public int MailConfigureID { get; set; }
        public decimal GroupID { get; set; }
        public string ConfigureType { get; set; }
        public string SenderName { get; set; }
        public string SMTPServer { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? PerHourMail { get; set; } 
        public int? PortNo { get; set; } 
        public string SmsUrl { get; set; }
        public string TokenWA { get; set; }
        public string PhoneWA { get; set; } 
        public string BasedOn { get; set; }
        public string Provider { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}


