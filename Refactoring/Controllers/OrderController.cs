using MediatR;
using Refactoring.Application.Features.ProcessOrder;

namespace Refactoring.Controllers;

using Microsoft.AspNetCore.Mvc;

[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ApiController]
[Route("/api/[controller]")]
// TODO NOTE - I would design API with plural endings - i.e. orders in this case
public class OrderController(IMediator mediator) : ControllerBase
{
    [HttpPost("process", Name = "ProcessOrder")]
    public ActionResult ProcessOrder([FromQuery] ProcessOrderDto processOrderDto, CancellationToken cancellationToken)
    {
        mediator.Send(ProcessOrderMappings.DtoToCommand(processOrderDto), cancellationToken);
        
        return Ok();
    }
}