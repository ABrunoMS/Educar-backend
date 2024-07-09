using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Educar.Backend.Infrastructure.Services;

public class KeycloakService : IIdentityService
{
    private const string ContentType = "application/json";
    private string _accessToken = "";
    private readonly AuthOptions _authOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<KeycloakService> _logger;

    public KeycloakService(IHttpClientFactory httpClientFactory, IConfiguration configuration,
        ILogger<KeycloakService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authOptions = configuration.GetAuthOptions();
    }

    public async Task<Guid> CreateUser(string email, string name, UserRole role, CancellationToken cancellationToken)
    {
        await PopulateToken(cancellationToken);

        Guid userId;

        try
        {
            var url = $"{_authOptions.AdminUrl}/users";
            var randomPassword = GenerateRandomPassword();

            var payload = JsonSerializer.Serialize(new
            {
                username = email,
                firstName = name,
                lastName = string.Empty,
                enabled = true,
                credentials = new[]
                {
                    new
                    {
                        temporary = true,
                        type = "password",
                        value = randomPassword
                    }
                }
            });

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(payload, Encoding.UTF8, ContentType)
            };
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            userId = await GetUserIdByUsername(email, cancellationToken);

            // Assign role to user
            await AssignRoleToUser(userId, role.ToString(), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user in Keycloak");
            return Guid.Empty;
        }

        return userId;
    }

    private async Task<Guid> GetUserIdByUsername(string username, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_authOptions.AdminUrl}/users?username={username}");
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var userContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var users = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(userContent);
        var user = users?.FirstOrDefault();

        var id = user?["id"].ToString();
        return id != null ? Guid.Parse(id) : Guid.Empty;
    }

    private async Task AssignRoleToUser(Guid userId, string roleName, CancellationToken cancellationToken)
    {
        var roleId = await GetRoleIdByName(roleName, cancellationToken);
        if (roleId == null) throw new Exception($"Realm Role {roleName} not found");

        var roleMappingPayload = JsonSerializer.Serialize(new[]
        {
            new
            {
                id = roleId,
                name = roleName
            }
        });

        var request =
            new HttpRequestMessage(HttpMethod.Post, $"{_authOptions.AdminUrl}/users/{userId}/role-mappings/realm")
            {
                Content = new StringContent(roleMappingPayload, Encoding.UTF8, ContentType)
            };
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private async Task<string?> GetRoleIdByName(string roleName, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_authOptions.AdminUrl}/roles/{roleName}");
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var roleContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var roleData = JsonSerializer.Deserialize<Dictionary<string, object>>(roleContent);
        var roleId = roleData?["id"];
        return roleId?.ToString();
    }


    private string GenerateRandomPassword()
    {
        const int passwordLength = 12;
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
        var random = new Random();
        return new string(Enumerable.Repeat(validChars, passwordLength)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public async Task<bool> CreateRole(string roleName, CancellationToken cancellationToken)
    {
        await PopulateToken(cancellationToken);

        var rolesUrl = $"{_authOptions.AdminUrl}/roles";
        var request = new HttpRequestMessage(HttpMethod.Post, rolesUrl);
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));

        var role = new { name = roleName };
        request.Content = new StringContent(JsonSerializer.Serialize(role), Encoding.UTF8, ContentType);

        var client = _httpClientFactory.CreateClient();
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

            var clientHttp = _httpClientFactory.CreateClient();
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