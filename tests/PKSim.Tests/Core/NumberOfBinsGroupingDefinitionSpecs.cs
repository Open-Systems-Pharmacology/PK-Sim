using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Maths;

namespace PKSim.Core
{
   public abstract class concern_for_NumberOfBinsGroupingDefinition : ContextSpecification<NumberOfBinsGroupingDefinition>
   {
      private string _fieldName;
      protected PopulationAnalysisNumericField _numericField;
      protected IPopulationDataCollector _populationDataCollector;

      protected override void Context()
      {
         _fieldName = "TOTO";
         _numericField = A.Fake<PopulationAnalysisNumericField>().WithName(_fieldName);
         _populationDataCollector = A.Fake<IPopulationDataCollector>(); 
         sut = new NumberOfBinsGroupingDefinition(_fieldName);
      }
   }

   public class When_calculating_the_limits_for_a_field_containing_nan_values:concern_for_NumberOfBinsGroupingDefinition
   {
      private List<double> _values;
      private float[] _validValues;

      protected override void Context()
      {
         base.Context();
         _values=new List<double>{1d,2d, double.NaN, double.PositiveInfinity,3d, 4d};
         _validValues = new List<double> {1d, 2d, 3d, 4d}.ToFloatArray();
         sut.NumberOfBins = 3;
         A.CallTo(() => _numericField.GetValues(_populationDataCollector)).Returns(_values);
      }

      protected override void Because()
      {
         sut.CreateLimits(_populationDataCollector, _numericField);
      }

      [Observation]
      public void should_filter_out_the_NaN_and_calculate_the_limits_based_on_the_remaining_values()
      {
         var sortedFloatArray = new SortedFloatArray(_validValues, true);
         sut.Limits.ShouldOnlyContainInOrder(sortedFloatArray.Quantile(1 / 3.0), sortedFloatArray.Quantile(2 / 3.0));
      }
   }
}	