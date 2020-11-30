using OSPSuite.Presentation.Views;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IOntogenySelectionView : IView<IOntogenySelectionPresenter>, IResizableView
   {
      /// <summary>
      ///    Bind the view to the given protein and set the selected ontogeny
      /// </summary>
      void BindTo(IndividualMolecule individualMolecule);

      /// <summary>
      ///    sets if the show ontogeny button is enabled or disabled
      /// </summary>
      bool ShowOntogenyEnabled { set; }
   }
}