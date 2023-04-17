using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.DTOs.REST;
using Microsoft.AspNetCore.Mvc;
using UserFeature.Entities;
using UsersFeature.Repositories;

namespace UserFeature.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost]

    public async Task<ActionResult<IResponse<User>>> AddUser([FromBody] User user)
    {
        var createdUser = await _userRepository.Insert(user);

        IResponse<User> response = new();
        response.WithData(createdUser)
            .WithSuccess()
            .WithStatusCode(201)
            .WithPagination(new IPagination() { PageNumber = 0, PageSize = 1, TotalCount = 1, TotalPages = 1 });

        return Ok(response);

    }
}
