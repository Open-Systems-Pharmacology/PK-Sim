using System.Linq;
using System.Xml.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using OSPSuite.Core.Serialization.Xml.Extensions;

namespace PKSim.Infrastructure.ProjectConverter.v5_5
{
   public class Converter54To551 : IObjectConverter
   {
      private bool _converted;

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V5_4_1;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         return (ProjectVersions.V5_5_1, false);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         _converted = false;
         element.DescendantsAndSelfNamed("IndividualSimulation", "PopulationSimulation").Each(convertSimulationSettingsIn);
         return (ProjectVersions.V5_5_1, _converted);
      }

      private void convertSimulationSettingsIn(XElement simulationElement)
      {
         var allOutputs = new XElement("AllOutputs");
         var outputSelections = new XElement("OutputSelections",allOutputs);
       
         foreach (var quantitySelection in simulationElement.Descendants("QuantitySelection").ToList())
         {
            allOutputs.Add(quantitySelection);
         }

         //remove selected quantities node
         foreach (var simulationSettingsNode in simulationElement.Descendants("SelectedQuantities").Select(x=>x.Parent).ToList())
         {
            simulationSettingsNode.Remove();
         }

         simulationElement.Add(outputSelections);
         _converted = true;
      }
   }
}