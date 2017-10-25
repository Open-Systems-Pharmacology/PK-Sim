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
using DataColumn = OSPSuite.Core.Domain.Data.DataColumn;
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
      protected PopulationAnalysisPKParameterField _pkParameterField;
      protected PopulationAnalysisCovariateField _covariateField;
      protected GroupingItem _groupingItem1;
      protected PopulationAnalysisOutputField _outputField;
      protected PopulationAnalysisGroupingField _groupingField;
      private GroupingDefinition _groupingDefinition;
      protected Snapshots.GroupingDefinition _snapshotGroupingDefinition;
      private IDimensionFactory _dimensionFactory;

      protected override Task Context()
      {
         _groupingDefinitionMapper = A.Fake<GroupingDefinitionMapper>();
         _dimension = DomainHelperForSpecs.TimeDimensionForSpecs();
         _unit = _dimension.Unit("h");
         _dimensionFactory = A.Fake<IDimensionFactory>();

         sut = new PopulationAnalysisFieldMapper(_groupingDefinitionMapper, _dimensionFactory);

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
            QuantityType = QuantityType.Observer | QuantityType.Drug
         };

         _outputField = new PopulationAnalysisOutputField
         {
            Name = "Output",
            Dimension = _dimension,
            DisplayUnit = _unit,
            QuantityType = QuantityType.BaseGrid,
            QuantityPath = "A|B|C",
            Color = Color.AntiqueWhite
         };

         _groupingDefinition = new FixedLimitsGroupingDefinition("AA");
         _groupingField = new PopulationAnalysisGroupingField(_groupingDefinition)
         {
            Name = "Grouping",
         };


         _covariateField = new PopulationAnalysisCovariateField
         {
            Name = "Gender",
            Covariate = "Gender",
         };
         _covariateField.AddGroupingItem(_groupingItem1);

         _snapshotGroupingDefinition = new Snapshots.GroupingDefinition();
         A.CallTo(() => _groupingDefinitionMapper.MapToSnapshot(_groupingDefinition)).Returns(_snapshotGroupingDefinition);

         A.CallTo(() => _dimensionFactory.Dimension(_dimension.Name)).Returns(_dimension);
         A.CallTo(() => _dimensionFactory.MergedDimensionFor(A<DataColumn>._)).Returns(_dimension);
         return _completed;
      }
   }

   public class When_mapping_a_parameter_field_to_snapshot : concern_for_PopulationAnalysisFieldMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_parameterField);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.Dimension.ShouldBeEqualTo(_dimension.Name);
         _snapshot.Unit.ShouldBeEqualTo(_unit.Name);
         _snapshot.ParameterPath.ShouldBeEqualTo(_parameterField.ParameterPath);
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
         _snapshot.GroupingDefinition.ShouldBeNull();
      }
   }

   public class When_mapping_a_pk_parameter_field_to_snapshot : concern_for_PopulationAnalysisFieldMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_pkParameterField);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.PKParameter.ShouldBeEqualTo(_pkParameterField.PKParameter);
         _snapshot.QuantityPath.ShouldBeEqualTo(_pkParameterField.QuantityPath);
         _snapshot.QuantityType.ShouldBeEqualTo(_pkParameterField.QuantityType.ToString());
         _snapshot.Dimension.ShouldBeEqualTo(_dimension.Name);
         _snapshot.Unit.ShouldBeEqualTo(_unit.Name);
      }

      [Observation]
      public void should_have_set_irrelevant_properties_to_null()
      {
         _snapshot.ParameterPath.ShouldBeNull();
         _snapshot.Color.ShouldBeNull();
         _snapshot.Covariate.ShouldBeNull();
         _snapshot.GroupingItems.ShouldBeNull();
         _snapshot.GroupingDefinition.ShouldBeNull();
      }
   }

   public class When_mapping_a_covariate_field_to_snapshot : concern_for_PopulationAnalysisFieldMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_covariateField);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.Covariate.ShouldBeEqualTo(_covariateField.Covariate);
         _snapshot.GroupingItems.ShouldContain(_groupingItem1);
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
         _snapshot = await sut.MapToSnapshot(_outputField);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.Dimension.ShouldBeEqualTo(_dimension.Name);
         _snapshot.Unit.ShouldBeEqualTo(_unit.Name);
         _snapshot.QuantityPath.ShouldBeEqualTo(_outputField.QuantityPath);
         _snapshot.QuantityType.ShouldBeEqualTo(_outputField.QuantityType.ToString());
         _snapshot.Color.ShouldBeEqualTo(_outputField.Color);
      }

      [Observation]
      public void should_have_set_irrelevant_properties_to_null()
      {
         _snapshot.Covariate.ShouldBeNull();
         _snapshot.ParameterPath.ShouldBeNull();
         _snapshot.PKParameter.ShouldBeNull();
         _snapshot.GroupingItems.ShouldBeNull();
         _snapshot.GroupingDefinition.ShouldBeNull();
      }
   }

   public class When_mapping_a_grouping_field_to_snapshot : concern_for_PopulationAnalysisFieldMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_groupingField);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.GroupingDefinition.ShouldBeEqualTo(_snapshotGroupingDefinition);
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

   public class When_mapping_a_parameter_field_snapshot_to_population_analysis_field : concern_for_PopulationAnalysisFieldMapper
   {
      private PopulationAnalysisParameterField _newParameterField;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_parameterField);
      }

      protected override async Task Because()
      {
         _newParameterField = await sut.MapToModel(_snapshot) as PopulationAnalysisParameterField;
      }

      [Observation]
      public void should_return_a_parameter_field_with_the_properties_set_as_expected()
      {
         _newParameterField.ShouldNotBeNull();
         _newParameterField.Dimension.ShouldBeEqualTo(_dimension);
         _newParameterField.DisplayUnit.ShouldBeEqualTo(_unit);
         _newParameterField.ParameterPath.ShouldBeEqualTo(_snapshot.ParameterPath);
      }
   }

   public class When_mapping_a_pk_parameter_field_snapshot_to_population_analysis_field : concern_for_PopulationAnalysisFieldMapper
   {
      private PopulationAnalysisPKParameterField _newPKParameterField;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_pkParameterField);
      }

      protected override async Task Because()
      {
         _newPKParameterField = await sut.MapToModel(_snapshot) as PopulationAnalysisPKParameterField;
      }

      [Observation]
      public void should_return_a_pk_parameter_field_with_the_properties_set_as_expected()
      {
         _newPKParameterField.ShouldNotBeNull();
         _newPKParameterField.Dimension.ShouldBeEqualTo(_dimension);
         _newPKParameterField.DisplayUnit.ShouldBeEqualTo(_unit);
         _newPKParameterField.PKParameter.ShouldBeEqualTo(_snapshot.PKParameter);
         _newPKParameterField.QuantityType.ShouldBeEqualTo(_pkParameterField.QuantityType);
         _newPKParameterField.QuantityPath.ShouldBeEqualTo(_snapshot.QuantityPath);
      }
   }

   public class When_mapping_a_covariate_field_snapshot_to_population_analysis_field : concern_for_PopulationAnalysisFieldMapper
   {
      private PopulationAnalysisCovariateField _newCovariateField;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_covariateField);
      }

      protected override async Task Because()
      {
         _newCovariateField = await sut.MapToModel(_snapshot) as PopulationAnalysisCovariateField;
      }

      [Observation]
      public void should_return_a_covariate_field_with_the_properties_set_as_expected()
      {
         _newCovariateField.ShouldNotBeNull();
         _newCovariateField.Covariate.ShouldBeEqualTo(_covariateField.Covariate);
         _newCovariateField.GroupingItems.ShouldOnlyContainInOrder(_covariateField.GroupingItems);
      }
   }

   public class When_mapping_an_output_field_snapshot_to_population_analysis_field : concern_for_PopulationAnalysisFieldMapper
   {
      private PopulationAnalysisOutputField _newOutputField;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_outputField);
      }

      protected override async Task Because()
      {
         _newOutputField = await sut.MapToModel(_snapshot) as PopulationAnalysisOutputField;
      }

      [Observation]
      public void should_return_an_output_field_with_the_properties_set_as_expected()
      {
         _newOutputField.ShouldNotBeNull();
         _newOutputField.Name.ShouldBeEqualTo(_outputField.Name);
         _newOutputField.Color.ShouldBeEqualTo(_outputField.Color);
         _newOutputField.QuantityType.ShouldBeEqualTo(_outputField.QuantityType);
         _newOutputField.QuantityPath.ShouldBeEqualTo(_outputField.QuantityPath);
      }
   }

   public class When_mapping_a_grouping_field_snapshot_to_population_analysis_field : concern_for_PopulationAnalysisFieldMapper
   {
      private PopulationAnalysisGroupingField _newGroupingField;
      private GroupingDefinition _newGroupingDefinition;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_groupingField);
         _newGroupingDefinition = A.Fake<GroupingDefinition>();
         A.CallTo(() => _groupingDefinitionMapper.MapToModel(_snapshot.GroupingDefinition)).Returns(_newGroupingDefinition);
      }

      protected override async Task Because()
      {
         _newGroupingField = await sut.MapToModel(_snapshot) as PopulationAnalysisGroupingField;
      }

      [Observation]
      public void should_return_a_grouping_field_with_the_properties_set_as_expected()
      {
         _newGroupingField.ShouldNotBeNull();
         _newGroupingField.Name.ShouldBeEqualTo(_groupingField.Name);
         _newGroupingField.GroupingDefinition.ShouldBeEqualTo(_newGroupingDefinition);
      }
   }
}