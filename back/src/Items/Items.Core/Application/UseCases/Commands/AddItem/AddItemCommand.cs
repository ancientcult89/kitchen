using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace Items.Core.Application.UseCases.Commands.AddItem
{
    public class AddItemCommand : IRequest<UnitResult<Error>>
    {
        public AddItemCommand(string name, int measureType)
        {
            Name = name;
            MeasureType = measureType;
        }

        public string Name { get; set; }
        public int MeasureType { get; set; }
    }
}
