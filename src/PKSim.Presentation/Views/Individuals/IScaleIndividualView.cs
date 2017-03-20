using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IScaleIndividualView : IModalView<IScaleIndividualPresenter>, IWizardView
   {
      void BindToProperties(ObjectBaseDTO scaleIndividualPropertiesDTO);
   }
}