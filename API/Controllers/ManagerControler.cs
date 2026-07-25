using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/manager")]
public class ManagerController : ControllerBase
{
    private readonly ManagerService _managerService;

    public ManagerController(ManagerService managerService)
    {
        _managerService = managerService;
    }

    [HttpPost("execute")]
    public async Task<ActionResult> Execute(
        ManagerCommandRequest request)
    {
        try
        {
            var result = await _managerService.ExecuteAsync(
                request.Action,
                request.ProductId,
                request.Value
            );

            return Ok(new
            {
                status = "success",
                data = result
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                status = "error",
                message = exception.Message
            });
        }
    }
}

public class ManagerCommandRequest
{
    public required string Action { get; set; }

    public int ProductId { get; set; }

    public int Value { get; set; }
}
