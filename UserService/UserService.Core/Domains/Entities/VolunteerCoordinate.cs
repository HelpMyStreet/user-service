using System;
using System.Runtime.Serialization;

namespace UserService.Core.Domains.Entities
{
    [DataContract(Name = "volunteerCoordinate")]
    public class VolunteerCoordinate : IEquatable<VolunteerCoordinate>
    {
        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lng")]
        public double Longitude { get; set; }

        [DataMember(Name = "type")]
        public VolunteerType VolunteerType { get; set; }

        [DataMember(Name = "verif")]
        public bool IsVerified { get; set; }


        public bool Equals(VolunteerCoordinate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VolunteerCoordinate) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Latitude.GetHashCode() * 397) ^ Longitude.GetHashCode();
            }
        }

        public static bool operator ==(VolunteerCoordinate left, VolunteerCoordinate right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VolunteerCoordinate left, VolunteerCoordinate right)
        {
            return !Equals(left, right);
        }
    }
}
