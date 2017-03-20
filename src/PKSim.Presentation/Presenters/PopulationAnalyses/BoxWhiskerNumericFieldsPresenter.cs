using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IBoxWhiskerNumericFieldsPresenter : IPopulationAnalysisNumericFieldsPresenter
   {
      void ShowOutliersChanged();
   }

   public class BoxWhiskerNumericFieldsPresenter : AbstractSubPresenter<IBoxWhiskerNumericFieldsView, IBoxWhiskerNumericFieldsPresenter>, IBoxWhiskerNumericFieldsPresenter
   {
      private readonly IMultipleNumericFieldsPresenter _multipleNumericFieldsPresenter;
      private readonly IEventPublisher _eventPublisher;
      private readonly BoxWhiskerNumericFieldDTO _dto;
      private PopulationBoxWhiskerAnalysis _boxWiskerAnalysis;

      public BoxWhiskerNumericFieldsPresenter(IBoxWhiskerNumericFieldsView view, IMultipleNumericFieldsPresenter multipleNumericFieldsPresenter,IEventPublisher eventPublisher) : base(view)
      {
         _multipleNumericFieldsPresenter = multipleNumericFieldsPresenter;
         _eventPublisher = eventPublisher;
         _multipleNumericFieldsPresenter.AllowedType = typeof (PopulationAnalysisNumericField);
         view.AddFieldSelectionView(multipleNumericFieldsPresenter.BaseView);
         AddSubPresenters(multipleNumericFieldsPresenter);
         _dto = new BoxWhiskerNumericFieldDTO();
      }

      public void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         _boxWiskerAnalysis = populationAnalysis.DowncastTo<PopulationBoxWhiskerAnalysis>();
         _multipleNumericFieldsPresenter.StartAnalysis(populationDataCollector, populationAnalysis);
         _dto.ShowOutliers = _boxWiskerAnalysis.ShowOutliers;
         _view.BindTo(_dto);
      }

      public void RefreshAnalysis()
      {
         _multipleNumericFieldsPresenter.RefreshAnalysis();
      }

      public void ShowOutliersChanged()
      {
         _boxWiskerAnalysis.ShowOutliers = _dto.ShowOutliers;
         _eventPublisher.PublishEvent(new PopulationAnalysisDataSelectionChangedEvent(_boxWiskerAnalysis));
      }
   }
}