using PKSim.Core;
using System.Xml.Linq;

namespace PKSim.Infrastructure.ProjectConverter.v13
{
   public class Converter12To13 : IObjectConverter
   {
      // 13.0 is not compatible with 12.0, but you don't need an explicit conversion to move forward.
      // To satisfy the next converter, the object must pass through v13.0 conversion
      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V12;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         return (ProjectVersions.V13, false);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         return (ProjectVersions.V13, false);
      }
   }
}
