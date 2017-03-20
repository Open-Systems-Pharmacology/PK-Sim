using System.Data;
using PKSim.Presentation.Presenters.ProteinExpression;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.ProteinExpression
{
   public interface IProteinSelectionView : IView<IProteinSelectionPresenter>
   {
      void SetAvailableProteinsForSearchCriteria(DataTable selectionTable);
      void ActualizeSelection();
      DataRow SelectedProteinData { get; }
      bool SelectionChanged { get; }
      void Activate();
   }
}