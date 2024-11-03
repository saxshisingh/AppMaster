using PingaUnitBooking.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingaUnitBooking.Infrastructure.Interfaces
{
    public interface IBookingInterface
    {
        Task<ResponseDataResults<List<BookingData>>> bookingUnitList(string SearchType,string SearchText, decimal? groupID, int? userID);
        Task<ResponseDataResults<List<BookingData>>> ubmDetailsByUnitID(decimal? groupID, int? ubmID);
        Task<ResponseDataResults<List<ProjectAllData>>> getProjectDataforBooking(SearchData _searchData);
        Task<ResponseDataResults<BookingData>> addBookedUnit(BookingData _bookingData);
        Task<ResponseDataResults<int>> addPaymentModel(PaymentModel _paymentModel);
        Task<ResponseDataResults<List<PaymentModel>>> getPaymentModelList(decimal? groupID , int? ubmID);
        Task<ResponseDataResults<List<ApplicationDoc>>> getApplicantDocument(decimal? groupID , int? ubmID);
        Task<ResponseDataResults<int>> addApplicantDocuments(ApplicationDoc _doc);
        Task<ResponseDataResults<int>> DeleteAttachments(ApplicationDoc _doc);

        Task<ResponseDataResults<int>> addApplicantDetails(ApplicantData _applicantData);

        Task<ResponseDataResults<List<ApplicantData>>> getApplicantList(decimal? groupID, int? ubmID, int? appType);

        Task<ResponseDataResults<int>> deleteCoApplicant(ApplicantData _applicantData);
        Task<ResponseDataResults<int>> ChangeUbmStatus(UbmStatus _UbmStatus);
        Task<ResponseDataResults<int>> ChangeUbmAuthorization(UbmStatus _UbmStatus);
        Task<ResponseDataResults<List<string>>> GetApplicationDocument(decimal? groupID, string ApplicationType);
        Task<ResponseDataResults<int>> DeletePaymentPlan(decimal? groupID, decimal? paymentID);
        Task<ResponseDataResults<UnitInfo>> GetUnitInfo(decimal? groupID,int UnitID);

        Task<ResponseDataResults<string>> getTncTemplate(decimal groupID, decimal? ubmID);

        Task<ResponseDataResults<List<UnitLogs>>> GetUnitLogs(decimal? groupID, int? ubmID);
        Task<ResponseDataResults<List<KeyValuePair<int, string>>>> GetBrokerList(decimal GroupID);


    }
}
