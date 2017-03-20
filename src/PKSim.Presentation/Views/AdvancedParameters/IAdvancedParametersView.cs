using PKSim.Presentation.Presenters.AdvancedParameters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.AdvancedParameters
{
   public interface IAdvancedParametersView : IView<IAdvancedParametersPresenter>
   {
      void AddConstantParameterGroupsView(IView view);
      void AddAdvancedParameterGroupsView(IView view);
      bool AddEnabled { get; set; }
      bool RemoveEnabled { get; set; }
      void AddAdvancedParameterView(IView advancedParameterView);
   }
}