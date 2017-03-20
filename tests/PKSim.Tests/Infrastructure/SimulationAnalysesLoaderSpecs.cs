using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using PKSim.Infrastructure.Serialization.ORM.Queries;
using PKSim.Infrastructure.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SimulationAnalysesLoader : ContextSpecification<ISimulationAnalysesLoader>
   {
      protected ISimulationAnalysesQuery _simulationAnalysesQuery;
      protected ICompressedSerializationManager _serializationManager;

      protected override void Context()
      {
         _simulationAnalysesQuery= A.Fake<ISimulationAnalysesQuery>();
         _serializationManager= A.Fake<ICompressedSerializationManager>();
         sut = new SimulationAnalysesLoader(_simulationAnalysesQuery,_serializationManager);
      }
   }

   public class When_loading_the_simulation_pk_analyses_for_a_population_for_which_no_analyses_were_saved : concern_for_SimulationAnalysesLoader
   {
      private PopulationSimulation _populationSimulation;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>().WithId("Toto");
         A.CallTo(() => _simulationAnalysesQuery.ResultFor(_populationSimulation.Id)).Returns(null);
      }

      protected override void Because()
      {
         sut.LoadAnalysesFor(_populationSimulation);
      }

      [Observation]
      public void should_have_set_an_empty_pk_analyses()
      {
         _populationSimulation.PKAnalyses.IsNull().ShouldBeTrue();
      }
   }

   public class When_loading_the_simulation_pk_analyses_for_a_population_for_which_analyses_were_saved : concern_for_SimulationAnalysesLoader
   {
      private PopulationSimulation _populationSimulation;
      private SimulationAnalysesMetaData _anaylysesMetaData;
      private PopulationSimulationPKAnalyses _simPkAnalyses;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>().WithId("Toto");
         _anaylysesMetaData= A.Fake<SimulationAnalysesMetaData>();
         _simPkAnalyses=new PopulationSimulationPKAnalyses();
         A.CallTo(() => _simulationAnalysesQuery.ResultFor(_populationSimulation.Id)).Returns(_anaylysesMetaData);
         A.CallTo( _serializationManager).WithReturnType<PopulationSimulationPKAnalyses>().Returns(_simPkAnalyses);
      }

      protected override void Because()
      {
         sut.LoadAnalysesFor(_populationSimulation);
      }

      [Observation]
      public void should_retrieve_the_pk_analyses()
      {
         _populationSimulation.PKAnalyses.ShouldBeEqualTo(_simPkAnalyses);
      }
   }
}	