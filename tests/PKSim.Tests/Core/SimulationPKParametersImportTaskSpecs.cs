using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Import.Services;
using PKSim.Core.Model;
using ISimulationPKParametersImportTask = PKSim.Core.Services.ISimulationPKParametersImportTask;
using SimulationPKParametersImportTask = PKSim.Core.Services.SimulationPKParametersImportTask;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationPKAnalysesImportTask : ContextSpecificationAsync<ISimulationPKParametersImportTask>
   {
      protected SimulationPKParametersImport _simulationPKParametersImport;
      protected PopulationSimulation _populationSimulation;
      protected string _fileFullPath = "File";
      protected CancellationToken _token;
      protected readonly List<QuantityPKParameter> _importedPKParameters = new List<QuantityPKParameter>();
      protected IImportLogger _logger;
      protected PopulationSimulationPKAnalyses _pkAnalyses;
      private OSPSuite.Infrastructure.Import.Services.ISimulationPKParametersImportTask _corePKParameterImporter;

      protected override Task Context()
      {
         _token = new CancellationToken();
         _populationSimulation = A.Fake<PopulationSimulation>();
         _pkAnalyses = A.Fake<PopulationSimulationPKAnalyses>();
         _populationSimulation.PKAnalyses = _pkAnalyses;
         _corePKParameterImporter = A.Fake<OSPSuite.Infrastructure.Import.Services.ISimulationPKParametersImportTask>();
         var coreSimulationPKParameterImport = new SimulationPKParametersImport {PKParameters = _importedPKParameters};
         A.CallTo(_corePKParameterImporter).WithReturnType<Task<SimulationPKParametersImport>>().Returns(coreSimulationPKParameterImport);
         sut = new SimulationPKParametersImportTask(_corePKParameterImporter);

         return _completed;
      }
   }

   public class When_importing_pk_parameters_for_a_simulation_that_were_already_defined : concern_for_SimulationPKAnalysesImportTask
   {
      private QuantityPKParameter _importedPKParameter;
      private QuantityPKParameter _existingPKParameter;

      protected override async Task Context()
      {
         await base.Context();
         _importedPKParameter = A.Fake<QuantityPKParameter>();
         _existingPKParameter = A.Fake<QuantityPKParameter>();
         _importedPKParameter.QuantityPath = "Quantity";
         A.CallTo(() => _importedPKParameter.Id).Returns("IMPORTED PARAMETER");
         A.CallTo(() => _importedPKParameter.ToString()).Returns(_importedPKParameter.Id);
         _importedPKParameters.Add(_importedPKParameter);
         A.CallTo(() => _pkAnalyses.PKParameterBy(_importedPKParameter.Id)).Returns(_existingPKParameter);
      }

      protected override async Task Because()
      {
         _simulationPKParametersImport = await sut.ImportPKParameters(_populationSimulation, _fileFullPath, _token);
      }

      [Observation]
      public void should_warn_the_user_that_the_analysis_will_be_overwritten()
      {
         _simulationPKParametersImport.Status.Is(NotificationType.Warning).ShouldBeTrue(_simulationPKParametersImport.Status.ToString());
      }
   }


   public class When_the_number_of_imported_values_for_a_pk_parameter_does_not_match_the_number_of_individual_in_a_simulation : concern_for_SimulationPKAnalysesImportTask
   {
      private QuantityPKParameter _importedPKParameter;

      protected override async Task Context()
      {
         await base.Context();
         _importedPKParameter = A.Fake<QuantityPKParameter>();
         _importedPKParameter.QuantityPath = "Quantity";
         A.CallTo(() => _importedPKParameter.Count).Returns(10);
         A.CallTo(() => _populationSimulation.NumberOfItems).Returns(15);
         _importedPKParameters.Add(_importedPKParameter);
      }

      protected override async Task Because()
      {
         _simulationPKParametersImport = await sut.ImportPKParameters(_populationSimulation, _fileFullPath, _token);
      }

      [Observation]
      public void should_notify_an_error_during_the_import()
      {
         _simulationPKParametersImport.Status.Is(NotificationType.Error).ShouldBeTrue(_simulationPKParametersImport.Status.ToString());
      }
   }
}