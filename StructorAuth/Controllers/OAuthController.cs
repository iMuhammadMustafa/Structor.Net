using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Structor.Auth.Services;

namespace Structor.Auth.Controllers;

[ApiController]
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

