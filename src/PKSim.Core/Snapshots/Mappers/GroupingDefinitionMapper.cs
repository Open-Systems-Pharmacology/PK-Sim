using System;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using ModelGroupingDefinition = PKSim.Core.Model.PopulationAnalyses.GroupingDefinition;
using SnapshotGroupingDefinition = PKSim.Core.Snapshots.GroupingDefinition;

namespace PKSim.Core.Snapshots.Mappers
{
   public class GroupingDefinitionMapper : SnapshotMapperBase<ModelGroupingDefinition, SnapshotGroupingDefinition>
   {
      private readonly IDimensionRepository _dimensionRepository;

      public GroupingDefinitionMapper(IDimensionRepository dimensionRepository)
      {
         _dimensionRepository = dimensionRepository;
      }

      public override async Task<SnapshotGroupingDefinition> MapToSnapshot(ModelGroupingDefinition groupingDefinition)
      {
         var snapshot = await SnapshotFrom(groupingDefinition, x => x.FieldName = groupingDefinition.FieldName);

         mapIf<ValueMappingGroupingDefinition>(snapshot, groupingDefinition, mapValueGroupingToSnapshot);
         mapIf<FixedLimitsGroupingDefinition>(snapshot, groupingDefinition, mapFixedLimitsGroupingToSnapshot);
         mapIf<NumberOfBinsGroupingDefinition>(snapshot, groupingDefinition, mapNumberOfBinsGroupingToSnapshot);

         return snapshot;
      }

      public override Task<ModelGroupingDefinition> MapToModel(SnapshotGroupingDefinition snapshot)
      {
         var groupingDefinition = createGroupingFrom(snapshot);
         mapIf<ValueMappingGroupingDefinition>(snapshot, groupingDefinition, mapValueGroupingToModel);
         mapIf<FixedLimitsGroupingDefinition>(snapshot, groupingDefinition, mapFixedLimitsGroupingToModel);
         mapIf<NumberOfBinsGroupingDefinition>(snapshot, groupingDefinition, mapNumberOfBinsGroupingToModel);

         return Task.FromResult(groupingDefinition);
      }

      private ModelGroupingDefinition createGroupingFrom(SnapshotGroupingDefinition snapshot)
      {
         if (snapshot.Mapping != null)
            return new ValueMappingGroupingDefinition(snapshot.FieldName);

         if (snapshot.Limits != null)
            return new FixedLimitsGroupingDefinition(snapshot.FieldName);

         return new NumberOfBinsGroupingDefinition(snapshot.FieldName);
      }

      private void mapNumberOfBinsGroupingToSnapshot(SnapshotGroupingDefinition snapshot, NumberOfBinsGroupingDefinition groupingDefinition)
      {
         mapIntervalGroupingToSnapshot(snapshot, groupingDefinition);
         snapshot.NumberOfBins = groupingDefinition.NumberOfBins;
         snapshot.StartColor = groupingDefinition.StartColor;
         snapshot.EndColor = groupingDefinition.EndColor;
         snapshot.NamingPattern = groupingDefinition.NamingPattern;
         snapshot.Strategy = groupingDefinition.Strategy.Id;
      }

      private void mapNumberOfBinsGroupingToModel(SnapshotGroupingDefinition snapshot, NumberOfBinsGroupingDefinition groupingDefinition)
      {
         mapIntervalGroupingToModel(snapshot, groupingDefinition);
         groupingDefinition.NumberOfBins = ModelValueFor(snapshot.NumberOfBins);
         groupingDefinition.StartColor = ModelValueFor(snapshot.StartColor);
         groupingDefinition.EndColor = ModelValueFor(snapshot.EndColor);
         groupingDefinition.NamingPattern = snapshot.NamingPattern;
         groupingDefinition.Strategy = LabelGenerationStrategies.ById(ModelValueFor(snapshot.Strategy));

      }
      private void mapFixedLimitsGroupingToSnapshot(SnapshotGroupingDefinition snapshot, FixedLimitsGroupingDefinition groupingDefinition)
      {
         mapIntervalGroupingToSnapshot(snapshot, groupingDefinition);
         snapshot.Limits = groupingDefinition.ConvertToDisplayValues(groupingDefinition.Limits).ToArray();
      }

      private void mapFixedLimitsGroupingToModel(SnapshotGroupingDefinition snapshot, FixedLimitsGroupingDefinition groupingDefinition)
      {
         mapIntervalGroupingToModel(snapshot, groupingDefinition);
         groupingDefinition.SetLimits(groupingDefinition.ConvertToBaseValues(snapshot.Limits).OrderBy(x => x));
      }

      private void mapValueGroupingToSnapshot(SnapshotGroupingDefinition snapshot, ValueMappingGroupingDefinition groupingDefinition)
      {
         snapshot.Mapping = groupingDefinition.Mapping.ToDictionary();
      }

      private void mapValueGroupingToModel(SnapshotGroupingDefinition snapshot, ValueMappingGroupingDefinition groupingDefinition)
      {
         snapshot.Mapping.Each(kv => { groupingDefinition.AddValueLabel(kv.Key, kv.Value); });
      }

      private void mapIntervalGroupingToSnapshot(SnapshotGroupingDefinition snapshot, IntervalGroupingDefinition groupingDefinition)
      {
         snapshot.Dimension = groupingDefinition.Dimension?.Name;
         snapshot.Unit = groupingDefinition.DisplayUnit?.Name;
         snapshot.Items = groupingDefinition.Items.ToArray();
      }

      private void mapIntervalGroupingToModel(SnapshotGroupingDefinition snapshot, IntervalGroupingDefinition groupingDefinition)
      {
         groupingDefinition.Dimension = _dimensionRepository.DimensionByName(snapshot.Dimension);
         groupingDefinition.DisplayUnit = groupingDefinition.Dimension.UnitOrDefault(ModelValueFor(snapshot.Unit));
         groupingDefinition.AddItems(snapshot.Items);
      }

      private void mapIf<T>(SnapshotGroupingDefinition snapshot, ModelGroupingDefinition modelGroupingDefinition, Action<SnapshotGroupingDefinition, T> mapAction) where T : ModelGroupingDefinition
      {
         var groupDefinition = modelGroupingDefinition as T;
         if (groupDefinition == null)
            return;

         mapAction(snapshot, groupDefinition);
      }
   }
}