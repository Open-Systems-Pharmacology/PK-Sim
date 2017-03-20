using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisParameterSelectionPresenter : IPopulationAnalysisItemPresenter,
      IListener<FieldUnitChangedInPopulationAnalysisEvent>,
      IListener<FieldRenamedInPopulationAnalysisEvent>

   {
      void AddParameter();
      void RemoveParameter();
      bool ScalingVisible { get; set; }
   }

   public class PopulationAnalysisParameterSelectionPresenter : PopulationAnalysisSelectionWithDistributionPresenter<IPopulationAnalysisParameterSelectionView, IPopulationAnalysisParameterSelectionPresenter>, IPopulationAnalysisParameterSelectionPresenter
   {
      private readonly IPopulationParameterGroupsPresenter _allPopulationParametersPresenter;
      private readonly IPopulationAnalysisParameterFieldsPresenter _selectedPopulationParametersPresenter;
      private readonly IEntityPathResolver _entityPathResolver;

      public PopulationAnalysisParameterSelectionPresenter(IPopulationAnalysisParameterSelectionView view, IPopulationParameterGroupsPresenter allPopulationParametersPresenter, IPopulationAnalysisParameterFieldsPresenter selectedPopulationParametersPresenter, IEntityPathResolver entityPathResolver, IPopulationAnalysisFieldDistributionPresenter populationAnalysisFieldDistributionPresenter)
         : base(view, populationAnalysisFieldDistributionPresenter)
      {
         _allPopulationParametersPresenter = allPopulationParametersPresenter;
         _selectedPopulationParametersPresenter = selectedPopulationParametersPresenter;
         _entityPathResolver = entityPathResolver;
         view.AddPopulationParametersView(_allPopulationParametersPresenter.View);
         view.AddSelectedParametersView(_selectedPopulationParametersPresenter.BaseView);
         view.AddDistributionView(_populationAnalysisFieldDistributionPresenter.BaseView);
         AddSubPresenters(allPopulationParametersPresenter, selectedPopulationParametersPresenter);
         registerUpdateDistributionEvents();
      }

      public override void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         base.StartAnalysis(populationDataCollector, populationAnalysis);
         _allPopulationParametersPresenter.AddParamtersAndCovariates(allVisibleParametersIn(populationDataCollector), populationDataCollector.AllCovariateNames(), displayParameterUsingGroupStructure: populationDataCollector.ComesFromPKSim);
         _selectedPopulationParametersPresenter.StartAnalysis(populationDataCollector, populationAnalysis);
      }

      private IEnumerable<IParameter> allVisibleParametersIn(IPopulationDataCollector populationDataCollector)
      {
         return populationDataCollector.AllVectorialParameters(_entityPathResolver).Where(p => p.Visible);
      }

      private void drawDistributionFor(IParameter parameter, PopulationAnalysisParameterField parameterField = null)
      {
         if (parameter == null) return;
         _populationAnalysisFieldDistributionPresenter.Plot(_populationDataCollector, parameter, parameterField);
      }

      private void drawDistributionFor(PopulationAnalysisCovariateField covariateField)
      {
         _populationAnalysisFieldDistributionPresenter.Plot(_populationDataCollector, covariateField);
      }

      private void drawDistributionFor(string covariateFieldName)
      {
         _populationAnalysisFieldDistributionPresenter.Plot(_populationDataCollector, covariateFieldName);
      }

      public void AddParameter()
      {
         var parameter = _allPopulationParametersPresenter.SelectedParameter;
         if (parameter != null)
         {
            addParameter(parameter);
            return;
         }

         var covariate = _allPopulationParametersPresenter.SelectedCovariate;
         if (covariate != null)
            addCovariate(covariate);
      }

      private void addParameter(IParameter parameter)
      {
         if (parameter == null) return;
         _selectedPopulationParametersPresenter.AddParameter(parameter);
      }

      private void addCovariate(string covariate)
      {
         _selectedPopulationParametersPresenter.AddCovariate(covariate);
      }

      public void RemoveParameter()
      {
         _selectedPopulationParametersPresenter.RemoveSelection();
      }

      public bool ScalingVisible
      {
         set { _selectedPopulationParametersPresenter.ScalingVisible = value; }
         get { return _selectedPopulationParametersPresenter.ScalingVisible; }
      }

      private void registerUpdateDistributionEvents()
      {
         _allPopulationParametersPresenter.GroupNodeSelected += (o, e) => ClearDistribution();
         _allPopulationParametersPresenter.ParameterNodeSelected += (o, e) => drawDistributionFor(e.Parameter);
         _allPopulationParametersPresenter.CovariateNodeSelected += (o, e) => drawDistributionFor(e.Node.Id);
         _allPopulationParametersPresenter.ParameterNodeDoubleClicked += (o, e) => addParameter(e.Parameter);
         _allPopulationParametersPresenter.CovariateNodeDoubleClicked += (o, e) => addCovariate(e.Covariate);
         _selectedPopulationParametersPresenter.ParameterFieldSelected += (o, e) => drawDistributionFor(e.Parameter, e.ParameterField);
         _selectedPopulationParametersPresenter.CovariateFieldSelected += (o, e) => drawDistributionFor(e.CovariateField);
         _selectedPopulationParametersPresenter.DerivedFieldSelected += (o, e) => DrawDistributionFor(e.DerivedField);
         _selectedPopulationParametersPresenter.NoFieldSelected += (o, e) => ClearDistribution();
      }

      protected override void RedrawDistribution(PopulationAnalysisFieldEvent eventToHandle)
      {
         var field = eventToHandle.PopulationAnalysisField;
         if (field.IsAnImplementationOf<PopulationAnalysisParameterField>())
            drawDistributionFor(_selectedPopulationParametersPresenter.SelectedParameter, field.DowncastTo<PopulationAnalysisParameterField>());

         else if (field.IsAnImplementationOf<PopulationAnalysisCovariateField>())
            drawDistributionFor(eventToHandle.PopulationAnalysisField.DowncastTo<PopulationAnalysisCovariateField>());

         else if (field.IsAnImplementationOf<PopulationAnalysisDerivedField>())
            DrawDistributionFor(eventToHandle.PopulationAnalysisField.DowncastTo<PopulationAnalysisDerivedField>());
      }
   }
}