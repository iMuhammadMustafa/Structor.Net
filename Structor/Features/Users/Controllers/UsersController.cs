using Microsoft.AspNetCore.Mvc;
using Structor.Auth.DTOs;
using Structor.Features.Users.Services;
using Structor.Infrastructure.DTOs.REST;

namespace Structor.Features.Users.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class UsersController : ControllerBase
{
    private readonly IUserServices _usersServices;
    private readonly IConfiguration _configuration;

    public UsersController(IUserServices usersServices, IConfiguration configuration)
    {
        _usersServices = usersServices;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<ActionResult<Response<JwtDto>>> Register(NewUserDto newUser)
    {

        var jwtResponse = await _usersServices.Register(newUser);

        return CreatedAtAction(nameof(Login), jwtResponse);
    }

    [HttpPost]
    public async Task<ActionResult<Response<JwtDto>>> Login([FromBody] string username, string Password)
    {
        var jwtResponse = await _usersServices.Login(username, Password);

        return Ok(jwtResponse);
    }

    //[HttpPost]
    //public async Task<IActionResult> ValidateToken([FromForm] string token, [FromForm] JWTEnum jWTEnum)
    //{

    //    return Ok(_jWTService.ValidateToken(token, jWTEnum));
    //}

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
    [HttpGet]
    [Route("{provider}")]
    public IActionResult Login([FromRoute] string provider)
    {
        var redirectPath =  _usersServices.GetProviderRedirect(provider);

        return Redirect(redirectPath);
    }

    [HttpGet]
    [Route("{provider}")]
    public async Task<ActionResult<Response<JwtDto>>> CallBack([FromRoute] string provider, [FromQuery] string code, [FromQuery] string state)
    {
        var jwtResponse = await _usersServices.HandleExternal(provider, code, state);
        return CreatedAtAction(nameof(Login), jwtResponse);
    }




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
