using System.Threading.Tasks;

namespace UserService.UnitTests
{
    public interface ITestDataGetter
    {
        Task<string> GetDataAsync();
    }
}