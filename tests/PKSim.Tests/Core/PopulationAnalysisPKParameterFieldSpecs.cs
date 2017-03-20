using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysisPKParameterField : ContextSpecification<PopulationAnalysisPKParameterField>
   {
      protected IPopulationDataCollector _populationDataCollector;
      protected PopulationSimulationPKAnalyses _pkAnalysis;

      protected override void Context()
      {
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         sut = new PopulationAnalysisPKParameterField {PKParameter = "PK", QuantityPath = "Path"};
      }
   }

   public class When_a_pk_parameter_field_is_retrieving_the_value_for_an_existing_pk_parameter : concern_for_PopulationAnalysisPKParameterField
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _populationDataCollector.AllPKParameterValuesFor(sut.QuantityPath, sut.PKParameter)).Returns(new List<double> {1, 2, 3});
      }

      [Observation]
      public void should_return_the_values_defined_for_this_pk_parameter()
      {
         sut.GetValues(_populationDataCollector).ShouldOnlyContainInOrder(1d, 2d, 3d);
      }
   }
}