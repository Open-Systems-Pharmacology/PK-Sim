using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ICompoundProcessView : IProcessView<CompoundProcessDTO>, IView<ICompoundProcessPresenter>
   {

   }
}