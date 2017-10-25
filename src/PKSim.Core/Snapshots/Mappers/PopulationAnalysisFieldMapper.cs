using System;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Snapshots.Mappers
{
   public class PopulationAnalysisFieldMapper : ObjectBaseSnapshotMapperBase<IPopulationAnalysisField, PopulationAnalysisField>
   {
      private readonly GroupingDefinitionMapper _groupingDefinitionMapper;
      private readonly IDimensionFactory _dimensionFactory;

      public PopulationAnalysisFieldMapper(GroupingDefinitionMapper groupingDefinitionMapper, IDimensionFactory dimensionFactory)
      {
         _groupingDefinitionMapper = groupingDefinitionMapper;
         _dimensionFactory = dimensionFactory;
      }

      public override async Task<PopulationAnalysisField> MapToSnapshot(IPopulationAnalysisField field)
      {
         var snapshot = await SnapshotFrom(field);
         mapIf<PopulationAnalysisOutputField>(snapshot, field, mapOutputFieldToSnapshot);
         mapIf<PopulationAnalysisParameterField>(snapshot, field, mapParameterFieldToSnapshot);
         mapIf<PopulationAnalysisPKParameterField>(snapshot, field, mapPKParameterFieldToSnapshot);
         mapIf<PopulationAnalysisCovariateField>(snapshot, field, mapCovariateField);
         await mapGroupingFieldProperties(snapshot, field as PopulationAnalysisGroupingField);
         return snapshot;
      }

      public override async Task<IPopulationAnalysisField> MapToModel(PopulationAnalysisField snapshot)
      {
         var populationAnalysisField = await createFieldFrom(snapshot);
         MapSnapshotPropertiesToModel(snapshot, populationAnalysisField);
         mapIf<PopulationAnalysisParameterField>(snapshot, populationAnalysisField, mapParameterFieldToModel);
         mapIf<PopulationAnalysisPKParameterField>(snapshot, populationAnalysisField, mapPKParameterFieldToModel);
         mapIf<PopulationAnalysisCovariateField>(snapshot, populationAnalysisField, mapCovariateFieldToModel);
         mapIf<PopulationAnalysisOutputField>(snapshot, populationAnalysisField, mapOutputFieldToModel);
         return populationAnalysisField;
      }

      private void mapCovariateField(PopulationAnalysisField snapshot, PopulationAnalysisCovariateField field)
      {
         snapshot.Covariate = field.Covariate;
         snapshot.GroupingItems = field.GroupingItems.ToArray();
      }

      private void mapCovariateFieldToModel(PopulationAnalysisField snapshot, PopulationAnalysisCovariateField field)
      {
         field.Covariate = snapshot.Covariate;
         snapshot.GroupingItems?.Each(field.AddGroupingItem);
      }

      private async Task mapGroupingFieldProperties(PopulationAnalysisField snapshot, PopulationAnalysisGroupingField field)
      {
         if (field == null)
            return;

         snapshot.GroupingDefinition = await _groupingDefinitionMapper.MapToSnapshot(field.GroupingDefinition);
      }

      private void mapPKParameterFieldToSnapshot(PopulationAnalysisField snapshot, PopulationAnalysisPKParameterField field)
      {
         mapQuantityFieldToSnapshot(snapshot, field);
         snapshot.PKParameter = field.PKParameter;
      }

      private void mapPKParameterFieldToModel(PopulationAnalysisField snapshot, PopulationAnalysisPKParameterField field)
      {
         mapQuantityFieldToModel(snapshot, field);
         field.PKParameter = snapshot.PKParameter;
      }

      private void mapParameterFieldToSnapshot(PopulationAnalysisField snapshot, PopulationAnalysisParameterField field)
      {
         mapNumericFieldToSnapshot(snapshot, field);
         snapshot.ParameterPath = field.ParameterPath;
      }

      private void mapParameterFieldToModel(PopulationAnalysisField snapshot, PopulationAnalysisParameterField field)
      {
         mapNumericFieldToModel(snapshot, field);
         field.ParameterPath = snapshot.ParameterPath;
      }

      private void mapOutputFieldToSnapshot(PopulationAnalysisField snapshot, PopulationAnalysisOutputField field)
      {
         mapQuantityFieldToSnapshot(snapshot, field);
         snapshot.Color = field.Color;
      }

      private void mapOutputFieldToModel(PopulationAnalysisField snapshot, PopulationAnalysisOutputField field)
      {
         mapQuantityFieldToModel(snapshot, field);
         field.Color = ModelValueFor(snapshot.Color, field.Color);
      }

      private void mapQuantityFieldToSnapshot(PopulationAnalysisField snapshot, IQuantityField field)
      {
         mapNumericFieldToSnapshot(snapshot, field);
         snapshot.QuantityPath = field.QuantityPath;
         snapshot.QuantityType = field.QuantityType.ToString();
      }

      private void mapQuantityFieldToModel(PopulationAnalysisField snapshot, IQuantityField field)
      {
         mapNumericFieldToModel(snapshot, field);
         field.QuantityPath = snapshot.QuantityPath;
         if (snapshot.QuantityType != null)
            field.QuantityType = EnumHelper.ParseValue<QuantityType>(snapshot.QuantityType);
      }

      private void mapNumericFieldToSnapshot(PopulationAnalysisField snapshot, INumericValueField field)
      {
         snapshot.Dimension = field.Dimension.Name;
         snapshot.Unit = SnapshotValueFor(field.DisplayUnit.Name);
         snapshot.Scaling = field.Scaling;
      }

      private void mapNumericFieldToModel(PopulationAnalysisField snapshot, INumericValueField field)
      {
         field.Dimension = _dimensionFactory.Dimension(snapshot.Dimension);
         field.Dimension = _dimensionFactory.OptimalDimension(field.Dimension);
         field.DisplayUnit = field.Dimension.Unit(ModelValueFor(snapshot.Unit));
         field.Scaling = ModelValueFor(snapshot.Scaling, field.Scaling);
      }

      private void mapIf<T>(PopulationAnalysisField snapshot, IPopulationAnalysisField populationAnalysisField, Action<PopulationAnalysisField, T> mapAction) where T : class, IPopulationAnalysisField
      {
         var field = populationAnalysisField as T;
         if (field == null)
            return;

         mapAction(snapshot, field);
      }

      private async Task<IPopulationAnalysisField> createFieldFrom(PopulationAnalysisField snapshot)
      {
         if (snapshot.ParameterPath != null)
            return new PopulationAnalysisParameterField();

         if (snapshot.PKParameter != null)
            return new PopulationAnalysisPKParameterField();

         if (snapshot.Covariate != null)
            return new PopulationAnalysisCovariateField();

         if (snapshot.GroupingDefinition != null)
         {
            var groupingDefintion = await _groupingDefinitionMapper.MapToModel(snapshot.GroupingDefinition);
            return new PopulationAnalysisGroupingField(groupingDefintion);
         }

         return new PopulationAnalysisOutputField();
      }
   }
}