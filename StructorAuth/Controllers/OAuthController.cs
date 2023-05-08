using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StructorAuth.Config;
using StructorAuth.Config.Providers;
using StructorAuth.Entities;
using StructorAuth.Services;

namespace StructorAuth.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class OAuthController : ControllerBase
{
    private readonly IDataProtectionProvider _protectionProvider;

    public OAuthController(IDataProtectionProvider protectionProvider)
    {
        _protectionProvider = protectionProvider;
    }





    /// <summary>
    /// Redirects user to the OAuth provider's login page to initiate the authentication flow.
    /// </summary>
    /// <param name="provider">The name of the OAuth provider to authenticate with.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the asynchronous operation.</returns>
    ///1. Users clicks on Login With Google on the client => Redirect to api/oauth/provider/google
    ///2. Api Redirects to Google's Login => User login 
    ///3. Google Redirect to CallBackPath with code and state 
    ///4. Api Verifies & Generates claims & Generates Token
    ///5. Api Redirect to Client with the Tokens
    [HttpGet("{provider}")]
    public IActionResult Login(string provider)
    {

        if (!Enum.TryParse(provider, ignoreCase: true, out StructorOAuthProvidersEnum oAuthProvider))
        {
            throw new BadHttpRequestException("Not a valid Provider");
        }

        switch (oAuthProvider)
        {
            case StructorOAuthProvidersEnum.Github:
                {
                    var _dataProtector = _protectionProvider.CreateProtector(StructorOAuthProvidersEnum.Github.ToString());

                    var redirectPath = $"{Github.AuthorizationEndpoint}?client_id={Github.ClientId}&scope={Github.Scope}" +
                                        $"&response_type=code&redirect_uri={Github.CallbackPath}&state={_dataProtector.Protect("Hello")}";


                    return Redirect(redirectPath);
                }
            default:
                {
                    break;
                }
        }


        return BadRequest("Not a valid Provider");
    }
}

