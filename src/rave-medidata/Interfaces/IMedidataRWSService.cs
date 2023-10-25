
namespace Theradex.Rave.Medidata.Interfaces
{
    public interface IMedidataRWSService
    {
        Task<bool> GetData(DateTime startDate, DateTime endDate, string tableName, int pageNo, int pageSize);
    }
}