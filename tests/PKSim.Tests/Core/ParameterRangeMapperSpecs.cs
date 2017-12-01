using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterRangeMapper : ContextSpecificationAsync<ParameterRangeMapper>
   {
      protected ParameterRange _parameterRange;
      protected Snapshots.ParameterRange _snapshot;

      protected override Task Context()
      {
         sut = new ParameterRangeMapper();
         _parameterRange = new ParameterRange {
            Dimension = DomainHelperForSpecs.LengthDimensionForSpecs(),
         };
         _parameterRange.Unit = _parameterRange.Dimension.Unit("cm");
         _parameterRange.MaxValueInDisplayUnit = 180;
         _parameterRange.MinValueInDisplayUnit = 120;

         return Task.FromResult(true);
      }
   }

   public class When_mapping_a_null_parameter_range_to_snapshot : concern_for_ParameterRangeMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(null);
      }

      [Observation]
      public void should_return_null()
      {
         _snapshot.ShouldBeNull();
      }
   }

   public class When_mapping_a_parameter_range_wiotut_constraints_to_snapshot : concern_for_ParameterRangeMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(new ParameterRange());
      }

      [Observation]
      public void should_return_null()
      {
         _snapshot.ShouldBeNull();
      }
   }

   public class When_mapping_a_parameter_range_with_at_least_one_constraint_to_snapshot : concern_for_ParameterRangeMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _parameterRange.MaxValueInDisplayUnit = 180;
         _parameterRange.MinValueInDisplayUnit = 120;
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_parameterRange);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_constraints_set_in_unit()
      {
         _snapshot.Min.ShouldBeEqualTo(120);
         _snapshot.Max.ShouldBeEqualTo(180);
         _snapshot.Unit.ShouldBeEqualTo("cm");
      }
   }

   public class When_mapping_a_null_parameter_range_snapshot_to_model : concern_for_ParameterRangeMapper
   {
      private ParameterRange _result;

      protected override async Task Because()
      {
         _result= await sut.MapToModel(null, _parameterRange);
      }

      [Observation]
      public void should_return_the_model()
      {
         _result.ShouldBeEqualTo(_parameterRange);
      }
   }

   public class When_mapping_a_valid_parameter_range_snapshot_to_model : concern_for_ParameterRangeMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_parameterRange);
         _snapshot.Min = 160;
         _snapshot.Max = null;
      }
      protected override async Task Because()
      {
         await sut.MapToModel(_snapshot, _parameterRange);
      }

      [Observation]
      public void should_update_the_model_mapped_with_the_expected_properties()
      {
         _parameterRange.MinValueInDisplayUnit.ShouldBeEqualTo(160);
         _parameterRange.MaxValueInDisplayUnit.ShouldBeNull();
      }
   }
}