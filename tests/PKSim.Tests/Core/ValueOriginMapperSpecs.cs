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
         _valueOriginRepository = A.Fake<IValueOriginRepository>();

         sut = new ValueOriginMapper(_valueOriginRepository);

         _valueOrigin = new ValueOrigin
         {
            Id = 5,
            Source = ValueOriginSources.Database,
            Method = ValueOriginDeterminationMethods.InVitro
         };

         return _completed;
      }
   }

   public class When_mapping_a_value_origin_to_snapshot_that_comes_from_the_database : concern_for_ValueOriginMapper
   {
      private OSPSuite.Core.Snapshots.ValueOrigin _snapshotValueOrigin;
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
}