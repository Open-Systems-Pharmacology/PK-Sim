using PKSim.Core;
using System.Xml.Linq;

namespace PKSim.Infrastructure.ProjectConverter.v13
{
   public class Converter11To12 : IObjectConverter
   {
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
