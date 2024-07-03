using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Educar.Backend.Application.Interfaces;
using Educar.Backend.Infrastructure.Options;
using Microsoft.Extensions.Configuration;

namespace Educar.Backend.Infrastructure.Services;

public class KeycloakService(
    IHttpClientFactory httpClient,
    IConfiguration configuration)
    : IIdentityService
{
    private const string ContentType = "application/json";
    private readonly AuthOptions _authOptions = configuration.GetAuthOptions();
    private string _accessToken = "";

    public async Task<bool> CreateRole(string roleName, CancellationToken cancellationToken)
    {
        await PopulateToken(cancellationToken);

        var rolesUrl = $"{_authOptions.AdminUrl}/roles";
        var request = new HttpRequestMessage(HttpMethod.Post, rolesUrl);
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));

        var role = new { name = roleName };
        request.Content = new StringContent(JsonSerializer.Serialize(role), Encoding.UTF8, ContentType);

        var client = httpClient.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        var result = await client.SendAsync(request, cancellationToken);

        return result.StatusCode == HttpStatusCode.Created;
    }

    public Task<bool> DeleteRole(string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task PopulateToken(CancellationToken cancellationToken)
    {
        if (_accessToken == "")
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _authOptions.TokenUrl);
            request.Headers.Clear();
            const string contentType = "application/x-www-form-urlencoded";
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

            var auth = GetBasicAuth(_authOptions.AdminClientId, _authOptions.AdminClientSecret);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
            request.Content = new FormUrlEncodedContent(new[]
                { new KeyValuePair<string, string>("grant_type", "client_credentials") });

            var clientHttp = httpClient.CreateClient();
            var result = clientHttp.Send(request, cancellationToken);

            var content = await result.Content.ReadAsStringAsync(cancellationToken);
            using var document = JsonDocument.Parse(content);
            _accessToken = document.RootElement.GetProperty("access_token").GetString() ?? string.Empty;
        }
    }

    private static string GetBasicAuth(string clientId, string clientSecret)
    {
        var userpass = $"{clientId}:{clientSecret}";
        var encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(userpass));
        return $"{encoded}";
    }
}