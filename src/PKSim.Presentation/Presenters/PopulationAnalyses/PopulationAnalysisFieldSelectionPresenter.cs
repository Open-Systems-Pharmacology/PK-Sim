using System.Collections.Generic;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisFieldSelectionPresenter : IPopulationAnalysisPresenter
   {
      void RefreshAnalysis();
   }

   public abstract class PopulationAnalysisFieldSelectionPresenter<TView> : AbstractSubPresenter<TView, IPopulationAnalysisFieldSelectionPresenter>, IPopulationAnalysisFieldSelectionPresenter
      where TView : IPopulationAnalysisFieldSelectionView
   {
      private readonly IPopulationAnalysisFieldsArrangementPresenter _fieldsArrangementPresenter;
      private readonly IPopulationAnalysisNumericFieldsPresenter _numericFieldsPresenter;
      private PopulationPivotAnalysis _populationPivotAnalysis;

      protected PopulationAnalysisFieldSelectionPresenter(TView view, IPopulationAnalysisFieldsArrangementPresenter fieldsArrangementPresenter, IPopulationAnalysisNumericFieldsPresenter numericFieldsPresenter)
         : base(view)
      {
         _fieldsArrangementPresenter = fieldsArrangementPresenter;
         _numericFieldsPresenter = numericFieldsPresenter;
         view.SetArrangementFieldView(_fieldsArrangementPresenter.View);
         view.SetDataFieldView(_numericFieldsPresenter.BaseView);
         AddSubPresenters(_fieldsArrangementPresenter, _numericFieldsPresenter);
      }

      public void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationPivotAnalysis)
      {
         _populationPivotAnalysis = populationPivotAnalysis.DowncastTo<PopulationPivotAnalysis>();
         _fieldsArrangementPresenter.StartAnalysis(populationDataCollector, populationPivotAnalysis);
         _numericFieldsPresenter.StartAnalysis(populationDataCollector, populationPivotAnalysis);
      }

      public void RefreshAnalysis()
      {
         _fieldsArrangementPresenter.RefreshAnalysis();
         _numericFieldsPresenter.RefreshAnalysis();
      }

      public string FieldCaptionFor(string fieldName)
      {
         var numericField = _populationPivotAnalysis.FieldByName(fieldName) as INumericValueField;
         return numericField == null ? fieldName : Constants.NameWithUnitFor(fieldName, numericField.DisplayUnit);
      }
   }
}