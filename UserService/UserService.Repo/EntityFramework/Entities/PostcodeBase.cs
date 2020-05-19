using System;

namespace UserService.Repo.EntityFramework.Entities
{
    /// <summary>
    /// Provides the properties for Postcode, PostcodeSwitch and PostcodeOld.  PostcodeSwitch and PostcodeOld are used when postcode coordinates are loaded and need to be identical due to use of a switch statement.
    /// </summary>
    public abstract class PostcodeBase 
    {
        public int Id { get; set; }
        public string Postcode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
