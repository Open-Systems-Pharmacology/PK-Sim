using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ContainerParameterBySpeciesSerializer : BaseXmlSerializer<ContainerParameterBySpecies>
   {
      public override void PerformMapping()
      {
         Map(x => x.ContainerPath);
         Map(x => x.ParameterName);
         Map(x => x.SpeciesCount);
      }
   }
}