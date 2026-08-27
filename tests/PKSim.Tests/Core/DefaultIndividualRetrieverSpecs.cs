using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Exceptions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_DefaultIndividualRetriever : ContextSpecification<IDefaultIndividualRetriever>
   {
      protected ISpeciesRepository _speciesRepository;
      protected IIndividualFactory _individualFactory;
      protected OriginDataMapper _originDataMapper;
      protected SpeciesPopulation _speciesPopulation;

      protected override void Context()
      {
         _speciesRepository = A.Fake<ISpeciesRepository>();
         _individualFactory = A.Fake<IIndividualFactory>();
         _originDataMapper = A.Fake<OriginDataMapper>();

         _speciesPopulation = new SpeciesPopulation {Name = "POP", Species = "HUMAN"};
         _speciesPopulation.AddGender(new Gender {Name = "MALE"});

         sut = new DefaultIndividualRetriever(_speciesRepository, _individualFactory, _originDataMapper);
      }
   }

   public class When_retrieving_the_default_individual_for_the_same_population_from_multiple_threads : concern_for_DefaultIndividualRetriever
   {
      private Individual _firstResult;
      private Individual _secondResult;
      private ManualResetEventSlim _creationStarted;
      private ManualResetEventSlim _releaseCreation;
      private ManualResetEventSlim _secondRetrievalStarted;
      private bool _secondRetrievalWasBlocked;

      protected override void Context()
      {
         base.Context();
         _creationStarted = new ManualResetEventSlim();
         _releaseCreation = new ManualResetEventSlim();
         _secondRetrievalStarted = new ManualResetEventSlim();

         A.CallTo(() => _individualFactory.CreateStandardFor(A<OriginData>._)).ReturnsLazily(() =>
         {
            _creationStarted.Set();
            _releaseCreation.Wait(TimeSpan.FromSeconds(5));
            return A.Fake<Individual>();
         });
      }

      protected override void Because()
      {
         var firstRetrieval = Task.Run(() => sut.DefaultIndividualFor(_speciesPopulation));
         _creationStarted.Wait(TimeSpan.FromSeconds(5)).ShouldBeTrue();

         var secondRetrieval = Task.Run(() =>
         {
            _secondRetrievalStarted.Set();
            return sut.DefaultIndividualFor(_speciesPopulation);
         });

         //the second retrieval is running and has to wait for the creation already in progress. A retrieval
         //that never reached the cache in time would also read as blocked here: the guard was verified by
         //making the creation eager, which makes the observation fail
         _secondRetrievalStarted.Wait(TimeSpan.FromSeconds(5)).ShouldBeTrue();
         _secondRetrievalWasBlocked = !secondRetrieval.Wait(TimeSpan.FromMilliseconds(500));

         _releaseCreation.Set();
         Task.WaitAll(new[] {firstRetrieval, secondRetrieval}, TimeSpan.FromSeconds(5)).ShouldBeTrue();
         _firstResult = firstRetrieval.Result;
         _secondResult = secondRetrieval.Result;
      }

      public override void Cleanup()
      {
         base.Cleanup();
         _creationStarted.Dispose();
         _releaseCreation.Dispose();
         _secondRetrievalStarted.Dispose();
      }

      [Observation]
      public void should_make_the_second_retrieval_wait_for_the_creation()
      {
         _secondRetrievalWasBlocked.ShouldBeTrue();
      }

      [Observation]
      public void should_create_the_default_individual_only_once()
      {
         A.CallTo(() => _individualFactory.CreateStandardFor(A<OriginData>._)).MustHaveHappenedOnceExactly();
      }

      [Observation]
      public void should_serve_both_callers_from_the_same_cache_entry()
      {
         ReferenceEquals(_firstResult, _secondResult).ShouldBeTrue();
      }
   }

   public class When_retrieving_the_default_individual_for_different_populations_from_multiple_threads : concern_for_DefaultIndividualRetriever
   {
      private SpeciesPopulation _otherPopulation;
      private ManualResetEventSlim _creationStarted;
      private ManualResetEventSlim _releaseCreation;
      private bool _otherRetrievalCompleted;

      protected override void Context()
      {
         base.Context();
         _creationStarted = new ManualResetEventSlim();
         _releaseCreation = new ManualResetEventSlim();

         _otherPopulation = new SpeciesPopulation {Name = "OTHER_POP", Species = "HUMAN"};
         _otherPopulation.AddGender(new Gender {Name = "MALE"});

         //the mapper is a fake, so the created individuals cannot be told apart by origin data: the first creation
         //is the blocked one and any later creation returns straight away
         A.CallTo(() => _individualFactory.CreateStandardFor(A<OriginData>._)).ReturnsLazily(() =>
         {
            _creationStarted.Set();
            _releaseCreation.Wait(TimeSpan.FromSeconds(5));
            return A.Fake<Individual>();
         }).Once().Then.ReturnsLazily(() => A.Fake<Individual>());
      }

      protected override void Because()
      {
         var blockedRetrieval = Task.Run(() => sut.DefaultIndividualFor(_speciesPopulation));
         _creationStarted.Wait(TimeSpan.FromSeconds(5)).ShouldBeTrue();

         //a different population must not wait for the creation already in progress
         _otherRetrievalCompleted = Task.Run(() => sut.DefaultIndividualFor(_otherPopulation)).Wait(TimeSpan.FromSeconds(2));

         _releaseCreation.Set();
         blockedRetrieval.Wait(TimeSpan.FromSeconds(5)).ShouldBeTrue();
      }

      public override void Cleanup()
      {
         base.Cleanup();
         _creationStarted.Dispose();
         _releaseCreation.Dispose();
      }

      [Observation]
      public void should_not_block_the_retrieval_for_an_unrelated_population()
      {
         _otherRetrievalCompleted.ShouldBeTrue();
      }
   }

   public class When_the_creation_of_the_default_individual_fails : concern_for_DefaultIndividualRetriever
   {
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
         A.CallTo(() => _individualFactory.CreateStandardFor(A<OriginData>._)).Throws<OSPSuiteException>().Once()
            .Then.Returns(_individual);
      }

      [Observation]
      public void should_not_cache_the_failure_and_create_the_individual_on_the_next_call()
      {
         The.Action(() => sut.DefaultIndividualFor(_speciesPopulation)).ShouldThrowAn<OSPSuiteException>();
         ReferenceEquals(sut.DefaultIndividualFor(_speciesPopulation), _individual).ShouldBeTrue();
      }
   }
}
