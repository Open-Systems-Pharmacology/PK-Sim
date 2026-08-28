using System;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_StartableWarmup : ContextSpecification<IStartableWarmup>
   {
      protected IContainer _container;
      protected IStartable _startable;

      protected override void Context()
      {
         _container = A.Fake<IContainer>();
         _startable = A.Fake<IStartable>();
         A.CallTo(() => _container.ResolveAll<IStartable>()).Returns(new[] {_startable});
         sut = new StartableWarmup(_container);
      }
   }

   public class When_awaiting_completion_without_a_started_warmup : concern_for_StartableWarmup
   {
      protected override void Because()
      {
         sut.AwaitCompletion();
         sut.AwaitCompletion();
      }

      //hosts that never call Begin (CLI, R, qualification) must still get warmed repositories, exactly once
      [Observation]
      public void should_start_every_startable_exactly_once()
      {
         A.CallTo(() => _startable.Start()).MustHaveHappenedOnceExactly();
      }
   }

   public class When_awaiting_a_warmup_started_with_begin : concern_for_StartableWarmup
   {
      private IStartable _given;

      protected override void Context()
      {
         base.Context();
         _given = A.Fake<IStartable>();
         sut.Begin(new[] {_given});
      }

      protected override void Because()
      {
         sut.AwaitCompletion();
         sut.AwaitCompletion();
      }

      [Observation]
      public void should_start_the_given_startables_exactly_once()
      {
         A.CallTo(() => _given.Start()).MustHaveHappenedOnceExactly();
      }

      [Observation]
      public void should_not_resolve_the_startables_itself()
      {
         A.CallTo(() => _container.ResolveAll<IStartable>()).MustNotHaveHappened();
      }
   }

   public class When_the_warmup_fails : concern_for_StartableWarmup
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _startable.Start()).Throws<InvalidOperationException>().Once();
      }

      //a failed warm-up is not cached: the next call retries and succeeds
      [Observation]
      public void should_rethrow_the_failure_and_retry_on_the_next_call()
      {
         The.Action(() => sut.AwaitCompletion()).ShouldThrowAn<InvalidOperationException>();
         sut.AwaitCompletion();
         A.CallTo(() => _startable.Start()).MustHaveHappenedTwiceExactly();
      }
   }
}
