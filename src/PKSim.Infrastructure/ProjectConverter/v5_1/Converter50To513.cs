using System;
using System.Linq;
using System.Xml.Linq;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Services;
using OSPSuite.Core.Converter.v5_2;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Infrastructure.ProjectConverter.v5_1
{
   public class Converter50To513 : IObjectConverter,
                                   IVisitor<Individual>,
                                   IVisitor<Simulation>,
                                   IVisitor<Compound>
   {
      private readonly IModelPropertiesTask _modelPropertiesTask;
      private readonly IDimensionConverter _dimensionConverter;
      private const string _transportType = "transportType";

      public Converter50To513(IModelPropertiesTask modelPropertiesTask, IDimensionConverter dimensionConverter)
      {
         _modelPropertiesTask = modelPropertiesTask;
         _dimensionConverter = dimensionConverter;
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V5_0_1;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         this.Visit(objectToConvert);
         return (ProjectVersions.V5_1_3, true);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         _dimensionConverter.ConvertDimensionIn(element);

         //only need to convert individual
         if (!elementNeedsToBeConverted(element))
            return (ProjectVersions.V5_1_3, true);

         //here we need to convert the nodes for transporter
         var allTransporterNodes = element.Descendants("IndividualTransporter");
         foreach (var transporterNode in allTransporterNodes)
         {
            var containerByType = transporterNode.Descendants("TransporterExpressionContainer").GroupBy(x => x.GetAttribute(_transportType))
               .Select(g => new {Count = g.Count(), TranstportType = g.Key})
               .OrderByDescending(x => x.Count)
               .FirstOrDefault();

            //should never happen
            if (containerByType == null)
               continue;

            transporterNode.AddAttribute(_transportType, containerByType.TranstportType);
         }

         return (ProjectVersions.V5_1_3, true);
      }

      private static bool elementNeedsToBeConverted(XElement element)
      {
         return element.Name == "Individual" || element.Name == "IndividualSimulation" || element.Name == "PopulationSimulation";
      }

      public void Visit(Individual individual)
      {
         convertAllLogNormalDistributedParametersIn(individual);
      }

      private void convertAllLogNormalDistributedParametersIn(IContainer container)
      {
         var allDistributedParameter = container.GetAllChildren<IDistributedParameter>(x => x.Formula.DistributionType() == DistributionTypes.LogNormal);
         foreach (var deviation in allDistributedParameter.Select(p => p.Parameter(Constants.Distribution.GEOMETRIC_DEVIATION)))
         {
            deviation.Value = Math.Exp(deviation.Value);
         }
      }

      public void Visit(Simulation simulation)
      {
         Visit(simulation.Individual);
         Visit(simulation.BuildingBlock<Compound>());

         var root = simulation.Model.Root;
         convertAllLogNormalDistributedParametersIn(root);

         //sink vs non sink
         updateDrugAbsorptionToMucosa(simulation);

         var lumen = root.Container(Constants.ORGANISM)
            .Container(CoreConstants.Organ.Lumen);

         updateSinkValue(lumen, CoreConstants.Parameter.TRANS_ABSORBTION_SINK);
         updateSinkValue(lumen, CoreConstants.Parameter.PARA_ABSORBTION_SINK);

         //make sure model properties are uptodate
         _modelPropertiesTask.UpdateCategoriesIn(simulation.ModelProperties, simulation.Individual.OriginData);
      }

      private void updateSinkValue(IContainer lumen, string paraSinkName)
      {
         var oldValue = lumen.Parameter(paraSinkName).Value;
         lumen.Parameter(paraSinkName).Value = 1 - oldValue;
      }

      private static void updateDrugAbsorptionToMucosa(Simulation simulation)
      {
         foreach (var segmName in CoreConstants.Compartment.LumenSegmentsDuodenumToRectum)
         {
            //DrugAbsorptionToMucosaCell - transports available in all lumen segments
            replaceSinkFormula(simulation, segmName, "_cell", "k_Liquid_trans");

            //DrugAbsorptionToMucosaPlasma - transports available only in small intestine segments
            replaceSinkFormula(simulation, segmName, "_pls", "k_Liquid_para");
         }
      }

      private static void replaceSinkFormula(Simulation simulation,
                                             string lumenSegmentName,
                                             string targetCompartmentExtension,
                                             string oldSinkConditionAlias)
      {
         var lumenSegmentExtension = ConverterConstants.LumenSegmentExtensionFor(lumenSegmentName);
         string neighborhoodName = $"Lumen{lumenSegmentExtension}_{lumenSegmentName}{targetCompartmentExtension}";

         var drugAbsorptionRateToMucosa = simulation.Model.Root
            .Container(Constants.NEIGHBORHOODS)
            .Container(neighborhoodName)
            .GetAllChildren<IParameter>(x => x.IsNamed(ConverterConstants.Parameter.DrugAbsorptionLumenToMucosaRate))
            .FirstOrDefault();

         var formula = drugAbsorptionRateToMucosa?.Formula as ExplicitFormula;
         if (formula == null) return;

         formula.FormulaString = formula.FormulaString.Replace(oldSinkConditionAlias, "NOT " + oldSinkConditionAlias);
      }

      public void Visit(Compound compound)
      {
         var allSystemicProcesses = compound.AllProcesses<SystemicProcess>();
         allSystemicProcesses.Each(sp =>
            {
               sp.DataSource = sp.Name;
               sp.RefreshName();
            });
      }
   }
}