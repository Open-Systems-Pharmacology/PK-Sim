using System.Xml.Linq;
using PKSim.Core.Model;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ProcessSelectionGroupXmlSerializer : BaseXmlSerializer<ProcessSelectionGroup>
   {
      public override void PerformMapping()
      {
         Map(x => x.ProductNameTemplate);
         MapEnumerable(x => x.AllPartialProcesses(),x => x.AddPartialProcessSelection);
         MapEnumerable(x => x.AllSystemicProcesses(),x => x.AddSystemicProcessSelection);
      }

      public override ProcessSelectionGroup CreateObject(XElement element, SerializationContext context)
      {
         //no default constructor necessary here. just return a standard object
         return new ProcessSelectionGroup(string.Empty);
      }
   }
}