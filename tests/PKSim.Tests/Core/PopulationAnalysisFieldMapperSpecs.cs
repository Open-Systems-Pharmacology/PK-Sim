using System.Drawing;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using GroupingDefinition = PKSim.Core.Model.PopulationAnalyses.GroupingDefinition;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysisFieldMapper : ContextSpecificationAsync<PopulationAnalysisFieldMapper>
   {
      protected GroupingDefinitionMapper _groupingDefinitionMapper;
      protected PopulationAnalysisParameterField _parameterField;
      protected PopulationAnalysisField _snapshot;
      protected IDimension _dimension;
      protected Unit _unit;
      protected PopulationPivotAnalysis _populationAnalysis;
      protected PopulationAnalysisPKParameterField _pkParameterField;
      protected PopulationAnalysisCovariateField _covariateField;
      protected GroupingItem _referenceGroupingItem;
      protected GroupingItem _groupingItem1;
      protected PopulationAnalysisOutputField _outputField;
      protected PopulationAnalysisGroupingField _groupingField;
      private GroupingDefinition _groupingDefinition;
      protected Snapshots.GroupingDefinition _snapshotGroupingDefinition;

      protected override Task Context()
      {
         _groupingDefinitionMapper = A.Fake<GroupingDefinitionMapper>();
         _dimension = DomainHelperForSpecs.TimeDimensionForSpecs();
         _unit = _dimension.Unit("h");
         _populationAnalysis = new PopulationPivotAnalysis();

         sut = new PopulationAnalysisFieldMapper(_groupingDefinitionMapper);

         _referenceGroupingItem = new GroupingItem();
         _groupingItem1 = new GroupingItem();

         _parameterField = new PopulationAnalysisParameterField
         {
            Name = "P",
            ParameterPath = "A|B",
            Scaling = Scalings.Linear,
            Dimension = _dimension,
            DisplayUnit = _unit
         };

         _pkParameterField = new PopulationAnalysisPKParameterField
         {
            Name = "PK",
            Dimension = _dimension,
            DisplayUnit = _unit,
            PKParameter = "AUC",
            QuantityPath = "A|B|C",
            QuantityType = QuantityType.Complex
         };

         _outputField = new PopulationAnalysisOutputField
         {
            Dimension = _dimension,
            DisplayUnit = _unit,
            QuantityType = QuantityType.BaseGrid,
            QuantityPath = "A|B|C",
            Color = Color.AntiqueWhite
         };

         _groupingDefinition = new FixedLimitsGroupingDefinition("AA");
         _groupingField = new PopulationAnalysisGroupingField(_groupingDefinition)
         {
            ReferenceGroupingItem = _referenceGroupingItem
         };


         _covariateField = new PopulationAnalysisCovariateField
         {
            Covariate = "Gender",
            ReferenceGroupingItem = _referenceGroupingItem
         };
         _covariateField.AddGroupingItem(_groupingItem1);


         _populationAnalysis.Add(_parameterField);
         _populationAnalysis.SetPosition(_parameterField, PivotArea.DataArea);

         _populationAnalysis.Add(_pkParameterField);
         _populationAnalysis.SetPosition(_pkParameterField, PivotArea.FilterArea);

         _populationAnalysis.Add(_covariateField);
         _populationAnalysis.SetPosition(_covariateField, PivotArea.ColorArea);

         _populationAnalysis.Add(_outputField);
         _populationAnalysis.SetPosition(_outputField, PivotArea.RowArea);

         _populationAnalysis.Add(_groupingField);
         _populationAnalysis.SetPosition(_groupingField, PivotArea.RowArea);

         _snapshotGroupingDefinition = new Snapshots.GroupingDefinition();
         A.CallTo(() => _groupingDefinitionMapper.MapToSnapshot(_groupingDefinition)).ReturnsAsync(_snapshotGroupingDefinition);
         return _completed;
      }
   }

   public class When_mapping_a_parameter_field_to_snapshot : concern_for_PopulationAnalysisFieldMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_parameterField, _populationAnalysis);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.Dimension.ShouldBeEqualTo(_dimension.Name);
         _snapshot.Unit.ShouldBeEqualTo(_unit.Name);
         _snapshot.ParameterPath.ShouldBeEqualTo(_parameterField.ParameterPath);
         _snapshot.Area.ShouldBeEqualTo(_populationAnalysis.GetPosition(_parameterField).Area);
         _snapshot.Index.ShouldBeEqualTo(_populationAnalysis.GetPosition(_parameterField).Index);
      }

      [Observation]
      public void should_have_set_irrelevant_properties_to_null()
      {
         _snapshot.PKParameter.ShouldBeNull();
         _snapshot.QuantityPath.ShouldBeNull();
         _snapshot.QuantityType.ShouldBeNull();
         _snapshot.Color.ShouldBeNull();
         _snapshot.Covariate.ShouldBeNull();
         _snapshot.GroupingItems.ShouldBeNull();
         _snapshot.ReferenceGroupingItem.ShouldBeNull();
         _snapshot.GroupingDefinition.ShouldBeNull();
      }
   }

   public class When_mapping_a_pk_parameter_field_to_snapshot : concern_for_PopulationAnalysisFieldMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_pkParameterField, _populationAnalysis);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.PKParameter.ShouldBeEqualTo(_pkParameterField.PKParameter);
         _snapshot.QuantityPath.ShouldBeEqualTo(_pkParameterField.QuantityPath);
         _snapshot.QuantityType.ShouldBeEqualTo(_pkParameterField.QuantityType);
         _snapshot.Dimension.ShouldBeEqualTo(_dimension.Name);
         _snapshot.Unit.ShouldBeEqualTo(_unit.Name);
         _snapshot.Area.ShouldBeEqualTo(_populationAnalysis.GetPosition(_pkParameterField).Area);
         _snapshot.Index.ShouldBeEqualTo(_populationAnalysis.GetPosition(_pkParameterField).Index);
      }

      [Observation]
      public void should_have_set_irrelevant_properties_to_null()
      {
         _snapshot.ParameterPath.ShouldBeNull();
         _snapshot.Color.ShouldBeNull();
         _snapshot.Covariate.ShouldBeNull();
         _snapshot.GroupingItems.ShouldBeNull();
         _snapshot.ReferenceGroupingItem.ShouldBeNull();
         _snapshot.GroupingDefinition.ShouldBeNull();
      }
   }

   public class When_mapping_a_covariate_field_to_snapshot : concern_for_PopulationAnalysisFieldMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_covariateField, _populationAnalysis);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.Covariate.ShouldBeEqualTo(_covariateField.Covariate);
         _snapshot.GroupingItems.ShouldContain(_groupingItem1);
         _snapshot.ReferenceGroupingItem.ShouldBeEqualTo(_referenceGroupingItem);
         _snapshot.Area.ShouldBeEqualTo(_populationAnalysis.GetPosition(_covariateField).Area);
         _snapshot.Index.ShouldBeEqualTo(_populationAnalysis.GetPosition(_covariateField).Index);
      }

      [Observation]
      public void should_have_set_irrelevant_properties_to_null()
      {
         _snapshot.ParameterPath.ShouldBeNull();
         _snapshot.PKParameter.ShouldBeNull();
         _snapshot.QuantityPath.ShouldBeNull();
         _snapshot.QuantityType.ShouldBeNull();
         _snapshot.Color.ShouldBeNull();
         _snapshot.GroupingDefinition.ShouldBeNull();
      }
   }

   public class When_mapping_an_output_field_to_snapshot : concern_for_PopulationAnalysisFieldMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_outputField, _populationAnalysis);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.Dimension.ShouldBeEqualTo(_dimension.Name);
         _snapshot.Unit.ShouldBeEqualTo(_unit.Name);
         _snapshot.QuantityPath.ShouldBeEqualTo(_outputField.QuantityPath);
         _snapshot.QuantityType.ShouldBeEqualTo(_outputField.QuantityType);
         _snapshot.Color.ShouldBeEqualTo(_outputField.Color);
         _snapshot.Area.ShouldBeEqualTo(_populationAnalysis.GetPosition(_outputField).Area);
         _snapshot.Index.ShouldBeEqualTo(_populationAnalysis.GetPosition(_outputField).Index);
      }

      [Observation]
      public void should_have_set_irrelevant_properties_to_null()
      {
         _snapshot.Covariate.ShouldBeNull();
         _snapshot.ParameterPath.ShouldBeNull();
         _snapshot.PKParameter.ShouldBeNull();
         _snapshot.GroupingItems.ShouldBeNull();
         _snapshot.ReferenceGroupingItem.ShouldBeNull();
         _snapshot.GroupingDefinition.ShouldBeNull();
      }
   }

   public class When_mapping_a_grouping_field_to_snapshot : concern_for_PopulationAnalysisFieldMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_groupingField, _populationAnalysis);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.GroupingDefinition.ShouldBeEqualTo(_snapshotGroupingDefinition);
         _snapshot.ReferenceGroupingItem.ShouldBeEqualTo(_referenceGroupingItem);
         _snapshot.Area.ShouldBeEqualTo(_populationAnalysis.GetPosition(_groupingField).Area);
         _snapshot.Index.ShouldBeEqualTo(_populationAnalysis.GetPosition(_groupingField).Index);
      }

      [Observation]
      public void should_have_set_irrelevant_properties_to_null()
      {
         _snapshot.Color.ShouldBeNull();
         _snapshot.QuantityType.ShouldBeNull();
         _snapshot.QuantityPath.ShouldBeNull();
         _snapshot.Unit.ShouldBeNull();
         _snapshot.Dimension.ShouldBeNull();
         _snapshot.Covariate.ShouldBeNull();
         _snapshot.ParameterPath.ShouldBeNull();
         _snapshot.PKParameter.ShouldBeNull();
         _snapshot.GroupingItems.ShouldBeNull();
      }
   }
}