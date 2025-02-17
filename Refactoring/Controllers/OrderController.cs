using MediatR;
using Refactoring.Application.Features.ProcessOrder;
using Refactoring.Domain.Exceptions;

namespace Refactoring.Controllers;

using Microsoft.AspNetCore.Mvc;

[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ApiController]
[Route("/api/[controller]")]
public class OrderController(IMediator mediator) : ControllerBase
{
    [HttpPost("process", Name = "ProcessOrder")]
    public async Task<ActionResult> ProcessOrder([FromQuery] ProcessOrderDto processOrderDto, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(ProcessOrderMappings.DtoToCommand(processOrderDto), cancellationToken);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        
        return Ok();
    }
}