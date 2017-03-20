using OSPSuite.Assets;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface ICreatePopulationAnalysisView : IWizardView, IModalView<ICreatePopulationAnalysisPresenter>
   {
      ApplicationIcon Image {  set; }
   }
}