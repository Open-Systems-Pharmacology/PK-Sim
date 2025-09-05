using OSPSuite.Core.Qualification;
using PKSim.CLI.Core.Services;
using PKSim.Core.Services;

namespace PKSim.UI.Starter;

public static class SimulationTransferConstructor
{
   public static object CreateSimulationTransfer(string projectSnapshot)
   {
      var container = CLIApplicationStartup.Initialize();
      var projectSnapshotToSimulationTransferMapper = container.Resolve<IProjectSnapshotToSimulationTransferMapper>();

      return projectSnapshotToSimulationTransferMapper.MapFrom(projectSnapshot).transfer;
   }

   public static object CreateSimulationTransferAndExportInputs(string projectSnapshot, QualificationConfiguration qualificationConfiguration)
   {
      var container = CLIApplicationStartup.Initialize();
      var projectSnapshotToSimulationTransferMapper = container.Resolve<IProjectSnapshotToSimulationTransferMapper>();
      var qualificationInputTask = container.Resolve<IQualificationInputTask>();

      var (simulationTransfer, project) = projectSnapshotToSimulationTransferMapper.MapFrom(projectSnapshot);

      var inputMappings = qualificationInputTask.ExportInputs(project, qualificationConfiguration);
      
      return (simulationTransfer, inputMappings);
   }
}