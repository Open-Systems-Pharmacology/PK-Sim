using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using ObserverSet = PKSim.Core.Model.ObserverSet;

namespace PKSim.Core
{
   public abstract class concern_for_ObserverSetMapper : ContextSpecificationAsync<ObserverSetMapper>
   {
      protected ObserverMapper _observerMapper;
      protected IObserverSetFactory _observerSetFactory;
      protected ObserverSet _observerSet;
      private ObserverBuilder _observer;
      protected Observer _observerSnapshot;

      protected override Task Context()
      {
         _observerMapper = A.Fake<ObserverMapper>();
         _observerSetFactory = A.Fake<IObserverSetFactory>();
         _observer = new AmountObserverBuilder();
         _observerSet = new ObserverSet {_observer};
         _observerSet.Name = "OBS_SET";
         _observerSet.Description = "OBS_SET_DESCRIPTION";
         sut = new ObserverSetMapper(_observerMapper, _observerSetFactory);
         _observerSnapshot = new Observer();
         A.CallTo(() => _observerMapper.MapToSnapshot(_observer)).Returns(_observerSnapshot);
         return _completed;
      }
   }

   public class When_mapping_a_model_observer_set_to_snapshot : concern_for_ObserverSetMapper
   {
      private Snapshots.ObserverSet _snapshot;

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_observerSet);
      }

      [Observation]
      public void should_have_set_the_observer_set_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_observerSet.Name);
         _snapshot.Description.ShouldBeEqualTo(_observerSet.Description);
      }

      [Observation]
      public void should_have_mapped_the_underlying_observers()
      {
         _snapshot.Observers.ShouldOnlyContain(_observerSnapshot);
      }
   }

   public class When_mapping_a_valid_observer_set_snapshot_to_observer_set : concern_for_ObserverSetMapper
   {
      private Snapshots.ObserverSet _snapshot;
      private ObserverSet _newObserverSet;
      private ObserverBuilder _newObserverBuilder;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_observerSet);
         A.CallTo(() => _observerSetFactory.Create()).Returns(new ObserverSet());

         _snapshot.Name = "New Observer Set";
         _snapshot.Description = "The description that will be deserialized";

         _newObserverBuilder  =new ContainerObserverBuilder();
         A.CallTo(() => _observerMapper.MapToModel(_observerSnapshot, A<SnapshotContext>._)).Returns(_newObserverBuilder);
      }

      protected override async Task Because()
      {
         _newObserverSet = await sut.MapToModel(_snapshot, new SnapshotContext());
      }

      [Observation]
      public void should_have_created_an_observer_set_with_the_expected_properties()
      {
         _newObserverSet.Name.ShouldBeEqualTo(_snapshot.Name);
         _newObserverSet.Description.ShouldBeEqualTo(_snapshot.Description);
      }

      [Observation]
      public void should_have_mapped_the_underlying_observers()
      {
         _newObserverSet.Observers.ShouldOnlyContain(_newObserverBuilder);
      }
   }
}