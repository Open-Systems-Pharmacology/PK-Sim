using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Chart;
using PKSim.Presentation.Views.Charts;

namespace PKSim.Presentation.Presenters.Charts
{
   public interface IIndividualSimulationComparisonMdiPresenter : ISingleStartPresenter<IndividualSimulationComparison>, IPresenter<IIndividualSimulationComparisonMdiView>
   {
      void OnDragDrop(object sender, IDragEvent e);
      void OnDragOver(object sender, IDragEvent e);
   }

   public class IndividualSimulationComparisonMdiPresenter : SingleStartPresenter<IIndividualSimulationComparisonMdiView, IIndividualSimulationComparisonMdiPresenter>, IIndividualSimulationComparisonMdiPresenter
   {
      private readonly IIndividualSimulationComparisonPresenter _individualSimulationComparisonPresenter;

      public IndividualSimulationComparisonMdiPresenter(IIndividualSimulationComparisonMdiView view, IIndividualSimulationComparisonPresenter individualSimulationComparisonPresenter) : base(view)
      {
         _individualSimulationComparisonPresenter = individualSimulationComparisonPresenter;
         AddSubPresenters(_individualSimulationComparisonPresenter);
         view.ChartVisible = false;
         view.AddChartView(_individualSimulationComparisonPresenter.BaseView);
      }

      public void OnDragOver(object sender, IDragEvent e)
      {
         _individualSimulationComparisonPresenter.DragOver(sender, e);
      }

      public void OnDragDrop(object sender, IDragEvent e)
      {
         _individualSimulationComparisonPresenter.DragDrop(sender, e);
         updateChartVisibility();
      }

      public override void Edit(object subject)
      {
         Edit(subject as IndividualSimulationComparison);
      }

      protected override void UpdateCaption()
      {
         _view.Caption = _individualSimulationComparisonPresenter.ChartName;
      }

      public void Edit(IndividualSimulationComparison individualSimulationComparison)
      {
         _individualSimulationComparisonPresenter.Edit(individualSimulationComparison);
         UpdateCaption();
         updateChartVisibility();
         _view.Display();
      }

      private void updateChartVisibility()
      {
         View.ChartVisible = _individualSimulationComparisonPresenter.AnyCurves();
      }

      public override object Subject => _individualSimulationComparisonPresenter.Subject;

      public override void Close()
      {
         _view.CloseView();
      }

      public override void OnFormClosed()
      {
         base.OnFormClosed();
         Cleanup();
      }

      protected virtual void Cleanup()
      {
         _individualSimulationComparisonPresenter.Clear();
      }
   }
}