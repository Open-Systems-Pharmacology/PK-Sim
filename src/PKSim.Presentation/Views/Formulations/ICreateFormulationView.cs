using PKSim.Presentation.Presenters.Formulations;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Formulations
{
   public interface ICreateFormulationView : IModalView<ICreateFormulationPresenter>, IContainerView
   {
      void BindToProperties(ObjectBaseDTO formulationDTO);
   }
}