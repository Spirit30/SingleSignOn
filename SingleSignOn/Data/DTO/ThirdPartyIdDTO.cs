namespace SingleSignOn.Data.DTO
{
    public class ThirdPartyIdDTO
    {
        public ThirdPartyProvider Provider { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public enum ThirdPartyProvider
    {
        Google,
        Apple,
        Facebook
    }
}