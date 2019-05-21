using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class PopulationFileXmlSerializer : BaseXmlSerializer<PopulationImportFile>
   {
      public override void PerformMapping()
      {
         Map(x => x.FilePath);
         Map(x => x.NumberOfIndividuals);
      }
   }
}