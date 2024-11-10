using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.ProjectConverter.v12
{
   public class Converter11To12 : IObjectConverter,
      IVisitor<Simulation>,
      IVisitor<Individual>,
      IVisitor<Population>
   {
      private readonly IDefaultIndividualRetriever _defaultIndividualRetriever;
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      private readonly ICloner _cloner;
      private bool _converted;
      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V11;

      public Converter11To12(
         IDefaultIndividualRetriever defaultIndividualRetriever,
         IBuildingBlockRepository buildingBlockRepository,
         ISimulationBuildingBlockUpdater simulationBuildingBlockUpdater,
         ICloner cloner)
      {
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _buildingBlockRepository = buildingBlockRepository;
         _simulationBuildingBlockUpdater = simulationBuildingBlockUpdater;
         _cloner = cloner;
      }

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V12, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         _converted = false;
         if (element.Name.IsOneOf("PopulationSimulation", "IndividualSimulation"))
         {
            convertSimulationElement(element);
         }

         foreach (var calculationMethodCacheElement in element.DescendantsAndSelf("CalculationMethodCache"))
         {
            convertCalculationMethods(calculationMethodCacheElement);
         }

         element.DescendantsAndSelf().Where(x => x.GetAttribute("mode") != null).Each(convertBuildMode);

         return (ProjectVersions.V12, _converted);
      }

      private void convertBuildMode(XElement parameterNode)
      {
         var buildMode = parameterNode.GetAttribute("mode");

         if (!string.Equals(buildMode, "Property"))
            return;

         parameterNode.SetAttributeValue("mode", "Global");
         _converted = true;
      }

      private void convertCalculationMethods(XElement calculationMethodCacheElement)
      {
         var all = calculationMethodCacheElement.Element("All");
         if (all == null)
            return;

         //TO list because we are going to modify the list while we iterate
         foreach (var calculationMethodElement in all.Elements().ToList())
         {
            var name = calculationMethodElement.GetAttribute("name");
            //BMI Calculation method was split into TWO calculation methods. So we need to rename one and ADD A new one
            if (string.Equals(name, ConverterConstants.CalculationMethod.BMI))
            {
               calculationMethodElement.SetAttributeValue("name", ConverterConstants.CalculationMethod.Individual_HeightDependent);
               var newElement = new XElement(calculationMethodElement.Name);
               newElement.SetAttributeValue("name", ConverterConstants.CalculationMethod.Individual_AgeDependent);
               all.Add(newElement);
               _converted = true;
            }
         }
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

      public void Visit(Simulation simulation)
      {
         convertSimulation(simulation);
      }

      public void Visit(Individual individual)
      {
         convertIndividual(individual);
      }

      public void Visit(Population population)
      {
         convertPopulation(population);
      }

      private void convertPopulation(Population population)
      {
         convertIndividual(population.FirstIndividual);
      }

      private void convertIndividual(Individual individual)
      {
         if (individual == null)
            return;

         if (!individual.IsHuman)
            return;

         if (individual.Organism.Parameter(CoreConstants.Parameters.PMA) != null)
            return;

         var defaultHuman = _defaultIndividualRetriever.DefaultHuman();
         var pma = defaultHuman.Organism.Parameter(CoreConstants.Parameters.PMA);

         individual.Organism.Add(_cloner.Clone(pma));
      }

      private void convertSimulation(Simulation simulation)
      {
         convertIndividual(simulation.BuildingBlock<Individual>());

         addMissingExpressionBuildingBlock(simulation);
      }

      private void addMissingExpressionBuildingBlock(Simulation simulation)
      {
         //in v11, the usage of the expression building block was not saved in the used building block. We need to add it if it's not there already

         var usedSimulationSubject = simulation.UsedBuildingBlockInSimulation<ISimulationSubject>();

         //this can happen for an imported simulation
         if (usedSimulationSubject == null)
            return;

         var templateSimulationSubject = _buildingBlockRepository.ById<ISimulationSubject>(usedSimulationSubject.TemplateId);
         //this should never happen
         if (templateSimulationSubject == null)
            return;

         _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(simulation, templateSimulationSubject.AllExpressionProfiles(), PKSimBuildingBlockType.ExpressionProfile);
      }
   }
}