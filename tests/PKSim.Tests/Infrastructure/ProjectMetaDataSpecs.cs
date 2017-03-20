using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using NHibernate;
using PKSim.Core;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ProjectMetaData : ContextSpecification<ProjectMetaData>
   {
      protected ProjectMetaData _sourceProject;
      protected BuildingBlockMetaData _childInAllProjects;
      protected BuildingBlockMetaData _childInSourceProject;
      protected BuildingBlockMetaData _childInSutProject;
      protected BuildingBlockMetaData _childWithCommonIdInSource;
      protected BuildingBlockMetaData _childWithCommonIdInSut;

      protected override void Context()
      {
         sut = new ProjectMetaData {Name = "toto"};
         _sourceProject = new ProjectMetaData {Name = "tata"};
         _childInAllProjects = new CompoundMetaData().WithIdentifier("1");
         _childInSourceProject = new CompoundMetaData().WithIdentifier("2");
         _childInSutProject = new CompoundMetaData().WithIdentifier("3");
         _childWithCommonIdInSource = new IndividualMetaData {Description = "tralalal"}.WithIdentifier("4");
         _childWithCommonIdInSut = new IndividualMetaData {Description = "asdsadsad"}.WithIdentifier("4");

         sut.AddBuildingBlock(_childInAllProjects);
         _sourceProject.AddBuildingBlock(_childInAllProjects);

         _sourceProject.AddBuildingBlock(_childInSourceProject);
         sut.AddBuildingBlock(_childInSutProject);

         _sourceProject.AddBuildingBlock(_childWithCommonIdInSource);
         sut.AddBuildingBlock(_childWithCommonIdInSut);
      }
   }

   public class When_being_updated_from_a_source_project : concern_for_ProjectMetaData
   {
      private ISession _session;

      protected override void Context()
      {
         base.Context();
         _session = A.Fake<ISession>();
      }

      protected override void Because()
      {
         sut.UpdateFrom(_sourceProject, _session);
      }

      [Observation]
      public void should_add_to_its_children_all_newly_added_child_from_the_source_project()
      {
         sut.BuildingBlocks.Contains(_childInSourceProject).ShouldBeTrue();
      }

      [Observation]
      public void should_remove_from_his_children_all_deleted_child_from_the_source_project()
      {
         sut.BuildingBlocks.Contains(_childInSutProject).ShouldBeFalse();
      }

      [Observation]
      public void should_call_the_update_from_the_source_child_to_the_current_child()
      {
         _childWithCommonIdInSut.Description.ShouldBeEqualTo(_childWithCommonIdInSource.Description);
      }

      [Observation]
      public void should_update_the_project_properties()
      {
         sut.Name.ShouldBeEqualTo(_sourceProject.Name);
      }
   }

   public abstract class concern_for_ProjectMetaDataIntegration : ContextSpecificationWithSerializationDatabase<ProjectMetaData>
   {
      protected SimulationMetaData _simulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = new ProjectMetaData {Name = "Project", Id = 1};

         _simulation = new SimulationMetaData {Id = "SimulationMetaDataId", Name = "tralala"};
         _simulation.SimulationResults = DomainHelperForSpecs.CreateSimulationResults();
         sut.BuildingBlocks.Add(_simulation);
      }
   }

   public class When_removing_a_simulation_with_results_from_a_project_that_was_already_saved : concern_for_ProjectMetaDataIntegration
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(sut);
            transaction.Commit();
         }


         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            var projectFromDb = session.Get<ProjectMetaData>(sut.Id);
            var simulation = session.Get<SimulationMetaData>(_simulation.Id);
            projectFromDb.BuildingBlocks.Remove(simulation);
            transaction.Commit();
         }
      }

      [Observation]
      public void should_be_able_to_delete_the_simulation()
      {
         using (var session = _sessionFactory.OpenSession())
         {
            var projectFromDb = session.Get<ProjectMetaData>(sut.Id);
            projectFromDb.BuildingBlocks.Count.ShouldBeEqualTo(0);
         }
      }

      [Observation]
      public void should_have_deleted_the_results_of_the_simulation_as_well()
      {
         using (var session = _sessionFactory.OpenSession())
         {
            var simulation = session.Get<SimulationMetaData>(_simulation.Id);
            simulation.ShouldBeNull();

            var data = session.Get<SimulationResults>(_simulation.SimulationResults.Id);
            data.ShouldBeNull();
         }
      }
   }

   public class When_adding_an_observed_data_to_a_project : concern_for_ProjectMetaDataIntegration
   {
      private ObservedDataMetaData _observedDataMetaData;

      public override void GlobalContext()
      {
         base.GlobalContext();

         sut = new ProjectMetaData {Name = "Project", Id = 1};
         _observedDataMetaData = new ObservedDataMetaData {Id = "DataRepo", DataRepository = new DataRepositoryMetaData {Id = "DataRepo", Name = "DataRepo"}};
         sut.AddObservedData(_observedDataMetaData);

         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(sut);
            transaction.Commit();
         }
      }

      [Observation]
      public void should_be_able_to_retrieve_the_data_repositories()
      {
         using (var session = _sessionFactory.OpenSession())
         {
            var projectFromDb = session.Get<ProjectMetaData>(sut.Id);
            projectFromDb.AllObservedData.Count.ShouldBeEqualTo(1);
            var observedData = session.Get<ObservedDataMetaData>(_observedDataMetaData.Id);
            observedData.ShouldNotBeNull();
         }
      }
   }

   public class When_deleting_an_observed_data_from_a_project : concern_for_ProjectMetaDataIntegration
   {
      private ObservedDataMetaData _observedDataMetaData;

      public override void GlobalContext()
      {
         base.GlobalContext();

         sut = new ProjectMetaData {Name = "Project", Id = 1};
         _observedDataMetaData = new ObservedDataMetaData {Id = "DataRepo", DataRepository = new DataRepositoryMetaData {Id = "DataRepo", Name = "DataRepo"}};
         sut.AddObservedData(_observedDataMetaData);

         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(sut);
            transaction.Commit();
         }

         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            var projectFromDb = session.Get<ProjectMetaData>(sut.Id);
            projectFromDb.AllObservedData.Clear();
            transaction.Commit();
         }
      }

      [Observation]
      public void should_be_able_to_delete_the_data_repositories()
      {
         using (var session = _sessionFactory.OpenSession())
         {
            var projectFromDb = session.Get<ProjectMetaData>(sut.Id);
            projectFromDb.AllObservedData.Count.ShouldBeEqualTo(0);
            var observedData = session.Get<ObservedDataMetaData>(_observedDataMetaData.Id);
            observedData.ShouldBeNull();

            var dataRepository = session.Get<DataRepositoryMetaData>(_observedDataMetaData.DataRepository.Id);
            dataRepository.ShouldBeNull();
         }
      }
   }
}