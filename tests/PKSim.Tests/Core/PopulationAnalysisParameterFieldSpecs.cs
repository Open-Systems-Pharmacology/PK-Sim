using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysisParameterField : ContextSpecification<PopulationAnalysisParameterField>
   {
      protected IPopulationDataCollector _populationDataCollector;

      protected override void Context()
      {
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         sut = new PopulationAnalysisParameterField {ParameterPath = "Liver|P"};
      }
   }

   public class When_a_parameter_field_is_retrieving_the_value_for_an_existing_parameter : concern_for_PopulationAnalysisParameterField
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _populationDataCollector.AllValuesFor(sut.ParameterPath)).Returns(new List<double> {1, 2, 3});
      }

      [Observation]
      public void should_return_the_values_defined_for_this_parameter_in_the_population_simulation()
      {
         sut.GetValues(_populationDataCollector).ShouldOnlyContainInOrder(1d, 2d, 3d);
      }
   }
}