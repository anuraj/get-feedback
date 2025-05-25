using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api
{
    public class GoogleAuth
    {
        private readonly ILogger<GoogleAuth> _logger;

        public GoogleAuth(ILogger<GoogleAuth> logger)
        {
            _logger = logger;
        }

        [Function("GoogleAuth")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string token = data?.token;
            if (string.IsNullOrEmpty(token))
            {
                return new BadRequestObjectResult("Missing token.");
            }

            // Validate Google token
            var httpClient = new HttpClient();
            var googleClientId = "284242807372-7dd4tdh6plj40nuf1ikud02l7c9eadg6.apps.googleusercontent.com";
            var validationUrl = $"https://oauth2.googleapis.com/tokeninfo?id_token={token}";
            var response = await httpClient.GetAsync(validationUrl);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new UnauthorizedObjectResult("Invalid token.");
            }
            var payload = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            if ((string)payload.aud != googleClientId)
            {
                return new UnauthorizedObjectResult("Invalid audience.");
            }
            // Optionally, check email_verified, exp, etc.
            var result = new RedirectResult("/web/dashboard.html");
            return result;

        }
    }
}
