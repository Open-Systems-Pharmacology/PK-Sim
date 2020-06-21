using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Infrastructure.Import.Services;
using PKSim.Core.Model;
using ICoreSimulationResultsImportTask = OSPSuite.Infrastructure.Import.Services.ISimulationResultsImportTask;
using ISimulationResultsImportTask = PKSim.Core.Services.ISimulationResultsImportTask;
using SimulationResultsImportTask = PKSim.Core.Services.SimulationResultsImportTask;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SimulationResultsImportTask : ContextSpecificationAsync<ISimulationResultsImportTask>
   {
      protected SimulationResultsImport _results;
      protected readonly List<IndividualResults> _individualResults1 = new List<IndividualResults>();
      protected readonly List<IndividualResults> _individualResults2 = new List<IndividualResults>();
      protected PopulationSimulation _populationSimulation;
      protected List<string> _files;
      protected CancellationToken _cancellationToken;
      protected ICoreSimulationResultsImportTask _coreSimulationResultsImportTask;

      protected override Task Context()
      {
         _populationSimulation = A.Fake<PopulationSimulation>();
         _cancellationToken = new CancellationToken();
         _coreSimulationResultsImportTask = A.Fake<ICoreSimulationResultsImportTask>();
         sut = new SimulationResultsImportTask(_coreSimulationResultsImportTask);

         return _completed;
      }
   }

   public class When_importing_the_simulation_results_defined_in_a_valid_set_of_files : concern_for_SimulationResultsImportTask
   {
      private SimulationResultsImport _simulationResults;

      protected override async Task Context()
      {
         await base.Context();
         _simulationResults = new SimulationResultsImport();
         _simulationResults.SimulationResults.Add(new IndividualResults());
         _simulationResults.SimulationResults.Add(new IndividualResults());
         A.CallTo(() => _coreSimulationResultsImportTask.ImportResults(_populationSimulation, _files, _cancellationToken, true)).Returns(_simulationResults);
         A.CallTo(() => _populationSimulation.NumberOfItems).Returns(2);
      }

      protected override async Task Because()
      {
         _results = await sut.ImportResults(_populationSimulation, _files, _cancellationToken);
      }

      [Observation]
      public void should_return_a_valid_status()
      {
         _results.HasError.ShouldBeFalse();
      }
   }

   public class When_the_number_of_imported_results_does_not_match_the_number_of_individual_in_the_population : concern_for_SimulationResultsImportTask
   {
      private SimulationResultsImport _simulationResults;

      protected override async Task Context()
      {
         await base.Context();
         _simulationResults = new SimulationResultsImport();
         _simulationResults.SimulationResults.Add(new IndividualResults());
         _simulationResults.SimulationResults.Add(new IndividualResults());
         A.CallTo(() => _coreSimulationResultsImportTask.ImportResults(_populationSimulation, _files, _cancellationToken, true)).Returns(_simulationResults);
         A.CallTo(() => _populationSimulation.NumberOfItems).Returns(3);
      }

      protected override async Task Because()
      {
         _results = await sut.ImportResults(_populationSimulation, _files, _cancellationToken);
      }

      [Observation]
      public void should_return_a_invalid_status()
      {
         _results.HasError.ShouldBeTrue();
      }
   }
}