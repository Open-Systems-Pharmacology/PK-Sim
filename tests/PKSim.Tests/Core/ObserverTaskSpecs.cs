using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ObserverTask : ContextSpecification<IObserverTask>
   {
      protected IExecutionContext _executionContext;
      protected IObserverLoader _observerLoader;
      protected IObserverBuilder _observer1;
      protected IObserverBuilder _observer2;
      protected PKSimObserverBuildingBlock _observerBuildingBlock;
      protected IObjectIdResetter _objectIdResetter;

      protected override void Context()
      {
         _observerLoader = A.Fake<IObserverLoader>();
         _executionContext = A.Fake<IExecutionContext>();
         _objectIdResetter= A.Fake<IObjectIdResetter>();
         sut = new ObserverTask(_executionContext, _observerLoader, _objectIdResetter);

         _observer1 = new AmountObserverBuilder().WithName("OBS1");
         _observer2 = new AmountObserverBuilder().WithName("OBS2");
         _observerBuildingBlock = new PKSimObserverBuildingBlock();
      }
   }

   public class When_adding_an_observer_to_an_observer_building_block : concern_for_ObserverTask
   {
      private IPKSimCommand _command;

      protected override void Because()
      {
         _command = sut.AddObserver(_observer1, _observerBuildingBlock);
      }

      [Observation]
      public void should_have_added_the_observer_to_the_building_block()
      {
         _observerBuildingBlock.Observers.ShouldOnlyContain(_observer1);
      }

      [Observation]
      public void should_return_the_expected_commnad()
      {
         _command.ShouldBeAnInstanceOf<AddObserverToObserverBuildingBlockCommand>();
      }
   }

   public class When_removing_an_observer_from_an_observer_building_block : concern_for_ObserverTask
   {
      private IPKSimCommand _command;

      protected override void Context()
      {
         base.Context();
         sut.AddObserver(_observer1, _observerBuildingBlock);
         sut.AddObserver(_observer2, _observerBuildingBlock);
      }

      protected override void Because()
      {
         _command = sut.RemoveObserver(_observer1, _observerBuildingBlock);
      }

      [Observation]
      public void should_have_removed_the_observer_to_the_building_block()
      {
         _observerBuildingBlock.Observers.ShouldOnlyContain(_observer2);
      }

      [Observation]
      public void should_return_the_expected_commnad()
      {
         _command.ShouldBeAnInstanceOf<RemoveObserverFromObserverBuildingBlockCommand>();
      }
   }

   public class When_loading_an_observer_from_file : concern_for_ObserverTask
   {
      private readonly string _fileName = "OBS_FILE";
      private IObserverBuilder _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _observerLoader.Load(_fileName)).Returns(_observer1);
      }

      protected override void Because()
      {
         _result = sut.LoadObserverFrom(_fileName);
      }

      [Observation]
      public void should_delegate_to_the_observer_loader_to_load_the_observer()
      {
         _result.ShouldBeEqualTo(_observer1);
      }

      [Observation]
      public void should_have_resetted_the_obserer_ids()
      {
         A.CallTo(() => _objectIdResetter.ResetIdFor(_observer1)).MustHaveHappened();
      }
   }
}