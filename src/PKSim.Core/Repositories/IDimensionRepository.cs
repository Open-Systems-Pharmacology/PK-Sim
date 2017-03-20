using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Repositories
{
   public interface IDimensionRepository : IStartableRepository<IDimension>
   {
      IDimension DimensionByName(string dimensionName);
      IDimension DosePerBodyWeight { get; }
      IDimension InputDose { get; }
      IDimension Time { get; }
      IDimension MassConcentration { get; }
      IDimension MolarConcentration { get; }
      IDimension NoDimension { get; }
      IDimension Fraction { get; }
      IDimension Amount { get; }
      IDimension Mass { get; }
      IDimensionFactory DimensionFactory { get; }
      IDimension AucMolar { get; }
      IDimension Auc { get; }
      IDimension AgeInYears { get; }
      IDimension Length { get; }
      IDimension BMI { get; }
      IDimension Volume { get; }
      IDimension AmountPerTime { get; }
      IDimension AgeInWeeks { get; }
      IDimension MergedDimensionFor(IWithDimension objectWithDimension);
      /// <summary>
      /// Returns the dimension if the <paramref name="objectThatMightHaveDimension"/> implements the <see cref="IWithDimension"/> interface
      /// of the NO_DIMENSION otherwise
      /// </summary>
      IDimension MergedDimensionFor(object objectThatMightHaveDimension);
   }
}