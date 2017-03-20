using PKSim.Presentation.Presenters.Parameters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Parameters
{
   public interface INoItemInSelectionView : IView<INoItemInSelectionPresenter>
   {
      string Description { get; set; }
   }
}