using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Structor.Auth.DTOs;
using Structor.Features.Users.Entities;
using Structor.Features.Users.Services;
using Structor.Infrastructure.DTOs.REST;
using Structor.Infrastructure.Globals;

namespace Structor.Features.Users.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersServices _usersServices;

    public UsersController(IUsersServices usersServices)
    {
        _usersServices = usersServices;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<IResponse<JwtDto>>> Register([FromBody] User newUser)
    {

        var jwt = await _usersServices.Register(newUser);

        IResponse<JwtDto> response = new()
        {
            Data = jwt,
            Status = ResponseStatus.Success,
            StatusCode = 204,

        };

        return Ok(response);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<IResponse<JwtDto>>> Login([FromBody] string username, string Password)
    {

        var jwt = await _usersServices.Login(username, Password);

        IResponse<JwtDto> response = new()
        {
            Data = jwt,
            Status = ResponseStatus.Success,
            StatusCode = 204,

        };


        return Ok(response);
    }

    //[HttpPost]
    //public async Task<IActionResult> ValidateToken([FromForm] string token, [FromForm] JWTEnum jWTEnum)
    //{

    //    return Ok(_jWTService.ValidateToken(token, jWTEnum));
    //}

    [HttpGet]
    [Route("{provider}")]
    [AllowAnonymous]
    public IActionResult ProviderLogin([FromRoute] string provider)
    {
        string oauthUrl = AppSettings.OAUTH_URL.Replace("provider",provider); 

        return Redirect(oauthUrl);
    }


    [HttpGet]
    [Route("~/api/oauth/{provider}/callback")]
    public async Task<ActionResult<IResponse<JwtDto>>> ProviderCallBack([FromRoute] string provider, [FromQuery] string code, [FromQuery] string state)
    {
        var jwt = await _usersServices.HandleExternal(provider, code, state);

        IResponse<JwtDto> response = new();
        response.WithData(jwt);

        return Ok(response);

    }


    //[HttpPost]
    //[Route("{provider}")]
    //[AllowAnonymous]
    //public async Task<ActionResult<IResponse<JwtDto>>> ProviderRedirect([FromRoute] string provider, [FromBody] JsonNode content)
    //{

    //    var jwt = await _usersServices.HandleExternal(provider, content);

    //    IResponse<JwtDto> response = new();
    //    response.WithData(jwt);

    //    return Ok(response);
    //}




    //[HttpGet]
    //[Route("~/api/oauth/{provider}/callback")]
    //public async Task<ActionResult<IResponse<JwtDto>>> ProviderCallBack([FromRoute] string provider, [FromQuery] string code, [FromQuery] string state)
    //{

    //    var tokens = await _usersServices.HandleExternal(provider, code, state);
    //    var accessCookie = _usersServices.CreateAccessCookie(tokens);
    //    var refreshCookie = _usersServices.CreateRefreshCookie(tokens);



    //    IResponse<JwtDto> response = new()
    //    {
    //        Data = tokens,
    //        Status = ResponseStatus.Success,
    //        StatusCode = 204,

    //    };


    //    Response.Headers.Append("Set-Cookie", accessCookie.ToString());
    //    Response.Headers.Append("Set-Cookie", refreshCookie.ToString());

    //    return Redirect("/swagger/index.html");

    //    //return Ok(response);
    //}

}
