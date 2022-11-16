using System;
using System.Collections.Generic;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.Presenters.Charts;
using PKSim.Core.Model;

namespace PKSim.Presentation.Services
{
   public interface IChartTemplatingTask : OSPSuite.Presentation.Services.Charts.IChartTemplatingTask
   {
      /// <summary>
      ///    Clone the chart belonging in the simulation
      /// </summary>
      /// <param name="originalChart">original chart to clone</param>
      /// <param name="simulation">Simulation containing the chart to clone</param>
      T CloneChart<T>(T originalChart, IndividualSimulation simulation) where T : AnalysisChart;

      void InitFromTemplate(CurveChart chart, IChartEditorAndDisplayPresenter chartEditorPresenter,
         IReadOnlyCollection<DataColumn> allAvailableColumns, IReadOnlyCollection<IndividualSimulation> simulations, Func<DataColumn, string> nameForColumn, CurveChartTemplate defaultChartTemplate = null);

      void LoadCurves(CurveChart chart, IndividualSimulation simulation);
      void LoadCurves(IndividualSimulation simulation);
   }
}