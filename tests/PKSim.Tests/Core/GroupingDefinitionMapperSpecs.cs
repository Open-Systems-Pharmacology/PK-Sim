using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
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
      private IDimensionRepository _dimensionRepository;

      protected override Task Context()
      {
         _dimensionRepository= A.Fake<IDimensionRepository>();   
         sut = new GroupingDefinitionMapper(_dimensionRepository);
         _groupingItem1 = new GroupingItem();
         _groupingItem2 = new GroupingItem();

         _dimension = DomainHelperForSpecs.TimeDimensionForSpecs();
         _unit = _dimension.Unit("h");

         A.CallTo(() => _dimensionRepository.DimensionByName(_dimension.Name)).Returns(_dimension);
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

   public class When_mapping_a_value_mapping_grouping_snapshot_to_grouping_definition : concern_for_GroupingDefinitionMapper
   {
      private ValueMappingGroupingDefinition _newValueMappingGrouping;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_valueMappingGrouping);
      }

      protected override async Task Because()
      {
         _newValueMappingGrouping = await sut.MapToModel(_snapshot) as ValueMappingGroupingDefinition;
      }

      [Observation]
      public void should_return_a_value_mapping_grouping_definition_with_all_properties_set_as_expected()
      {
         _newValueMappingGrouping.ShouldNotBeNull();
         _newValueMappingGrouping.FieldName.ShouldBeEqualTo(_valueMappingGrouping.FieldName);
         _newValueMappingGrouping.Mapping.ShouldOnlyContainInOrder(_valueMappingGrouping.Mapping);
         _newValueMappingGrouping.Labels.ShouldOnlyContainInOrder(_valueMappingGrouping.Labels);
      }
   }

   public class When_mapping_a_fixed_limit_grouping_snapshot_to_grouping_definition : concern_for_GroupingDefinitionMapper
   {
      private FixedLimitsGroupingDefinition _newFixedLimitsGrouping;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_fixedLimitGrouping);
      }

      protected override async Task Because()
      {
         _newFixedLimitsGrouping = await sut.MapToModel(_snapshot) as FixedLimitsGroupingDefinition;
      }

      [Observation]
      public void should_return_a_fixed_limit_grouping_definition_with_all_properties_set_as_expected()
      {
         _newFixedLimitsGrouping.ShouldNotBeNull();
         _newFixedLimitsGrouping.FieldName.ShouldBeEqualTo(_fixedLimitGrouping.FieldName);
         _newFixedLimitsGrouping.DisplayUnit.ShouldBeEqualTo(_unit);
         _newFixedLimitsGrouping.Dimension.ShouldBeEqualTo(_dimension);
         _newFixedLimitsGrouping.Labels.ShouldOnlyContainInOrder(_fixedLimitGrouping.Labels);
      }

      [Observation]
      public void should_have_converted_limits_into_vase_unit()
      {
         _newFixedLimitsGrouping.Limits.ShouldOnlyContainInOrder(60, 120);
      }
   }

   public class When_mapping_a_number_of_bins_grouping_snapshot_to_grouping_definition : concern_for_GroupingDefinitionMapper
   {
      private NumberOfBinsGroupingDefinition _newNumberOfBinsGrouping;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_numberOfBinsGrouping);
      }

      protected override async Task Because()
      {
         _newNumberOfBinsGrouping = await sut.MapToModel(_snapshot) as NumberOfBinsGroupingDefinition;
      }

      [Observation]
      public void should_return_a_number_of_bins_grouping_definition_with_all_properties_set_as_expected()
      {
         _newNumberOfBinsGrouping.ShouldNotBeNull();
         _newNumberOfBinsGrouping.FieldName.ShouldBeEqualTo(_numberOfBinsGrouping.FieldName);
         _newNumberOfBinsGrouping.DisplayUnit.ShouldBeEqualTo(_unit);
         _newNumberOfBinsGrouping.Dimension.ShouldBeEqualTo(_dimension);
         _newNumberOfBinsGrouping.Labels.ShouldOnlyContainInOrder(_numberOfBinsGrouping.Labels);
         _newNumberOfBinsGrouping.NamingPattern.ShouldOnlyContainInOrder(_numberOfBinsGrouping.NamingPattern);
         _newNumberOfBinsGrouping.NumberOfBins.ShouldBeEqualTo(_numberOfBinsGrouping.NumberOfBins);
         _newNumberOfBinsGrouping.StartColor.ShouldBeEqualTo(_numberOfBinsGrouping.StartColor);
         _newNumberOfBinsGrouping.EndColor.ShouldBeEqualTo(_numberOfBinsGrouping.EndColor);
         _newNumberOfBinsGrouping.Strategy.ShouldBeEqualTo(_numberOfBinsGrouping.Strategy);
         _newNumberOfBinsGrouping.Items.ShouldOnlyContainInOrder(_numberOfBinsGrouping.Items);
      }
   }
}