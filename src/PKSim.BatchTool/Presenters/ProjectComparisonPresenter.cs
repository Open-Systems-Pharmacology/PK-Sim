using System.IO;
using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core.Batch;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;

namespace PKSim.BatchTool.Presenters
{
   public interface IProjectComparisonPresenter : IBatchPresenter
   {
   }

   public class ProjectComparisonPresenter : InputAndOutputBatchPresenter<ProjectComparisonRunner>, IProjectComparisonPresenter
   {
      public ProjectComparisonPresenter(IInputAndOutputBatchView view, ProjectComparisonRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IBatchLogger batchLogger)
         : base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
         view.Caption = "PK-Sim BatchTool: Comparison of simulation results in existing projects";
      }

      public override void SelectInputFolder()
      {
         base.SelectInputFolder();
         if (string.IsNullOrEmpty(_dto.InputFolder)) return;
         _dto.OutputFolder = Path.Combine(_dto.InputFolder, "Output");
      }
   }
}