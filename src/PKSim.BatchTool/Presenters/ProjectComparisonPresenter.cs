using System.IO;
using PKSim.BatchTool.Services;
using PKSim.BatchTool.Views;
using PKSim.Core.Batch;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;

namespace PKSim.BatchTool.Presenters
{
   public interface IProjectComparisonPresenter : IBatchPresenter<ProjectComparisonOptions>
   {
   }

   public class ProjectComparisonPresenter : InputAndOutputBatchPresenter<ProjectComparisonRunner, ProjectComparisonOptions>, IProjectComparisonPresenter
   {
      public ProjectComparisonPresenter(IInputAndOutputBatchView<ProjectComparisonOptions> view, ProjectComparisonRunner batchRunner, IDialogCreator dialogCreator, ILogPresenter logPresenter, IBatchLogger batchLogger)
         : base(view, batchRunner, dialogCreator, logPresenter, batchLogger)
      {
         view.Caption = "PK-Sim BatchTool: Comparison of simulation results in existing projects";
      }

      public override bool SelectInputFolder()
      {
         if (!base.SelectInputFolder())
            return false;

         _startOptions.OutputFolder = Path.Combine(_startOptions.InputFolder, "Output");
         return true;
      }
   }
}