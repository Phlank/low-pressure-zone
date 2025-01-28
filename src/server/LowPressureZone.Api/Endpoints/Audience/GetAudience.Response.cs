namespace LowPressureZone.Api.Endpoints.Audience
{
    public class GetAudienceResponse : AudienceDto
    {
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}