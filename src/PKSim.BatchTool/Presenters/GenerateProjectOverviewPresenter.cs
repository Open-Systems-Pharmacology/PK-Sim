using System.Threading.Tasks;
using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core.Batch;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;

namespace PKSim.BatchTool.Presenters
{
   public interface IGenerateProjectOverviewPresenter : IBatchPresenter
   {
      void SelectInputFolder();
   }

   public class GenerateProjectOverviewPresenter : BatchPresenter<IGenerateProjectOverviewView, IGenerateProjectOverviewPresenter, ProjectOverviewRunner>, IGenerateProjectOverviewPresenter
   {
      private OutputBatchDTO _dto;

      public GenerateProjectOverviewPresenter(IGenerateProjectOverviewView view, ProjectOverviewRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IBatchLogger batchLogger) : base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
      }

      public virtual void SelectInputFolder()
      {
         string inputFolder = _dialogCreator.AskForFolder("Select input folder where projects are located", Constants.DirectoryKey.PROJECT);
         if (string.IsNullOrEmpty(inputFolder)) return;
         _dto.OutputFolder = inputFolder;
      }

      protected override Task StartBatch()
      {
         return _batchRunner.RunBatch(
            new
            {
               inputFolder = _dto.OutputFolder
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