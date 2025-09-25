using Products.Api.Adapters.http.v1.Contract;
using Products.Core.Application.UseCases.Commands.AddProduct;
using Products.Core.Application.UseCases.Commands.ArchiveProduct;
using Products.Core.Application.UseCases.Commands.UnArchiveProduct;
using Products.Core.Application.UseCases.Query.GetAllProducts;
using Products.Core.Application.UseCases.Query.GetProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Primitives;
using Swashbuckle.AspNetCore.Annotations;

namespace Products.Api.Adapters.http.v1
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [Consumes("application/json")]
        [SwaggerOperation("AddProduct")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "Ошибка валидации")]
        [SwaggerResponse(statusCode: 409, type: typeof(Error), description: "Ошибка выполнения бизнес логики")]
        [SwaggerResponse(statusCode: 0, type: typeof(Error), description: "Ошибка")]
        public async Task<IActionResult> AddItem([FromBody] NewProduct newItem)
        {
            var addItemCommand = new AddProductCommand(newItem.Name, newItem.MeasureType);

            var response = await _mediator.Send(addItemCommand);

            if (response.IsSuccess)
                return Ok();

            return Conflict(response.Error);
        }

        [HttpPost]
        [Route("{productId}/archive")]
        [Consumes("application/json")]
        [SwaggerOperation("ArchiveProduct")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "Ошибка валидации")]
        [SwaggerResponse(statusCode: 409, type: typeof(Error), description: "Ошибка выполнения бизнес логики")]
        [SwaggerResponse(statusCode: 0, type: typeof(Error), description: "Ошибка")]
        public async Task<IActionResult> ArchiveItem(Guid productId)
        {
            var ArchiveItemCommand = new ArchiveProductCommand(productId);

            var response = await _mediator.Send(ArchiveItemCommand);

            if (response.IsSuccess)
                return Ok();

            return Conflict(response.Error);
        }

        [HttpPost]
        [Route("{productId}/unarchive")]
        [Consumes("application/json")]
        [SwaggerOperation("UnArchiveProduct")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "Ошибка валидации")]
        [SwaggerResponse(statusCode: 409, type: typeof(Error), description: "Ошибка выполнения бизнес логики")]
        [SwaggerResponse(statusCode: 0, type: typeof(Error), description: "Ошибка")]
        public async Task<IActionResult> UnArchiveItem(Guid productId)
        {
            var UnArchiveItemCommand = new UnArchiveProductCommand(productId);

            var response = await _mediator.Send(UnArchiveItemCommand);

            if (response.IsSuccess)
                return Ok();

            return Conflict(response.Error);
        }

        [HttpGet]
        [Consumes("application/json")]
        [SwaggerOperation("GetAllProducts")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "Ошибка валидации")]
        [SwaggerResponse(statusCode: 409, type: typeof(Error), description: "Ошибка выполнения бизнес логики")]
        [SwaggerResponse(statusCode: 0, type: typeof(Error), description: "Ошибка")]
        public async Task<IActionResult> GetAllItems()
        {
            var getAllItemsQuery = new GetAllProductsQuery();

            var response = await _mediator.Send(getAllItemsQuery);

            if (response.HasNoValue)
                return NotFound();

            return Ok(response.Value.Products);
        }

        [HttpGet]
        [Route("{productId}")]
        [Consumes("application/json")]
        [SwaggerOperation("GetProduct")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "Ошибка валидации")]
        [SwaggerResponse(statusCode: 409, type: typeof(Error), description: "Ошибка выполнения бизнес логики")]
        [SwaggerResponse(statusCode: 0, type: typeof(Error), description: "Ошибка")]
        public async Task<IActionResult> GetProduct(Guid productId)
        {
            var getItemQuery = new GetProductQuery(productId);

            var response = await _mediator.Send(getItemQuery);

            if (response.HasNoValue)
                return NotFound();

            return Ok(response.Value.Product);
        }
    }
}
