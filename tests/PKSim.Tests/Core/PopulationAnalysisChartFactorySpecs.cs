using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysisChartFactory : ContextSpecification<IPopulationAnalysisChartFactory>
   {
      private IPopulationAnalysisFactory _populationAnalysisFactory;

      protected override void Context()
      {
         _populationAnalysisFactory= A.Fake<IPopulationAnalysisFactory>();
         sut = new PopulationAnalysisChartFactory(_populationAnalysisFactory);
      }
   }

   public class When_creating_a_population_analysis_chart_by_analysis_type : concern_for_PopulationAnalysisChartFactory
   {
      [Observation]
      public void should_return_the_expected_analysis_chart()
      {
         sut.Create(PopulationAnalysisType.BoxWhisker).ShouldBeAnInstanceOf<BoxWhiskerAnalysisChart>();
         sut.Create(PopulationAnalysisType.TimeProfile).ShouldBeAnInstanceOf<TimeProfileAnalysisChart>();
         sut.Create(PopulationAnalysisType.Scatter).ShouldBeAnInstanceOf<ScatterAnalysisChart>();
         sut.Create(PopulationAnalysisType.Range).ShouldBeAnInstanceOf<RangeAnalysisChart>();
      }
   }
}	