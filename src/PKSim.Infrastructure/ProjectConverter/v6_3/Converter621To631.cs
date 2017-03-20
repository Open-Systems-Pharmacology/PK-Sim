using System.Xml.Linq;
using PKSim.Core;
using OSPSuite.Core.Converter.v6_3;

namespace PKSim.Infrastructure.ProjectConverter.v6_3
{
   public class Converter621To631 : IObjectConverter
   {
      private readonly Converter62To63 _coreConverter;

      public Converter621To631(Converter62To63 coreConverter)
      {
         _coreConverter = coreConverter;
      }

      public bool IsSatisfiedBy(int version)
      {
         return version == ProjectVersions.V6_2_1;
      }

      public int Convert(object objectToConvert, int originalVersion)
      {
         _coreConverter.Convert(objectToConvert);
         return ProjectVersions.V6_3_1;
      }

      public int ConvertXml(XElement element, int originalVersion)
      {
         _coreConverter.ConvertXml(element);
         return ProjectVersions.V6_3_1;
      }
   }
}