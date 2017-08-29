using System.Threading.Tasks;
using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core.Batch;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;

namespace PKSim.BatchTool.Presenters
{
   public interface IGenerateProjectOverviewPresenter : IBatchPresenter<ProjectOverviewOptions>
   {
      void SelectInputFolder();
   }

   public class GenerateProjectOverviewPresenter : BatchPresenter<IGenerateProjectOverviewView, IGenerateProjectOverviewPresenter, ProjectOverviewRunner, ProjectOverviewOptions>, IGenerateProjectOverviewPresenter
   {

      public GenerateProjectOverviewPresenter(IGenerateProjectOverviewView view, ProjectOverviewRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IBatchLogger batchLogger) : base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
      }

      public virtual void SelectInputFolder()
      {
         string inputFolder = _dialogCreator.AskForFolder("Select input folder where projects are located", Constants.DirectoryKey.PROJECT);
         if (string.IsNullOrEmpty(inputFolder)) return;
         _startOptions.InputFolder = inputFolder;
      }

   }
}