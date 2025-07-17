using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using static OSPSuite.Assets.Error;
using static PKSim.Assets.PKSimConstants.Error;
using Project = PKSim.Core.Snapshots.Project;
using Simulation = PKSim.Core.Snapshots.Simulation;

namespace PKSim.CLI.Core.Services
{
   public class QualificationRunner : QualificationRunner<Project, PKSimProject>
   {
      private readonly ICoreWorkspace _workspace;
      private readonly IWorkspacePersistor _workspacePersistor;
      private readonly IExportSimulationRunner _exportSimulationRunner;
      private readonly IMarkdownReporterTask _markdownReporterTask;

      public QualificationRunner(ISnapshotTask snapshotTask,
         IJsonSerializer jsonSerializer,
         ICoreWorkspace workspace,
         IWorkspacePersistor workspacePersistor,
         IExportSimulationRunner exportSimulationRunner,
         IDataRepositoryExportTask dataRepositoryExportTask,
         IMarkdownReporterTask markdownReporterTask,
         IOSPSuiteLogger logger
      ) : base(logger, dataRepositoryExportTask, jsonSerializer, snapshotTask)
      {
         _workspace = workspace;
         _workspacePersistor = workspacePersistor;
         _exportSimulationRunner = exportSimulationRunner;
         _markdownReporterTask = markdownReporterTask;
      }

      protected override void SaveProjectContext(string projectFile) => _workspacePersistor.SaveSession(_workspace, projectFile);

      protected override void LoadProjectContext(PKSimProject project) => _workspace.Project = project;

      protected override void ValidateInputs(Project snapshotProject, QualificationConfiguration configuration)
      {
         configuration.Inputs?.Each(x =>
         {
            var buildingBlock = snapshotProject.BuildingBlockByTypeAndName(x.Type, x.Name);
            if (buildingBlock == null)
               throw new QualificationRunException(CannotFindBuildingBlockInSnapshot(x.Type.ToString(), x.Name, snapshotProject.Name));
         });
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

      protected override Task ExportToMarkdown(object buildingBlock, string fileFullPath, int? inputSectionLevel)
      {
         return _markdownReporterTask.ExportToMarkdown(buildingBlock, fileFullPath, inputSectionLevel);
      }
   }
}