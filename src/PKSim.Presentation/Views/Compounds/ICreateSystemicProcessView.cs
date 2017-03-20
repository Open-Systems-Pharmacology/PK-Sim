
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ICreateSystemicProcessView : IModalView<ICreateSystemicProcessPresenter>, ICreateProcessView
   {
      void BindTo(SystemicProcessDTO systemicProcessDTO);
   }
}