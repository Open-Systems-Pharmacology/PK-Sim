using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Extensions;
using PKSim.Core;

namespace PKSim.Infrastructure.ProjectConverter.v12
{
   public class Converter11To12 : IObjectConverter
   {
      private bool _converted;
      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V11;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         return (ProjectVersions.V12, false);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         _converted = false;
         if (element.Name.IsOneOf("PopulationSimulation", "IndividualSimulation"))
         {
            renameSimulationSettings(element);
            _converted = true;
         }

         return (ProjectVersions.V12, _converted);
      }

      private void renameSimulationSettings(XElement element)
      {
         foreach (var settingsElement in element.Descendants("SimulationSettings").ToList())
         {
            settingsElement.Name = "Settings";
         }
      }
   }
}