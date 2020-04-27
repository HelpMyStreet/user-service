using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace UserService.Core.Domains.Entities
{
    public class GetVolunteerCoordinatesRequest : IRequest<GetVolunteerCoordinatesResponse>
    {
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }
        
        [Required]
        public byte VolunteerType { get; set; }

        // a workaround... https://github.com/Azure/azure-webjobs-sdk-extensions/issues/486 (the suggested workaround there didn't work)
        public VolunteerType VolunteerTypeEnum => (VolunteerType)VolunteerType;

        [Required]
        public byte IsVerifiedType { get; set; }

        public IsVerifiedType IsVerifiedTypeEnum => (IsVerifiedType)IsVerifiedType;

        [Range(0, int.MaxValue)]
        public int RadiusInMetres { get; set; }

        [Range(0, int.MaxValue)]
        public int MinDistanceBetweenInMetres { get; set; }

        public override string ToString()
        {
            return $"{nameof(Latitude)}: {Latitude}, {nameof(Longitude)}: {Longitude}, {nameof(RadiusInMetres)}: {RadiusInMetres}, {nameof(VolunteerType)}: {VolunteerType},  {nameof(IsVerifiedType)}: {IsVerifiedType}, {nameof(MinDistanceBetweenInMetres)}: {MinDistanceBetweenInMetres}";
        }
    }
}
