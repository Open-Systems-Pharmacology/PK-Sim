using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IOntogenySelectionView : IView<IOntogenySelectionPresenter>
   {
      /// <summary>
      /// Bind the view to the given protein and set the selected ontogeny
      /// </summary>
      void BindTo(PKSim.Core.Model.IndividualMolecule individualMolecule);

      /// <summary>
      /// sets if the show ontogeny button is enabled or disabled
      /// </summary>
      bool ShowOntogenyEnabled { set; }
   }
}