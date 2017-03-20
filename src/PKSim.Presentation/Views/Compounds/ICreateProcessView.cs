using OSPSuite.Assets;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ICreateProcessView : IModalView
   {
      void BindProcessTypes();
      void AddParametersView(IView parametersView);
      bool SpeciesVisible { set; }
      string TemplateDescription { set; }
      void SetIcon(ApplicationIcon icon);
      void AdjustParametersHeight(int optimalHeight);
   }
}