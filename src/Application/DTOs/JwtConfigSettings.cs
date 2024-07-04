namespace Application.DTOs
{
    public class JwtConfigSettings
    {
        public string Secret { get; set; }
        public int TokenLifespan { get; set; }
        public string ValidIssuer { get; set; }
    }
}
