using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.DTO.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Services;
using OSPSuite.Assets;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Mappers;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.UI
{
   public interface IToolTipCreator : OSPSuite.UI.Services.IToolTipCreator
   {
      SuperToolTip ToolTipFor(IParameterDTO parameterDTO);
      SuperToolTip ToolTipFor(SystemicProcessDTO systemicProcessDTO);
      SuperToolTip ToolTipFor(TransporterExpressionParameterDTO transporterExpressionContainerDTO);
      SuperToolTip ToolTipFor(CategoryCategoryItemDTO categoryCalculationMethodDTO);
      SuperToolTip ToolTipForPKAnalysis(string parameterDisplayName, string displayValue, string warning);
      SuperToolTip CreateToolTip(string content, string title);
      SuperToolTip CreateToolTip(string content);
      SuperToolTip CreateToolTip(string content, Image image);
      SuperToolTip WarningToolTip(string warning);
      SuperToolTip ToolTipFor(ParameterAlternativeDTO parameterAlternativeDTO);
      SuperToolTip ToolTipFor(IEnumerable<ToolTipPart> toolTipParts);
      SuperToolTip ToolTipFor(SimulationResultsFileSelectionDTO simulationResultsFileSelectionDTO);
      SuperToolTip ToolTipFor(QuantityPKParameterDTO quantityPKParameterDTO);
      SuperToolTip ToolTipFor(IPopulationAnalysisField populationAnalysisField);

      SuperToolTip ToolTipFor<TX, TY>(CurveData<TX, TY> curveData, double xDisplayValue, double yDisplayValue)
         where TX : IXValue
         where TY : IYValue;

   }

   public class ToolTipCreator : OSPSuite.UI.Services.ToolTipCreator, IToolTipCreator
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IToolTipPartCreator _toolTipPartCreator;
      private readonly ToolTipPartsToSuperToolTipMapper _toolTipMapper;

      public ToolTipCreator(IRepresentationInfoRepository representationInfoRepository, IToolTipPartCreator toolTipPartCreator)
      {
         _representationInfoRepository = representationInfoRepository;
         _toolTipPartCreator = toolTipPartCreator;
         _toolTipMapper = new ToolTipPartsToSuperToolTipMapper();
      }

      public SuperToolTip ToolTipFor(IParameterDTO parameterDTO)
      {
         var toolTip = CreateToolTip(parameterDTO.Description, PKSimConstants.Information.ParameterDescription);
         addValueDescriptionToolTip(toolTip, parameterDTO);
         addFormulaToolTipTo(toolTip, parameterDTO);
         return toolTip;
      }

      public SuperToolTip ToolTipFor(SystemicProcessDTO systemicProcessDTO)
      {
         return CreateToolTip(systemicProcessDTO.Description);
      }

      public SuperToolTip ToolTipFor(TransporterExpressionParameterDTO containerDTO)
      {
         var transportDirection = containerDTO.TransportDirection;
         if (transportDirection == TransportDirections.None)
            return null;

         var path = new List<string>();
         if(!string.IsNullOrEmpty(containerDTO.ContainerName))
            path.Add(containerDTO.ContainerName);

         if (!string.IsNullOrEmpty(containerDTO.CompartmentName))
            path.Add(containerDTO.CompartmentName);

         var containerDisplay = path.ToString(" - ");
         return CreateToolTip(transportDirection.Description, containerDisplay, transportDirection.Icon);
      }

      public SuperToolTip WarningToolTip(string warning)
      {
         return CreateToolTip(warning, PKSimConstants.UI.Warning, ApplicationIcons.Warning);
      }

      public SuperToolTip ToolTipFor(ParameterAlternativeDTO parameterAlternativeDTO)
      {
         return ToolTipFor(parameterAlternativeDTO.ValueOrigin);
      }

      public SuperToolTip ToolTipFor(IEnumerable<ToolTipPart> toolTipParts)
      {
         return _toolTipMapper.MapFrom(toolTipParts.ToList());
      }

      public SuperToolTip ToolTipFor(SimulationResultsFileSelectionDTO simulationResultsFileSelectionDTO)
      {
         string message = PKSimConstants.UI.FileSuccessfullyImported;
         if (simulationResultsFileSelectionDTO.Status == NotificationType.Error)
            message = simulationResultsFileSelectionDTO.Message;

         return CreateToolTip(message, simulationResultsFileSelectionDTO.FilePath, simulationResultsFileSelectionDTO.Image);
      }

      public SuperToolTip ToolTipFor(QuantityPKParameterDTO quantityPKParameterDTO)
      {
         var toolTip = CreateToolTip(quantityPKParameterDTO.Description, quantityPKParameterDTO.DisplayName, ApplicationIcons.PKAnalysis);

         toolTip.Items.AddSeparator();
         toolTip.WithTitle(PKSimConstants.UI.Output);
         toolTip.WithText(quantityPKParameterDTO.QuantityDisplayPath);

         return toolTip;
      }

      public SuperToolTip ToolTipFor(IPopulationAnalysisField populationAnalysisField)
      {
         return ToolTipFor(_toolTipPartCreator.ToolTipFor(populationAnalysisField));
      }

      public SuperToolTip ToolTipFor<TX, TY>(CurveData<TX, TY> curveData, double xDisplayValue, double yDisplayValue)
         where TX : IXValue
         where TY : IYValue
      {
         var pointIndex = curveData.GetPointIndexForDisplayValues(xDisplayValue, yDisplayValue);

         var toolTip = CreateToolTip();
         if (!string.IsNullOrEmpty(curveData.PaneCaption))
            toolTip.WithTitle(curveData.PaneCaption);

         if (!string.IsNullOrEmpty(curveData.Caption))
            toolTip.WithTitle(curveData.Caption);

         toolTip.WithText(curveData.XDisplayValueAt(pointIndex));
         toolTip.WithText(curveData.YDisplayValueAt(pointIndex));

         return toolTip;
      }

      public SuperToolTip ToolTipFor(CategoryCategoryItemDTO categoryCategoryItemDTO)
      {
         var repInfo = _representationInfoRepository.InfoFor(categoryCategoryItemDTO.CategoryItem);
         var toolTip = CreateToolTip(categoryCategoryItemDTO.Description, categoryCategoryItemDTO.DisplayName);
         toolTip.Items.AddSeparator();
         toolTip.WithTitle(repInfo.DisplayName);
         toolTip.WithText(repInfo.Description);
         return toolTip;
      }

      public SuperToolTip ToolTipForPKAnalysis(string parameterDisplayName, string description, string warning)
      {
         var toolTip = CreateToolTip(description, parameterDisplayName, ApplicationIcons.PKAnalysis);
         if (!string.IsNullOrEmpty(warning))
         {
            toolTip.Items.AddSeparator();
            toolTip.WithTitle(PKSimConstants.UI.Warning);
            var item = toolTip.Items.Add(warning);
            item.Image = ApplicationIcons.Warning;
         }
         return toolTip;
      }

      public SuperToolTip CreateToolTip(string content, Image image)
      {
         return CreateToolTip(content, string.Empty, image);
      }

      public SuperToolTip CreateToolTip(string content, string title)
      {
         return CreateToolTip(content, title, null);
      }

      public SuperToolTip CreateToolTip(string content)
      {
         return CreateToolTip(content, string.Empty);
      }

      private void addValueDescriptionToolTip(SuperToolTip toolTip, IParameterDTO parameterDTO)
      {
         var valueOriginAsString = parameterDTO.ValueOrigin.Display;
         if (string.IsNullOrEmpty(valueOriginAsString))
            return;

         toolTip.Items.AddSeparator();
         toolTip.WithTitle(Captions.ValueOrigin);
         toolTip.WithText(valueOriginAsString);
      }

      private void addFormulaToolTipTo(SuperToolTip toolTip, IParameterDTO parameterDTO)
      {
         if (parameterDTO.FormulaType != FormulaType.Rate)
            return;

         var formula = parameterDTO.Parameter.Formula as ExplicitFormula;
         if (formula == null) return;

         toolTip.Items.AddSeparator();
         toolTip.WithTitle(PKSimConstants.Information.Formula);
         toolTip.WithText(formula.FormulaString);

         toolTip.Items.AddSeparator();
         toolTip.WithTitle(PKSimConstants.Information.ObjectReferences);

         var sb = new StringBuilder();
         foreach (var objectPath in formula.ObjectPaths)
         {
            sb.AppendLine($"<I>{objectPath.Alias}</I> is defined as: {displayObjectPathFrom(objectPath)}");
         }

         toolTip.WithText(sb.ToString());
      }

      private string displayObjectPathFrom(IFormulaUsablePath objectPath)
      {
         var display = objectPath.PathAsString;
         display = display.Replace($"{Constants.ROOT}|", string.Empty);
         return display;
      }
   }
}