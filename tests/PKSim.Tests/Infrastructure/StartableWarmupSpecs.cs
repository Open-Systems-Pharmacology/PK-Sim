using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using PKSim.Core;
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

   public class When_awaiting_completion_from_several_threads : concern_for_StartableWarmup
   {
      private ManualResetEventSlim _startBlocked;
      private ManualResetEventSlim _releaseStart;
      private bool _bothCallersCompleted;

      protected override void Context()
      {
         base.Context();
         _startBlocked = new ManualResetEventSlim();
         _releaseStart = new ManualResetEventSlim();
         A.CallTo(() => _startable.Start()).Invokes(() =>
         {
            _startBlocked.Set();
            _releaseStart.Wait(TimeSpan.FromSeconds(5));
         });
      }

      protected override void Because()
      {
         var firstCaller = Task.Run(() => sut.AwaitCompletion());
         _startBlocked.Wait(TimeSpan.FromSeconds(5)).ShouldBeTrue();

         //the warm-up is already running: the second caller must share it, not start another one
         var secondCaller = Task.Run(() => sut.AwaitCompletion());

         _releaseStart.Set();
         _bothCallersCompleted = Task.WaitAll(new[] {firstCaller, secondCaller}, TimeSpan.FromSeconds(5));
      }

      public override void Cleanup()
      {
         base.Cleanup();
         _startBlocked.Dispose();
         _releaseStart.Dispose();
      }

      [Observation]
      public void should_block_both_callers_until_the_same_warmup_completed()
      {
         _bothCallersCompleted.ShouldBeTrue();
      }

      [Observation]
      public void should_start_every_startable_exactly_once()
      {
         A.CallTo(() => _startable.Start()).MustHaveHappenedOnceExactly();
      }
   }

   public class When_the_warmup_fails : concern_for_StartableWarmup
   {
      private Exception _firstFailure;
      private Exception _secondFailure;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _startable.Start()).Throws<InvalidOperationException>();
      }

      protected override void Because()
      {
         _firstFailure = failureOf(() => sut.AwaitCompletion());
         _secondFailure = failureOf(() => sut.AwaitCompletion());
      }

      private static Exception failureOf(Action action)
      {
         try
         {
            action();
            return null;
         }
         catch (Exception e)
         {
            return e;
         }
      }

      [Observation]
      public void should_fail_naming_the_startable_that_could_not_start()
      {
         _firstFailure.ShouldBeAnInstanceOf<PKSimException>();
         _firstFailure.Message.Contains("failed to start").ShouldBeTrue();
      }

      //a failed warm-up stays failed: the same exception is rethrown without running Start again on a
      //repository that already mutated part of its state
      [Observation]
      public void should_rethrow_the_same_failure_without_starting_again()
      {
         ReferenceEquals(_firstFailure, _secondFailure).ShouldBeTrue();
         A.CallTo(() => _startable.Start()).MustHaveHappenedOnceExactly();
      }
   }
}
