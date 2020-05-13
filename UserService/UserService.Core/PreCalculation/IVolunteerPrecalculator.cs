using System.Threading;
using System.Threading.Tasks;

namespace UserService.Core.PreCalculation
{
    public interface IVolunteerPrecalculator
    {
        Task LoadPreCalculatedVolunteers(CancellationToken cancellationToken);
    }
}