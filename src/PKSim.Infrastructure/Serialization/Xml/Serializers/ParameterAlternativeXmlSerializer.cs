using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ParameterAlternativeXmlSerializer : PKSimContainerXmlSerializer<ParameterAlternative>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.IsDefault);
      }
   }

   public class ParameterAlternativeWithSpeciesXmlSerialier : PKSimContainerXmlSerializer<ParameterAlternativeWithSpecies>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.IsDefault);
         Map(x => x.Species);
      }
   }
}