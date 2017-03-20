using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class IndividualPropertiesCacheXmlSerializer : BaseXmlSerializer<IndividualPropertiesCache>
   {
      public override void PerformMapping()
      {
         Map(x => x.ParameterValuesCache);
         MapEnumerable(x => x.AllCovariates, cache => cache.AllCovariates.Add);
      }
   }
}