using OSPSuite.Presentation.Views;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IExpressionLocalizationView : IView<IExpressionLocalizationPresenter>
   {
      void BindTo(IndividualProtein individualProtein);
   }
}