
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IShowOntogenyDataView : IModalView<IShowOntogenyDataPresenter>
   {
      void BindTo(ShowOntogenyDataDTO showOntogenyDataDTO);
      void UpdateContainers();
      void AddChart(IView view);
   }
}