using System.Xml.Linq;
using PKSim.Core;
using OSPSuite.Core.Converters.v6_3;

namespace PKSim.Infrastructure.ProjectConverter.v6_3
{
   public class Converter621To631 : IObjectConverter
   {
      private readonly Converter62To63 _coreConverter;

      public Converter621To631(Converter62To63 coreConverter)
      {
         _coreConverter = coreConverter;
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V6_2_1;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _coreConverter.Convert(objectToConvert);
         return (ProjectVersions.V6_3_1, true);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         _coreConverter.ConvertXml(element);
         return (ProjectVersions.V6_3_1, true);
      }
   }
}