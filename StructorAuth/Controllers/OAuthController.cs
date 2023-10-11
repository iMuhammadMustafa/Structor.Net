using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Structor.Auth.Enums;
using StructorAuth.Config.Providers;
using StructorAuth.Services;

namespace StructorAuth.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class OAuthController : ControllerBase
{
    private readonly IDataProtectionProvider _protectionProvider;
    private readonly IOAuthService _oAuthService;

    public OAuthController(IDataProtectionProvider protectionProvider, IOAuthService oAuthService)
    {
        _protectionProvider = protectionProvider;
        _oAuthService = oAuthService;
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
        var redirectPath = _oAuthService.GetProviderRedirect(provider);
        return Redirect(redirectPath);
    }

    //[HttpGet]
    //[Route("~/api/oauth/{provider}/callback")]
    //public async Task<IActionResult> ProviderCallBack([FromRoute] string provider, [FromQuery] string code, [FromQuery] string state)
    //{
    //    var r = await _oAuthService.HandleProviderCallback(provider, code, state);



    //    return Ok();

    //}
}

