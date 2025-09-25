using CSharpFunctionalExtensions;
using Primitives;

namespace Products.Core.Domain.Model.SharedKernel
{
    public class MeasureType : Entity<int>
    {
        //Название меры измерения
        public string Name { get; private set; }

        /// <summary>
        /// Единицы измерения по массе
        /// </summary>
        public static MeasureType Weight => new(1, nameof(Weight).ToLowerInvariant());

        /// <summary>
        /// Единицы измерения для жидкости
        /// </summary>
        public static MeasureType Liquid => new(2, nameof(Liquid).ToLowerInvariant());
        private MeasureType() { }
        private MeasureType(int id, string name) : this()
        {
            Id = id;
            Name = name;
        }

        public static IEnumerable<MeasureType> List() =>
            new[] { Weight, Liquid };

        public static Result<MeasureType, Error> CreateFromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                return new Error("unknown.measure.type", $"Possible values for {nameof(MeasureType)}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static Result<MeasureType, Error> CreateFromId(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                return new Error("unknown.measure.type", $"Possible values for {nameof(MeasureType)}: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
