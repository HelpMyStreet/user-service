namespace UserService.Core.Dto
{
    public class HelperPostcodeRadiusDto
    {
        public int UserId { get; set; }
        public string Postcode { get; set; }
        public double SupportRadiusMiles { get; set; }
    }
}
