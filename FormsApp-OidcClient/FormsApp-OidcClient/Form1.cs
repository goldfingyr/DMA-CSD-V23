using FormsApp_OidcClient.Browser;
using FormsApp_OidcClient.Models;
using IdentityModel.OidcClient;
using Newtonsoft.Json;
using System.Text;

namespace FormsApp_OidcClient
{
    public partial class Form1 : Form
    {
        OidcClient _oidcClient;
        public Form1()
        {
            InitializeComponent();
            var options = new OidcClientOptions
            {
                Authority = Environment.GetEnvironmentVariable("OpenIDRealmURI") + "/",
                ClientId = Environment.GetEnvironmentVariable("OpenIDClient"),
                ClientSecret = Environment.GetEnvironmentVariable("OpenIDSecret"),
                Scope = Environment.GetEnvironmentVariable("Scope"),
                RedirectUri = Environment.GetEnvironmentVariable("OpenIDRedirectURI"),
                Browser = new WinFormsWebView()
            };
            // IdentityModel.OidcClient will not accept self signed certificates
            options.Policy.Discovery.RequireHttps = false;
            _oidcClient = new OidcClient(options);
        }

        private async void BtLogin_Click(object sender, EventArgs e)
        {
            var result = await _oidcClient.LoginAsync();

            if (result.IsError)
            {
                MessageBox.Show(this, result.Error, "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var sb = new StringBuilder(128);
                foreach (var claim in result.User.Claims)
                {
                    sb.AppendLine($"{claim.Type}: {claim.Value}");
                }

                if (!string.IsNullOrWhiteSpace(result.RefreshToken))
                {
                    sb.AppendLine();
                    sb.AppendLine($"refresh token: {result.RefreshToken}");
                }

                if (!string.IsNullOrWhiteSpace(result.IdentityToken))
                {
                    sb.AppendLine();
                    sb.AppendLine($"identity token: {result.IdentityToken}");
                }

                if (!string.IsNullOrWhiteSpace(result.AccessToken))
                {
                    sb.AppendLine();
                    sb.AppendLine($"access token: {result.AccessToken}");
                }

                TbResponse.Text = sb.ToString();

                // Call the API

                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7000/WeatherForecast");
                    request.Headers.Add("Authorization", "Bearer " + result.AccessToken);
                    using (var response = client.SendAsync(request).Result)
                    {
                        try
                        {
                            response.EnsureSuccessStatusCode();
                            string apiResponse = response.Content.ReadAsStringAsync().Result;
                            TbApiResponse.Text = apiResponse;
                        }
                        catch (HttpRequestException ex)
                        {
                            TbApiResponse.Text = ex.Message;
                        }

                    }
                }

            }
        }

        private void getCccfToken_Click(object sender, EventArgs e)
        {
            OpenIDTokenResponse tokenResponse;
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://auth.a.ucnit.eu/realms/xOIDCx/protocol/openid-connect/token");
                var collection = new List<KeyValuePair<string, string>>();
                collection.Add(new("response_type", "code"));
                collection.Add(new("client_id", Environment.GetEnvironmentVariable("OpenIDClient")));
                collection.Add(new("client_secret", Environment.GetEnvironmentVariable("OpenIDSecret")));
                collection.Add(new("grant_type", "client_credentials"));
                var content = new FormUrlEncodedContent(collection);
                request.Content = content;
                using (var response = client.SendAsync(request).Result)
                {
                    response.EnsureSuccessStatusCode();
                    // Get the response
                    string tokenJsonString = response.Content.ReadAsStringAsync().Result;
                    tokenResponse = JsonConvert.DeserializeObject<OpenIDTokenResponse>(tokenJsonString);
                    TbResponse.Text = tokenResponse.access_token;
                }
            }

            // Call the API

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7000/WeatherForecast");
                request.Headers.Add("Authorization", "Bearer " + tokenResponse.access_token);
                using (var response = client.SendAsync(request).Result)
                {
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        string apiResponse = response.Content.ReadAsStringAsync().Result;
                        TbApiResponse.Text = apiResponse;
                    }
                    catch (HttpRequestException ex)
                    {
                        TbApiResponse.Text = ex.Message;
                    }

                }
            }
        }
    }
}