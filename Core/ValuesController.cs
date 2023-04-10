using Infrastructure.DTOs.REST;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core;
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
            .WithPagination(new IPagination
            {
                PageNumber = 1,
                PageSize = 10,
                TotalPages = 10,
                TotalCount = 10,
            });


        return res;
    }

    [HttpGet("IDA")]
    public ActionResult<IResponse<Test>> GetIDA(int id)
    {
        var test = new Test()
        {
            ID = id
        };
        var res = new IResponse<Test>().WithData(test, 201);


        throw new DirectoryNotFoundException();
        return res;
    }

}
public class Test
{
    public int ID { get; set; }
}