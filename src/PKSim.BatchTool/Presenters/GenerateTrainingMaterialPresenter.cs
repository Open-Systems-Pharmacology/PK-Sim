﻿using System.Threading.Tasks;
using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core.Batch;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;

namespace PKSim.BatchTool.Presenters
{
   public interface IGenerateTrainingMaterialPresenter : IBatchPresenter<TrainingMaterialsOptions>
   {
      void SelectOutputFolder();
   }

   public class GenerateTrainingMaterialPresenter : BatchPresenter<IGenerateTrainingMaterialView, IGenerateTrainingMaterialPresenter, TrainingMaterialRunner, TrainingMaterialsOptions>, IGenerateTrainingMaterialPresenter
   {

      public GenerateTrainingMaterialPresenter(IGenerateTrainingMaterialView view, TrainingMaterialRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IBatchLogger batchLogger)
         : base(view, batchRunner, dialogCreator, logPresenter,batchLogger)
      {
      }

      public virtual void SelectOutputFolder()
      {
         string outputFolder = _dialogCreator.AskForFolder("Select output folder where files will be generated", Constants.DirectoryKey.TEMPLATE);
         if (string.IsNullOrEmpty(outputFolder)) return;
         _startOptions.OutputFolder = outputFolder;
      }

   }
}