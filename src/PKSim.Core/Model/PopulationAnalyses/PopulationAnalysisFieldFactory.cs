using System.Drawing;
using System.Linq;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public interface IPopulationAnalysisFieldFactory
   {
      PopulationAnalysisParameterField CreateFor(IParameter parameter);
      PopulationAnalysisPKParameterField CreateFor(QuantityPKParameter pkParameter, QuantityType quantityType, string quantityDisplayPath);
      PopulationAnalysisCovariateField CreateFor(string covariate, IPopulationDataCollector populationDataCollector);
      PopulationAnalysisGroupingField CreateGroupingField(GroupingDefinition groupingDefiniton, IPopulationAnalysisField populationAnalysisField);
      PopulationAnalysisOutputField CreateFor(IQuantity quantity, string defaultName);
   }

   public class PopulationAnalysisFieldFactory : IPopulationAnalysisFieldFactory
   {
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IFullPathDisplayResolver _fullPathDisplayResolver;
      private readonly IGenderRepository _genderRepository;
      private readonly IColorGenerator _colorGenerator;
      private readonly IPKParameterRepository _pkParameterRepository;

      public PopulationAnalysisFieldFactory(IEntityPathResolver entityPathResolver, IFullPathDisplayResolver fullPathDisplayResolver,
         IGenderRepository genderRepository, IColorGenerator colorGenerator, IPKParameterRepository pkParameterRepository)
      {
         _entityPathResolver = entityPathResolver;
         _fullPathDisplayResolver = fullPathDisplayResolver;
         _genderRepository = genderRepository;
         _colorGenerator = colorGenerator;
         _pkParameterRepository = pkParameterRepository;
      }

      public PopulationAnalysisParameterField CreateFor(IParameter parameter)
      {
         var field = new PopulationAnalysisParameterField
         {
            ParameterPath = _entityPathResolver.PathFor(parameter),
            Name = _fullPathDisplayResolver.FullPathFor(parameter),
         };
         updateDimension(parameter, field);
         return field;
      }

      public PopulationAnalysisPKParameterField CreateFor(QuantityPKParameter pkParameter, QuantityType quantityType, string quantityDisplayPath)
      {
         return new PopulationAnalysisPKParameterField
         {
            Name = new[] {quantityDisplayPath, _pkParameterRepository.DisplayNameFor(pkParameter.Name)}.ToPathString(),
            QuantityPath = pkParameter.QuantityPath,
            QuantityType = quantityType,
            PKParameter = pkParameter.Name,
            Dimension = pkParameter.Dimension,
         };
      }

      public PopulationAnalysisOutputField CreateFor(IQuantity quantity, string defaultName)
      {
         var field = new PopulationAnalysisOutputField
         {
            QuantityPath = _entityPathResolver.PathFor(quantity),
            QuantityType = quantity.QuantityType,
            Name = defaultName,
            Color = _colorGenerator.NextColor(),
         };
         updateDimension(quantity, field);
         return field;
      }

      public PopulationAnalysisCovariateField CreateFor(string covariate, IPopulationDataCollector populationDataCollector)
      {
         var covariateFields = new PopulationAnalysisCovariateField
         {
            Covariate = covariate,
            Name = covariate
         };

         var allDistinctValues = covariateFields.GetValues(populationDataCollector).Distinct();
         foreach (var covariateValue in allDistinctValues)
         {
            covariateFields.AddGroupingItem(groupingItemFor(covariateValue));
         }

         return covariateFields;
      }

      private GroupingItem groupingItemFor(string covariateValue)
      {
         var groupingItem = new GroupingItem {Label = covariateValue};

         //special use cases for genders
         if (string.Equals(covariateValue, _genderRepository.Male.DisplayName))
            updateGroupingItem(groupingItem, PKSimColors.Male, Symbols.Circle);

         else if (string.Equals(covariateValue, _genderRepository.Female.DisplayName))
            updateGroupingItem(groupingItem, PKSimColors.Female, Symbols.Diamond);

         else
            updateGroupingItem(groupingItem, _colorGenerator.NextColor(), Symbols.Triangle);

         return groupingItem;
      }

      private void updateGroupingItem(GroupingItem groupingItem, Color color, Symbols symbol)
      {
         groupingItem.Color = color;
         groupingItem.Symbol = symbol;
      }

      public PopulationAnalysisGroupingField CreateGroupingField(GroupingDefinition groupingDefiniton, IPopulationAnalysisField populationAnalysisField)
      {
         var interval = groupingDefiniton as IntervalGroupingDefinition;
         var numericField = populationAnalysisField as INumericValueField;
         updateDimension(numericField, interval);
         return new PopulationAnalysisGroupingField(groupingDefiniton);
      }

      private void updateDimension(IWithDisplayUnit source, IWithDisplayUnit target)
      {
         if (source == null || target == null)
            return;

         target.Dimension = source.Dimension;
         target.DisplayUnit = source.DisplayUnit;
      }
   }
}