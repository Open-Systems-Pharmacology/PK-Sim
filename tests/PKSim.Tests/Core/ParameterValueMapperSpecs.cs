using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Snapshots;
using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using ModelParameterValue = OSPSuite.Core.Domain.Builder.ParameterValue;
using ParameterValueMapper = PKSim.Core.Snapshots.Mappers.ParameterValueMapper;
using SnapshotParameterValue = PKSim.Core.Snapshots.ParameterValue;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterValueMapper : ContextSpecificationAsync<ParameterValueMapper>
   {
      protected IDimensionRepository _dimensionRepository;
      protected IDimension _dimension;
      protected ModelParameterValue _parameterValue;
      protected SnapshotParameterValue _snapshot;
      protected SnapshotContext _snapshotContext;

      protected override Task Context()
      {
         _dimension = DomainHelperForSpecs.LengthDimensionForSpecs();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         A.CallTo(() => _dimensionRepository.DimensionByName(_dimension.Name)).Returns(_dimension);
         A.CallTo(() => _dimensionRepository.DimensionByName(null)).Returns(Constants.Dimension.NO_DIMENSION);

         _snapshotContext = new SnapshotContext(new PKSimProject(), SnapshotVersions.Current);

         var valueOriginRepository = A.Fake<IValueOriginRepository>();
         A.CallTo(() => valueOriginRepository.FindBy(A<int?>._)).Returns(new OSPSuite.Core.Domain.ValueOrigin());
         sut = new ParameterValueMapper(_dimensionRepository, new PKSim.Core.Snapshots.Mappers.ValueOriginMapper(valueOriginRepository));

         _parameterValue = new ModelParameterValue
         {
            Path = "Organism|Aspirin|Permeability".ToObjectPath(),
            Value = 0.05,
            Dimension = _dimension,
            DisplayUnit = _dimension.Unit("cm"),
            Info = new ParameterInfo { MinValue = 0, MinIsAllowed = true, MaxValue = 0.2, MaxIsAllowed = false }
         };

         _parameterValue.ValueOrigin.Description = "From literature";

         return _completed;
      }
   }

   public class When_mapping_a_parameter_value_to_snapshot : concern_for_ParameterValueMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_parameterValue);
      }

      [Observation]
      public void should_map_the_path()
      {
         _snapshot.Path.ShouldBeEqualTo("Organism|Aspirin|Permeability");
      }

      [Observation]
      public void should_map_the_dimension_and_the_display_unit()
      {
         _snapshot.Dimension.ShouldBeEqualTo(_dimension.Name);
         _snapshot.Unit.ShouldBeEqualTo("cm");
      }

      [Observation]
      public void should_map_the_value_in_display_unit()
      {
         _snapshot.Value.ShouldBeEqualTo(5);
      }

      [Observation]
      public void should_map_the_allowed_range_in_display_unit()
      {
         _snapshot.MinValue.ShouldBeEqualTo(0);
         _snapshot.MaxValue.ShouldBeEqualTo(20);
      }

      [Observation]
      public void should_only_map_the_min_and_max_is_allowed_flags_that_differ_from_the_default()
      {
         _snapshot.MinIsAllowed.ShouldBeNull();
         _snapshot.MaxIsAllowed.ShouldBeEqualTo(false);
      }

      [Observation]
      public void should_map_the_value_origin()
      {
         _snapshot.ValueOrigin.Description.ShouldBeEqualTo("From literature");
      }
   }

   public class When_mapping_a_dimensionless_parameter_value_to_snapshot : concern_for_ParameterValueMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _parameterValue.Dimension = Constants.Dimension.NO_DIMENSION;
         _parameterValue.DisplayUnit = Constants.Dimension.NO_DIMENSION.DefaultUnit;
         _parameterValue.Info = null;
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_parameterValue);
      }

      [Observation]
      public void should_not_map_dimension_and_unit()
      {
         _snapshot.Dimension.ShouldBeNull();
         _snapshot.Unit.ShouldBeNull();
      }

      [Observation]
      public void should_map_the_value_as_it_is()
      {
         _snapshot.Value.ShouldBeEqualTo(0.05);
      }
   }

   public class When_mapping_a_parameter_value_without_an_allowed_range_to_snapshot : concern_for_ParameterValueMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _parameterValue.Info = null;
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_parameterValue);
      }

      [Observation]
      public void should_not_map_any_bound()
      {
         _snapshot.MinValue.ShouldBeNull();
         _snapshot.MaxValue.ShouldBeNull();
         _snapshot.MinIsAllowed.ShouldBeNull();
         _snapshot.MaxIsAllowed.ShouldBeNull();
      }
   }

   public class When_mapping_a_parameter_value_snapshot_to_a_parameter_value : concern_for_ParameterValueMapper
   {
      private ModelParameterValue _result;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_parameterValue);
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot, _snapshotContext);
      }

      [Observation]
      public void should_restore_the_path_the_dimension_and_the_display_unit()
      {
         _result.Path.ToString().ShouldBeEqualTo("Organism|Aspirin|Permeability");
         _result.Dimension.ShouldBeEqualTo(_dimension);
         _result.DisplayUnit.Name.ShouldBeEqualTo("cm");
      }

      [Observation]
      public void should_restore_the_value_in_base_unit()
      {
         _result.Value.ShouldBeEqualTo(0.05);
      }

      [Observation]
      public void should_restore_the_allowed_range_in_base_unit()
      {
         _result.Info.MinValue.ShouldBeEqualTo(0);
         _result.Info.MinIsAllowed.ShouldBeTrue();
         _result.Info.MaxValue.ShouldBeEqualTo(0.2);
         _result.Info.MaxIsAllowed.ShouldBeFalse();
      }

      [Observation]
      public void should_restore_the_value_origin()
      {
         _result.ValueOrigin.Description.ShouldBeEqualTo("From literature");
      }
   }

   public class When_mapping_a_parameter_value_snapshot_without_dimension_and_allowed_range_to_a_parameter_value : concern_for_ParameterValueMapper
   {
      private ModelParameterValue _result;

      protected override async Task Because()
      {
         _result = await sut.MapToModel(new SnapshotParameterValue { Path = "Organism|Aspirin|Permeability", Value = 0.05 }, _snapshotContext);
      }

      [Observation]
      public void should_use_the_value_as_base_unit_value()
      {
         _result.Value.ShouldBeEqualTo(0.05);
      }

      [Observation]
      public void should_not_define_an_allowed_range()
      {
         _result.Info.ShouldBeNull();
      }
   }
}
