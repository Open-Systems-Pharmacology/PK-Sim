using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationModelCreator : ContextSpecification<ISimulationModelCreator>
   {
      protected ISimulationConfigurationTask _simulationConfigurationTask;
      protected IModelConstructor _modelConstructor;
      protected IContainerTask _containerTask;

      protected override void Context()
      {
         _simulationConfigurationTask = A.Fake<ISimulationConfigurationTask>();
         _modelConstructor = A.Fake<IModelConstructor>();
         _containerTask = A.Fake<IContainerTask>();
         sut = new SimulationModelCreator(
            _simulationConfigurationTask,
            _modelConstructor,
            A.Fake<IParameterIdUpdater>(),
            A.Fake<ISimulationSettingsFactory>(),
            A.Fake<Services.ISimulationPersistableUpdater>(),
            A.Fake<Services.ISimulationConfigurationValidator>(),
            A.Fake<IEntityPathResolver>(),
            _containerTask,
            A.Fake<IOverwriteParameterSetApplicationTask>());
      }
   }

   public class When_creating_the_model_for_a_simulation : concern_for_SimulationModelCreator
   {
      private Simulation _simulation;
      private SimulationConfiguration _configuration;
      private bool? _showProgressDuringConstruction;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         _configuration = new SimulationConfiguration();
         A.CallTo(() => _simulationConfigurationTask.CreateFor(_simulation, A<bool>._, A<bool>._)).Returns(_configuration);

         A.CallTo(() => _modelConstructor.CreateModelFrom(_configuration, A<string>._)).ReturnsLazily(() =>
         {
            _showProgressDuringConstruction = _configuration.ShowProgress;
            return new CreationResult(A.Fake<IModel>(), A.Fake<SimulationBuilder>());
         });

         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(A<IContainer>._)).Returns(new PathCache<IParameter>(new EntityPathResolverForSpecs()));
      }

      protected override void Because()
      {
         sut.CreateModelFor(_simulation);
      }

      //parallel snapshot loading relies on the default: concurrent constructions must not interleave the core progress stream
      [Observation]
      public void should_construct_the_model_without_core_progress()
      {
         _showProgressDuringConstruction.ShouldBeEqualTo(false);
      }
   }
}
