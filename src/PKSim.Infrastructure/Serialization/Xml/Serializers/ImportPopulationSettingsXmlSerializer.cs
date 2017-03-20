using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ImportPopulationSettingsXmlSerializer : BaseXmlSerializer<ImportPopulationSettings>
   {
      public override void PerformMapping()
      {
         Map(x => x.BaseIndividual);
         MapEnumerable(x => x.AllFiles, x => x.AddFile);
      }
   }
}