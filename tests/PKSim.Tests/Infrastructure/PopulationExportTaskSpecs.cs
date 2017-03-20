using System.Data;
using System.IO;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters;

using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization.SimModel.Services;
using OSPSuite.Presentation.Core;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_PopulationExportTask : ContextSpecification<IPopulationExportTask>
   {
      protected Population _population;
      protected IEntityPathResolver _entityPathResolver;
      protected ILazyLoadTask _lazyLoadTask;
      protected ISimModelExporter _simModelExporter;
      protected ISimulationToModelCoreSimulationMapper _modelCoreSimulationMapper;
      private IPKSimConfiguration _configuration;
      private IWorkspace _workspace;
      private IApplicationController _applicationController;
      protected ISelectFilePresenter _selectFilePresenter;
      protected IDialogCreator _dialogCreator;
      protected ISimulationSettingsRetriever _simulationSettingsRetriever;
      private ICloner _cloner;

      protected override void Context()
      {
         _configuration = A.Fake<IPKSimConfiguration>();
         _applicationController = A.Fake<IApplicationController>();
         _workspace = A.Fake<IWorkspace>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _selectFilePresenter = A.Fake<ISelectFilePresenter>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _simModelExporter = A.Fake<ISimModelExporter>();
         _modelCoreSimulationMapper = A.Fake<ISimulationToModelCoreSimulationMapper>();
         _simulationSettingsRetriever = A.Fake<ISimulationSettingsRetriever>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _cloner= A.Fake<ICloner>();
         _population = A.Fake<Population>().WithName("MyPop");
         A.CallTo(() => _population.AllCovariateNames()).Returns(new[] {CoreConstants.Covariates.GENDER, CoreConstants.Covariates.RACE});
         A.CallTo(() => _applicationController.Start<ISelectFilePresenter>()).Returns(_selectFilePresenter);
         sut = new PopulationExportTask(_applicationController, _entityPathResolver, _lazyLoadTask, _simModelExporter,
            _modelCoreSimulationMapper, _workspace, _configuration, _simulationSettingsRetriever, _dialogCreator,_cloner);
      }
   }

   internal class When_exporting_a_population_for_cluster_calculation : concern_for_PopulationExportTask
   {
      private PopulationSimulation _populationSimulation;
      private const string _clusterInputDirectory = @"c:\temp\PKSIMTEST";
      private IModelCoreSimulation _coreSimulation;
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
         A.CallTo(() => _selectFilePresenter.SelectDirectory(PKSimConstants.UI.ExportForClusterSimulationTitle, Constants.DirectoryKey.SIM_MODEL_XML)).Returns(_clusterExport);
         _coreSimulation = A.Fake<IModelCoreSimulation>();
         A.CallTo(() => _modelCoreSimulationMapper.MapFrom(_populationSimulation,  false)).Returns(_coreSimulation);
      }

      protected override void Because()
      {
         sut.ExportForCluster(_populationSimulation);
      }

      [Observation]
      public void should_ask_for_settings()
      {
         A.CallTo(() => _simulationSettingsRetriever.SettingsFor(_populationSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_write_simulation_file()
      {
         A.CallTo(() => _simModelExporter.Export(_coreSimulation, Path.Combine(_clusterInputDirectory, $"{_populationSimulation.Name}.{"xml"}"))).MustHaveHappened();
      }

      [Observation]
      public void should_write_population_data()
      {
         File.Exists(Path.Combine(_clusterInputDirectory, $"{_populationSimulation.Name}.csv")).ShouldBeTrue();
         File.Exists(Path.Combine(_clusterInputDirectory, $"{_populationSimulation.Name}{CoreConstants.Population.TableParameterExport}.csv")).ShouldBeTrue();
      }

      [Observation]
      public void should_ask_for_cluster_output_directory()
      {
         A.CallTo(() => _selectFilePresenter.SelectDirectory(PKSimConstants.UI.ExportForClusterSimulationTitle, Constants.DirectoryKey.SIM_MODEL_XML)).MustHaveHappened();
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         Directory.Delete(_clusterInputDirectory, recursive: true);
      }
   }
}