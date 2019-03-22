using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;


namespace PKSim.Core
{
   public abstract class concern_for_AnalysableToSimulationAnalysisWorkflowMapper : ContextSpecification<IAnalysableToSimulationAnalysisWorkflowMapper>
   {

      protected override void Context()
      {
         sut = new AnalysableToSimulationAnalysisWorkflowMapper();
      }
   }

   public class When_mapping_a_population_simulation_to_a_simulation_analysis_workflow : concern_for_AnalysableToSimulationAnalysisWorkflowMapper
   {
      private PopulationSimulation _populationSimulation;
      private ISimulationAnalysis _analysis1;
      private ISimulationAnalysis _analysis2;
      private SimulationAnalysisWorkflow _result;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = new PopulationSimulation {SimulationSettings = new SimulationSettings()};
         _analysis1= A.Fake<ISimulationAnalysis>();
         _analysis2= A.Fake<ISimulationAnalysis>();
         _populationSimulation.AddAnalysis(_analysis1);
         _populationSimulation.AddAnalysis(_analysis2);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_populationSimulation);
      }

      [Observation]
      public void should_create_a_new_workflow_containing_each_analysis_defined_in_the_simulation()
      {
         _result.AllAnalyses.ShouldOnlyContain(_analysis1,_analysis2 );
      }


      [Observation]
      public void should_create_a_new_workflow_containing_the_output_selection_of_the_simulation()
      {
         _result.OutputSelections.ShouldBeEqualTo(_populationSimulation.OutputSelections);
      }
   }
}	