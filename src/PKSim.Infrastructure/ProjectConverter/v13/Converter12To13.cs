using PKSim.Core;
using System.Xml.Linq;
using CoreConverter121To130 = OSPSuite.Core.Converters.v13.Converter121To130;

namespace PKSim.Infrastructure.ProjectConverter.v13
{
   public class Converter12To13 : IObjectConverter
   {
      private readonly CoreConverter121To130 _coreConverter;

      public Converter12To13(CoreConverter121To130 coreConverter)
      {
         _coreConverter = coreConverter;
      }
      // 13.0 is not compatible with 12.0, but you don't need an explicit conversion to move forward.
      // To satisfy the next converter, the object must pass through v13.0 conversion
      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V12;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         (_, bool converted) = _coreConverter.Convert(objectToConvert);
         return (ProjectVersions.V13, converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         (_, bool converted) = _coreConverter.ConvertXml(element);
         return (ProjectVersions.V13, converted);
      }
   }
}
