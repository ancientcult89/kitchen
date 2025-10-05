using CSharpFunctionalExtensions;
using MediatR;
using Primitives;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Domain.Model.SharedKernel;

namespace Products.Core.Application.UseCases.Commands.AddProduct
{
    public class AddProductCommand : IRequest<UnitResult<Error>>
    {
        private AddProductCommand(ProductName name, MeasureType measureType)
        {
            Name = name;
            MeasureType = measureType;
        }

        public static Result<AddProductCommand, Error> Create(string name, int measureTypeId)
        {
            var productName = ProductName.Create(name);

            if (!productName.IsSuccess)
                return productName.Error;

            var measureType = MeasureType.CreateFromId(measureTypeId);
            if(!measureType.IsSuccess)
                return measureType.Error;

            return new AddProductCommand(productName.Value, measureType.Value);
        }

        public ProductName Name { get; set; }
        public MeasureType MeasureType { get; set; }
    }
}
