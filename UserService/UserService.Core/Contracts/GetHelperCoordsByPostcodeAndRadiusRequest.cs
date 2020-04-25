using MediatR;
using System.ComponentModel.DataAnnotations;

namespace UserService.Core.Contracts
{
    public class GetHelperCoordsByPostcodeAndRadiusRequest : IRequest<GetHelperCoordsByPostcodeAndRadiusResponse>
    {
        [Required]
        public string Postcode { get; set; }

        [Required]
        public int RadiusInMetres { get; set; }

        [Required]
        public byte VolunteerType { get; set; }

        // a workaround ... https://github.com/Azure/azure-webjobs-sdk-extensions/issues/486 (the suggested workaround there didn't work)
        public VolunteerType VolunteerTypeEnum => (VolunteerType)VolunteerType;

        [Required]
        public byte IsVerifiedType { get; set; }

        public IsVerifiedType IsVerifiedTypeEnum => (IsVerifiedType)IsVerifiedType;
    }
}
