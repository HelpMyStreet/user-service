using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;

namespace UserService.Core.PreCalculation
{
    public class VolunteerPrecalculator : IVolunteerPrecalculator
    {
        private readonly IRepository _repository;
        private readonly IPrecalculatedVolunteersGetter _precalculatedVolunteersGetter;

        public VolunteerPrecalculator(IRepository repository, IPrecalculatedVolunteersGetter precalculatedVolunteersGetter)
        {
            _repository = repository;
            _precalculatedVolunteersGetter = precalculatedVolunteersGetter;
        }

        public async Task LoadPreCalculatedVolunteers(CancellationToken cancellationToken)
        {
            IEnumerable<PrecalculatedVolunteerDto> precalculatedVolunteers = await _precalculatedVolunteersGetter.GetAllPrecalculatedVolunteersAsync(cancellationToken);

            await _repository.AddOrUpdatePreCalculatedVolunteers(precalculatedVolunteers);

        }
    }
}
