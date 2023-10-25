using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Structor.Auth.Enums;
using Structor.Auth.Services;
using Structor.Auth.Config.Providers;

namespace Structor.Auth.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class OAuthController : ControllerBase
{
    private readonly IDataProtectionProvider _protectionProvider;
    private readonly IOAuthService _oAuthService;

    public OAuthController(IDataProtectionProvider protectionProvider, IOAuthService oAuthService)
    {
        _protectionProvider = protectionProvider;
        _oAuthService = oAuthService;
    }




    //[HttpGet]
    //[Route("~/api/oauth/{provider}/callback")]
    //public async Task<IActionResult> ProviderCallBack([FromRoute] string provider, [FromQuery] string code, [FromQuery] string state)
    //{
    //    var r = await _oAuthService.HandleProviderCallback(provider, code, state);



    //    return Ok();

    //}
}

