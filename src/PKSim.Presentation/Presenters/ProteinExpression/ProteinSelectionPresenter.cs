using System;
using System.Data;
using PKSim.Core.Services;
using PKSim.Presentation.Views.ProteinExpression;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.ProteinExpression
{
   public interface IProteinSelectionPresenter : IExpressionItemPresenter
   {
      void SearchProtein(string proteinName);
      void SelectProtein();
      bool ProteinSelected { get; }
      bool ProteinHasData { get; set; }
      bool ProteinSelectionChanged { get; }
      event Action<DataRow> OnSelectProtein;
      event Action OnProteinSearched;
      event Action OnSetActiveControl;
      void ActualizeSelection();
      void Activate();
      void SetActiveControl();
   }

   public class ProteinSelectionPresenter : AbstractSubPresenter<IProteinSelectionView, IProteinSelectionPresenter>, IProteinSelectionPresenter
   {
      private readonly IProteinExpressionQueries _proteinExpressionQueries;
      public event Action<DataRow> OnSelectProtein = delegate { };
      public event Action OnProteinSearched = delegate { };
      public event Action OnSetActiveControl = delegate { };
      public bool ProteinSelected { get; private set; }
      public bool ProteinHasData { get; set; }

      public ProteinSelectionPresenter(IProteinSelectionView view, IProteinExpressionQueries proteinExpressionQueries) : base(view)
      {
         _proteinExpressionQueries = proteinExpressionQueries;
      }

      public void SearchProtein(string proteinName)
      {
         _view.SetAvailableProteinsForSearchCriteria(_proteinExpressionQueries.GetProteinsByName(proteinName));
         OnProteinSearched();
      }

      public void SetActiveControl()
      {
         OnSetActiveControl();
      }

      public void SelectProtein()
      {
         DataRow selectedRow = _view.SelectedProteinData;
         if (selectedRow == null)
         {
            ProteinSelected = false;
            return;
         }
         var hasData = (int) selectedRow[DatabaseConfiguration.ProteinColumns.HAS_DATA];
         ProteinSelected = (hasData == 1);
         OnSelectProtein(selectedRow);
      }

      public bool ProteinSelectionChanged
      {
         get { return View.SelectionChanged; }
      }

      public void ActualizeSelection()
      {
         View.ActualizeSelection();
      }

      public void Activate()
      {
         View.Activate();
      }
   }
}