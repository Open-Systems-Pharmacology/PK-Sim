using OSPSuite.Core.Serialization.Xml;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class TableFormulaWithReferenceXmlSerializer : FormulaXmlSerializer<TableFormulaWithXArgument>, IPKSimXmlSerializer
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.TableObjectAlias);
         Map(x => x.XArgumentAlias);
      }
   }
}