using FluentAssertions;
using Primitives;
using Products.Core.Domain.Model.MeasureAggregate;
using Products.Core.Domain.Model.SharedKernel;

namespace Products.UnitTests.Domain.Model.MeasureAggregateTests
{
    public class MeasureTests
    {
        public static IEnumerable<object[]> ValidMeasureTypes => new List<object[]>
        {
            new object[] { MeasureType.Weight },
            new object[] { MeasureType.Liquid }
        };

        public static IEnumerable<object[]> ValidMeasureData => new List<object[]>
        {
            new object[]
            {
                MeasureFullName.Create("Килограмм").Value,
                MeasureShortName.Create("кг").Value,
                MeasureType.Weight
            },
            new object[]
            {
                MeasureFullName.Create("Грамм").Value,
                MeasureShortName.Create("г").Value,
                MeasureType.Weight
            },
            new object[]
            {
                MeasureFullName.Create("Литр").Value,
                MeasureShortName.Create("л").Value,
                MeasureType.Liquid
            },
            new object[]
            {
                MeasureFullName.Create("Миллилитр").Value,
                MeasureShortName.Create("мл").Value,
                MeasureType.Liquid
            }
        };

        [Theory]
        [MemberData(nameof(ValidMeasureData))]
        public void Create_WithValidParameters_ShouldReturnMeasure(
            MeasureFullName fullName,
            MeasureShortName shortName,
            MeasureType measureType)
        {
            // Act
            var result = Measure.Create(fullName, shortName, measureType);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().NotBe(Guid.Empty);
            result.Value.FullName.Should().Be(fullName);
            result.Value.ShortName.Should().Be(shortName);
            result.Value.MeasureType.Should().Be(measureType);
            result.Value.IsArchive.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(ValidMeasureTypes))]
        public void Create_WithDifferentMeasureTypes_ShouldSucceed(MeasureType measureType)
        {
            // Arrange
            var fullName = MeasureFullName.Create("Тест").Value;
            var shortName = MeasureShortName.Create("т").Value;

            // Act
            var result = Measure.Create(fullName, shortName, measureType);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.MeasureType.Should().Be(measureType);
        }

