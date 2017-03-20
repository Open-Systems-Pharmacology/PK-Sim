using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Individuals
{
   public interface ICreateIndividualView : IWizardView,IModalView<ICreateIndividualPresenter>
   {
      void BindToProperties(ObjectBaseDTO individualPropertiesDTO);
   }
}