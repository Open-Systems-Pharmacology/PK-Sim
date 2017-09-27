using System;
using System.Linq;
using System.Threading.Tasks;
using PKSim.Core.Model.PopulationAnalyses;
using ModelPopulationAnalysis = PKSim.Core.Model.PopulationAnalyses.PopulationAnalysis;

namespace PKSim.Core.Snapshots.Mappers
{
   public class PopulationAnalysisFieldMapper : ObjectBaseSnapshotMapperBase<IPopulationAnalysisField, PopulationAnalysisField, ModelPopulationAnalysis, ModelPopulationAnalysis>
   {
      private readonly GroupingDefinitionMapper _groupingDefinitionMapper;

      public PopulationAnalysisFieldMapper(GroupingDefinitionMapper groupingDefinitionMapper)
      {
         _groupingDefinitionMapper = groupingDefinitionMapper;
      }

      public override async Task<PopulationAnalysisField> MapToSnapshot(IPopulationAnalysisField field, ModelPopulationAnalysis populationAnalysis)
      {
         var snapshot = await SnapshotFrom(field);
         updatePositionFor(snapshot, field, populationAnalysis);
         mapField<PopulationAnalysisOutputField>(snapshot, field, mapOutputField);
         mapField<PopulationAnalysisParameterField>(snapshot, field, mapParameterField);
         mapField<PopulationAnalysisPKParameterField>(snapshot, field, mapPKParameterField);
         mapField<PopulationAnalysisCovariateField>(snapshot, field, mapCovariateField);
         await mapGroupingFieldProperties(snapshot, field as PopulationAnalysisGroupingField);

         return snapshot;
      }

      private void mapCovariateField(PopulationAnalysisField snapshot, PopulationAnalysisCovariateField field)
      {
         snapshot.Covariate = field.Covariate;
         snapshot.ReferenceGroupingItem = field.ReferenceGroupingItem;
         snapshot.GroupingItems = field.GroupingItems.ToArray();
      }

      private async Task mapGroupingFieldProperties(PopulationAnalysisField snapshot, PopulationAnalysisGroupingField field)
      {
         if (field == null)
            return;

         snapshot.ReferenceGroupingItem = field.ReferenceGroupingItem;
         snapshot.GroupingDefinition = await _groupingDefinitionMapper.MapToSnapshot(field.GroupingDefinition);
      }

      private void updatePositionFor(PopulationAnalysisField snapshot, IPopulationAnalysisField field, ModelPopulationAnalysis populationAnalysis)
      {
         var pivotPopulatonAnalysis = populationAnalysis as PopulationPivotAnalysis;
         var position = pivotPopulatonAnalysis?.GetPosition(field);
         if (position == null)
            return;

         snapshot.Area = position.Area;
         snapshot.Index = position.Index;
      }

      private void mapPKParameterField(PopulationAnalysisField snapshot, PopulationAnalysisPKParameterField field)
      {
         mapQuantityFieldProperties(snapshot, field);
         snapshot.PKParameter = field.PKParameter;
      }

      private void mapParameterField(PopulationAnalysisField snapshot, PopulationAnalysisParameterField field)
      {
         mapNumericFieldProperties(snapshot, field);
         snapshot.ParameterPath = field.ParameterPath;
      }

      private void mapOutputField(PopulationAnalysisField snapshot, PopulationAnalysisOutputField field)
      {
         mapQuantityFieldProperties(snapshot, field);
         snapshot.Color = field.Color;
      }

      private void mapQuantityFieldProperties(PopulationAnalysisField snapshot, IQuantityField field)
      {
         mapNumericFieldProperties(snapshot, field);
         snapshot.QuantityPath = field.QuantityPath;
         snapshot.QuantityType = field.QuantityType;
      }

      private void mapNumericFieldProperties(PopulationAnalysisField snapshot, INumericValueField field)
      {
         snapshot.Dimension = field.Dimension.Name;
         snapshot.Unit = field.DisplayUnit.Name;
         snapshot.Scaling = field.Scaling;
      }

      private void mapField<T>(PopulationAnalysisField snapshot, IPopulationAnalysisField populationAnalysisField, Action<PopulationAnalysisField, T> mapAction) where T : class, IPopulationAnalysisField
      {
         var field = populationAnalysisField as T;
         if (field == null)
            return;

         mapAction(snapshot, field);
      }

      public override Task<IPopulationAnalysisField> MapToModel(PopulationAnalysisField snapshot, ModelPopulationAnalysis populationAnalysis)
      {
         throw new NotImplementedException();
      }
   }
}