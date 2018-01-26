using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.Views
{
   public interface IEditValueOriginView : IView<IEditValueOriginPresenter>, IResizableView
   {
      void BindTo(ValueOriginDTO valueOriginDTO);
   }
}