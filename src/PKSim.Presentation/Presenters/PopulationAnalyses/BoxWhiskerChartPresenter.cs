using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IBoxWhiskerChartPresenter : IPresenter<IBoxWhiskerChartView>, IPopulationAnalysisChartPresenter<BoxWhiskerXValue, BoxWhiskerYValue>
   {
      /// <summary>
      ///    Extracts all individuals whose individualId were gathered in the curveData
      /// </summary>
      void ExtractIndividuals(BoxWhiskerYValue boxWhiskerYValue);

      /// <summary>
      ///    Extracts all individuals with id in <paramref name="individualIds" />
      /// </summary>
      void ExtractIndividuals(IEnumerable<int> individualIds);
   }

   public class BoxWhiskerChartPresenter : PopulationAnalysisChartPresenter<IBoxWhiskerChartView, IBoxWhiskerChartPresenter, BoxWhiskerXValue, BoxWhiskerYValue>, IBoxWhiskerChartPresenter
   {
      private readonly IIndividualExtractor _individualExtractor;
      private readonly IObjectTypeResolver _objectTypeResolver;

      public BoxWhiskerChartPresenter(
         IBoxWhiskerChartView view, 
         IPopulationAnalysisChartSettingsPresenter populationAnalysisChartSettingsPresenter,
         IApplicationSettings applicationSettings,
         IIndividualExtractor individualExtractor, 
         IObjectTypeResolver objectTypeResolver) : base(view, populationAnalysisChartSettingsPresenter,applicationSettings)
      {
         _individualExtractor = individualExtractor;
         _objectTypeResolver = objectTypeResolver;
      }

      public void ExtractIndividuals(BoxWhiskerYValue boxWhiskerYValue)
      {
         if (boxWhiskerYValue == null)
            return;

         ExtractIndividuals(boxWhiskerYValue.AllValues.Select(x => x.IndividualId));
      }

      public void ExtractIndividuals(IEnumerable<int> individualIds)
      {
         var analyzable = AnalysisChart.DowncastTo<BoxWhiskerAnalysisChart>().Analysable;
         var populationSimulation = analyzable as PopulationSimulation;
         if (populationSimulation == null)
            throw new PKSimException(PKSimConstants.Error.CannotExtractIndividualFrom(_objectTypeResolver.TypeFor(analyzable)));

         _individualExtractor.ExtractIndividualsFrom(populationSimulation.Population, individualIds);
      }
   }
}