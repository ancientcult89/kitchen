using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace Items.Core.Application.UseCases.Commands.AddItem
{
    public class AddItemCommand : IRequest<UnitResult<Error>>
    {
        public AddItemCommand(string name, string measureType)
        {
            Name = name;
            MeasureType = measureType;
        }

        public string Name { get; set; }
        public string MeasureType { get; set; }
    }
}
