using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public class When_serializing_a_population_analysis_workflow : ContextForSerialization<SimulationAnalysisWorkflow>
   {
      private SimulationAnalysisWorkflow _workflow;
      private SimulationAnalysisWorkflow _deserializedWorkflow;

      protected override void Context()
      {
         base.Context();
         _workflow = new SimulationAnalysisWorkflow();
         _workflow.Add(new BoxWhiskerAnalysisChart{PopulationAnalysis = new PopulationBoxWhiskerAnalysis()});
         _workflow.Add(new TimeProfileAnalysisChart{ PopulationAnalysis = new PopulationStatisticalAnalysis()} );
         _workflow.Add(new ScatterAnalysisChart { PopulationAnalysis = new PopulationPivotAnalysis() });

         _workflow.OutputSelections = new OutputSelections();
         _workflow.OutputSelections.AddOutput(new QuantitySelection("PATH", QuantityType.Enzyme));
      }

      protected override void Because()
      {
         _deserializedWorkflow = SerializeAndDeserialize(_workflow);
      }

      [Observation]
      public void should_have_deserialized_the_analyses()
      {
         _deserializedWorkflow.AllAnalyses.Count.ShouldBeEqualTo(3);
         _deserializedWorkflow.AllAnalyses[0].ShouldBeAnInstanceOf<BoxWhiskerAnalysisChart>();
         _deserializedWorkflow.AllAnalyses[1].ShouldBeAnInstanceOf<TimeProfileAnalysisChart>();
         _deserializedWorkflow.AllAnalyses[2].ShouldBeAnInstanceOf<ScatterAnalysisChart>();
      }

      [Observation]
      public void should_have_deserialized_the_output_selection()
      {
         var quantitySeleciton = _deserializedWorkflow.OutputSelections.AllOutputs.ElementAt(0);
         quantitySeleciton.Path.ShouldBeEqualTo("PATH");
      }
   }
}