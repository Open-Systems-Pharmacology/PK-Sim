using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class PopulationSimulationPKAnalysesXmlSerializer : BaseXmlSerializer<PopulationSimulationPKAnalyses>
   {
      public override void PerformMapping()
      {
         MapEnumerable(x => x.All(), x => x.AddPKAnalysis);
      }
   }
}