using OSPSuite.Presentation.Presenters;
using PKSim.CLI.Core.RunOptions;

namespace PKSim.BatchTool.Presenters
{
   public interface ISnapshotPresenter : IPresenter
   {
      SnapshotRunOptions RunOptions { get; }
      void AdjustViewHeight();
   }
}