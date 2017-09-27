using System;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using PKSim.Core.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using ModelGroupingDefinition = PKSim.Core.Model.PopulationAnalyses.GroupingDefinition;
using SnapshotGroupingDefinition = PKSim.Core.Snapshots.GroupingDefinition;

namespace PKSim.Core.Snapshots.Mappers
{
   public class GroupingDefinitionMapper : SnapshotMapperBase<ModelGroupingDefinition, SnapshotGroupingDefinition>
   {
      public override async Task<SnapshotGroupingDefinition> MapToSnapshot(ModelGroupingDefinition groupingDefinition)
      {
         var snapshot = await SnapshotFrom(groupingDefinition, x => x.FieldName = groupingDefinition.FieldName);

         mapGroup<ValueMappingGroupingDefinition>(snapshot, groupingDefinition, mapValueGroupingProperties);
         mapGroup<FixedLimitsGroupingDefinition>(snapshot, groupingDefinition, mapFixedLimitsGroupingProperties);
         mapGroup<NumberOfBinsGroupingDefinition>(snapshot, groupingDefinition, mapNumberOfBinsGroupingProperties);

         return snapshot;
      }

      private void mapNumberOfBinsGroupingProperties(SnapshotGroupingDefinition snapshot, NumberOfBinsGroupingDefinition groupingDefinition)
      {
         mapIntervalGroupingDefinition(snapshot, groupingDefinition);
         snapshot.NumberOfBins = groupingDefinition.NumberOfBins;
         snapshot.StartColor = groupingDefinition.StartColor;
         snapshot.EndColor = groupingDefinition.EndColor;
         snapshot.NamingPattern = groupingDefinition.NamingPattern;
         snapshot.Strategy = groupingDefinition.Strategy.Id;
      }

      private void mapFixedLimitsGroupingProperties(SnapshotGroupingDefinition snapshot, FixedLimitsGroupingDefinition groupingDefinition)
      {
         mapIntervalGroupingDefinition(snapshot, groupingDefinition);
         snapshot.Limits = groupingDefinition.ConvertToDisplayValues(groupingDefinition.Limits).ToArray();
      }

      private void mapValueGroupingProperties(SnapshotGroupingDefinition snapshot, ValueMappingGroupingDefinition groupingDefinition)
      {
         snapshot.Mapping = groupingDefinition.Mapping.ToDictionary();
      }

      private void mapIntervalGroupingDefinition(SnapshotGroupingDefinition snapshot, IntervalGroupingDefinition groupingDefinition)
      {
         snapshot.Dimension = groupingDefinition.Dimension.Name;
         snapshot.Unit = groupingDefinition.DisplayUnit.Name;
         snapshot.Items = groupingDefinition.Items.ToArray();
      }

      private void mapGroup<T>(SnapshotGroupingDefinition snapshot, ModelGroupingDefinition modelGroupingDefinition, Action<SnapshotGroupingDefinition, T> mapAction) where T : ModelGroupingDefinition
      {
         var groupDefinition = modelGroupingDefinition as T;
         if (groupDefinition == null)
            return;

         mapAction(snapshot, groupDefinition);
      }

      public override Task<ModelGroupingDefinition> MapToModel(SnapshotGroupingDefinition snapshot)
      {
         throw new NotImplementedException();
      }
   }
}