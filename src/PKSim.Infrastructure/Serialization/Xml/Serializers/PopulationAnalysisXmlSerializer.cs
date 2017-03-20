using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class PopulationAnalysisXmlSerializer<T> : BaseXmlSerializer<T> where T : PopulationAnalysis
   {
      public override void PerformMapping()
      {
         MapEnumerable(x => x.AllFields, x => x.Add);
      }
   }

   public class PopulationAnalysisXmlSerializer : PopulationAnalysisXmlSerializer<PopulationAnalysis>
   {
   }
}