namespace SingleSignOn.Data.Model
{
    public class User
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public string ThirdPartyId { get; set; }

        public string AccessToken { get; set; }

        public string BearerToken { get; set; }

        public string Code { get; set; }

        public long CodeTimestamp { get; set; }

        public long AccessTokenTimestamp { get; set; }
    }
}