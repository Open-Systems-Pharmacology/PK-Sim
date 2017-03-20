using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using NHibernate;
using PKSim.Core;
using PKSim.Infrastructure;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Core.Domain.Data;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_PopulationSimulationSerialization : ContextSpecificationWithSerializationDatabase<SimulationResults>
   {
      protected ProjectMetaData _projectMetaData;
      private SimulationMetaData _simulationMetaData;
      protected const int _numberOfIndividuals = 5;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _projectMetaData = new ProjectMetaData {Name = "Project"};
         _simulationMetaData = new SimulationMetaData {Id = "PopSim", SimulationMode = SimulationMode.Population, Name = "Sim"};
         _simulationMetaData.SimulationResults = DomainHelperForSpecs.CreateSimulationResults(_numberOfIndividuals, numberOfPaths: 2);
         _projectMetaData.AddBuildingBlock(_simulationMetaData);

         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(_projectMetaData);
            transaction.Commit();
         }
      }
   }

   public class When_saving_a_project_with_a_population_simulation_and_some_results : concern_for_PopulationSimulationSerialization
   {
      [Observation]
      public void should_be_able_to_retrieve_the_results_in_a_lazy_fashion()
      {
         using (var session = _sessionFactory.OpenSession())
         {
            var simulationMetaData = session.Get<SimulationMetaData>("PopSim");
            simulationMetaData.ShouldNotBeNull();
            NHibernateUtil.IsInitialized(simulationMetaData.SimulationResults).ShouldBeFalse();

            //access results
            int count = simulationMetaData.SimulationResults.AllIndividualResults.Count();
            NHibernateUtil.IsInitialized(simulationMetaData.SimulationResults).ShouldBeTrue();
         }
      }
   }

   public class When_saving_a_simulation_with_results_and_then_deleting_the_simulation : concern_for_PopulationSimulationSerialization
   {
      public override void GlobalContext()
      {
         base.GlobalContext();

         //then delete simulation
         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            var projectMetaData = session.CreateCriteria<ProjectMetaData>().List<ProjectMetaData>().First();
            projectMetaData.BuildingBlocks.Clear();
            session.Save(projectMetaData);
            transaction.Commit();
         }
      }

      [Observation]
      public void should_have_remove_the_results_associated_with_the_simulation()
      {
         using (var session = _sessionFactory.OpenSession())
         {
            var projectMetaData = session.CreateCriteria<ProjectMetaData>().List<ProjectMetaData>().First();
            projectMetaData.BuildingBlocks.Count.ShouldBeEqualTo(0);

            var simulationResults = session.CreateCriteria<SimulationResults>().List<SimulationResults>();
            simulationResults.Count.ShouldBeEqualTo(0);
         }
      }
   }

   public class When_updating_the_results_in_a_simulation : concern_for_PopulationSimulationSerialization
   {
      public override void GlobalContext()
      {
         base.GlobalContext();

         //then delete simulation
         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            var simulationResults = session.Get<SimulationResults>(1);
            simulationResults.ShouldNotBeNull();


            var simulationMetaData = session.Get<SimulationMetaData>("PopSim");
            session.Delete(simulationMetaData.SimulationResults);

            simulationMetaData.SimulationResults = DomainHelperForSpecs.CreateSimulationResults();
            session.Save(simulationMetaData);
            transaction.Commit();
         }
      }

      [Observation]
      public void should_remove_the_old_results()
      {
         using (var session = _sessionFactory.OpenSession())
         {
            var simulationResults = session.Get<SimulationResults>(1);
            simulationResults.ShouldBeNull();
         }
      }
   }
}