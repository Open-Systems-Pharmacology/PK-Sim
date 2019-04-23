
using PKSim.Presentation.Presenters.Compounds;

using PKSim.Presentation.Views.Parameters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ICompoundParameterGroupView : IView<ICompoundParameterGroupPresenter>, IResizableView
   {
   }

   public interface ICompoundAdvancedParameterGroupView : IView<ICompoundAdvancedParameterGroupPresenter>, IResizableView
   {
      void SetParameterView(IMultiParameterEditView view);
      string Hint { set; }
      bool IsLargeHint {  set; }
   }

   public interface ICompoundParameterGroupWithAlternativeView : IView<ICompoundParameterGroupWithAlternativePresenter>, IResizableView
   {
   }

   public interface ICompoundParameterGroupWithCalculatedDefaultView : ICompoundParameterGroupWithAlternativeView
   {
      void SetDynamicParameterView(IView view);
   }
}