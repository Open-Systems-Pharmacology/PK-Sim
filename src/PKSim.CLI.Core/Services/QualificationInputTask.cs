using OSPSuite.Core.Qualification;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static OSPSuite.Assets.Error;
using static OSPSuite.Core.Domain.Constants.Filter;
using Project = PKSim.Core.Snapshots.Project;

namespace PKSim.CLI.Core.Services
{
   public interface IQualificationInputTask
   {
      InputMapping[] ExportInputs(PKSimProject project, QualificationConfiguration configuration);

      void ValidateInputs(Project snapshotProject, QualificationConfiguration configuration);
   }
   
   public class QualificationInputTask : IQualificationInputTask
   {
      private readonly IOSPSuiteLogger _logger;
      private readonly IMarkdownReporterTask _markdownReporterTask;

      public QualificationInputTask(IOSPSuiteLogger logger, 
         IMarkdownReporterTask markdownReporterTask)
      {
         _logger = logger;
         _markdownReporterTask = markdownReporterTask;
      }

      public InputMapping[] ExportInputs(PKSimProject project, QualificationConfiguration configuration)
      {
         if (configuration.Inputs == null)
            return Array.Empty<InputMapping>();
         //            return Task.FromResult(Array.Empty<InputMapping>());

         //TODO Enable parallel runs once https://github.com/Open-Systems-Pharmacology/OSPSuite.Utility/issues/26 is fixed
         //  return Task.WhenAll(configuration.Inputs.Select(x => exportInput(project, configuration, x)));

         return configuration.Inputs.Where(x => string.Equals(x.Project, project.Name)).Select(x => exportInput(project, configuration, x)).ToArray();
      }

      public void ValidateInputs(Project snapshotProject, QualificationConfiguration configuration)
      {
         configuration.Inputs?.Each(x =>
         {
            var buildingBlock = snapshotProject.BuildingBlockByTypeAndName(x.Type, x.Name);
            if (buildingBlock == null)
               throw new QualificationRunException(CannotFindBuildingBlockInSnapshot(x.Type.ToString(), x.Name, snapshotProject.Name));
         });
      }

      private InputMapping exportInput(PKSimProject project, QualificationConfiguration configuration, Input input)
      {
         var buildingBlock = project.BuildingBlockByName(input.Name, input.Type);

         var inputsFolder = configuration.InputsFolder;
         var projectName = FileHelper.RemoveIllegalCharactersFrom(project.Name);
         var buildingBlockName = FileHelper.RemoveIllegalCharactersFrom(input.Name);
         var targetFolder = Path.Combine(inputsFolder, projectName, input.Type.ToString());
         DirectoryHelper.CreateDirectory(targetFolder);

         var fileFullPath = Path.Combine(targetFolder, $"{buildingBlockName}{MARKDOWN_EXTENSION}");

         exportToMarkdown(buildingBlock, fileFullPath, input.SectionLevel).Wait();
         _logger.AddDebug($"Input data for {input.Type} '{input.Name}' exported to '{fileFullPath}'", project.Name);

         return new InputMapping
         {
            SectionId = input.SectionId,
            SectionReference = input.SectionReference,
            Path = relativePath(fileFullPath, configuration.OutputFolder)
         };
      }

      private Task exportToMarkdown(object buildingBlock, string fileFullPath, int? inputSectionLevel) => _markdownReporterTask.ExportToMarkdown(buildingBlock, fileFullPath, inputSectionLevel);

      private string relativePath(string path, string relativeTo) => FileHelper.CreateRelativePath(path, relativeTo, useUnixPathSeparator: true);
   }
}
