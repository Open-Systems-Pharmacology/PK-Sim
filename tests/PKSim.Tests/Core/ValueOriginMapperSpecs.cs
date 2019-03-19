using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;



namespace PKSim.Core
{
   public abstract class concern_for_ValueOriginMapper : ContextSpecificationAsync<ValueOriginMapper>
   {
      protected ValueOrigin _valueOrigin;
      protected IValueOriginRepository _valueOriginRepository;

      protected override Task Context()
      {
         _valueOriginRepository= A.Fake<IValueOriginRepository>(); 

         sut = new ValueOriginMapper(_valueOriginRepository);

         _valueOrigin = new ValueOrigin
         {
            Id=5,
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
         _snapshotValueOrigin.Id.ShouldBeEqualTo(_valueOrigin.Id);
      }

   }

   public class When_mapping_a_value_origin_to_snapshot_that_comes_from_the_database : concern_for_ValueOriginMapper
   {
      private Snapshots.ValueOrigin _snapshotValueOrigin;
      private ValueOrigin _sameValueOrigin;

      protected override async Task Context()
      {
         await base.Context();
         _sameValueOrigin = _valueOrigin.Clone();
         A.CallTo(() => _valueOriginRepository.FindBy(_valueOrigin.Id)).Returns(_sameValueOrigin);

      }
      protected override async Task Because()
      {
         _snapshotValueOrigin = await sut.MapToSnapshot(_valueOrigin);
      }

      [Observation]
      public void should_return_null()
      {
         _snapshotValueOrigin.ShouldBeNull();
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