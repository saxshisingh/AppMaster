using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Infrastructure.Interfaces
{
    public interface IDbInterface
    {
        Task<string> getREMSConnectionString();
        Task<string> getUnitBookingConnectionString();
    }
}
