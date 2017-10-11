﻿using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using PKSim.BatchTool.DTO;
using PKSim.BatchTool.Views;
using PKSim.CLI.Core.RunOptions;
using PKSim.Core;

namespace PKSim.BatchTool.Presenters
{
   public interface ISingleFolderSnapshotPresenter : ISnapshotPresenter, IPresenter<ISingleFolderSnapshotView>
   {
      void SelectInputFolder();
      void SelectOutputFolder();
   }

   public class SingleFolderSnapshotPresenter : AbstractPresenter<ISingleFolderSnapshotView, ISingleFolderSnapshotPresenter>, ISingleFolderSnapshotPresenter
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly SnapshotSingleFolderDTO _dto;

      public SingleFolderSnapshotPresenter(ISingleFolderSnapshotView view, IDialogCreator dialogCreator) : base(view)
      {
         _dialogCreator = dialogCreator;
         _dto = new SnapshotSingleFolderDTO();
         _view.BindTo(_dto);
      }

      public void SelectInputFolder()
      {
         var inputFolder = _dialogCreator.AskForFolder("Select input folder", CoreConstants.DirectoryKey.BATCH);
         if (string.IsNullOrEmpty(inputFolder))
            return;

         _dto.InputFolder = inputFolder;
      }

      public void SelectOutputFolder()
      {
         var outputFolder = _dialogCreator.AskForFolder("Select output folder where results will be exported", CoreConstants.DirectoryKey.BATCH);
         if (string.IsNullOrEmpty(outputFolder))
            return;

         _dto.OutputFolder = outputFolder;
         _dto.LogFileFullPath = CoreConstants.DefaultBatchLogFullPath(_dto.OutputFolder);
      }

      public SnapshotRunOptions RunOptions
      {
         get
         {
            return new SnapshotRunOptions
            {
               InputFolder = _dto.InputFolder,
               OutputFolder = _dto.OutputFolder,
               LogFileFullPath = _dto.LogFileFullPath,
               ExportMode = _dto.ExportMode
            };
         }
      }

      public void AdjustViewHeight()
      {
         _view.AdjustHeight();
      }
   }
}