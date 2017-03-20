using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class AgingDataXmlSerializer : BaseXmlSerializer<AgingData>
   {
      public override void PerformMapping()
      {
         MapEnumerable(x => x.AllParameterData, x => x.Add);
      }
   }

   public class ParameterAgingDataXmlSerializer : BaseXmlSerializer<ParameterAgingData>
   {
      public override void PerformMapping()
      {
         Map(x => x.ParameterPath);
         Map(x => x.IndividualIndexes);
         Map(x => x.Values);
         Map(x => x.Times);
      }
   }
}