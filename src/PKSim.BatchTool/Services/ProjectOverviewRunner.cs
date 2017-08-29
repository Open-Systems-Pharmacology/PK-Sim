using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Batch;
using PKSim.Presentation.Core;
using Compound = PKSim.Core.Model.Compound;

namespace PKSim.BatchTool.Services
{
   public class ProjectOverviewRunner : IBatchRunner<ProjectOverviewOptions>
   {
      private readonly IBatchLogger _logger;
      private readonly IWorkspacePersistor _workspacePersistor;
      private readonly IWorkspace _workspace;

      public ProjectOverviewRunner(IBatchLogger logger, IWorkspacePersistor workspacePersistor, IWorkspace workspace)
      {
         _logger = logger;
         _workspacePersistor = workspacePersistor;
         _workspace = workspace;
      }

      public Task RunBatch(ProjectOverviewOptions options)
      {
         return Task.Run(() =>
         {
            string inputFolder = options.InputFolder;

            clear();

            var inputDirectory = new DirectoryInfo(inputFolder);
            if (!inputDirectory.Exists)
               throw new ArgumentException($"Input folder '{inputFolder}' does not exist");

            var allProjectFiles = inputDirectory.GetFiles(CoreConstants.Filter.PROJECT_FILTER, SearchOption.AllDirectories);
            if (allProjectFiles.Length == 0)
               throw new ArgumentException($"No project file found in '{inputFolder}'");

            var outputFile = Path.Combine(inputFolder, "output.json");

            _logger.AddInSeparator($"Starting project overview run for {allProjectFiles.Length} projects: {DateTime.Now.ToIsoFormat()}");

            var begin = DateTime.UtcNow;

            var allProjects = new AllProjects {InputFolder = inputFolder};
            foreach (var projectFile in allProjectFiles)
            {
               allProjects.Projects.Add(addProjectInfo(projectFile));
            }

            exportAllProjects(allProjects, outputFile);
            var end = DateTime.UtcNow;
            var timeSpent = end - begin;

            _logger.AddInSeparator($"Finished project overview run for {allProjectFiles.Length} projects in {timeSpent.ToDisplay()}'");
         });
      }

      private ProjectInfo addProjectInfo(FileInfo projectFile)
      {
         var projectInfo = new ProjectInfo {FullPath = projectFile.FullName, Name = projectFile.Name};
         _logger.AddInSeparator($"Loading project file '{projectFile.FullName}'");

         _workspacePersistor.LoadSession(_workspace, projectFile.FullName);
         var project = _workspace.Project;

         projectInfo.CompoundNames = project.All<Compound>().AllNames().ToList();


         foreach (var observedData in project.AllObservedData)
         {
            projectInfo.ObservedDataList.Add(observedDataInfoFrom(observedData));
         }

         _workspace.CloseProject();

         return projectInfo;
      }

      private void exportAllProjects(AllProjects allProjects, string outputFile)
      {
         // serialize JSON directly to a file
         using (var file = File.CreateText(outputFile))
         {
            var serializer = new JsonSerializer();
            serializer.Serialize(file, allProjects);
         }
      }

      private ObservedDataInfo observedDataInfoFrom(DataRepository observedData)
      {
         var observedDataInfo = new ObservedDataInfo {Name = observedData.Name};
         observedData.ExtendedProperties.Each(prop => { observedDataInfo.MetaData.Add($"{prop.DisplayName}={prop.ValueAsObject}"); });
         return observedDataInfo;
      }

      private void clear()
      {
         _logger.Clear();
      }

      internal class AllProjects
      {
         public List<ProjectInfo> Projects { get; set; } = new List<ProjectInfo>();
         public string InputFolder { get; set; }
      }

      internal class ProjectInfo
      {
         public string Name { get; set; }
         public string FullPath { get; set; }
         public List<string> CompoundNames { get; set; } = new List<string>();
         public List<ObservedDataInfo> ObservedDataList { get; set; } = new List<ObservedDataInfo>();
      }

      internal class ObservedDataInfo
      {
         public string Name { get; set; }
         public List<string> MetaData { get; set; } = new List<string>();
      }
   }
}