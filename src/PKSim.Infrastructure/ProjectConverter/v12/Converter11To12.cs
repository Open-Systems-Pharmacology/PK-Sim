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
            convertSimulationElement(element);
         }

         return (ProjectVersions.V12, _converted);
      }

      private void convertSimulationElement(XElement simulationElement)
      {
         renameSimulationSettings(simulationElement);
         restructureReactions(simulationElement);
         _converted = true;
      }

      private void restructureReactions(XElement simulationElement)
      {
         var reactionNode = simulationElement.Element("Reactions");
         if (reactionNode == null)
            return;

         reactionNode.Name = "ReactionBuildingBlock";
         var reactions = new XElement("Reactions");
         reactions.Add(reactionNode);
         simulationElement.Add(reactions);
      }

      private void renameSimulationSettings(XElement simulationElement)
      {
         foreach (var settingsElement in simulationElement.Descendants("SimulationSettings").ToList())
         {
            settingsElement.Name = "Settings";
         }
      }
   }
}