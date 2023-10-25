using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Structor.Auth.Configurations;
using Structor.Auth.Enums;

namespace Structor.Auth.Services;

public interface IOAuthService
{
    string GetProviderRedirect(string providerString);
    OAuthProvider GetProviderEnum(string provider);
    Task<JsonNode> HandleProviderCallback(string providerString, string code, string state);
}

public class OAuthService : IOAuthService
{

    private readonly IDataProtectionProvider _protectionProvider;
    private readonly OAuthOptions _gitHubOptions;
    private readonly OAuthOptions _googleOptions;

    public OAuthService(IDataProtectionProvider protectionProvider, IOptionsSnapshot<OAuthOptions> oAuthOptions)
    {
        _protectionProvider = protectionProvider;
        _gitHubOptions = oAuthOptions.Get(OAuthProvider.Github.ToString());
        _googleOptions = oAuthOptions.Get(OAuthProvider.Google.ToString());
    }

    private OAuthOptions GetProviderData(OAuthProvider provider)
    {
        switch (provider)
        {
            case OAuthProvider.Github:
                {
                    return _gitHubOptions;
                }
            case OAuthProvider.Google:
                {
                    return _googleOptions;
                }
            default:
                throw new BadHttpRequestException("Not a valid Provider");
        }
    }

    public async Task<JsonNode> HandleProviderCallback(string providerString, string code, string state)
    {
        var provider = GetProviderEnum(providerString);
        var providerData = GetProviderData(provider);

        var dataProtector = providerData.DataProtector;
        var dataProtectionSecret = providerData.DataProtectionSecret;
        var userInfoEndpoint = providerData.UserInformationEndpoint;


        // Validate the State
        var isValidState = ValidateState(dataProtector, dataProtectionSecret, state);
        if (!isValidState)
        {
            throw new Exception("State is not valid");
        };


        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Structor");


        // Get Access Token
        string accessToken = await GetProviderAccessToken(providerData, code, httpClient);

        // Get User Data
        JsonNode responseData = await GetUserData(userInfoEndpoint, accessToken, httpClient);

        // Send User Data to client
        return responseData;
        //return await SendUserData(redirectUrl, responseData);

    }


    public OAuthProvider GetProviderEnum(string provider)
    {
        if (!Enum.TryParse(provider, ignoreCase: true, out OAuthProvider oAuthProvider))
        {
            throw new BadHttpRequestException("Not a valid Provider");
        }

        return oAuthProvider;
    }
    private bool ValidateState(string dataProtector, string stateSecret, string state)
    {
        var _dataProtector = _protectionProvider.CreateProtector(dataProtector);
        var stateUnProtected = _dataProtector.Unprotect(state);
        if (stateUnProtected == null ||
            stateUnProtected != stateSecret)
        {
            throw new UnauthorizedAccessException("State is not valid");
        }

        return true;
    }

    private async Task<string> GetProviderAccessToken(OAuthOptions providerData, string code, HttpClient httpClient)
    {
        //var jsonPayload = JsonConvert.SerializeObject(new
        var jsonPayload = JsonSerializer.Serialize<dynamic>(new
        {
            client_id = providerData.ClientId,
            client_secret = providerData.ClientSecret,
            code,
            redirect_uri = providerData.CallbackUrl,
            response_type = "token",
            grant_type = "authorization_code"
        });
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(providerData.TokenEndpoint, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        //var responseData = JsonConvert.DeserializeObject<dynamic>(responseContent);
        var responseData = JsonSerializer.Deserialize<JsonNode>(responseContent);

        string accessToken = responseData!["access_token"]!.GetValue<string>();

        return accessToken;
    }

    private async Task<JsonNode> GetUserData(string userInfoEndPoint, string accessToken, HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.GetAsync(userInfoEndPoint);
        var responseContent = await response.Content.ReadAsStringAsync();
        //var responseData = JsonConvert.DeserializeObject<dynamic>(responseContent);
        var responseData = JsonSerializer.Deserialize<JsonNode>(responseContent);

        if (responseData == null)
        {
            throw new UnauthorizedAccessException("No user data returned");
        }

        return responseData;
    }

    private async Task<HttpResponseMessage> SendUserData(string url, JsonNode data)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Structor");

        //var jsonPayload = JsonConvert.SerializeObject(data);
        var jsonPayload = JsonSerializer.Serialize(data);
        var content = new StringContent(jsonPayload.ToString(), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(url, content);

        return response;
    }

    public string GetProviderRedirect(string providerString)
    {
        var provider = GetProviderEnum(providerString);
        var providerData = GetProviderData(provider);


        var _dataProtector = _protectionProvider.CreateProtector(providerData.DataProtector);

        var redirectUrl = $"{providerData.AuthorizationEndpoint}?client_id={providerData.ClientId}&scope={providerData.Scope}" +
                           $"&response_type=code&redirect_uri={providerData.CallbackUrl}&state={_dataProtector.Protect(providerData.DataProtectionSecret)}";

        return redirectUrl;
    }
}
