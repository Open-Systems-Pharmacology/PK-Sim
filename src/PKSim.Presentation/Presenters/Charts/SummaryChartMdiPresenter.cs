using System.Windows.Forms;
using PKSim.Core.Chart;
using PKSim.Presentation.Views.Charts;
using SBSuite.Presentation.Presenters;
using SBSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Charts
{
   public interface ISummaryChartMdiPresenter : ISingleStartPresenter<SummaryChart>, IPresenter<ISummaryChartMdiView>
   {
      void OnDragDrop(object sender, DragEventArgs e);
      void OnDragOver(object sender, DragEventArgs e);
      IView SubView { get; }
   }

   public class SummaryChartMdiPresenter : SingleStartPresenter<ISummaryChartMdiView, ISummaryChartMdiPresenter>, ISummaryChartMdiPresenter
   {
      private readonly ISummaryChartPresenter _summaryChartPresenter;

      public SummaryChartMdiPresenter(ISummaryChartMdiView view, ISummaryChartPresenter summaryChartPresenter) : base(view)
      {
         _summaryChartPresenter = summaryChartPresenter;
         AddSubPresenters(_summaryChartPresenter);
         view.ChartVisible = false;
         view.AddChartView(_summaryChartPresenter.BaseView);
      }

      public void OnDragOver(object sender, DragEventArgs e)
      {
         _summaryChartPresenter.DragOver(sender, e);
      }

      public IView SubView
      {
         get { return _summaryChartPresenter.BaseView; }
      }

      public void OnDragDrop(object sender, DragEventArgs e)
      {
         _summaryChartPresenter.DragDrop(sender, e);
         updateChartVisibility();
      }

      private void updateChartVisibility()
      {
         View.ChartVisible = _summaryChartPresenter.AnyCurves();
      }

      public override void Edit(object subject)
      {
         Edit(subject as SummaryChart);
      }

      protected override void UpdateCaption()
      {
         _view.Caption = _summaryChartPresenter.ChartName;
      }

      public void Edit(SummaryChart objectToEdit)
      {
         _summaryChartPresenter.Edit(objectToEdit);
         UpdateCaption();
         updateChartVisibility();
         _view.Display();
      }

      public override object Subject
      {
         get { return _summaryChartPresenter.Subject; }
      }

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
         _summaryChartPresenter.Clear();
      }
   }
}