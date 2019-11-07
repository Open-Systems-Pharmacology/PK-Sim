using System.Xml.Linq;
using OSPSuite.Utility;
using PKSim.Core;

namespace PKSim.Infrastructure.ProjectConverter
{
   public interface IObjectConverter : ISpecification<int>
   {
      /// <summary>
      /// Converss the <paramref name="objectToConvert"/> and returns the version corresponding to the new state of the object as well as a boolean indicating 
      /// if a conversion actually took place for the <paramref name="objectToConvert"/>
      /// For example, if the converter converts from 5.0 to 5.1 and the current version is 5.2, it should return 5.2
      /// 
      /// </summary>
      (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion);

      /// <summary>
      /// Convert the xml This should only be implemented if the xml structure has changed so drastically, that a basic object conversion cannot do the job
      /// </summary>
      (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion);
   }

   public class NullConverter : IObjectConverter
   {
      public bool IsSatisfiedBy(int version)
      {
         return false;
      }

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion) 
      {
         return (ProjectVersions.Current, false);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         return (ProjectVersions.Current, false);
      }
   }
}