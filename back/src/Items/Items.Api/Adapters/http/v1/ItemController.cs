using Items.Api.Adapters.http.v1.Contract;
using Items.Core.Application.UseCases.Commands.AddItem;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Primitives;
using Swashbuckle.AspNetCore.Annotations;

namespace Items.Api.Adapters.http.v1
{
    [ApiController]
    [Route("api/v1/items")]
    public class ItemController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ItemController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [Consumes("application/json")]
        [SwaggerOperation("AddItem")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "Ошибка валидации")]
        [SwaggerResponse(statusCode: 409, type: typeof(Error), description: "Ошибка выполнения бизнес логики")]
        [SwaggerResponse(statusCode: 0, type: typeof(Error), description: "Ошибка")]
        public async Task<IActionResult> AddItem([FromBody] NewItem newItem)
        {
            var addItemCommand = new AddItemCommand(newItem.Name, newItem.MeasureType);

            var response = await _mediator.Send(addItemCommand);

            if(response.IsSuccess)
                return Ok();

            return Conflict();
        }
    }
}
