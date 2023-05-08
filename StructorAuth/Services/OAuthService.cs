using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StructorAuth.Config.Providers;
using StructorAuth.Entities;

namespace StructorAuth.Services;

public interface IOAuthService
{
    Task<OAuthResponseEntity> GetUserData(string provider, string code, string state);
}

public class OAuthService : IOAuthService
{

    private readonly IDataProtectionProvider _protectionProvider;

    public OAuthService(IDataProtectionProvider protectionProvider)
    {
        _protectionProvider = protectionProvider;

    }
    public async Task<OAuthResponseEntity> GetUserData(string provider, string code, string state)
    {
        var _dataProtector = _protectionProvider.CreateProtector(provider);
        var stateUnProtected = _dataProtector.Unprotect(state);


        if (stateUnProtected == null ||
            stateUnProtected != "Hello")
        {
            throw new UnauthorizedAccessException("State is not valid");
        }


        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Structor");
        var jsonPayload = JsonConvert.SerializeObject(new
        {
            client_id = Github.ClientId,
            client_secret = Github.ClientSecret,
            code,
            redirect_uri = Github.CallbackPath,
        });
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(Github.TokenEndpoint, content);

        //var responseBody = await response.Content.ReadFromJsonAsync<JsonElement>();
        //var accessToken = responseBody.GetProperty("access_token").ToString();

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = JsonConvert.DeserializeObject<dynamic>(responseContent);
        string accessToken = responseData!.access_token;

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        response = await httpClient.GetAsync(Github.UserInformationEndpoint);
        responseContent = await response.Content.ReadAsStringAsync();
        responseData = JsonConvert.DeserializeObject<dynamic>(responseContent);

        if (responseData == null)
        {
            throw new UnauthorizedAccessException("No user data returned");
        }


        OAuthResponseEntity obj = new()
        {
            Email = responseData.email,
            Username = responseData.login,
            ProfileUrl = responseData.url

        };

        return obj;
    }




}
