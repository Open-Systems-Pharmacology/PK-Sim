using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisFieldsArrangementPresenter : IPresenter<IPopulationAnalysisFieldsArrangementView>, IPopulationAnalysisPresenter
   {
      void RefreshAnalysis();
      void UpdateDescription(PivotArea area, string description);
      void UpdateAreaVisibility(PivotArea area, bool visible);
   }

   public class PopulationAnalysisFieldsArrangementPresenter : AbstractPresenter<IPopulationAnalysisFieldsArrangementView, IPopulationAnalysisFieldsArrangementPresenter>, IPopulationAnalysisFieldsArrangementPresenter
   {
      private readonly IPopulationAnalysisFieldListPresenter _allFieldsPresenter;
      private readonly IPopulationAnalysisFieldListPresenter _rowFieldsPresenter;
      private readonly IPopulationAnalysisFieldListPresenter _colorFieldPresenter;
      private readonly IPopulationAnalysisFieldListPresenter _symbolFieldPresenter;
      private readonly IEventPublisher _eventPublisher;
      private PopulationPivotAnalysis _populationAnalysis;
      private readonly Cache<PivotArea, IPopulationAnalysisFieldListPresenter> _fieldListPresenterCache = new Cache<PivotArea, IPopulationAnalysisFieldListPresenter>(x => x.Area);

      public PopulationAnalysisFieldsArrangementPresenter(IPopulationAnalysisFieldsArrangementView view,
         IPopulationAnalysisFieldListPresenter allFieldsPresenter,
         IPopulationAnalysisFieldListPresenter rowFieldsPresenter,
         IPopulationAnalysisFieldListPresenter colorFieldPresenter,
         IPopulationAnalysisFieldListPresenter symbolFieldPresenter,
         IEventPublisher eventPublisher)
         : base(view)
      {
         _allFieldsPresenter = allFieldsPresenter;
         _rowFieldsPresenter = rowFieldsPresenter;
         _colorFieldPresenter = colorFieldPresenter;
         _symbolFieldPresenter = symbolFieldPresenter;
         _eventPublisher = eventPublisher;
         AddSubPresenters(_allFieldsPresenter, _rowFieldsPresenter, _colorFieldPresenter, _symbolFieldPresenter);
         initializeSubPresenters();
      }

      private void initializeSubPresenters()
      {
         initializeSubPresenter(_allFieldsPresenter, PivotArea.FilterArea, PKSimConstants.UI.AvailableFields);
         initializeSubPresenter(_rowFieldsPresenter, PivotArea.RowArea, PKSimConstants.UI.Panes);
         initializeSubPresenter(_colorFieldPresenter, PivotArea.ColorArea, PKSimConstants.UI.Colors, 1);
         initializeSubPresenter(_symbolFieldPresenter, PivotArea.SymbolArea, PKSimConstants.UI.Symbols, 1);

         _fieldListPresenterCache.Each(p => View.SetAreaView(p.Area, p.View));
      }

      private void initializeSubPresenter(IPopulationAnalysisFieldListPresenter presenter, PivotArea area, string description, int? maxNumberOfFields = null)
      {
         presenter.Area = area;
         presenter.MaximNumberOfAllowedFields = maxNumberOfFields;
         presenter.UpdateDescription(description);
         presenter.FieldsMoved += fieldsMoved;
         _fieldListPresenterCache.Add(presenter);
      }

      public void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         _populationAnalysis = populationAnalysis.DowncastTo<PopulationPivotAnalysis>();
         _fieldListPresenterCache.Each(p => p.StartAnalysis(populationDataCollector, populationAnalysis));
      }

      private void fieldsMoved(object sender, FieldsMovedEventArgs e)
      {
         var targetIndex = _populationAnalysis.AllFieldsOn(e.Area).Count - 1;
         if (e.Target != null)
            targetIndex = _populationAnalysis.GetAreaIndex(e.Target);

         foreach (var movedField in e.Fields)
         {
            _populationAnalysis.SetPosition(movedField, e.Area, ++targetIndex);
         }

         if (e.Target != null)
            _populationAnalysis.SetPosition(e.Target, e.Area, ++targetIndex);

         RefreshAnalysis();
         _eventPublisher.PublishEvent(new FieldsMovedInPopulationAnalysisEvent(_populationAnalysis, e.Fields));
      }

      public void RefreshAnalysis()
      {
         _fieldListPresenterCache.Each(p => p.RefreshAnalysis());
      }

      public void UpdateDescription(PivotArea area, string description)
      {
         _fieldListPresenterCache[area].UpdateDescription(description);
      }

      public void UpdateAreaVisibility(PivotArea area, bool visible)
      {
         View.SetAreaVisibility(area, visible);
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _fieldListPresenterCache.Each(p => p.FieldsMoved -= fieldsMoved);
      }
   }
}