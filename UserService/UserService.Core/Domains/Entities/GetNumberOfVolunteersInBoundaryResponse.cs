using System.Runtime.Serialization;

namespace UserService.Core.Domains.Entities
{
    [DataContract(Name = "getNumberOfVolunteersInBoundaryResponse")]
    public class GetNumberOfVolunteersInBoundaryResponse
    {
        [DataMember(Name = "numberOfHelpers")]
        public int NumberOfHelpers { get; set; }

        [DataMember(Name = "numberOfStreetChampions")]
        public int NumberOfStreetChampions { get; set; }

        [DataMember(Name = "totalNumberOfVolunteers")]
        public int TotalNumberOfVolunteers => NumberOfHelpers + NumberOfStreetChampions;
    }
}
