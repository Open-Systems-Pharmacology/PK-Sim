using System.Threading.Tasks;
using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core.Batch;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;

namespace PKSim.BatchTool.Presenters
{
   public interface IGenerateTrainingMaterialPresenter : IBatchPresenter, IPresenter<IGenerateTrainingMaterialView>
   {
      void SelectOutputFolder();
   }

   public class GenerateTrainingMaterialPresenter : BatchPresenter<IGenerateTrainingMaterialView, IGenerateTrainingMaterialPresenter, TrainingMaterialRunner>, IGenerateTrainingMaterialPresenter
   {
      private OutputBatchDTO _dto;

      public GenerateTrainingMaterialPresenter(IGenerateTrainingMaterialView view, TrainingMaterialRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IBatchLogger batchLogger)
         : base(view, batchRunner, dialogCreator, logPresenter,batchLogger)
      {
      }

      public virtual void SelectOutputFolder()
      {
         string outputFolder = _dialogCreator.AskForFolder("Select output folder where files will be generated", Constants.DirectoryKey.TEMPLATE);
         if (string.IsNullOrEmpty(outputFolder)) return;
         _dto.OutputFolder = outputFolder;
      }

      protected override Task StartBatch()
      {
         return _batchRunner.RunBatch(
            new
            {
               outputFolder = _dto.OutputFolder
            });
      }

      public override Task InitializeWith(BatchStartOptions startOptions)
      {
         _dto = new OutputBatchDTO();
         _view.BindTo(_dto);
         _view.Display();
         _startedFromCommandLine = false;
         return base.InitializeWith(startOptions);
      }
   }
}