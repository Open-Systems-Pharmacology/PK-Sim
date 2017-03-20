using PKSim.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views
{
   public interface IUserDisplayUnitsView : IView<IUserDisplayUnitsPresenter>
   {
      void AddView(IDisplayUnitsView view);
   }
}