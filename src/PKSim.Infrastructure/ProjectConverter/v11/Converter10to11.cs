using System.Xml.Linq;
using PKSim.Core;

namespace PKSim.Infrastructure.ProjectConverter.v11
{
   public class Converter10to11 : IObjectConverter
   {
      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V10;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         return (ProjectVersions.V11, false);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         return (ProjectVersions.V11, false);
      }
   }
}