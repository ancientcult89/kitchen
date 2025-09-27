using CSharpFunctionalExtensions;
using MediatR;
using Primitives;
using Products.Core.Domain.Model.SharedKernel;

namespace Products.Core.Application.UseCases.Commands.AddProduct
{
    public class AddProductCommand : IRequest<UnitResult<Error>>
    {
        private AddProductCommand(string name, int measureTypeId)
        {
            Name = name;
            MeasureTypeId = measureTypeId;
        }

        public static Result<AddProductCommand, Error> Create(string name, int measureTypeId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return GeneralErrors.ValueIsRequired(nameof(name));

            if (!MeasureType.List().Select(mt => mt.Id).Contains(measureTypeId))
                return GeneralErrors.ValueIsNotInRange(nameof(measureTypeId), MeasureType.Weight.Id, MeasureType.Liquid.Id);

            return new AddProductCommand(name, measureTypeId);
        }

        public string Name { get; set; }
        public int MeasureTypeId { get; set; }
    }
}
