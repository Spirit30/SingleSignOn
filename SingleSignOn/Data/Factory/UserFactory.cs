using SingleSignOn.Data.Model;
using System;

namespace SingleSignOn.Data.Factory
{
    public static class UserFactory
    {
        public static User Create(int id, string email, string name, string code)
        {
            return new User
            {
                UserId = id,
                Email = email,
                AccessToken = Guid.NewGuid().ToString(),
                BearerToken = Guid.NewGuid().ToString(),
                DisplayName = name,
                Code = code,
                CodeTimestamp = DateTime.UtcNow.Ticks,
                AccessTokenTimestamp = DateTime.UtcNow.Ticks
            };
        }

        public static User Create(int id, string thirdPartyId, string name)
        {
            return new User
            {
                UserId = id,
                ThirdPartyId = thirdPartyId,
                AccessToken = Guid.NewGuid().ToString(),
                BearerToken = Guid.NewGuid().ToString(),
                DisplayName = name,
                CodeTimestamp = DateTime.UtcNow.Ticks,
                AccessTokenTimestamp = DateTime.UtcNow.Ticks
            };
        }
    }
}