using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class SimulationAnalysisWorkflowXmlSerializer : BaseXmlSerializer<SimulationAnalysisWorkflow>
   {
      public override void PerformMapping()
      {
         Map(x => x.OutputSelections);
         MapEnumerable(x => x.AllAnalyses, x => x.Add);
      }
   }
}