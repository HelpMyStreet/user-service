namespace UserService.Repo.EntityFramework.Entities
{
    /// <summary>
    /// Used to load postcode data into as part of the postcode coordinate load process.  This shouldn't be queried at application runtime.
    /// </summary>
    public  class PostcodeStaging
    {
        public int Id { get; set; }
        public string Postcode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsActive { get; set; }
    }
}
