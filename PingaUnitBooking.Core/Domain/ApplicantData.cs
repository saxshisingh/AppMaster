using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Core.Domain
{
    public class ApplicantData
    {
        public int? ApplicantID {  get; set; }
         public int ApplicantType {  get; set; }
         public string Applicantunitno {  get; set; }
         public DateTime? ApplicantUnitDate {  get; set; }
         public string ApplicantBuyerName {  get; set; }
         public string Applicantswd {  get; set; }
         public DateTime? Applicantdob {  get; set; }
         public decimal ApplicantAge {  get; set; }
         public string ApplicantNationality {  get; set; }
         public string ApplicantOccupation {  get; set; }
         public string ApplicantPan {  get; set; }
         public string ApplicantAdharNo {  get; set; }
         public string ApplicantCorrAddress {  get; set; }
         public int ApplicantCityID {  get; set; }
         public int ApplicantStateID {  get; set; }
         public string ApplicantCountry {  get; set; }
         public string ApplicantPIN {  get; set; }
         public string ApplicantEmail {  get; set; }
         public decimal ApplicantMob {  get; set; }
         public decimal GroupID { get; set;}
         public int UbmID  { get; set;}
         public int CreatedBy  { get; set;}
    }


    public class UbmStatus
    {
        public int UbmID { get; set; }
        public decimal GroupID { get; set; }
        public int BookingStatusID { get; set; }
        public int CreatedBy { get; set; }
        public string? Remarks { get; set; }
        public string? authorizationDropdown { get; set; }
    }
}
