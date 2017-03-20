using System;
using System.Collections.Generic;
using OSPSuite.Utility.Events;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Populations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisFieldDistributionPresenter : IPresenter
   {
      void Plot(IPopulationDataCollector populationDataCollector, QuantityPKParameter pkParameter, PopulationAnalysisPKParameterField pkParameterField);
      void Plot(IPopulationDataCollector populationDataCollector, PopulationAnalysisDerivedField derivedField, PopulationAnalysis populationAnalysis);
      void Plot(IPopulationDataCollector populationDataCollector, IParameter parameter, PopulationAnalysisParameterField parameterField);
      void Plot(IPopulationDataCollector populationDataCollector, PopulationAnalysisCovariateField covariateField);
      void Plot(IPopulationDataCollector populationDataCollector, string covariateName);
      void ResetPlot();
   }

   public class PopulationAnalysisFieldDistributionPresenter : IPopulationAnalysisFieldDistributionPresenter
   {
      private readonly IPopulationDistributionPresenter _populationDistributionPresenter;
      private readonly IPopulationAnalysisFlatTableCreator _flatTableCreator;
      private readonly IPopulationAnalysisFieldFactory _populationAnalysisFieldFactory;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IDisplayUnitRetriever _displayUnitRetriever;
      public event EventHandler StatusChanged = delegate { };

      public PopulationAnalysisFieldDistributionPresenter(IPopulationDistributionPresenter populationDistributionPresenter, IPopulationAnalysisFlatTableCreator flatTableCreator,
         IPopulationAnalysisFieldFactory populationAnalysisFieldFactory, IDimensionRepository dimensionRepository, IDisplayUnitRetriever displayUnitRetriever)
      {
         _populationDistributionPresenter = populationDistributionPresenter;
         _flatTableCreator = flatTableCreator;
         _populationAnalysisFieldFactory = populationAnalysisFieldFactory;
         _dimensionRepository = dimensionRepository;
         _displayUnitRetriever = displayUnitRetriever;
      }

      public void Plot(IPopulationDataCollector populationDataCollector, PopulationAnalysisDerivedField derivedField, PopulationAnalysis populationAnalysis)
      {
         var fields = new List<IPopulationAnalysisField>(populationAnalysis.AllFieldsReferencedBy(derivedField)) {derivedField};
         plotDiscreteData(populationDataCollector, fields, derivedField);
      }

      public void Plot(IPopulationDataCollector populationDataCollector, PopulationAnalysisCovariateField covariateField)
      {
         var fields = new List<IPopulationAnalysisField> {covariateField};
         plotDiscreteData(populationDataCollector, fields, covariateField);
      }

      public void Plot(IPopulationDataCollector populationDataCollector, QuantityPKParameter pkParameter, PopulationAnalysisPKParameterField pkParameterField)
      {
         plotContinousData(populationDataCollector, pkParameter, pkParameterField, x => x.Plot);
      }

      public void Plot(IPopulationDataCollector populationDataCollector, IParameter parameter, PopulationAnalysisParameterField parameterField)
      {
         plotContinousData(populationDataCollector, parameter, parameterField, x => x.Plot);
      }

      public void Plot(IPopulationDataCollector populationDataCollector, string covariateName)
      {
         Plot(populationDataCollector, _populationAnalysisFieldFactory.CreateFor(covariateName, populationDataCollector));
      }

      private void plotDiscreteData(IPopulationDataCollector populationDataCollector, IReadOnlyList<IPopulationAnalysisField> dataFields, IPopulationAnalysisField fieldToPlot)
      {
         var data = _flatTableCreator.Create(populationDataCollector, dataFields);
         var fieldValues = data.AllValuesInColumn<string>(fieldToPlot.Name);
         _populationDistributionPresenter.Plot(populationDataCollector, fieldValues, fieldToPlot.Name, fieldToPlot);
      }

      public void ResetPlot()
      {
         _populationDistributionPresenter.ResetPlot();
      }

      private void plotContinousData<TObject>(IPopulationDataCollector populationDataCollector, TObject objectToPlot, INumericValueField numericValueField,
         Func<IPopulationDistributionPresenter, Action<IPopulationDataCollector, TObject, DistributionSettings, IDimension, Unit>> plotFunc)
         where TObject : class, IWithDimension

      {
         if (objectToPlot == null)
            return;

         var displayUnit = _displayUnitRetriever.PreferredUnitFor(objectToPlot);
         var dimension = objectToPlot.Dimension;
         var settings = new DistributionSettings();
         if (numericValueField != null)
         {
            displayUnit = numericValueField.DisplayUnit;
            settings.PlotCaption = numericValueField.Name;
            dimension = _dimensionRepository.MergedDimensionFor(new NumericFieldContext(numericValueField, populationDataCollector));
         }
         plotFunc(_populationDistributionPresenter)(populationDataCollector, objectToPlot, settings, dimension, displayUnit);
      }

      public void ReleaseFrom(IEventPublisher eventPublisher)
      {
         /*nothing to do*/
      }

      public void ViewChanged()
      {
         /*nothing to do*/
      }

      public void Initialize()
      {
         /*nothing to do*/
      }

      public bool CanClose => true;

      public IView BaseView => _populationDistributionPresenter.BaseView;
   }
}