
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Extensions;

namespace UserService.Handlers
{
    public class GetNumberOfVolunteersInBoundaryHandler : IRequestHandler<GetNumberOfVolunteersInBoundaryRequest, GetNumberOfVolunteersInBoundaryResponse>
    {
        private readonly IVolunteerCache _volunteerCache;

        public GetNumberOfVolunteersInBoundaryHandler(IVolunteerCache volunteerCache)
        {
            _volunteerCache = volunteerCache;
        }

        public async Task<GetNumberOfVolunteersInBoundaryResponse> Handle(GetNumberOfVolunteersInBoundaryRequest request, CancellationToken cancellationToken)
        {
            IsVerifiedType verifiedTypes = IsVerifiedType.IsVerified | IsVerifiedType.IsNotVerified;

            IEnumerable<CachedVolunteerDto> cachedHelpersDtos = await _volunteerCache.GetCachedVolunteersAsync(VolunteerType.Helper, verifiedTypes, cancellationToken);
            IEnumerable<CachedVolunteerDto> cachedStreetChampionDtos = await _volunteerCache.GetCachedVolunteersAsync(VolunteerType.StreetChampion, verifiedTypes, cancellationToken);

            int numberOfHelpersInBoundary = cachedHelpersDtos.WhereWithinBoundary(request.SwLatitude, request.SwLongitude, request.NeLatitude, request.NeLongitude).Count();

            int numberOfStreetChampionsInBoundary = cachedStreetChampionDtos.WhereWithinBoundary(request.SwLatitude, request.SwLongitude, request.NeLatitude, request.NeLongitude).Count();

            GetNumberOfVolunteersInBoundaryResponse getNumberOfVolunteersInBoundaryResponse = new GetNumberOfVolunteersInBoundaryResponse()
            {
                NumberOfHelpers = numberOfHelpersInBoundary,
                NumberOfStreetChampions = numberOfStreetChampionsInBoundary
            };

            return getNumberOfVolunteersInBoundaryResponse;
        }

    }
}