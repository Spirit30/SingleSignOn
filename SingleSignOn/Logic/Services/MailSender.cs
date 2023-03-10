using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SingleSignOn.Logic.Services
{
    public class MailSender
    {
        public static bool IsValidEmail(string email, out string error)
        {
            try
            {
                error = null;
                MailAddress mailAddress = new MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                error = "Invalid email format.";
                return false;
            }
        }

        public static async Task Send(string email, string code)
        {
            string emailDataTemplate = File.ReadAllText("Resources/EmailDataTemplate.txt");
            string name = email.Split('@').First();
            string emailData = emailDataTemplate
                .Replace("---EMAIL---", email)
                .Replace("---NAME---", name)
                .Replace("---CODE---", code);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.ConnectionClose = true;

                var basicCredentials = "ab16e8f5f92a228e59e19a5d4c926278:99a0c9aeecc6cf185fca209021a588e9";
                var basicCredentialsBytes = Encoding.ASCII.GetBytes(basicCredentials);
                var basicCredentialsBase64 = Convert.ToBase64String(basicCredentialsBytes);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicCredentialsBase64);

                var content = new StringContent(emailData, Encoding.UTF8, "application/json");

                var result = await client.PostAsync("https://api.mailjet.com/v3.1/send", content);

                if (!result.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(result.StatusCode + " - " + result.ReasonPhrase);
                }
            }

        }
    }
}