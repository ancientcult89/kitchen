using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace Items.Core.Application.UseCases.Commands.AddItem
{
    public class AddItemCommand : IRequest<UnitResult<Error>>
    {
        public string Name { get; set; }
        public string MeasureType { get; set; }
    }
}
