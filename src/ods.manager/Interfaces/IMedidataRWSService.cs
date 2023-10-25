
using RestSharp;

namespace Theradex.ODS.Manager.Interfaces
{
    public interface IMedidataRWSService
    {
        Task<bool> GetData(DateTime startDate, DateTime endDate, string tableName, int pageNo, int pageSize);

        Task<RestResponse> GetAndWriteToDiskWithResponse(string tableName, string uri, string responseDataFileNameWithExtensionRAW);
    }
}