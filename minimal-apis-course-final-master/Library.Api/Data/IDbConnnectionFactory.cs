using System.Data;

namespace Library.Api.Data
{
    public interface IDbConnnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}
