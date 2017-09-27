using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Snapshots.Mappers;
using GroupingDefinition = PKSim.Core.Snapshots.GroupingDefinition;

namespace PKSim.Core
{
   public abstract class concern_for_GroupingDefinitionMapper : ContextSpecificationAsync<GroupingDefinitionMapper>
   {
      protected ValueMappingGroupingDefinition _valueMappingGrouping;
      protected GroupingDefinition _snapshot;
      protected GroupingItem _groupingItem1;
      protected GroupingItem _groupingItem2;
      protected FixedLimitsGroupingDefinition _fixedLimitGrouping;
      protected IDimension _dimension;
      protected Unit _unit;
      protected NumberOfBinsGroupingDefinition _numberOfBinsGrouping;

      protected override Task Context()
      {
         sut = new GroupingDefinitionMapper();
         _groupingItem1 = new GroupingItem();
         _groupingItem2 = new GroupingItem();

         _dimension = DomainHelperForSpecs.TimeDimensionForSpecs();
         _unit = _dimension.Unit("h");

         _valueMappingGrouping = new ValueMappingGroupingDefinition("F1");
         _valueMappingGrouping.AddValueLabel("Item1", _groupingItem1);
         _valueMappingGrouping.AddValueLabel("Item2", _groupingItem2);

         _fixedLimitGrouping = new FixedLimitsGroupingDefinition("F1")
         {
            DisplayUnit = _unit,
            Dimension = _dimension,
         };

         _fixedLimitGrouping.SetLimits(new[] {60, 120.0}.OrderBy(x => x));
         _fixedLimitGrouping.AddItem(_groupingItem1);
         _fixedLimitGrouping.AddItem(_groupingItem2);

         _numberOfBinsGrouping = new NumberOfBinsGroupingDefinition("F1")
         {
            DisplayUnit = _unit,
            Dimension = _dimension,
            NumberOfBins = 4,
            NamingPattern = "TOTO",
            StartColor = Color.Aqua,
            EndColor = Color.Red,
            Strategy = LabelGenerationStrategies.Numeric
         };

         _numberOfBinsGrouping.AddItem(_groupingItem1);
         _numberOfBinsGrouping.AddItem(_groupingItem2);

         return _completed;
      }
   }

   public class When_mapping_value_mapping_grouping_to_snapshot : concern_for_GroupingDefinitionMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_valueMappingGrouping);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.FieldName.ShouldBeEqualTo(_valueMappingGrouping.FieldName);
         _snapshot.Mapping["Item1"].ShouldBeEqualTo(_groupingItem1);
         _snapshot.Mapping["Item2"].ShouldBeEqualTo(_groupingItem2);
      }

      [Observation]
      public void should_have_set_irrelevant_properties_to_null()
      {
         _snapshot.Dimension.ShouldBeNull();
         _snapshot.Unit.ShouldBeNull();
         _snapshot.Items.ShouldBeNull();
         _snapshot.Limits.ShouldBeNull();
         _snapshot.NamingPattern.ShouldBeNull();
         _snapshot.Strategy.ShouldBeNull();
         _snapshot.NumberOfBins.ShouldBeNull();
         _snapshot.EndColor.ShouldBeNull();
         _snapshot.StartColor.ShouldBeNull();
         _snapshot.NumberOfBins.ShouldBeNull();
      }
   }

   public class When_mapping_fixed_limit_grouping_to_snapshot : concern_for_GroupingDefinitionMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_fixedLimitGrouping);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.FieldName.ShouldBeEqualTo(_fixedLimitGrouping.FieldName);
         _snapshot.Dimension.ShouldBeEqualTo(_dimension.Name);
         _snapshot.Unit.ShouldBeEqualTo(_unit.Name);
         _snapshot.Items.ShouldContain(_groupingItem1, _groupingItem2);
      }

      [Observation]
      public void should_have_converted_limits_into_display_unit()
      {
         _snapshot.Limits.ShouldOnlyContainInOrder(1.0, 2.0);
      }

      [Observation]
      public void should_have_set_irrelevant_properties_to_null()
      {
         _snapshot.Mapping.ShouldBeNull();
         _snapshot.NamingPattern.ShouldBeNull();
         _snapshot.Strategy.ShouldBeNull();
         _snapshot.NumberOfBins.ShouldBeNull();
         _snapshot.EndColor.ShouldBeNull();
         _snapshot.StartColor.ShouldBeNull();
         _snapshot.NumberOfBins.ShouldBeNull();
      }
   }

   public class When_mapping_number_of_bins_grouping_to_snapshot : concern_for_GroupingDefinitionMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_numberOfBinsGrouping);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.FieldName.ShouldBeEqualTo(_numberOfBinsGrouping.FieldName);
         _snapshot.Dimension.ShouldBeEqualTo(_dimension.Name);
         _snapshot.Unit.ShouldBeEqualTo(_unit.Name);
         _snapshot.Items.ShouldContain(_groupingItem1, _groupingItem2);
         _snapshot.NamingPattern.ShouldBeEqualTo(_numberOfBinsGrouping.NamingPattern);
         _snapshot.Strategy.ShouldBeEqualTo(_numberOfBinsGrouping.Strategy.Id);
         _snapshot.NumberOfBins.ShouldBeEqualTo(_numberOfBinsGrouping.NumberOfBins);
         _snapshot.EndColor.ShouldBeEqualTo(_numberOfBinsGrouping.EndColor);
         _snapshot.StartColor.ShouldBeEqualTo(_numberOfBinsGrouping.StartColor);
      }

      [Observation]
      public void should_have_set_irrelevant_properties_to_null()
      {
         _snapshot.Mapping.ShouldBeNull();
         _snapshot.Limits.ShouldBeNull();
      }
   }

}