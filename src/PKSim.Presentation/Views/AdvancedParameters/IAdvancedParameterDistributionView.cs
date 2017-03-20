using PKSim.Core.Chart;
using PKSim.Presentation.Presenters.AdvancedParameters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.AdvancedParameters
{
   public interface IAdvancedParameterDistributionView : IView<IAdvancedParameterDistributionPresenter>
   {
      bool BarTypeVisible { set; }
      bool GenderSelectionVisible { set; }
      bool SettingsEnabled { get; set; }
      void BindToPlotOptions(DistributionSettings settings);
      void UpdateParameterGroupView(IView view);
      void AddDistributionView(IView view);
   }
}