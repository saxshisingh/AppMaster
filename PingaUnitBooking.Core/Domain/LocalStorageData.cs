using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Core.Domain
{
    public class LocalStorageData
    {
        public string Username { get; set; }
        public int UserID { get; set; }
        public decimal GroupID { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int RoleID { get; set; }
        public bool IsLogin { get; set; }

     }
}
