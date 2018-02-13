using OSPSuite.Presentation.Views;
using PKSim.Presentation.Presenters.Parameters;

namespace PKSim.Presentation.Views.Parameters
{
   public interface IFavoriteParametersView : IView<IFavoriteParametersPresenter>
   {
      void AddParametersView(IView view);
      bool UpEnabled { get; set; }
      bool DownEnabled { get; set; }
   }
}