using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Converter.v5_2;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization.Xml;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SerializationContextFactory : ContextSpecification<ISerializationContextFactory>
   {
      protected ISerializationDimensionFactory _dimensionFactory;
      protected IObjectBaseFactory _objectBaseFactory;
      protected ICloneManagerForModel _cloneManagerForModel;
      protected IContainer _container;
      protected IPKSimProjectRetriever _projectRetriever;
      protected PKSimProject _project;

      protected override void Context()
      {
         _dimensionFactory = A.Fake<ISerializationDimensionFactory>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _cloneManagerForModel = A.Fake<ICloneManagerForModel>();
         _container = A.Fake<IContainer>();

         sut = new SerializationContextFactory(_dimensionFactory, _objectBaseFactory, _container, _cloneManagerForModel);

         _projectRetriever = A.Fake<IPKSimProjectRetriever>();

         A.CallTo(() => _container.Resolve<IPKSimProjectRetriever>()).Returns(_projectRetriever);

         _project = new PKSimProject();
         A.CallTo(() => _projectRetriever.Current).Returns(_project);
      }
   }

   public class When_creating_a_serialization_context_in_an_empty_project : concern_for_SerializationContextFactory
   {
      private SerializationContext _result;

      protected override void Because()
      {
         _result = sut.Create();
      }

      [Observation]
      public void should_return_a_serialization_context_without_any_data_repositories_or_simulation_registered()
      {
         _result.IdRepository.All().ShouldBeEmpty();
         _result.Repositories.ShouldBeEmpty();
      }
   }

   public class When_creating_a_serialization_context_based_on_a_parent_serialization_context : concern_for_SerializationContextFactory
   {
      private SerializationContext _result;
      private DataRepository _dataRepo1;
      private IWithId _simmulation;

      protected override void Context()
      {
         base.Context();
         _dataRepo1 = new DataRepository("OBS1");
         _simmulation = new IndividualSimulation().WithId("ID");
      }

      protected override void Because()
      {
         _result = sut.Create(new []{_dataRepo1, }, new []{_simmulation, });
      }

      [Observation]
      public void should_add_the_repositories_defined_in_the_parent_context()
      {
         _result.Repositories.ShouldContain(_dataRepo1);
      }

      [Observation]
      public void should_register_the_object_registered_in_the_parent_context()
      {
         _result.IdRepository.All().ShouldContain(_simmulation);
      }
   }

   public class When_creating_a_serialization_context_with_a_defined_project : concern_for_SerializationContextFactory
   {
      private SerializationContext _result;
      private DataRepository _observedData;
      private IndividualSimulation _simmulation;
      private DataRepository _simulationResults;

      protected override void Context()
      {
         base.Context();
         _observedData = new DataRepository("OBS1");
         _simulationResults = DomainHelperForSpecs.ObservedData("RES");
         _simmulation = new IndividualSimulation().WithId("ID");

         _simmulation.DataRepository = _simulationResults;
         _project.AddBuildingBlock(_simmulation);
         _project.AddObservedData(_observedData);
      }

      protected override void Because()
      {
         _result = sut.Create();
      }

      [Observation]
      public void should_add_the_repositories_defined_in_the_parent_context()
      {
         _result.Repositories.ShouldContain(_observedData, _simulationResults);
      }

      [Observation]
      public void should_register_the_object_registered_in_the_parent_context()
      {
         _result.IdRepository.All().ShouldContain(_simmulation, _observedData, _simulationResults);
      }
   }
}