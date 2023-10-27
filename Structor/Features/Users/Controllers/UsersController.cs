using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Structor.Auth.Configurations;
using Structor.Auth.DTOs;
using Structor.Features.Enums;
using Structor.Features.Users.Dtos;
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
    private readonly JwtOptions _jwtOptions;

    public UsersController(IUserServices usersServices, IConfiguration configuration, IOptions<JwtOptions> jwtOptions)
    {
        _usersServices = usersServices;
        _configuration = configuration;
        _jwtOptions = jwtOptions.Value;
    }

    [HttpPost]
    public async Task<ActionResult<Response<JwtDto>>> Register(NewUserDto newUser)
    {

        var jwt = await _usersServices.Register(newUser);

        var response = new Response<JwtDto>().WithData(jwt, StatusCodes.Status201Created);

        return CreatedAtAction(nameof(Login), response);
    }

    [HttpPost]
    public async Task<ActionResult<Response<JwtDto>>> Login([FromBody] LoginDto loginDto)
    {
        var jwt = await _usersServices.Login(loginDto);
        var response = new Response<JwtDto>().WithData(jwt, StatusCodes.Status200OK);

        var cookieOptions = _usersServices.CreateRefreshTokenCookieOptions(jwt);

        Response.Cookies.Append(_jwtOptions.CookieHeaders.RefreshHeader, jwt.RefreshToken, cookieOptions);
        return Ok(response);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<Response<JwtDto>>> Refresh()
    {
        var refreshCookie = Request.Cookies.FirstOrDefault(c => c.Key == _jwtOptions.CookieHeaders.RefreshHeader);
        var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(UserClaim.Guid,StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrWhiteSpace(refreshCookie.Value))
        {
            return Unauthorized(new Response<JwtDto>().WithError("Refresh Token invalid or expired.", StatusCodes.Status401Unauthorized));
        }

        var jwt = await _usersServices.ValidateAndGenerateNewToken(userId.Value, refreshCookie.Value);

        var cookieOptions = _usersServices.CreateRefreshTokenCookieOptions(jwt);
        Response.Cookies.Append(_jwtOptions.CookieHeaders.RefreshHeader, jwt.RefreshToken, cookieOptions);
        

        return Ok(new Response<JwtDto>().WithData(jwt));
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
    [HttpGet]
    [Route("{provider}")]
    public IActionResult Login([FromRoute] string provider)
    {
        var redirectPath =  _usersServices.GetProviderRedirect(provider);

        return Redirect(redirectPath);
    }

    [HttpGet]
    [Route("{provider}/")]
    public async Task<ActionResult<Response<JwtDto>>> CallBack([FromRoute] string provider, [FromQuery] string code, [FromQuery] string state)
    {
        var jwt = await _usersServices.HandleExternal(provider, code, state);

        var cookieOptions = _usersServices.CreateRefreshTokenCookieOptions(jwt);
        Response.Cookies.Append(_jwtOptions.CookieHeaders.RefreshHeader, jwt.RefreshToken, cookieOptions);

        var response = new Response<JwtDto>().WithData(jwt, StatusCodes.Status200OK);

        return CreatedAtAction(nameof(Login), response);
    }




    //[HttpGet]
    //[Route("~/api/oauth/{provider}/callback")]
    //public async Task<ActionResult<Response<JwtDto>>> ProviderCallBack([FromRoute] string provider, [FromQuery] string code, [FromQuery] string state)
    //{

    //    var tokens = await _usersServices.HandleExternal(provider, code, state);
    //    var accessCookie = _usersServices.CreateAccessCookie(tokens);
    //    var refreshCookie = _usersServices.CreateRefreshHttpOnlyOptions(tokens);



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
