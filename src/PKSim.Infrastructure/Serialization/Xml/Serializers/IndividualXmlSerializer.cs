using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Xml;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.Xml.Extensions;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class IndividualXmlSerializer : BuildingBlockXmlSerializer<Individual>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.OriginData);
         Map(x => x.Seed);

      }
   }
}