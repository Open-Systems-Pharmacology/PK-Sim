using PKSim.Infrastructure.ProjectConverter.v9_0;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class IndividualCovariatesXmlSerializer : BaseXmlSerializer<IndividualCovariates>
   {
      public override void PerformMapping()
      {
         Map(x => x.Gender);
         Map(x => x.Race);
         Map(x => x.Attributes);
      }
   }
}