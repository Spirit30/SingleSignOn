using SingleSignOn.Data;
using SingleSignOn.Data.Model;
using System;
using System.Linq;

namespace SingleSignOn.Logic
{
    public class Auth
    {
        public static bool ExcangeTokenToUser(string accessToken, Context _context, out User user, out string error, bool validateTime = true)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                error = "Empty Access Token.";
                user = null;
                return false;
            }

            if (!Guid.TryParse(accessToken, out Guid guid))
            {
                error = "Invalid Access Token.";
                user = null;
                return false;
            }

            user = _context.Users.FirstOrDefault(u => u.AccessToken == accessToken);

            if (user == null)
            {
                error = "User not found.";
                return false;
            }

            if (validateTime)
            {
                var period = DateTime.UtcNow - new DateTime(user.AccessTokenTimestamp);

                if (period.TotalMinutes > Const.ACCESS_TOKEN_MINUTES_ALIVE)
                {
                    error = "Access Token is expired.";
                    return false;
                }
            }

            error = null;
            return true;
        }

        public static bool IsValidName(string name, Context _context, out string error)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                error = "Name is empty.";
                return false;
            }

            if (_context.Users.Any(u => u.DisplayName == name))
            {
                error = "Name is taken.";
                return false;
            }

            error = null;
            return true;
        }
    }
}