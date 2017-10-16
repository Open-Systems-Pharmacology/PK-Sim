using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using SnapshotParameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterMapper : ContextSpecificationAsync<ParameterMapper>
   {
      protected IParameter _parameter;
      protected TableFormulaMapper _tableFormulaMapper;
      protected IEntityPathResolver _entityPathResolver;
      protected ILogger _logger;

      protected override Task Context()
      {
         _tableFormulaMapper = A.Fake<TableFormulaMapper>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _logger = A.Fake<ILogger>();

         sut = new ParameterMapper(_tableFormulaMapper, _entityPathResolver, _logger);

         //5 mm is the value
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(10)
            .WithName("P1")
            .WithDescription("P1 description")
            .WithDimension(DomainHelperForSpecs.LengthDimensionForSpecs());

         _parameter.DisplayUnit = _parameter.Dimension.Unit("mm");

         return _completed;
      }
   }

   public class When_mapping_a_parameter_to_snapshot : concern_for_ParameterMapper
   {
      private SnapshotParameter _snapshotParameter;

      protected override async Task Because()
      {
         _snapshotParameter = await sut.MapToSnapshot(_parameter);
      }

      [Observation]
      public void should_return_an_object_having_all_default_properties_of_the_object()
      {
         _snapshotParameter.Name.ShouldBeEqualTo(_parameter.Name);
         _snapshotParameter.Unit.ShouldBeEqualTo(_parameter.DisplayUnit.Name);
      }

      [Observation]
      public void should_have_reset_the_snapshot_description()
      {
         _snapshotParameter.Description.ShouldBeNull();
      }

      [Observation]
      public void should_have_set_the_value_in_display_unit()
      {
         _snapshotParameter.Value.ShouldBeEqualTo(_parameter.ValueInDisplayUnit);
      }
   }

   public class When_mapping_a_parameter_having_a_table_formula_as_formula_to_snapshot : concern_for_ParameterMapper
   {
      private SnapshotParameter _snapshotParameter;
      private TableFormula _snapshotTableFormula;

      protected override async Task Context()
      {
         await base.Context();
         var tableFormula = new OSPSuite.Core.Domain.Formulas.TableFormula();
         _parameter.Formula = tableFormula;
         _snapshotTableFormula = new TableFormula();
         A.CallTo(() => _tableFormulaMapper.MapToSnapshot(tableFormula)).Returns(_snapshotTableFormula);
      }

      protected override async Task Because()
      {
         _snapshotParameter = await sut.MapToSnapshot(_parameter);
      }

      [Observation]
      public void should_use_the_formula_mapper_to_map_a_snapshot_of_the_table_formula()
      {
         _snapshotParameter.TableFormula.ShouldBeEqualTo(_snapshotTableFormula);
      }
   }

   public class When_updating_a_parameter_from_a_snapshot_object : concern_for_ParameterMapper
   {
      private SnapshotParameter _snapshotParameter;

      protected override async Task Context()
      {
         await base.Context();
         _snapshotParameter = await sut.MapToSnapshot(_parameter);
         _snapshotParameter.Value = 50; //50 mm
         _snapshotParameter.ValueDescription = "The value description for this value";
      }

      protected override async Task Because()
      {
         await sut.MapToModel(_snapshotParameter, _parameter);
      }

      [Observation]
      public void should_update_the_standard_parameter_properties_from_the_parameter_snapshot()
      {
         _parameter.ValueInDisplayUnit.ShouldBeEqualTo(_snapshotParameter.Value.Value);
         _parameter.ValueDescription.ShouldBeEqualTo(_snapshotParameter.ValueDescription);
      }
   }

   public class When_updating_a_paraneter_from_a_snapshot_object_using_a_table_formula_and_the_value_was_not_changed_by_the_user : concern_for_ParameterMapper
   {
      private SnapshotParameter _snapshotParameter;
      private double _parameterValue;

      protected override async Task Context()
      {
         await base.Context();
         _parameterValue = 1500;
         _parameter.Value = _parameterValue;
         _snapshotParameter = await sut.MapToSnapshot(_parameter);
         _snapshotParameter.Value = _parameter.ValueInDisplayUnit;
         _snapshotParameter.TableFormula = new TableFormula();
         var modelTableFormula = new OSPSuite.Core.Domain.Formulas.TableFormula();
         A.CallTo(() => _tableFormulaMapper.MapToModel(_snapshotParameter.TableFormula)).Returns(modelTableFormula);

         //Ensure that the first value is the parameter value
         modelTableFormula.AddPoint(0, _parameterValue);

         //set some dummy value to ensure reset
         _parameter.Value = 12345;
      }

      protected override async Task Because()
      {
         await sut.MapToModel(_snapshotParameter, _parameter);
      }

      [Observation]
      public void should_hsave_set_the_table_formula_into_the_parameter_and_respected_the_fixed_value_flag()
      {
         _parameter.Formula.ShouldBeAnInstanceOf<OSPSuite.Core.Domain.Formulas.TableFormula>();
         _parameter.Value.ShouldBeEqualTo(_parameterValue);
         _parameter.IsFixedValue.ShouldBeFalse();
      }
   }

   public class When_updating_a_paraneter_from_a_snapshot_object_using_a_table_formula_and_the_value_was_changed_by_the_user : concern_for_ParameterMapper
   {
      private SnapshotParameter _snapshotParameter;
      private double _parameterValue;

      protected override async Task Context()
      {
         await base.Context();
         _parameterValue = 1500;
         _parameter.Value = _parameterValue;
         _snapshotParameter = await sut.MapToSnapshot(_parameter);
         _snapshotParameter.Value = _parameter.ValueInDisplayUnit;
         _snapshotParameter.TableFormula = new TableFormula();
         var modelTableFormula = new OSPSuite.Core.Domain.Formulas.TableFormula();
         A.CallTo(() => _tableFormulaMapper.MapToModel(_snapshotParameter.TableFormula)).Returns(modelTableFormula);

         //Set a first value that is not the parameter value
         modelTableFormula.AddPoint(0, 1122);

         //set some dummy value to ensure reset
         _parameter.Value = 12345;
      }

      protected override async Task Because()
      {
         await sut.MapToModel(_snapshotParameter, _parameter);
      }

      [Observation]
      public void should_hsave_set_the_table_formula_into_the_parameter_and_respected_the_fixed_value_flag()
      {
         _parameter.Formula.ShouldBeAnInstanceOf<OSPSuite.Core.Domain.Formulas.TableFormula>();
         _parameter.Value.ShouldBeEqualTo(_parameterValue);
         _parameter.IsFixedValue.ShouldBeTrue();
      }
   }

   public class When_mapping_a_parameter_using_the_parameter_less_dimension : concern_for_ParameterMapper
   {
      private SnapshotParameter _snapshotParameter;

      protected override async Task Context()
      {
         await base.Context();
         _parameter.Value = 1;
         _parameter.Dimension = Constants.Dimension.NO_DIMENSION;
         _parameter.DisplayUnit = Constants.Dimension.NO_DIMENSION.DefaultUnit;
         _snapshotParameter = await sut.MapToSnapshot(_parameter);
         _parameter.Value = 10;
      }

      protected override async Task Because()
      {
         await sut.MapToModel(_snapshotParameter, _parameter);
      }

      [Observation]
      public void should_be_able_to_update_the_parameter_from_snapshot()
      {
         _parameter.Value.ShouldBeEqualTo(1);
      }
   }

   public class When_mapping_a_parameter_to_a_localized_parameter : concern_for_ParameterMapper
   {
      private LocalizedParameter _localParameter;

      protected override async Task Because()
      {
         _localParameter = await sut.LocalizedParameterFrom(_parameter, x => $"Path is {x.Name}");
      }

      [Observation]
      public void should_map_the_standard_properties_of_a_parameter()
      {
         _localParameter.Value.ShouldBeEqualTo(_parameter.ValueInDisplayUnit);
      }

      [Observation]
      public void should_have_set_the_path_to_the_expected_value()
      {
         _localParameter.Path.ShouldBeEqualTo($"Path is {_parameter.Name}");
      }

      [Observation]
      public void should_have_reset_the_name()
      {
         _localParameter.Name.ShouldBeNull();
      }
   }

   public class When_mapping_all_localized_parameters_defined_in_a_container : concern_for_ParameterMapper
   {
      private LocalizedParameter _localParameter;
      private IContainer _container;

      protected override async Task Context()
      {
         await base.Context();
         _container = new Container().WithName("ORG");
         _container.Add(_parameter);

         _localParameter = new LocalizedParameter
         {
            Path = "LOCALIZED_PATH",
            Value = 5,
            Unit = _parameter.DisplayUnit.Name
         };
         A.CallTo(() => _entityPathResolver.PathFor(_parameter)).Returns(_localParameter.Path);
      }

      protected override async Task Because()
      {
         await sut.MapLocalizedParameters(new[] {_localParameter}, _container);
      }

      [Observation]
      public void should_map_the_matching_parameters()
      {
         _parameter.ValueInDisplayUnit.ShouldBeEqualTo(5);
      }
   }

   public class When_mapping_all_localized_parameters_containing_an_unknown_parameter_by_path : concern_for_ParameterMapper
   {
      private LocalizedParameter _localParameter;
      private IContainer _container;

      protected override async Task Context()
      {
         await base.Context();
         _container = new Container().WithName("ORG");

         _localParameter = new LocalizedParameter
         {
            Path = "UNKNOW_PATH",
         };
      }

      protected override Task Because()
      {
         return sut.MapLocalizedParameters(new[] {_localParameter}, _container);
      }

      [Observation]
      public void should_warn_the_user_that_the_parameter_was_not_found_in_the_container()
      {
         A.CallTo(() => _logger.AddToLog(A<string>.That.Contains(_localParameter.Path), NotificationType.Warning)).MustHaveHappened();
      }
   }

   public class When_mapping_snapshot_parameters_defined_in_a_given_container : concern_for_ParameterMapper
   {
      private Container _container;
      private SnapshotParameter _snapshot;

      protected override async Task Context()
      {
         await base.Context();
         _container = new Container().WithName("ORG");
         _container.Add(_parameter);

         _snapshot = new SnapshotParameter
         {
            Name = _parameter.Name,
            Value = 5,
            Unit = _parameter.DisplayUnit.Name
         };
      }

      protected override async Task Because()
      {
         await sut.MapParameters(new[] {_snapshot,}, _container, _container.Name);
      }

      [Observation]
      public void should_map_the_matching_parameters()
      {
         _parameter.ValueInDisplayUnit.ShouldBeEqualTo(5);
      }
   }

   public class When_mapping_a_snaphsot_parameter_by_name_that_is_not_found_in_the_container : concern_for_ParameterMapper
   {
      private Container _container;
      private SnapshotParameter _snapshot;

      protected override async Task Context()
      {
         await base.Context();
         _container = new Container().WithName("ORG");
         _container.Add(_parameter);

         _snapshot = new SnapshotParameter
         {
            Name = "NOT FOUND",
         };
      }

      protected override async Task Because()
      {
         await sut.MapParameters(new[] {_snapshot,}, _container, _container.Name);
      }

      [Observation]
      public void should_warn_the_user_that_the_parameter_was_not_found_in_the_container()
      {
         A.CallTo(() => _logger.AddToLog(A<string>.That.Contains(_snapshot.Name), NotificationType.Warning)).MustHaveHappened();
      }
   }
}