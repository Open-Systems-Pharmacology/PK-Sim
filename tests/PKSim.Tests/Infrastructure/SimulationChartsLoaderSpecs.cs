using System.Collections.Generic;
using System.Text;
using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using PKSim.Infrastructure.Serialization.ORM.Queries;
using PKSim.Infrastructure.Services;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SimulationChartsLoader : ContextSpecification<ISimulationChartsLoader>
   {
      protected ISimulationChartsQuery _simulationChartQuery;
      protected ICompressedSerializationManager _serializationManager;
      protected IChartTask _chartTask;

      protected override void Context()
      {
         _simulationChartQuery = A.Fake<ISimulationChartsQuery>();
         _serializationManager = A.Fake<ICompressedSerializationManager>();
         _chartTask= A.Fake<IChartTask>();
         sut = new SimulationChartsLoader(_simulationChartQuery, _serializationManager,_chartTask);
      }
   }

   public class When_adding_the_charts_available_for_a_simulation : concern_for_SimulationChartsLoader
   {
      private Simulation _simulation;
      private ISimulationAnalysis _chart1;
      private ISimulationAnalysis _chart2;
      private readonly IList<SimulationChartMetaData> _allCharts = new List<SimulationChartMetaData>();

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>().WithId("sim");
         _chart1 = A.Fake<ISimulationAnalysis>();
         _chart2 = A.Fake<ISimulationAnalysis>();
         var chartMetaData1 = new SimulationChartMetaData {Content = {Data = Encoding.UTF8.GetBytes("chart1")}};
         var chartMetaData2 = new SimulationChartMetaData {Content = {Data = Encoding.UTF8.GetBytes("chart2")}};
         _allCharts.Add(chartMetaData1);
         _allCharts.Add(chartMetaData2);
         A.CallTo(() => _serializationManager.Deserialize<ISimulationAnalysis>(chartMetaData1.Content.Data, null)).Returns(_chart1);
         A.CallTo(() => _serializationManager.Deserialize<ISimulationAnalysis>(chartMetaData2.Content.Data, null)).Returns(_chart2);
         A.CallTo(() => _simulationChartQuery.ResultFor(_simulation.Id)).Returns(_allCharts);
      }

      protected override void Because()
      {
         sut.LoadChartsFor(_simulation);
      }

      [Observation]
      public void should_retrieve_the_chart_content_from_the_database()
      {
         A.CallTo(() => _simulationChartQuery.ResultFor(_simulation.Id)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_charts_to_the_simulation()
      {
         A.CallTo(() => _simulation.AddAnalysis(_chart1)).MustHaveHappened();
         A.CallTo(() => _simulation.AddAnalysis(_chart2)).MustHaveHappened();
      }

      [Observation]
      public void should_add_all_observed_data_defined_in_the_simulation()
      {
         A.CallTo(() => _chartTask.UpdateObservedDataInChartsFor(_simulation)).MustHaveHappened();
      }
   }
}