using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Structor.Auth.DTOs;
using Structor.Core.Globals;
using Structor.Features.Users.Entities;
using Structor.Features.Users.Services;
using Structor.Infrastructure.DTOs.REST;

namespace Structor.Features.Users.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersServices _usersServices;
    private readonly IConfiguration _configuration;

    public UsersController(IUsersServices usersServices, IConfiguration configuration)
    {
        _usersServices = usersServices;
        _configuration = configuration;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<Response<JwtDto>>> Register([FromBody] User newUser)
    {

        var jwt = await _usersServices.Register(newUser);

        Response<JwtDto> response = new()
        {
            Data = jwt,
            Status = ResponseStatus.Success,
            StatusCode = 204,

        };

        return Ok(response);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<Response<JwtDto>>> Login([FromBody] string username, string Password)
    {

        var jwt = await _usersServices.Login(username, Password);

        Response<JwtDto> response = new()
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
    public IActionResult Login([FromRoute] string provider)
    {
        var oAuthUrl = _configuration["OAuthUrl"];
        if (oAuthUrl is null)
        {
            throw new NullReferenceException("OAuthUrl is not defined");
        }

        string oauthUrl = oAuthUrl.Replace("provider",provider); 

        return Redirect(oauthUrl);
    }


    [HttpGet]
    [Route("~/api/oauth/{provider}/callback")]
    public async Task<ActionResult<Response<JwtDto>>> ProviderCallBack([FromRoute] string provider, [FromQuery] string code, [FromQuery] string state)
    {
        var jwt = await _usersServices.HandleExternal(provider, code, state);

        Response<JwtDto> response = new();
        response.WithData(jwt);

        return Ok(response);

    }


    //[HttpPost]
    //[Route("{provider}")]
    //[AllowAnonymous]
    //public async Task<ActionResult<Response<JwtDto>>> ProviderRedirect([FromRoute] string provider, [FromBody] JsonNode content)
    //{

    //    var jwt = await _usersServices.HandleExternal(provider, content);

    //    Response<JwtDto> response = new();
    //    response.WithData(jwt);

    //    return Ok(response);
    //}




    //[HttpGet]
    //[Route("~/api/oauth/{provider}/callback")]
    //public async Task<ActionResult<Response<JwtDto>>> ProviderCallBack([FromRoute] string provider, [FromQuery] string code, [FromQuery] string state)
    //{

    //    var tokens = await _usersServices.HandleExternal(provider, code, state);
    //    var accessCookie = _usersServices.CreateAccessCookie(tokens);
    //    var refreshCookie = _usersServices.CreateRefreshCookie(tokens);



    //    Response<JwtDto> response = new()
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
