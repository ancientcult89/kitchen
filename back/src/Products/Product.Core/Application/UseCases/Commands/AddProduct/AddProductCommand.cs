using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace Products.Core.Application.UseCases.Commands.AddProduct
{
    public class AddProductCommand : IRequest<UnitResult<Error>>
    {
        public AddProductCommand(string name, int measureType)
        {
            Name = name;
            MeasureType = measureType;
        }

        public string Name { get; set; }
        public int MeasureType { get; set; }
    }
}
