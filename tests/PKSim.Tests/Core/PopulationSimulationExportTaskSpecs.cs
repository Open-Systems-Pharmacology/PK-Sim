using System.Data;
using System.IO;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationSimulationExportTask : ContextSpecification<IPopulationSimulationExportTask>
   {
      protected ILazyLoadTask _lazyLoadTask;
      protected IMoBiExportTask _mobiExportTask;
      protected IDialogCreator _dialogCreator;
      protected ISimulationSettingsRetriever _simulationSettingsRetriever;
      protected ICloner _cloner;
      protected IPopulationExportTask _populationExportTask;

      protected override void Context()
      {
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _mobiExportTask = A.Fake<IMoBiExportTask>();
         _simulationSettingsRetriever = A.Fake<ISimulationSettingsRetriever>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _cloner = A.Fake<ICloner>();
         _populationExportTask = A.Fake<IPopulationExportTask>();
         sut = new PopulationSimulationExportTask(_lazyLoadTask, _simulationSettingsRetriever, _cloner, _dialogCreator, _mobiExportTask, _populationExportTask);
      }
   }

   internal class When_exporting_a_population_for_cluster_calculation : concern_for_PopulationSimulationExportTask
   {
      private PopulationSimulation _populationSimulation;
      private const string _clusterInputDirectory = @"c:\temp\PKSIMTEST";
      private FileSelection _clusterExport;

      public override void GlobalContext()
      {
         base.GlobalContext();
         Directory.CreateDirectory(_clusterInputDirectory);
      }

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>().WithName("ClusterSim");
         var agingData = new DataTable();
         agingData.Columns.Add(new DataColumn("name"));
         var row = agingData.NewRow();
         row["name"] = "Age";
         agingData.Rows.Add(row);
         A.CallTo(() => _populationSimulation.AgingData.ToDataTable()).Returns(agingData);
         _clusterExport = new FileSelection() {Description = "Desc", FilePath = _clusterInputDirectory};
      }

      protected override void Because()
      {
         sut.ExportForCluster(_populationSimulation, _clusterExport);
      }

      [Observation]
      public void should_ask_for_settings()
      {
         A.CallTo(() => _simulationSettingsRetriever.SettingsFor(_populationSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_write_simulation_file()
      {
         A.CallTo(() => _mobiExportTask.ExportSimulationToPkmlFile(_populationSimulation, Path.Combine(_clusterInputDirectory, $"{_populationSimulation.Name}.{"pkml"}"))).MustHaveHappened();
      }

      [Observation]
      public void should_write_population_data()
      {
         File.Exists(Path.Combine(_clusterInputDirectory, $"{_populationSimulation.Name}.csv")).ShouldBeTrue();
         File.Exists(Path.Combine(_clusterInputDirectory, $"{_populationSimulation.Name}{CoreConstants.Population.TABLE_PARAMETER_EXPORT}.csv")).ShouldBeTrue();
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         Directory.Delete(_clusterInputDirectory, recursive: true);
      }
   }
}