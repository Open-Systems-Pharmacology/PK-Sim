using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_StartableWarmup : ContextSpecification<IStartableWarmup>
   {
      protected IContainer _container;
      protected IOSPSuiteLogger _logger;
      protected IStartable _startable;

      protected override void Context()
      {
         _container = A.Fake<IContainer>();
         _logger = A.Fake<IOSPSuiteLogger>();
         _startable = A.Fake<IStartable>();
         A.CallTo(() => _container.ResolveAll<IStartable>()).Returns(new[] {_startable});
         sut = new StartableWarmup(_container, _logger);
      }
   }

   public class When_awaiting_completion_without_a_started_warmup : concern_for_StartableWarmup
   {
      private bool _result;

      protected override void Because()
      {
         _result = sut.AwaitCompletion();
         sut.AwaitCompletion();
      }

      //hosts that never call Begin (CLI, R, qualification) must still get warmed repositories, exactly once
      [Observation]
      public void should_start_every_startable_exactly_once()
      {
         A.CallTo(() => _startable.Start()).MustHaveHappenedOnceExactly();
      }

      [Observation]
      public void should_report_the_warmup_as_complete()
      {
         _result.ShouldBeTrue();
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
         _bothCallersCompleted = Task.WaitAll(new Task[] {firstCaller, secondCaller}, TimeSpan.FromSeconds(5));
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

   public class When_a_startable_fails_to_start : concern_for_StartableWarmup
   {
      private IStartable _failing;
      private bool _firstResult;
      private bool _secondResult;

      protected override void Context()
      {
         base.Context();
         _failing = A.Fake<IStartable>();
         A.CallTo(() => _failing.Start()).Throws<InvalidOperationException>();
         A.CallTo(() => _container.ResolveAll<IStartable>()).Returns(new[] {_failing, _startable});
      }

      protected override void Because()
      {
         _firstResult = sut.AwaitCompletion();
         _secondResult = sut.AwaitCompletion();
      }

      //one broken startable must not leave the remaining ones cold
      [Observation]
      public void should_report_the_incomplete_warmup_and_still_start_the_remaining_startables()
      {
         _firstResult.ShouldBeFalse();
         A.CallTo(() => _startable.Start()).MustHaveHappenedOnceExactly();
      }

      [Observation]
      public void should_log_the_failure_naming_the_startable()
      {
         A.CallTo(() => _logger.AddToLog(A<string>.That.Contains("failed to start"), LogLevel.Error, A<string>._)).MustHaveHappened();
      }

      //the failed startable stays cold: its only retry is the lazy initialization on its first use, the
      //warm-up never runs Start again on a repository that may already have mutated part of its state
      [Observation]
      public void should_not_start_the_failed_startable_again_on_the_next_call()
      {
         _secondResult.ShouldBeFalse();
         A.CallTo(() => _failing.Start()).MustHaveHappenedOnceExactly();
      }
   }

   public class When_the_warmup_runs_out_of_memory : concern_for_StartableWarmup
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _startable.Start()).Throws<OutOfMemoryException>();
      }

      //an out-of-memory failure must fail the operation rather than degrade it silently
      [Observation]
      public void should_rethrow_on_every_call_without_starting_again()
      {
         The.Action(() => sut.AwaitCompletion()).ShouldThrowAn<OutOfMemoryException>();
         The.Action(() => sut.AwaitCompletion()).ShouldThrowAn<OutOfMemoryException>();
         A.CallTo(() => _startable.Start()).MustHaveHappenedOnceExactly();
      }
   }
}
