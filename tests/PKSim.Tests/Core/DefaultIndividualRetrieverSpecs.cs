using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
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

      protected override void Context()
      {
         base.Context();
         _creationStarted = new ManualResetEventSlim();
         _releaseCreation = new ManualResetEventSlim();

         A.CallTo(() => _individualFactory.CreateStandardFor(A<OriginData>._)).ReturnsLazily(() =>
         {
            _creationStarted.Set();
            _releaseCreation.Wait(TimeSpan.FromSeconds(5)).ShouldBeTrue();
            return A.Fake<Individual>();
         });
      }

      protected override void Because()
      {
         var firstRetrieval = Task.Run(() => sut.DefaultIndividualFor(_speciesPopulation));
         //the second retrieval only starts once the first is creating, so it has to wait for that creation
         _creationStarted.Wait(TimeSpan.FromSeconds(5)).ShouldBeTrue();

         var secondRetrieval = Task.Run(() => sut.DefaultIndividualFor(_speciesPopulation));
         _releaseCreation.Set();

         Task.WaitAll(new[] { firstRetrieval, secondRetrieval }, TimeSpan.FromSeconds(5)).ShouldBeTrue();
         _firstResult = firstRetrieval.Result;
         _secondResult = secondRetrieval.Result;
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
}
