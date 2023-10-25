using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Structor.Infrastructure.DTOs.REST;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Structor.Features.Users.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class ValuesController : ControllerBase
{


    // GET api/<ValuesController>/5
    [HttpGet("{id}")]
    public ActionResult<Response<Test>> GetID(int id)
    {
        var test = new Test()
        {
            ID = id
        };
        var res = new Response<Test>();

        res.WithData(test)
            .WithPagination(new Pagination
            {
                Page = 1,
                Size = 10,
                TotalPages = 10,
                TotalCount = 10,
            });


        return res;
    }


    [HttpGet("{id}")]
    [AllowAnonymous]
    public ActionResult<Response<Test>> Get(int id)
    {
        var test = new Test()
        {
            ID = id
        };
        var res = new Response<Test>();

        res.WithData(test)
            .WithPagination(new Pagination
            {
                Page = 1,
                Size = 10,
                TotalPages = 10,
                TotalCount = 10,
            });


        return res;
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public ActionResult<Response<Test>> GetIDA(int id)
    {
        var test = new Test()
        {
            ID = id
        };
        var res = new Response<Test>().WithData(test, StatusCodes.Status201Created);


        return CreatedAtAction(nameof(Get), new { id = id}, res);
        //return Ok(res);


        //throw new DirectoryNotFoundException();
    }

}
public class Test
{
    public int ID { get; set; }
}