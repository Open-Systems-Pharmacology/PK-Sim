using System.Linq;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Serialization.Exchange;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ProjectSnapshotToSimulationMapper : ContextForSimulationIntegration<IProjectSnapshotToModuleMapper, IndividualSimulation>
   {
      protected Compound _compound;
      protected Individual _individual;
      protected Protocol _protocol;
      protected ICoreWorkspace _workspace;
      protected DataRepository _observedData;
      protected IModelCoreSimulationSnapshotUpdater _modelCoreSimulationSnapshotUpdater;
      private ISimulationToModelCoreSimulationMapper _simulationMapper;
      private ISimulationConfigurationTask _simulationConfigurationTask;
      protected IModelCoreSimulation _moBiSimulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _simulationConfigurationTask = IoC.Resolve<ISimulationConfigurationTask>();
         _simulationMapper = IoC.Resolve<ISimulationToModelCoreSimulationMapper>();

         _workspace = IoC.Resolve<ICoreWorkspace>();
         _modelCoreSimulationSnapshotUpdater = IoC.Resolve<IModelCoreSimulationSnapshotUpdater>();
         _workspace.Project = new PKSimProject();

         _observedData = DomainHelperForSpecs.IndividualSimulationDataRepositoryFor("S").WithName("obs data");

         _workspace.Project.AddBuildingBlock(_compound);
         _workspace.Project.AddBuildingBlock(_individual);
         _workspace.Project.AddBuildingBlock(_protocol);
         _workspace.Project.AddObservedData(_observedData);
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol) as IndividualSimulation;
         _simulation.AddUsedObservedData(UsedObservedData.From(_observedData));
         _workspace.Project.AddBuildingBlock(_simulation);

         var configuration = _simulationConfigurationTask.CreateFor(_simulation, shouldValidate: true, createAgingDataInSimulation: false);
         _moBiSimulation = _simulationMapper.MapFrom(_simulation, configuration, shouldCloneModel: true);

         _modelCoreSimulationSnapshotUpdater.AddSnapshotsToModelCoreSimulation(_simulation, _moBiSimulation);
      }
   }

   public class When_mapping_snapshot_string_to_model_core_simulation : concern_for_ProjectSnapshotToSimulationMapper
   {
      private Module _module;

      protected override void Because()
      {
         (_module, _) = sut.MapFrom(_moBiSimulation.Configuration.ModuleConfigurations.Single().Module.Snapshot);
      }

      [Observation]
      public void the_model_core_simulation_module_has_a_snapshot()
      {
         _module.Snapshot.ShouldNotBeNull();
      }
   }
}