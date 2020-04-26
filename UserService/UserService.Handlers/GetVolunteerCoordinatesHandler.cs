﻿using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.BusinessLogic;
using UserService.Core.Cache;
using UserService.Core.Domains.Entities;

namespace UserService.Handlers
{
    public class GetVolunteerCoordinatesHandler : IRequestHandler<GetVolunteerCoordinatesRequest, GetVolunteerCoordinatesResponse>
    {
        private readonly IGetVolunteerCoordinatesResponseGetter _getVolunteerCoordinatesResponseGetter;
        private readonly ICoordinatedResetCache _coordinatedResetCache;

        public GetVolunteerCoordinatesHandler(IGetVolunteerCoordinatesResponseGetter getVolunteerCoordinatesResponseGetter, ICoordinatedResetCache coordinatedResetCache)
        {
            _getVolunteerCoordinatesResponseGetter = getVolunteerCoordinatesResponseGetter;
            _coordinatedResetCache = coordinatedResetCache;
        }

        public async Task<GetVolunteerCoordinatesResponse> Handle(GetVolunteerCoordinatesRequest request, CancellationToken cancellationToken)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            GetVolunteerCoordinatesResponse getVolunteerCoordinatesResponse;

            // calculating coordinates that have a minimum distance between them is expensive so cache the result
            if (request.MinDistanceBetweenInMetres != null)
            {
                Debug.WriteLine($"MinDistanceBetween not null");
                string key = $"{nameof(GetVolunteerCoordinatesResponse)}_{request}";

                getVolunteerCoordinatesResponse = await _coordinatedResetCache.GetCachedDataAsync(async () => await _getVolunteerCoordinatesResponseGetter.GetVolunteerCoordinates(request, cancellationToken), key, CoordinatedResetCacheTime.OnHour);
            }
            else
            {
                Debug.WriteLine($"MinDistanceBetween is null");
                getVolunteerCoordinatesResponse = await _getVolunteerCoordinatesResponseGetter.GetVolunteerCoordinates(request, cancellationToken);
            }

            sw.Stop();
            Debug.WriteLine($"GetHelperCoordsByPostcodeAndRadiusHandler took: {sw.ElapsedMilliseconds}");
            return getVolunteerCoordinatesResponse;
        }
    }
}