using System.Xml.Linq;
using OSPSuite.Utility;
using PKSim.Core;

namespace PKSim.Infrastructure.ProjectConverter
{
   public interface IObjectConverter : ISpecification<int>
   {
      /// <summary>
      /// Should convert the object to convert and return the version corresponding to the new state of the object.
      /// For example, if the converter converts from 5.0 to 5.1 and the current version is 5.2, it should return 5.1
      /// </summary>
      int Convert(object objectToConvert, int originalVersion); 

      /// <summary>
      /// Convert the xml This should only be implemented if the xml strucutre has changed
      /// so drastically, that a basic object conversion cannot do the job
      /// </summary>
      int ConvertXml(XElement element, int originalVersion);
   }

   public class NullConverter : IObjectConverter
   {
      public bool IsSatisfiedBy(int version)
      {
         return false;
      }

      public int Convert(object objectToConvert, int originalVersion) 
      {
         return ProjectVersions.Current;
      }

      public int ConvertXml(XElement element, int originalVersion)
      {
         return ProjectVersions.Current;
      }
   }
}