using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class CompoundProcessesSelectionXmlSerializer : BaseXmlSerializer<CompoundProcessesSelection>
   {
      public override void PerformMapping()
      {
         Map(x => x.MetabolizationSelection);
         Map(x => x.TransportAndExcretionSelection);
         Map(x => x.SpecificBindingSelection);

      }
   }
}