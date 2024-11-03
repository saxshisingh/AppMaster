using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Infrastructure.Helpers
{
    public class ValidateLicenseHelper
    {
        public bool ValidateLicense(string Credential, string CredentialInfo, string ETADLLITEVITCA)
        {
            DateTime dateTime = DateTime.Now;
            string formattedDate = dateTime.ToString("dd-MMM-yyyy HH:mm:ss");
            if (EnDcHelper.EncryptionDecryption(Credential, false).ToUpper() == "FALSE" && DateTime.Parse(EnDcHelper.EncryptionDecryption(CredentialInfo, false)) >= DateTime.Parse(formattedDate))
            {
                if(DateTime.Parse(EnDcHelper.EncryptionDecryption(ETADLLITEVITCA, false)) >= DateTime.Parse(formattedDate))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;

            }
        }
    }
}
