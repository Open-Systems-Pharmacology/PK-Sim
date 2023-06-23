using System.Xml.Linq;
using PKSim.Core.Model;
using OSPSuite.Core.Serialization.Xml;
using static OSPSuite.Core.Domain.Constants;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class SystemicProcessSelectionXmlSerializer : BaseXmlSerializer<SystemicProcessSelection>
   {
      public override void PerformMapping()
      {
         Map(x => x.CompoundName);
         Map(x => x.ProcessType);
         Map(x => x.ProcessName);
      }

      protected override void TypedDeserialize(SystemicProcessSelection systemicProcessSelection, XElement element, SerializationContext context)
      {
         base.TypedDeserialize(systemicProcessSelection, element, context);
         //in 5.0.1, name of process did not contain data source.

         if (systemicProcessSelection.ProcessType == null || string.IsNullOrEmpty(systemicProcessSelection.ProcessName))
            return;

         var systemicProcessName = systemicProcessSelection.ProcessType.DisplayName;
         if (systemicProcessSelection.ProcessName.Contains(systemicProcessName))
            return;

         //we need to adjust the process name
         systemicProcessSelection.ProcessName = CompositeNameFor(systemicProcessName, systemicProcessSelection.ProcessName);
      }
   }
}