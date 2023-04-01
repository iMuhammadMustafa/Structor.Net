using Microsoft.AspNetCore.Mvc;
using Structor.Net.Infrastructure.DTOs.REST;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Structor.Net.Core;
[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{


    // GET api/<ValuesController>/5
    [HttpGet("{id}")]
    public ActionResult<IResponse<Test>> GetID(int id)
    {
        var test = new Test()
        {
            ID = id
        };
        var res = new IResponse<Test>();

        res.WithData(test)
            .WithSuccess();


        return res;
    }

    [HttpGet("IDA")]
    public ActionResult<IResponse<Test>> GetIDA(int id)
    {
        var test = new Test()
        {
            ID = id
        };
        var res = new IResponse<Test>();

        res.WithError();

        this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return res;
    }

}
public class Test
{
    public int ID { get; set; }
}