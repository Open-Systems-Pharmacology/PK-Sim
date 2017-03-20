using PKSim.BatchTool.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.BatchTool.Views
{
   public interface IGenerateProjectOverviewView: IView<IGenerateProjectOverviewPresenter>, IBatchView
   {
      void BindTo(OutputBatchDTO dto);
   }
}