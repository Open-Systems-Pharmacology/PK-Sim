using System.Linq;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Serialization.Exchange;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ProjectSnapshotToSimulationMapper : ContextForSimulationIntegration<IProjectSnapshotToSimulationTransferMapper, IndividualSimulation>
   {
      protected string _snapshotString;
      protected Compound _compound;
      protected Individual _individual;
      protected Protocol _protocol;
      protected ICoreWorkspace _workspace;
      protected DataRepository _observedData;
      protected ISimulationToProjectSnapshotMapper _simulationMapper;
      protected SimulationTransfer _simulationTransfer;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();

         _workspace = IoC.Resolve<ICoreWorkspace>();
         _simulationMapper = IoC.Resolve<ISimulationToProjectSnapshotMapper>();
         _workspace.Project = new PKSimProject();

         _observedData = DomainHelperForSpecs.IndividualSimulationDataRepositoryFor("S").WithName("obs data");

         _workspace.Project.AddBuildingBlock(_compound);
         _workspace.Project.AddBuildingBlock(_individual);
         _workspace.Project.AddBuildingBlock(_protocol);
         _workspace.Project.AddObservedData(_observedData);
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol) as IndividualSimulation;
         _simulation.AddUsedObservedData(UsedObservedData.From(_observedData));
         _workspace.Project.AddBuildingBlock(_simulation);

         _snapshotString = _simulationMapper.MapFrom(_simulation);
      }
   }

   public class When_mapping_snapshot_string_to_model_core_simulation : concern_for_ProjectSnapshotToSimulationMapper
   {
      protected override void Because()
      {
         (_simulationTransfer, _) = sut.MapFrom(_snapshotString);
      }

      [Observation]
      public void the_model_core_simulation_has_a_module_configuration()
      {
         _simulationTransfer.Simulation.Configuration.ModuleConfigurations.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void the_model_core_simulation_module_configuration_has_a_snapshot()
      {
         _simulationTransfer.Simulation.Configuration.ModuleConfigurations.First().Module.Snapshot.ShouldNotBeNull();
      }
   }
}