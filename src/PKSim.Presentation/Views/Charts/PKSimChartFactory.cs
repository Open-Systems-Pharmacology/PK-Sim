using System;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Presentation.Services;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Services;
using OSPSuite.Core.Chart.Simulations;

namespace PKSim.Presentation.Views.Charts
{
   public class PKSimChartFactory : ChartFactory, IPKSimChartFactory
   {
      private readonly IChartTask _chartTask;

      public PKSimChartFactory(IContainer container, IIdGenerator idGenerator, IPresentationUserSettings presentationUserSettings, 
         IDimensionFactory dimensionFactory, ITableFormulaToDataRepositoryMapper dataRepositoryMapper, IChartTask chartTask)
         : base(container, idGenerator, presentationUserSettings, dimensionFactory, dataRepositoryMapper)
      {
         _chartTask = chartTask;
      }

      public TChartType CreateChartFor<TChartType>(IndividualSimulation individualSimulation) where TChartType : ChartWithObservedData
      {
         var chart = Create<TChartType>();
         _chartTask.UpdateObservedDataInChartFor(individualSimulation, chart);
         _chartTask.SetOriginTextFor(individualSimulation.Name, chart);
         return chart;
      }

      public ISimulationComparison CreateSummaryChart()
      {
         return Create<IndividualSimulationComparison>();
      }

      public ChartWithObservedData Create(Type chartType)
      {
         if (chartType.IsAnImplementationOf<SimulationTimeProfileChart>())
            return Create<SimulationTimeProfileChart>();

         if (chartType.IsAnImplementationOf<IndividualSimulationComparison>())
            return Create<IndividualSimulationComparison>();

         throw new ArgumentOutOfRangeException(nameof(chartType));
      }
   }
}