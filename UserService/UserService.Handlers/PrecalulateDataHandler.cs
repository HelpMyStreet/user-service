using MediatR;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Domains.Entities;
using UserService.Core.PreCalculation;

namespace UserService.Handlers
{
    public class PrecalulateDataHandler : IRequestHandler<PrecalulateDataRequest, PrecalulateDataResponse>
    {
        private readonly IVolunteerPrecalculator _volunteerPrecalculator;

        public PrecalulateDataHandler(IVolunteerPrecalculator volunteerPrecalculator)
        {
            _volunteerPrecalculator = volunteerPrecalculator;
        }

        public async Task<PrecalulateDataResponse> Handle(PrecalulateDataRequest request, CancellationToken cancellationToken)
        {
            await _volunteerPrecalculator.LoadPreCalculatedVolunteers(cancellationToken);

            return new PrecalulateDataResponse();
        }
    }
}