        [Fact]
        public void Create_WithNullFullName_ShouldReturnError()
        {
            // Arrange
            var shortName = MeasureShortName.Create("кг").Value;
            var measureType = MeasureType.Weight;

            // Act
            var result = Measure.Create(null, shortName, measureType);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void Create_WithNullShortName_ShouldReturnError()
        {
            // Arrange
            var fullName = MeasureFullName.Create("Килограмм").Value;
            var measureType = MeasureType.Weight;

            // Act
            var result = Measure.Create(fullName, null, measureType);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void Create_WithNullMeasureType_ShouldReturnError()
        {
            // Arrange
            var fullName = MeasureFullName.Create("Килограмм").Value;
            var shortName = MeasureShortName.Create("кг").Value;

            // Act
            var result = Measure.Create(fullName, shortName, null);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void Create_WithAllNullParameters_ShouldReturnError()
        {
            // Act
            var result = Measure.Create(null, null, null);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void Create_ShouldGenerateNewGuid()
        {
            // Arrange
            var fullName = MeasureFullName.Create("Литр").Value;
            var shortName = MeasureShortName.Create("л").Value;
            var measureType = MeasureType.Liquid;

            // Act
            var result1 = Measure.Create(fullName, shortName, measureType);
            var result2 = Measure.Create(fullName, shortName, measureType);

            // Assert
            result1.Value.Id.Should().NotBe(Guid.Empty);
            result2.Value.Id.Should().NotBe(Guid.Empty);
            result1.Value.Id.Should().NotBe(result2.Value.Id);
        }

        [Fact]
        public void MakeArchive_WhenNotArchived_ShouldArchiveSuccessfully()
        {
            // Arrange
            var measure = CreateTestMeasure();

            // Act
            var result = measure.MakeArchive();

            // Assert
            result.IsSuccess.Should().BeTrue();
            measure.IsArchive.Should().BeTrue();
        }

        [Fact]
        public void MakeArchive_WhenAlreadyArchived_ShouldReturnErrorWithMeasureTypeName()
        {
            // Arrange
            var measure = CreateTestMeasure();
            measure.MakeArchive(); // Первая архивация

            // Act
            var result = measure.MakeArchive();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
            // Проверяем, что в ошибке содержится имя типа Measure
            result.Error.Message.Should().Contain(nameof(Measure));
        }

        [Fact]
        public void Measure_ShouldInheritFromArchivableAggregate()
        {
            // Arrange
            var measure = CreateTestMeasure();

            // Act & Assert
            measure.Should().BeAssignableTo<ArchivableAggregate<Guid>>();
        }

        [Fact]
        public void PrivateConstructor_ShouldCreateObjectWithCorrectProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var fullName = MeasureFullName.Create("Метр").Value;
            var shortName = MeasureShortName.Create("м").Value;
            var measureType = MeasureType.Weight;

            // Act - используем рефлексию для вызова приватного конструктора
            var measure = Activator.CreateInstance(typeof(Measure), true) as Measure;
            // Устанавливаем свойства через рефлексию (для тестирования приватного конструктора)
            var idProperty = typeof(Measure).GetProperty("Id");
            var fullNameProperty = typeof(Measure).GetProperty("FullName");
            var shortNameProperty = typeof(Measure).GetProperty("ShortName");
            var measureTypeProperty = typeof(Measure).GetProperty("MeasureType");
            var isArchiveProperty = typeof(Measure).GetProperty("IsArchive");

            idProperty?.SetValue(measure, id);
            fullNameProperty?.SetValue(measure, fullName);
            shortNameProperty?.SetValue(measure, shortName);
            measureTypeProperty?.SetValue(measure, measureType);
            isArchiveProperty?.SetValue(measure, false);

            // Assert
            measure.Should().NotBeNull();
            measure.Id.Should().Be(id);
            measure.FullName.Should().Be(fullName);
            measure.ShortName.Should().Be(shortName);
            measure.MeasureType.Should().Be(measureType);
            measure.IsArchive.Should().BeFalse();
        }

        [Fact]
        public void TwoMeasures_WithSameValues_ShouldNotBeEqualDueToDifferentIds()
        {
            // Arrange
            var fullName = MeasureFullName.Create("Штука").Value;
            var shortName = MeasureShortName.Create("шт").Value;
            var measureType = MeasureType.Weight;

            var measure1 = Measure.Create(fullName, shortName, measureType).Value;
            var measure2 = Measure.Create(fullName, shortName, measureType).Value;

            // Act & Assert
            measure1.Should().NotBe(measure2);
            measure1.Id.Should().NotBe(measure2.Id);
            measure1.FullName.Should().Be(measure2.FullName);
            measure1.ShortName.Should().Be(measure2.ShortName);
            measure1.MeasureType.Should().Be(measure2.MeasureType);
        }

        [Fact]
        public void Create_WithWeightMeasureType_ShouldSetCorrectProperties()
        {
            // Arrange
            var fullName = MeasureFullName.Create("Килограмм").Value;
            var shortName = MeasureShortName.Create("кг").Value;
            var measureType = MeasureType.Weight;

            // Act
            var result = Measure.Create(fullName, shortName, measureType);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.MeasureType.Id.Should().Be(1);
            result.Value.MeasureType.Name.Should().Be("weight");
        }

        [Fact]
        public void Create_WithLiquidMeasureType_ShouldSetCorrectProperties()
        {
            // Arrange
            var fullName = MeasureFullName.Create("Литр").Value;
            var shortName = MeasureShortName.Create("л").Value;
            var measureType = MeasureType.Liquid;

            // Act
            var result = Measure.Create(fullName, shortName, measureType);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.MeasureType.Id.Should().Be(2);
            result.Value.MeasureType.Name.Should().Be("liquid");
        }

        private static Measure CreateTestMeasure()
        {
            var fullName = MeasureFullName.Create("Килограмм").Value;
            var shortName = MeasureShortName.Create("кг").Value;
            var measureType = MeasureType.Weight;

            return Measure.Create(fullName, shortName, measureType).Value;
        }
    }
}
