using OSPSuite.Core.Domain;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisPKParameterSelectionPresenter : IPopulationAnalysisItemPresenter,
      IListener<FieldUnitChangedInPopulationAnalysisEvent>,
      IListener<FieldRenamedInPopulationAnalysisEvent>

   {
      void AddPKParameters();
      void RemovePKParameters();
      bool ScalingVisible { get; set; }
   }

   public class PopulationAnalysisPKParameterSelectionPresenter : PopulationAnalysisSelectionWithDistributionPresenter<IPopulationAnalysisPKParameterSelectionView, IPopulationAnalysisPKParameterSelectionPresenter>, IPopulationAnalysisPKParameterSelectionPresenter
   {
      private readonly IPopulationAnalysisAvailablePKParametersPresenter _allPKParametersPresenter;
      private readonly IPopulationAnalysisPKParameterFieldsPresenter _selectedPKParameterFieldsPresenter;

      public PopulationAnalysisPKParameterSelectionPresenter(
         IPopulationAnalysisPKParameterSelectionView view,
         IPopulationAnalysisAvailablePKParametersPresenter allPKParametersPresenter,
         IPopulationAnalysisPKParameterFieldsPresenter selectedPKParameterFieldsPresenter,
         IPopulationAnalysisFieldDistributionPresenter populationAnalysisFieldDistributionPresenter)
         : base(view, populationAnalysisFieldDistributionPresenter)
      {
         _allPKParametersPresenter = allPKParametersPresenter;
         _selectedPKParameterFieldsPresenter = selectedPKParameterFieldsPresenter;
         _view.AddAllPKParametersView(_allPKParametersPresenter.View);
         _view.AddSelectedPKParametersView(selectedPKParameterFieldsPresenter.BaseView);
         _view.AddDistributionView(_populationAnalysisFieldDistributionPresenter.BaseView);
         _allPKParametersPresenter.QuantityPKParameterDoubleClicked += (o, e) => addPKParameter(e.QuantityPKParameter);
         _allPKParametersPresenter.PKParameterSelected += (o, e) => drawDistributionFor(e.PKParameter, null);
         _selectedPKParameterFieldsPresenter.PKParameterSelected += (o, e) => drawDistributionFor(e);
         _selectedPKParameterFieldsPresenter.NoFieldSelected += (o, e) => ClearDistribution();
         _selectedPKParameterFieldsPresenter.DerivedFieldSelected += (o, e) => DrawDistributionFor(e.DerivedField);

         AddSubPresenters(_allPKParametersPresenter, _selectedPKParameterFieldsPresenter);
      }

      private void drawDistributionFor(PKParameterFieldSelectedEventArgs e) => drawDistributionFor(e.PKParameter, e.PKParameterField);

      private void drawDistributionFor(QuantityPKParameter pkParameter, PopulationAnalysisPKParameterField populationAnalysisPKParameterField)
      {
         _populationAnalysisFieldDistributionPresenter.Plot(_populationDataCollector, pkParameter, populationAnalysisPKParameterField);
      }

      public override void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         base.StartAnalysis(populationDataCollector, populationAnalysis);
         _allPKParametersPresenter.InitializeWith(populationDataCollector);
         _selectedPKParameterFieldsPresenter.StartAnalysis(populationDataCollector, populationAnalysis);
      }

      private void addPKParameter(QuantityPKParameter pk)
      {
         _selectedPKParameterFieldsPresenter.AddPKParameter(pk, _allPKParametersPresenter.QuantityPathDisplayFor(pk));
      }

      public void AddPKParameters()
      {
         var selectedPKParameters = _allPKParametersPresenter.SelectedPKParameters;
         selectedPKParameters.Each(addPKParameter);
      }

      public void RemovePKParameters()
      {
         _selectedPKParameterFieldsPresenter.RemoveSelection();
      }

      public bool ScalingVisible
      {
         set => _selectedPKParameterFieldsPresenter.ScalingVisible = value;
         get => _selectedPKParameterFieldsPresenter.ScalingVisible;
      }

      protected override void RedrawDistribution(PopulationAnalysisFieldEvent eventToHandle)
      {
         var field = eventToHandle.PopulationAnalysisField;
         if (field.IsAnImplementationOf<PopulationAnalysisPKParameterField>())
            drawDistributionFor(_selectedPKParameterFieldsPresenter.SelectedPKParameter, field.DowncastTo<PopulationAnalysisPKParameterField>());

         else if (field.IsAnImplementationOf<PopulationAnalysisDerivedField>())
            DrawDistributionFor(field.DowncastTo<PopulationAnalysisDerivedField>());
      }
   }
}