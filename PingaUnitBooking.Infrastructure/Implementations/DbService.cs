using Microsoft.Extensions.Configuration;
using PingaUnitBooking.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Infrastructure.Implementations
{
    public class DbService : IDbInterface
    {
       
        private readonly IConfiguration _configuration;
        public DbService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> getREMSConnectionString()
        {
            return await Task.FromResult(_configuration.GetConnectionString("RemsConn"));
        }
        public async Task<string> getUnitBookingConnectionString()
        {
            return await Task.FromResult(_configuration.GetConnectionString("RemsUnitBooking"));
        }

    }
}
