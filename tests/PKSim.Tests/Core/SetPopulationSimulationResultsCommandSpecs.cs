using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Events;

namespace PKSim.Core
{
   public abstract class concern_for_SetPopulationSimulationResultsCommand : ContextSpecification<SetPopulationSimulationResultsCommand>
   {
      protected PopulationSimulation _populationSimulation;
      protected SimulationResults _simulationResults;

      protected override void Context()
      {
         _populationSimulation= A.Fake<PopulationSimulation>();
         _simulationResults= A.Fake<SimulationResults>();
         sut = new SetPopulationSimulationResultsCommand(_populationSimulation,_simulationResults);
      }
   }

   public class When_settings_the_results_for_a_popuation_command : concern_for_SetPopulationSimulationResultsCommand
   {
      private IExecutionContext _context;
      private List<string> _allQuantityPaths;
      private OutputSelections _simulationSettings;
      private IEntitiesInContainerRetriever _quantityRetriever;
      private PathCache<IQuantity> _quantityCache;
      private IQuantity _quantity1;
      private IQuantity _quantity2;
      private IPKAnalysesTask _populationAnalysisTask;
      private PopulationSimulationPKAnalyses _pkAnalyses;

      protected override void Context()
      {
         base.Context();
         _context= A.Fake<IExecutionContext>();
         _quantityRetriever= A.Fake<IEntitiesInContainerRetriever>();
         _populationAnalysisTask= A.Fake<IPKAnalysesTask>();
         _pkAnalyses= A.Fake<PopulationSimulationPKAnalyses>();
         _quantityCache= new PathCacheForSpecs<IQuantity>();
         _quantity1= new MoleculeAmount();
         _quantity2 = new MoleculeAmount();
         _quantityCache.Add("PATH1",_quantity1);
         _quantityCache.Add("PATH2",_quantity2);
         A.CallTo(() => _context.Resolve<IEntitiesInContainerRetriever>()).Returns(_quantityRetriever);
         A.CallTo(() => _context.Resolve<IPKAnalysesTask>()).Returns(_populationAnalysisTask);
         A.CallTo(() => _quantityRetriever.QuantitiesFrom(_populationSimulation)).Returns(_quantityCache);
         A.CallTo(() => _populationAnalysisTask.CalculateFor(_populationSimulation)).Returns(_pkAnalyses);
         _allQuantityPaths = new List<string> {"PATH1", "PATH2"};
         _simulationSettings = new OutputSelections();
         A.CallTo(() => _populationSimulation.OutputSelections).Returns(_simulationSettings);
         A.CallTo(() => _simulationResults.AllQuantityPaths()).Returns(_allQuantityPaths);
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_make_sure_that_all_quantities_defined_in_the_results_are_also_selected_as_output_for_the_simulation()
      {
         _simulationSettings.Count().ShouldBeEqualTo(2);
         _simulationSettings.Select(x=>x.Path).ShouldContain("PATH1","PATH2");
      }

      [Observation]
      public void should_have_set_the_results_in_the_simulation()
      {
        _populationSimulation.Results.ShouldBeEqualTo(_simulationResults);
      }

      [Observation]
      public void should_calculate_the_pk_analysis_for_the_given_results_and_update_the_simulation()
      {
         _populationSimulation.PKAnalyses.ShouldBeEqualTo(_pkAnalyses);
      }

      [Observation]
      public void should_notify_that_the_simulation_was_updated()
      {
         A.CallTo(() => _context.PublishEvent(A<SimulationResultsUpdatedEvent>._)).MustHaveHappened();
      }
   }
}	