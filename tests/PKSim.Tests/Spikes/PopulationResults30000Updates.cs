//using System;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using OSPSuite.BDDHelper;
//using OSPSuite.BDDHelper.Extensions;
//using NHibernate.Linq;
//using NUnit.Framework;
//using PKSim.Core;
//using PKSim.Core.Extensions;
//using PKSim.Infrastructure;
//using PKSim.Infrastructure.Serialization.ORM.Queries;
//using OSPSuite.Core.Domain.Data;

//CODE FOR NHIBERNATE PERFORMRANCE DEBUG PURPOSE ONLY

//namespace PKSim.Spikes
//{
//   [Spike]
//   public abstract class concern_for_PopulationResults30000Updates : ContextSpecificationWithSerializationDatabase<SimulationResults>
//   {
//      protected int _numberOfPaths;
//      protected int _numberOfIndividuals;
//   }
//
//   public class When_serializing_a_lot_of_data_at_once : concern_for_PopulationResults30000Updates
//   {
//      private SimulationResults _simulationResults;
//
//      public override void GlobalContext()
//      {
//         base.GlobalContext();
//         _numberOfPaths = 5;
//         _numberOfIndividuals = 40000;
//         _simulationResults = DomainHelperForSpecs.CreateSimulationResults(_numberOfIndividuals, _numberOfPaths, numberOfPoints: 500);
//      }
//
//      [Observation]
//      public void should_not_take_too_much_time()
//      {
//         var begin = DateTime.UtcNow;
//         using (var session = _sessionFactory.OpenSession())
//         using (var transaction = session.BeginTransaction())
//         {
//            session.Save(_simulationResults);
//            transaction.Commit();
//         }
//
//         var end = DateTime.UtcNow;
//         var timeSpent = end - begin;
//         Debug.Print("Time spent saving {0} individuals with {1} curves is: {2}", _numberOfIndividuals, _numberOfPaths, timeSpent.ToDisplay());
//
//         begin = DateTime.UtcNow;
//
//         _sessionFactory.Close();
//         _sessionFactory = _sessionFactoryProvider.OpenSessionFactoryFor(_dataBaseFile);
//
//         var id = _simulationResults.Id;
//         _simulationResults = null;
//         GC.Collect();
//
//         var resultsQuery = new SimulationResultsQuery(null);
//         using (var session = _sessionFactory.OpenSession())
//         using (var transaction = session.BeginTransaction())
//         {
//            var simulationResults = resultsQuery.LoadSimulationResultsById(id, session);
//
//            Debug.Print("Results Count = {0}", simulationResults.Count);
//
//            //  var count = simulation.PopulationResults.IndividualResults.Count;
//            //NHibernateUtil.IsInitialized(simulation.PopulationResults).ShouldBeTrue();
//            simulationResults.ShouldNotBeNull();
//            transaction.Commit();
//         }
//         end = DateTime.UtcNow;
//         timeSpent = end - begin;
//         Debug.Print("Time spent loading {0} individuals with {1} curves is: {2}", _numberOfIndividuals, _numberOfPaths, timeSpent.ToDisplay());
//
//         Thread.Sleep(3000);
//      }
//
//      public override void GlobalCleanup()
//      {
//         //base.GlobalCleanup();
//         Debug.Print(_dataBaseFile);
//      }
//   }
//
//   public class When_deleting_the_population_results : concern_for_PopulationResults30000Updates
//   {
//      private SimulationResults _simulationResults;
//
//      public override void GlobalContext()
//      {
//         base.GlobalContext();
//         _numberOfPaths = 3;
//         _numberOfIndividuals = 10;
//         _simulationResults = DomainHelperForSpecs.CreateSimulationResults(_numberOfPaths, _numberOfIndividuals, numberOfPoints: 10);
//      }
//
//      [Observation]
//      public void should_also_delete_the_results_for_each_individuals()
//      {
//         using (var session = _sessionFactory.OpenSession())
//         using (var transaction = session.BeginTransaction())
//         {
//            session.Save(_simulationResults);
//            transaction.Commit();
//         }
//
//         _sessionFactory.Close();
//         _sessionFactory = _sessionFactoryProvider.OpenSessionFactoryFor(_dataBaseFile);
//
//         using (var session = _sessionFactory.OpenSession())
//         using (var transaction = session.BeginTransaction())
//         {
//            var population = session.Load<SimulationResults>(_simulationResults.Id);
//            //  Debug.Print("Results Count = {0}", population.AllIndividualResults.Count);
//
//            session.Delete(population);
//            transaction.Commit();
//         }
//
//         using (var session = _sessionFactory.OpenSession())
//         {
//            var allIndividualsResults = session.Query<IndividualResults>()
//               .Where(x => x.SimulationResults.Id == _simulationResults.Id)
//               .ToList();
//
//
//            allIndividualsResults.Count.ShouldBeEqualTo(0);
//         }
//      }
//   }
//
//   public class when_loading_an_existing_file : concern_for_PopulationResults30000Updates
//   {
//      public override void GlobalContext()
//      {
//         base.GlobalContext();
//         _sessionFactory.Close();
//         _sessionFactory = _sessionFactoryProvider.OpenSessionFactoryFor(@"C:\Tests\5.3.1\BIG_PROJECT.pksim5");
//      }
//
//      [Observation]
//      public void should_work()
//      {
//         int id = 2;
//         var begin = DateTime.UtcNow;
//         using (var session = _sessionFactory.OpenSession())
//         {
//            //     var simulationResults = session.Load<SimulationResults>(id);
//            Debug.Print("Population loaded");
//            //          Debug.Print("Results Count = {0}", simulationResults.AllIndividualResults.Count);
////
//            var allIndividualsResults = session.CreateQuery("select res from IndividualResults res where res.SimulationResults.Id=:id")
//               .SetParameter("id", id)
//               .List<IndividualResults>();
//
//
//            Debug.Print("Results Count = {0}", allIndividualsResults.Count);
//
//
//            //  var count = simulation.PopulationResults.IndividualResults.Count;
//            //NHibernateUtil.IsInitialized(simulation.PopulationResults).ShouldBeTrue();
////            population.ShouldNotBeNull();
//
////            foreach (var v in allIndividualsResults.SelectMany(x => x.AllValues))
////            {
////               v.Values.Length.ShouldBeEqualTo(150);
////            }
//
////            foreach (var individualsResult in simulationResults.AllIndividualResults)
////            {
////               Assert.ReferenceEquals(individualsResult.Time,simulationResults.Time);
////            }
//
////            simulationResults.Time.Values.Length.ShouldBeEqualTo(150);
//         }
//         var end = DateTime.UtcNow;
//         var timeSpent = end - begin;
//         Debug.Print("Time spent loading {0}", timeSpent.ToDisplay());
//      }
//   }
//}