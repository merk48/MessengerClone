namespace MessengerClone.API.ConfigurationOptions
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int Lifetime { get; set; }
        public string Secret { get; set; } = null!;

    }
}
