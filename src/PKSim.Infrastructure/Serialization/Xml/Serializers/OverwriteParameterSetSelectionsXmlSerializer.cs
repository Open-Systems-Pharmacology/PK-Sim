using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class OverwriteParameterSetSelectionsXmlSerializer : BaseXmlSerializer<OverwriteParameterSetSelections>
   {
      public override void PerformMapping()
      {
         MapEnumerable(x => x.Selections, x => x.SetSelectionForCompound);
      }
   }
}
