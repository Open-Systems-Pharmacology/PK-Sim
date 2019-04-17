using System.Linq;
using System.Text;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using NHibernate.Util;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SimulationMetaData : ContextSpecificationWithSerializationDatabase<SimulationMetaData>
   {
      protected SimulationMetaData _popSim;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = new SimulationMetaData {Id = "SimulationMetaDataId", Name = "tralala"};
         sut.AddBuildingBlock(new UsedBuildingBlockMetaData {Id = "BB", Name = "Name", Altered = false, BuildingBlockType = PKSimBuildingBlockType.Population, TemplateId = "xx"});
         sut.Content.Data = Encoding.UTF8.GetBytes("new content");

         _popSim = new SimulationMetaData {Id = "PopulationMetaDataId", Name = "toto"};
         _popSim.AddBuildingBlock(new UsedBuildingBlockMetaData {Id = "BB2", Name = "Name", Altered = false, BuildingBlockType = PKSimBuildingBlockType.Population, TemplateId = "xx"});
         _popSim.Content.Data = Encoding.UTF8.GetBytes("new content");
      }
   }

   public class When_retrieving_a_simulation_meta_data_with_results : concern_for_SimulationMetaData
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(sut);
            session.Save(_popSim);
            transaction.Commit();
         }
      }

      [Observation]
      public void should_be_able_to_retrieve_the_values_stored()
      {
         using (var session = _sessionFactory.OpenSession())
         {
            var simulationFromDb = session.Get<SimulationMetaData>(sut.Id);
            simulationFromDb.ShouldNotBeNull();

            var popSimFromDb = session.Get<SimulationMetaData>(_popSim.Id);
            popSimFromDb.ShouldNotBeNull();
         }
      }
   }

   public class When_deleting_a_simulation_that_was_already_saved_with_results : concern_for_SimulationMetaData
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         //first save
         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(sut);
            transaction.Commit();
         }

         //then delete
         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Delete(sut);
            transaction.Commit();
         }
      }

      [Observation]
      public void should_be_able_to_delete_the_simulation()
      {
         using (var session = _sessionFactory.OpenSession())
         {
            var simulationFromDb = session.Get<SimulationMetaData>(sut.Id);
            simulationFromDb.ShouldBeNull();
         }
      }

      [Observation]
      public void should_have_deleted_the_results()
      {
         using (var session = _sessionFactory.OpenSession())
         {
            var datas = session.CreateCriteria<DataRepositoryMetaData>().List<DataRepositoryMetaData>();
            datas.Count.ShouldBeEqualTo(0);
         }
      }
   }

   public class When_adding_some_observed_data_to_a_simulation_containing_already_observed_data : concern_for_SimulationMetaData
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = new SimulationMetaData {Id = "SimulationMetaDataId", Name = "tralala"};
         sut.AddObservedData("ObsData1");
         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(sut);
            transaction.Commit();
         }
      }

      [Observation]
      public void should_be_able_to_save_the_simulation()
      {
         var newSimulation = new SimulationMetaData {Id = "SimulationMetaDataId", Name = "tralala"};
         newSimulation.AddObservedData("ObsData1");
         newSimulation.AddObservedData("ObsData2");

         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            var simulationFromDb = session.Get<SimulationMetaData>(sut.Id);
            simulationFromDb.UpdateFrom(newSimulation, session);
            transaction.Commit();
         }

         using (var session = _sessionFactory.OpenSession())
         {
            var simulationFromDb = session.Get<SimulationMetaData>(sut.Id);
            simulationFromDb.UsedObservedData.Count.ShouldBeEqualTo(2);
         }
      }
   }

   public class When_saving_two_simulations_sharing_the_same_observed_data : concern_for_SimulationMetaData
   {
      private SimulationMetaData _sim1;
      private SimulationMetaData _sim2;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _sim1 = new SimulationMetaData {Id = "sim1", Name = "sim1"};
         _sim1.AddObservedData("ObsData1");
         _sim2 = new SimulationMetaData {Id = "sim2", Name = "sim2"};
         _sim2.AddObservedData("ObsData1");
      }

      [Observation]
      public void should_be_able_to_save_the_simulations()
      {
         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(_sim1);
            session.Save(_sim2);
            transaction.Commit();
         }
      }
   }

   public class When_saving_a_simulation_that_was_not_loaded_containing_chart : concern_for_SimulationMetaData
   {
      private SimulationMetaData _simulationMetaData;
      private ProjectMetaData _projectMetaData;
      private SimulationChartMetaData _simulationChartMetaData;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _projectMetaData = new ProjectMetaData {Name = "Project"};
         _simulationMetaData = new SimulationMetaData {Id = "Id", SimulationMode = SimulationMode.Individual, Name = "Sim"};
         _simulationChartMetaData = new SimulationChartMetaData {Id = "Chart", Name = "Chart"};
         _simulationMetaData.Charts.Add(_simulationChartMetaData);
         _projectMetaData.AddBuildingBlock(_simulationMetaData);

         //save project once
         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(_projectMetaData);
            transaction.Commit();
         }

         //then delete chart to simulation lazy loading
         _simulationMetaData.Charts.Clear();
         _simulationMetaData.Content.Data = null;
         _simulationMetaData.IsLoaded.ShouldBeFalse();

         //update project again. The charts should not be clear
         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            var projectMetaData = session.CreateCriteria<ProjectMetaData>().List<ProjectMetaData>().First();
            projectMetaData.UpdateFrom(_projectMetaData, session);
            transaction.Commit();
         }
      }

      [Observation]
      public void should_not_remove_the_charts()
      {
         using (var session = _sessionFactory.OpenSession())
         {
            var simulationFromDb = session.Get<SimulationMetaData>(_simulationMetaData.Id);
            simulationFromDb.Charts.Count.ShouldBeEqualTo(1);
         }
      }
   }
}