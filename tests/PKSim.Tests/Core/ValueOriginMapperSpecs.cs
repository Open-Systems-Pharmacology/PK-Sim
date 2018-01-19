using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_ValueOriginMapper : ContextSpecificationAsync<ValueOriginMapper>
   {
      protected ValueOrigin _valueOrigin;

      protected override Task Context()
      {
         sut = new ValueOriginMapper();

         _valueOrigin = new ValueOrigin
         {
            Source = ValueOriginSources.Database,
            Method = ValueOriginDeterminationMethods.InVitro
         };

         return _completed;
      }
   }

   public class When_mapping_a_value_origin_to_snapshot : concern_for_ValueOriginMapper
   {
      private Snapshots.ValueOrigin _snapshotValueOrigin;

      protected override async Task Because()
      {
         _snapshotValueOrigin = await sut.MapToSnapshot(_valueOrigin);
      }

      [Observation]
      public void should_return_an_object_having_the_expected_properties()
      {
         _snapshotValueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Database.Id);
         _snapshotValueOrigin.Method.ShouldBeEqualTo(ValueOriginDeterminationMethods.InVitro.Id);
      }

      [Observation]
      public void should_have_map_default_properties_to_null()
      {
         _snapshotValueOrigin.Id.ShouldBeNull();
         _snapshotValueOrigin.Default.ShouldBeNull();
      }
   }

   public class When_updating_a_value_origin_from_a_snapshot_value_origin : concern_for_ValueOriginMapper
   {
      private Snapshots.ValueOrigin _snapshot;

      protected override async Task Context()
      {
         await base.Context();

         _snapshot = new Snapshots.ValueOrigin
         {
            Id = 5,
            Source = ValueOriginSourceId.Database,
            Method = ValueOriginDeterminationMethodId.InVitro,
            Description = "Good morning"
         };
      }

      protected override Task Because()
      {
         sut.UpdateValueOrigin(_valueOrigin, _snapshot);
         return _completed;
      }

      [Observation]
      public void should_update_the_value_origin_according_to_snapshot_values()
      {
         _valueOrigin.Id.ShouldBeEqualTo(_snapshot.Id.Value);
         _valueOrigin.Description.ShouldBeEqualTo(_snapshot.Description);
         _valueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Database);
         _valueOrigin.Method.ShouldBeEqualTo(ValueOriginDeterminationMethods.InVitro);
      }
   }
}