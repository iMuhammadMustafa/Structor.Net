using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Structor.Features.Users.Entities;
using Structor.Features.Users.Repositories;
using Structor.Infrastructure.DTOs.REST;
using StructorAuth.Config;
using StructorAuth.Services;

namespace Structor.Features.Users.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IJWTService _jWTService;
    private readonly IOAuthService _oAuthService;

    public UsersController(IUserRepository userRepository, IJWTService jWTService, IOAuthService oAuthService)
    {
        _userRepository = userRepository;
        _jWTService = jWTService;
        _oAuthService = oAuthService;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] User newUser)
    {
        /*
         * 1. Generate Claims
         * 2. Generate Access and Refresh Tokens
         * 3. Insert to database
         * 4. return response
         */

        var claims = new Dictionary<string, string>();
        claims.Add("email", "id");
        claims.Add(JwtRegisteredClaimNames.Name, "name");
        var (access, refresh) = _jWTService.GenerateJWTokens(claims);

        return Ok(new { access, refresh });
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] string username, string Password)
    {
        /*
         * 1. Get User from database
         * 2. Verify User exists and Password is correct
         * 3. Generate claims and tokens
         * 4. return Tokens
         */

        var claims = new Dictionary<string, string>();
        claims.Add("email", "id");
        var (access, refresh) = _jWTService.GenerateJWTokens(claims);

        return Ok(new { access, refresh });
    }

    [HttpPost]
    public async Task<IActionResult> ValidateToken([FromForm] string token, [FromForm] JWTEnum jWTEnum)
    {

        return Ok(_jWTService.ValidateToken(token, jWTEnum));
    }

    [HttpGet]
    public async Task<IActionResult> LoginWithProvidder([FromQuery] string provider)
    {
        /*
         * 1. Verify Provider
         * 2. Redirect to provider login
         */

        return Redirect("/swagger/index.html");
    }

    [HttpGet]
    [Route("~/api/oauth/{provider}/callback")]
    public async Task<IActionResult> ProviderCallBack([FromRoute] string provider, [FromQuery] string code, [FromQuery] string state)
    {
        /*
         * 1. Get Access Token using code
         * 2. Get User info using Access Token
         * 3. Generate User's claims
         * 4. Generate User's Tokens
         * 5. Add User to database if it doesn't exist & update refresh token
         * 6. Redirect to frontend return URI
         */
        var obj = await _oAuthService.GetUserData("Github", code, state);

        var claims = new Dictionary<string, string>();
        claims.Add("Email", obj.Email ?? string.Empty);
        claims.Add("Username", obj.Username ?? string.Empty);
        claims.Add("Profile", obj.ProfileUrl ?? string.Empty);


        if (provider == "Github")
        {
            claims.Add("Provider", "Github");
        }
        var tokens = _jWTService.GenerateJWTokens(claims);

        var refreshCookie = new Cookie("X-RefreshToken", tokens.Item2)
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.Now.AddDays(double.Parse(AuthenticationSettings.JWT_RefreshDuration)),
        };
        Response.Headers.Add("Set-Cookie", refreshCookie.ToString());

        return Ok(tokens.accessToken);
    }









    //[HttpPost]
    //public async Task<ActionResult<IResponse<User>>> AddUser([FromBody] User user)
    //{
    //    var createdUser = await _userRepository.Insert(user);

    //    IResponse<User> response = new();
    //    response.WithData(createdUser)
    //        .WithSuccess()
    //        .WithStatusCode(201)
    //        .WithPagination(new IPagination() { PageNumber = 0, PageSize = 1, TotalCount = 1, TotalPages = 1 });

    //    return Ok(response);

    //}


    [HttpGet]
    public async Task LoginExternal()
    {
        await HttpContext.ChallengeAsync("Github", new AuthenticationProperties()
        {
            RedirectUri = "/swagger/index.html"
        });
    }

    //[HttpGet]
    //public async Task<IActionResult> AfterLogin(string tokens)
    //{


    //    return Ok("Hello World");
    //}


    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> XXXXX([FromBody] string json)
    {
        return Ok("Hello World");
    }
}
