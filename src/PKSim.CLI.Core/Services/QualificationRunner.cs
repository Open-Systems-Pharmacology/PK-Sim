using OSPSuite.CLI.Core.RunOptions;
using OSPSuite.CLI.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Qualification;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OSPSuite.Assets.Error;
using Project = PKSim.Core.Snapshots.Project;
using Simulation = PKSim.Core.Snapshots.Simulation;

namespace PKSim.CLI.Core.Services
{
   public class QualificationRunner : QualificationRunner<Project, PKSimProject, QualificationRunOptions>
   {
      private readonly ICoreWorkspace _workspace;
      private readonly IWorkspacePersistor _workspacePersistor;
      private readonly IExportSimulationRunner _exportSimulationRunner;
      private readonly IQualificationInputTask _qualificationInputTask;

      public QualificationRunner(ISnapshotTask snapshotTask,
         IJsonSerializer jsonSerializer,
         ICoreWorkspace workspace,
         IWorkspacePersistor workspacePersistor,
         IExportSimulationRunner exportSimulationRunner,
         IDataRepositoryExportTask dataRepositoryExportTask,
         IQualificationInputTask qualificationInputTask,
         IOSPSuiteLogger logger
      ) : base(logger, dataRepositoryExportTask, jsonSerializer, snapshotTask)
      {
         _workspace = workspace;
         _workspacePersistor = workspacePersistor;
         _exportSimulationRunner = exportSimulationRunner;
         _qualificationInputTask = qualificationInputTask;
      }

      protected override void SaveProjectContext(string projectFile) => _workspacePersistor.SaveSession(_workspace, projectFile);

      protected override void LoadProjectContext(PKSimProject project) => _workspace.Project = project;

      protected override void ValidateInputs(Project snapshotProject, QualificationConfiguration configuration) => 
         _qualificationInputTask.ValidateInputs(snapshotProject, configuration);

      protected override async Task<(PKSimProject, InputMapping[])> LoadProjectAndExportInputs(QualificationRunOptions runOptions, Project snapshot, QualificationConfiguration config)
      {
         var project = await _snapshotTask.LoadProjectFromSnapshotAsync(snapshot, runOptions.Run);
         
         return (project, _qualificationInputTask.ExportInputs(project, config));
      }

      protected override SimulationExportMode ExportMode(QualificationRunOptions runOptions) => runOptions.Run ? SimulationExportMode.Xml | SimulationExportMode.Csv : SimulationExportMode.Pkml;
      
      protected override string ProjectExtension => CoreConstants.Filter.PROJECT_EXTENSION;

      protected override IEnumerable<PlotMapping> RetrievePlotDefinitionsForSimulation(SimulationPlot simulationPlot, Project snapshotProject)
      {
         var simulationName = simulationPlot.Simulation;
         var simulation = simulationFrom(snapshotProject, simulationName);

         return simulation.Analyses.Select(plot => new PlotMapping
         {
            Plot = plot,
            SectionId = simulationPlot.SectionId,
            SectionReference = simulationPlot.SectionReference,
            Simulation = simulationName,
            Project = snapshotProject.Name
         });
      }

      protected override Task<SimulationMapping[]> ExportSimulationsIn(PKSimProject project, ExportRunOptions exportRunOptions)
      {
         return _exportSimulationRunner.ExportSimulationsIn(project, exportRunOptions);
      }

      protected override object BuildingBlockBy(PKSimProject project, Input input)
      {
         return project.BuildingBlockByName(input.Name, input.Type);
      }

      protected override async Task SwapBuildingBlockIn(Project projectSnapshot, BuildingBlockSwap buildingBlockSwap)
      {
         var (buildingBlockType, name, snapshotPath) = buildingBlockSwap;
         var referenceSnapshot = await SnapshotProjectFromFile(snapshotPath);
         var typeDisplay = buildingBlockType.ToString();

         var buildingBlockToUse = referenceSnapshot.BuildingBlockByTypeAndName(buildingBlockType, name);
         if (buildingBlockToUse == null)
            throw new QualificationRunException(CannotFindBuildingBlockInSnapshot(typeDisplay, name, referenceSnapshot.Name));

         var buildingBlock = projectSnapshot.BuildingBlockByTypeAndName(buildingBlockType, name);
         if (buildingBlock == null)
            throw new QualificationRunException(CannotFindBuildingBlockInSnapshot(typeDisplay, name, projectSnapshot.Name));

         projectSnapshot.Swap(buildingBlockToUse);
      }

      protected override async Task SwapSimulationParametersIn(Project projectSnapshot, SimulationParameterSwap simulationParameter)
      {
         var (parameterPath, simulationName, snapshotPath) = simulationParameter;
         var referenceSnapshot = await SnapshotProjectFromFile(snapshotPath);

         var referenceSimulation = simulationFrom(referenceSnapshot, simulationName);

         var referenceParameter = referenceSimulation.ParameterByPath(parameterPath);
         if (referenceParameter == null)
            throw new QualificationRunException(CannotFindSimulationParameterInSnapshot(parameterPath, simulationName, referenceSnapshot.Name));

         simulationParameter.TargetSimulations?.Each(targetSimulationName =>
         {
            var targetSimulation = simulationFrom(projectSnapshot, targetSimulationName);
            targetSimulation.AddOrUpdate(referenceParameter);
         });
      }

      private Simulation simulationFrom(Project snapshotProject, string simulationName)
      {
         var referenceSimulation = snapshotProject.Simulations?.FindByName(simulationName);
         return referenceSimulation ?? throw new QualificationRunException(CannotFindSimulationInSnapshot(simulationName, snapshotProject.Name));
      }
   }
}