using PKSim.Presentation.Presenters.Populations;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Populations
{
   public interface ICreatePopulationView : IWizardView
   {
      void BindToProperties(ObjectBaseDTO populationPropertiesDTO);
   }

   public interface ICreateRandomPopulationView : IModalView<ICreateRandomPopulationPresenter>, ICreatePopulationView
   {
   }
}