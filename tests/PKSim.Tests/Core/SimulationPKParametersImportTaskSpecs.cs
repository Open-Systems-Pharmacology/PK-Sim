using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Import.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationPKAnalysesImportTask : ContextSpecification<ISimulationPKParametersImportTask>
   {
      private IEntitiesInContainerRetriever _quantityRetriever;
      private ISimulationPKAnalysesImporter _pkAnalysesImporter;
      protected SimulationPKParametersImport _simulationPKParametersImport;
      protected PopulationSimulation _populationSimulation;
      protected string _fileFullPath = "File";
      protected CancellationToken _token;
      protected readonly List<QuantityPKParameter> _importedPKParameters = new List<QuantityPKParameter>();
      protected IImportLogger _logger;
      protected PopulationSimulationPKAnalyses _pkAnalyses;
      protected PathCache<IQuantity> _availableQuantities;

      protected override void Context()
      {
         _token = new CancellationToken();
         _populationSimulation = A.Fake<PopulationSimulation>();
         _pkAnalyses = A.Fake<PopulationSimulationPKAnalyses>();
         _populationSimulation.PKAnalyses = _pkAnalyses;
         _quantityRetriever = A.Fake<IEntitiesInContainerRetriever>();
         _pkAnalysesImporter = A.Fake<ISimulationPKAnalysesImporter>();
         _availableQuantities = new PathCacheForSpecs<IQuantity>();
         A.CallTo(_pkAnalysesImporter).WithReturnType<IEnumerable<QuantityPKParameter>>().Returns(_importedPKParameters);
         A.CallTo(() => _quantityRetriever.OutputsFrom(_populationSimulation)).Returns(_availableQuantities);
         sut = new SimulationPKParametersImportTask(_pkAnalysesImporter, _quantityRetriever);
      }
   }

   public class When_importing_pk_parameters_for_a_simulation_that_were_already_defined : concern_for_SimulationPKAnalysesImportTask
   {
      private QuantityPKParameter _importedPKParameter;
      private QuantityPKParameter _existingPKParameter;

      protected override void Context()
      {
         base.Context();
         _importedPKParameter = A.Fake<QuantityPKParameter>();
         _existingPKParameter = A.Fake<QuantityPKParameter>();
         _importedPKParameter.QuantityPath = "Quantity";
         _availableQuantities[_importedPKParameter.QuantityPath] = new Observer();
         A.CallTo(() => _importedPKParameter.Id).Returns("IMPORTED PARAMETER");
         A.CallTo(() => _importedPKParameter.ToString()).Returns(_importedPKParameter.Id);
         _importedPKParameters.Add(_importedPKParameter);
         A.CallTo(() => _pkAnalyses.PKParameterBy(_importedPKParameter.Id)).Returns(_existingPKParameter);
      }

      [Observation]
      public async Task should_warn_the_user_that_the_analysis_will_be_overwritten()
      {
         _simulationPKParametersImport = await sut.ImportPKParameters(_populationSimulation, _fileFullPath, _token);
         _simulationPKParametersImport.Status.Is(NotificationType.Warning).ShouldBeTrue(_simulationPKParametersImport.Status.ToString());
      }

      [Observation]
      public async Task should_have_added_the_information_to_the_log_that_the_pk_parameter_were_successfully_imported_nonetheless()
      {
         _simulationPKParametersImport = await sut.ImportPKParameters(_populationSimulation, _fileFullPath, _token);
         _simulationPKParametersImport.Log.Any(x => x.Contains(_importedPKParameter.ToString())).ShouldBeTrue();
      }
   }

   public class When_importing_pk_parameters_for_a_quantity_that_does_not_exist : concern_for_SimulationPKAnalysesImportTask
   {
      private QuantityPKParameter _importedPKParameter;

      protected override void Context()
      {
         base.Context();
         _importedPKParameter = A.Fake<QuantityPKParameter>();
         _importedPKParameter.QuantityPath = "DOES NOT EXIST";
         _importedPKParameters.Add(_importedPKParameter);
      }

      [Observation]
      public async Task should_notify_an_error_during_the_import()
      {
         _simulationPKParametersImport = await sut.ImportPKParameters(_populationSimulation, _fileFullPath, _token);
         _simulationPKParametersImport.Status.Is(NotificationType.Error).ShouldBeTrue(_simulationPKParametersImport.Status.ToString());
      }
   }

   public class When_the_number_of_imported_values_for_a_pk_parameter_does_not_match_the_number_of_individual_in_a_simulation : concern_for_SimulationPKAnalysesImportTask
   {
      private QuantityPKParameter _importedPKParameter;

      protected override void Context()
      {
         base.Context();
         _importedPKParameter = A.Fake<QuantityPKParameter>();
         _importedPKParameter.QuantityPath = "Quantity";
         _availableQuantities[_importedPKParameter.QuantityPath] = new Observer();
         A.CallTo(() => _importedPKParameter.Count).Returns(10);
         A.CallTo(() => _populationSimulation.NumberOfItems).Returns(15);
         _importedPKParameters.Add(_importedPKParameter);
      }

      [Observation]
      public async Task should_notify_an_error_during_the_import()
      {
         _simulationPKParametersImport = await sut.ImportPKParameters(_populationSimulation, _fileFullPath, _token);
         _simulationPKParametersImport.Status.Is(NotificationType.Error).ShouldBeTrue(_simulationPKParametersImport.Status.ToString());
      }
   }
}