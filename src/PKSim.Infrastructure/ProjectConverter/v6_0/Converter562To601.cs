﻿using System.Linq;
using System.Xml.Linq;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation;
using OSPSuite.Core.Converter.v6_0;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization.Xml.Extensions;

namespace PKSim.Infrastructure.ProjectConverter.v6_0
{
   public class Converter562To601 : IObjectConverter,
      IVisitor<Individual>,
      IVisitor<Population>,
      IVisitor<Simulation>,
      IVisitor<SimpleProtocol>,
      IVisitor<AdvancedProtocol>,
      IVisitor<IUserSettings>
   {
      private readonly Converter56To601 _coreConverter56To601;
      private readonly IIndividualCalculationMethodsUpdater _individualCalculationMethodsUpdater;
      private bool _converted;

      public Converter562To601(Converter56To601 coreConverter56To601, IIndividualCalculationMethodsUpdater individualCalculationMethodsUpdater)
      {
         _coreConverter56To601 = coreConverter56To601;
         _individualCalculationMethodsUpdater = individualCalculationMethodsUpdater;
      }

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V6_0_1, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         element.DescendantsAndSelfNamed("IndividualSimulation", "PopulationSimulation").Each(convertSimulation);
         element.DescendantsAndSelfNamed("CurveChart").Each(convertCurveChart);
         element.DescendantsAndSelfNamed("Favorites").Each(convertFavorites);
         return (ProjectVersions.V6_0_1, true);
      }

      private void convertFavorites(XElement favoritesElement)
      {
         var allNode = new XElement(Constants.Serialization.ALL);
         favoritesElement.Descendants("Favorite").ToList().Each(fav =>
         {
            fav.Remove();
            allNode.Add(fav);
         });
         favoritesElement.Add(allNode);
      }

      private void convertCurveChart(XElement curveChartElement)
      {
         _coreConverter56To601.ConvertXml(curveChartElement);
      }

      private void convertSimulation(XElement simulationNode)
      {
         var simulationSettingsElement = new XElement("SimulationSettings");

         addElementIfDefined(simulationNode.Element("OutputSelections"), simulationSettingsElement);
         addElementIfDefined(simulationNode.Descendants("SimulationSolverSettings").FirstOrDefault(), simulationSettingsElement, "Solver");
         addElementIfDefined(simulationNode.Descendants("OutputSchema").FirstOrDefault(), simulationSettingsElement);
         //Formula Cache node required for Building blocks
         simulationSettingsElement.Add(new XElement("FormulaCache"));
         simulationNode.Add(simulationSettingsElement);
      }

      private void addElementIfDefined(XElement elementToAdd, XElement parent, string newName = null)
      {
         if (elementToAdd == null) return;

         elementToAdd.Remove();
         if (newName != null)
            elementToAdd.Name = newName;

         parent.Add(elementToAdd);
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V5_6_2;

      private void convertIndividual(Individual individual)
      {
         if (individual == null) return;

         _individualCalculationMethodsUpdater.AddMissingCalculationMethodsTo(individual);

         individual.AllMolecules().SelectMany(m => m.AllExpressionsContainers())
            .Select(c => c.RelativeExpressionParameter)
            .Each(p => p.Dimension = Constants.Dimension.NO_DIMENSION);
     }

  
      public void Visit(Individual individual)
      {
         convertIndividual(individual);
         _converted = true;
      }

      public void Visit(Population population)
      {
         convertIndividual(population.FirstIndividual);
         _converted = true;
      }

      public void Visit(Simulation simulation)
      {
         convertIndividual(simulation.BuildingBlock<Individual>());
         simulation.AllBuildingBlocks<Protocol>().Each(this.Visit);
         changeBuildModeTypeOfMoleculesParameters(simulation);
         _converted = true;
      }

      private void changeBuildModeTypeOfMoleculesParameters(Simulation simulation)
      {
         var root = simulation.Model.Root;
         foreach (var compoundName in simulation.CompoundNames)
         {
            var parameters = ConverterConstants.Parameter.AllCompoundGlobalParameters
               .Select(parameterName => root.EntityAt<IParameter>(compoundName, parameterName))
               .Where(parameter => parameter != null).ToList();

            parameters.Each(p => p.BuildMode = ParameterBuildMode.Global);
         }
      }

      public void Visit(IUserSettings userSettings)
      {
         if (ValueComparer.AreValuesEqual(userSettings.AbsTol, ConverterConstants.OLD_DEFAULT_ABS_TOL))
            userSettings.AbsTol = CoreConstants.DEFAULT_ABS_TOL;

         if (ValueComparer.AreValuesEqual(userSettings.RelTol, ConverterConstants.OLD_DEFAULT_REL_TOL))
            userSettings.RelTol = CoreConstants.DEFAULT_REL_TOL;

         userSettings.ActiveSkin = CoreConstants.DEFAULT_SKIN;
         _converted = true;
      }

      public void Visit(SimpleProtocol simpleProtocol)
      {
         convertSchemaItem(simpleProtocol);
         _converted = true;
      }

      public void Visit(AdvancedProtocol advancedProtocol)
      {
         advancedProtocol.AllSchemas.SelectMany(x => x.SchemaItems).Each(convertSchemaItem);
         _converted = true;
      }

      private void convertSchemaItem(ISchemaItem schemaItem)
      {
         if (schemaItem == null) return;
         schemaItem.Dose.Visible = true;
      }
   }
}