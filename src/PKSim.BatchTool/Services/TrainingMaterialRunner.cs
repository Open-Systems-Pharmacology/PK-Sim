using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.BatchTool.Services.TrainingMaterials;
using PKSim.Core.Batch;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;

namespace PKSim.BatchTool.Services
{
   public class TrainingMaterialRunner : IBatchRunner
   {
      private readonly IBatchLogger _logger;
      private readonly IReadOnlyList<ITrainingMaterialExercise> _trainingMaterialExercises;

      public TrainingMaterialRunner(IBatchLogger logger, IRepository<ITrainingMaterialExercise> trainingMaterialExercises)
      {
         _logger = logger;
         _trainingMaterialExercises = trainingMaterialExercises.All().ToList();
      }

      public async Task RunBatch(dynamic parameters)
      {
         string outputFolder = parameters.outputFolder;
         EnvironmentHelper.UserName = () => "PK-Sim Master";

         _logger.AddInSeparator($"Starting training material generation: {DateTime.Now.ToIsoFormat()}");
         var outputDirectory = new DirectoryInfo(outputFolder);
         if (outputDirectory.Exists)
         {
            outputDirectory.Delete(recursive: true);
            _logger.AddInfo($"Deleting folder '{outputFolder}'");
         }

         outputDirectory.Create();

         var begin = DateTime.UtcNow;
         await generateTrainingMaterial(outputFolder);
         var end = DateTime.UtcNow;
         var timeSpent = end - begin;

         _logger.AddInSeparator($"Finished training material generation in {timeSpent.ToDisplay()}'");
      }

      private async Task generateTrainingMaterial(string outputFolder)
      {
         foreach (var trainingMaterialExercise in _trainingMaterialExercises)
         {
            await trainingMaterialExercise.Generate(outputFolder);
         }
      }
   }
}