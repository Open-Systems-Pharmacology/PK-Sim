using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface IProcessView<TProcessDTO>
   {
      void BindTo(TProcessDTO processDTO);
      void SetParametersView(IView parametersView);
      bool SpeciesVisible { set; }
      void AdjustParametersHeight(int parametersHeight);
   }
}