using PingaUnitBooking.Core.Domain; 

namespace PingaUnitBooking.Infrastructure.Interfaces
{
    public interface ITemplateInterface
    {
        Task<ResponseDataResults<List<KeyValuePair<int, string>>>> ProcessTypeList();
        Task<ResponseDataResults<string>> GetAppDocList(string ApplicationType, decimal GroupID);
        Task<ResponseDataResults<List<KeyValuePair<int, string>>>> GetTemplateEmbList(string ProcessType);
        Task<ResponseDataResults<List<Template>>> GetTemplateList(decimal GroupID);
        Task<ResponseDataResults<int>> SaveTemplate(Template tenp); 
        Task<ResponseDataResults<List<KeyValuePair<int, string>>>> GetProjects(decimal GroupID);
        Task<ResponseDataResults<int>> DeleteTemplate(int TemplateID);
    }
}
