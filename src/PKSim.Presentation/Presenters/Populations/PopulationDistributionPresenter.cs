using System;
using System.Collections.Generic;
using System.Drawing;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Views.Populations;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using DistributionSettings = PKSim.Core.Chart.DistributionSettings;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface IPopulationDistributionPresenter : 
      IPresenter<IPopulationParameterDistributionView>, 
      ICanCopyToClipboard
   {
      Color StartColorFor(string serie);
      Color EndColorFor(string serie);
      void ResetPlot();
      void Plot(IVectorialParametersContainer vectorialParametersContainer, IParameter parameter, DistributionSettings settings = null, IDimension dimension = null, Unit displayUnit = null);
      void Plot(IPopulationDataCollector populationDataCollector, QuantityPKParameter pkParameter, DistributionSettings settings = null, IDimension dimension = null, Unit displayUnit = null);
      void Plot(IVectorialParametersContainer vectorialParametersContainer, string covariate, DistributionSettings settings = null);

      /// <summary>
      ///    Plots the given <paramref name="values" />. The <paramref name="comparer" /> is used to order the values on the
      ///    x-axis
      /// </summary>
      void Plot(IVectorialParametersContainer vectorialParametersContainer, IReadOnlyList<string> values, string caption, IComparer<string> comparer, DistributionSettings settings = null);
   }

   public class PopulationDistributionPresenter : AbstractPresenter<IPopulationParameterDistributionView, IPopulationDistributionPresenter>, IPopulationDistributionPresenter
   {
      private readonly IDistributionDataCreator _distributionDataCreator;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IDisplayUnitRetriever _displayUnitRetriever;
      private readonly IPKParameterRepository _pkParameterRepository;
      private readonly IApplicationSettings _applicationSettings;

      public PopulationDistributionPresenter(
         IPopulationParameterDistributionView view,
         IDistributionDataCreator distributionDataCreator, 
         IRepresentationInfoRepository representationInfoRepository, 
         IDisplayUnitRetriever displayUnitRetriever, 
         IPKParameterRepository pkParameterRepository,
         IApplicationSettings applicationSettings
         )
         : base(view)
      {
         _distributionDataCreator = distributionDataCreator;
         _representationInfoRepository = representationInfoRepository;
         _displayUnitRetriever = displayUnitRetriever;
         _pkParameterRepository = pkParameterRepository;
         _applicationSettings = applicationSettings;
      }

      public void Plot(IVectorialParametersContainer vectorialParametersContainer, IParameter parameter, DistributionSettings settings = null, IDimension dimension = null, Unit displayUnit = null)
      {
         var displayUnitToUse = displayUnit ?? _displayUnitRetriever.PreferredUnitFor(parameter);
         plot(vectorialParametersContainer, parameter, _distributionDataCreator.CreateFor, updatePlotOptionsDefaultsFor, settings, dimension, displayUnitToUse);
      }

      public void Plot(IPopulationDataCollector populationDataCollector, QuantityPKParameter pkParameter, DistributionSettings settings = null, IDimension dimension = null, Unit displayUnit = null)
      {
         var displayUnitToUse = displayUnit ?? _displayUnitRetriever.PreferredUnitFor(pkParameter.Dimension);
         plot(populationDataCollector, pkParameter, _distributionDataCreator.CreateFor, updatePlotOptionsDefaultsFor, settings, dimension, displayUnitToUse);
      }

      private void plot<TObjectToPlot, TVectorialParametersContainer>(TVectorialParametersContainer populationDataCollector, TObjectToPlot objectToPlot,
         Func<TVectorialParametersContainer, DistributionSettings, TObjectToPlot, IDimension, Unit, ContinuousDistributionData> distributionCreator,
         Action<TObjectToPlot, DistributionSettings, Unit> updatePlotOptionAction,
         DistributionSettings settings, IDimension dimension, Unit displayUnit) where TObjectToPlot : IWithDimension
      {
         var dimensionToUse = dimension ?? objectToPlot.Dimension;
         var settingsToUse = settings ?? new DistributionSettings();
         updatePlotOptionAction(objectToPlot, settingsToUse, displayUnit);
         _view.Plot(distributionCreator(populationDataCollector, settingsToUse, objectToPlot, dimensionToUse, displayUnit), settingsToUse);
      }

      public void Plot(IVectorialParametersContainer vectorialParametersContainer, string covariate, DistributionSettings settings = null)
      {
         var settingsToUse = settings ?? new DistributionSettings();
         updatePlotOptionsDefaultsFor(covariate, settingsToUse);
         _view.Plot(_distributionDataCreator.CreateFor(vectorialParametersContainer, settingsToUse, covariate), settingsToUse);
      }

      public void Plot(IVectorialParametersContainer vectorialParametersContainer, IReadOnlyList<string> values, string caption, IComparer<string> comparer, DistributionSettings settings)
      {
         var settingsToUse = settings ?? new DistributionSettings();
         updatePlotOptionsDefaultsFor(caption, settingsToUse);
         _view.Plot(_distributionDataCreator.CreateFor(vectorialParametersContainer, settingsToUse, values, comparer), settingsToUse);
      }

      private void updatePlotOptionsDefaultsFor(IParameter parameter, DistributionSettings settings, Unit displayUnit)
      {
         updatePlotOptionsDefaultsFor(getTitleFor(parameter, displayUnit), settings, _representationInfoRepository.DisplayNameFor(parameter));
      }

      private void updatePlotOptionsDefaultsFor(QuantityPKParameter parameter, DistributionSettings settings, Unit displayUnit)
      {
         updatePlotOptionsDefaultsFor(getTitleFor(parameter, displayUnit), settings, parameter.QuantityPath);
      }

      private void updatePlotOptionsDefaultsFor(string xCaption, DistributionSettings settings, string plotCaption = null)
      {
         if (string.IsNullOrEmpty(settings.XAxisTitle))
            settings.XAxisTitle = xCaption;

         updateStandardSettings(settings);

         if (string.IsNullOrEmpty(settings.PlotCaption))
            settings.PlotCaption = plotCaption ?? xCaption;
      }

      private static void updateStandardSettings(DistributionSettings settings)
      {
         if (string.IsNullOrEmpty(settings.YAxisTitle))
            settings.YAxisTitle = settings.AxisCountMode.ToString();

         if (string.IsNullOrEmpty(settings.SelectedGender))
            settings.SelectedGender = CoreConstants.Population.AllGender;
      }

      private string getTitleFor(IParameter parameter, Unit displayUnit)
      {
         var parameterDisplayName = _representationInfoRepository.DisplayNameFor(parameter);
         return Constants.NameWithUnitFor(parameterDisplayName, displayUnit);
      }

      private string getTitleFor(QuantityPKParameter pkParameter, Unit displayUnit)
      {
         var parameterDisplayName = _pkParameterRepository.DisplayNameFor(pkParameter.Name);
         return Constants.NameWithUnitFor(parameterDisplayName, displayUnit);
      }

      public Color StartColorFor(string serie)
      {
         if (string.Equals(serie, PKSimConstants.UI.Female))
            return PKSimColors.Female;

         return PKSimColors.Male;
      }

      public Color EndColorFor(string serie)
      {
         if (string.Equals(serie, PKSimConstants.UI.Female))
            return PKSimColors.Female2;

         return PKSimColors.Male2;
      }

      public void ResetPlot()
      {
         _view.ResetPlot();
      }

      public void CopyToClipboard()
      {
         View.CopyToClipboard(_applicationSettings.WatermarkTextToUse);
      }
   }
}