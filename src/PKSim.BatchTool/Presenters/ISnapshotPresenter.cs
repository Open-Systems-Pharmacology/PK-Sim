using OSPSuite.CLI.Core.RunOptions;
using OSPSuite.Presentation.Presenters;

namespace PKSim.BatchTool.Presenters
{
   public interface ISnapshotPresenter : IPresenter
   {
      SnapshotRunOptions RunOptions { get; }
      void AdjustViewHeight();
   }
}