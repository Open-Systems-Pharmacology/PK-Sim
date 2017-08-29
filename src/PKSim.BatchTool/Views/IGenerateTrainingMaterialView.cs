using PKSim.BatchTool.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.BatchTool.Views
{
   public interface IGenerateTrainingMaterialView : IView<IGenerateTrainingMaterialPresenter>, IBatchView<TrainingMaterialsOptions>
   {
   }
}