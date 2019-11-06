using System.Text;
using FakeItEasy;
using NHibernate;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Infrastructure.Serialization.Services;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using PKSim.Infrastructure.Serialization.ORM.Queries;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SimulationChartsQuery : ContextSpecificationWithSerializationDatabase<ISimulationChartsQuery>
   {
      protected ISessionManager _sessionManager;

      protected override void Context()
      {
         _sessionManager = A.Fake<ISessionManager>();
         sut = new SimulationChartsQuery(_sessionManager);
      }
   }

   public class When_retrieving_the_simulation_charts_for_a_given_simulation_ : concern_for_SimulationChartsQuery
   {
      private ISession _session;
      private SimulationMetaData _simulation;

      protected override void Context()
      {
         base.Context();
         _session = _sessionFactory.OpenSession();
         A.CallTo(() => _sessionManager.OpenSession()).Returns(_session);

         _simulation = new SimulationMetaData {Id = "tralala", Name = " toto"};
         _simulation.AddChart(new SimulationChartMetaData {Id = "Chart1", Name = "Chart1", Content = {Data = Encoding.UTF8.GetBytes("chart1")}});
         _simulation.AddChart(new SimulationChartMetaData {Id = "Chart2", Name = "Chart2", Content = {Data = Encoding.UTF8.GetBytes("chart2")}});

         using (var session = _sessionFactory.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            session.Save(_simulation);
            transaction.Commit();
         }
      }

      [Observation]
      public void should_be_able_to_retrieve_the_serialized_content_for_the_simulation_charts()
      {
         var loadedCharts = sut.ResultFor(_simulation.Id);
         loadedCharts.Count.ShouldBeEqualTo(2);
         foreach (var chart in loadedCharts)
         {
            chart.Content.Data.ShouldNotBeNull();
         }
      }
   }
}