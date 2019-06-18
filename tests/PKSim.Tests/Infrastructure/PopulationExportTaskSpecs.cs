using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization.SimModel.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters;
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
      private IProjectRetriever _projectRetriever;
      private IApplicationController _applicationController;
      protected ISelectFilePresenter _selectFilePresenter;
      protected IDialogCreator _dialogCreator;
      protected ISimulationSettingsRetriever _simulationSettingsRetriever;
      private ICloner _cloner;

      protected override void Context()
      {
         _configuration = A.Fake<IPKSimConfiguration>();
         _applicationController = A.Fake<IApplicationController>();
         _projectRetriever = A.Fake<IProjectRetriever>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _selectFilePresenter = A.Fake<ISelectFilePresenter>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _simModelExporter = A.Fake<ISimModelExporter>();
         _modelCoreSimulationMapper = A.Fake<ISimulationToModelCoreSimulationMapper>();
         _simulationSettingsRetriever = A.Fake<ISimulationSettingsRetriever>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _cloner = A.Fake<ICloner>();
         _population = A.Fake<Population>().WithName("MyPop");
         A.CallTo(() => _population.AllCovariateNames).Returns(new[] {CoreConstants.Covariates.GENDER, CoreConstants.Covariates.RACE});
         A.CallTo(() => _applicationController.Start<ISelectFilePresenter>()).Returns(_selectFilePresenter);
         sut = new PopulationExportTask(_applicationController, _entityPathResolver, _lazyLoadTask, _simModelExporter,
            _modelCoreSimulationMapper, _projectRetriever, _configuration, _simulationSettingsRetriever, _dialogCreator, _cloner);
      }
   }

   public class When_exporting_a_population_to_csv_and_the_user_cancels_the_action : concern_for_PopulationExportTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(_selectFilePresenter).WithReturnType<FileSelection>().Returns(null);
      }

      protected override void Because()
      {
         sut.ExportToCSV(_population);
      }

      [Observation]
      public void should_do_nothing()
      {
         A.CallTo(() => _entityPathResolver.PathFor(A<IEntity>.Ignored)).MustNotHaveHappened();
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
         A.CallTo(() => _modelCoreSimulationMapper.MapFrom(_populationSimulation, false)).Returns(_coreSimulation);
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
         File.Exists(Path.Combine(_clusterInputDirectory, $"{_populationSimulation.Name}{CoreConstants.Population.TABLE_PARAMETER_EXPORT}.csv")).ShouldBeTrue();
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

   public abstract class population_data_for_given_population : concern_for_PopulationExportTask
   {
      private List<IParameter> _allVectorialParameters;
      private List<IParameter> _allAdvancedParameters;
      private IParameter _constantParameter;
      private IParameter _formulaParameter1;
      private IParameter _formulaParameter2;
      protected DataTable _result;
      private Gender _male;
      private SpeciesPopulation _american;

      protected override void Context()
      {
         base.Context();
         _male = new Gender();
         _american = new SpeciesPopulation();
         _constantParameter = new PKSimParameter().WithName("P1").WithFormula(new ConstantFormula(1));
         _formulaParameter1 = new PKSimParameter().WithName("P2").WithFormula(new ExplicitFormula("1+1")).WithDimension(DomainHelperForSpecs.LengthDimensionForSpecs());
         _formulaParameter2 = new PKSimParameter().WithName("P3").WithFormula(new ExplicitFormula("1+2"));

         A.CallTo(() => _entityPathResolver.PathFor(A<IParameter>._)).ReturnsLazily(s => ((IParameter) s.Arguments[0]).Name);
         _allVectorialParameters = new List<IParameter>(new[] {_constantParameter, _formulaParameter1, _formulaParameter2});
         _allAdvancedParameters = new List<IParameter>(new[] {_constantParameter, _formulaParameter1});

         A.CallTo(() => _population.AllVectorialParameters(_entityPathResolver)).Returns(_allVectorialParameters);
         A.CallTo(() => _population.AllAdvancedParameters(_entityPathResolver)).Returns(_allAdvancedParameters);

         A.CallTo(() => _population.NumberOfItems).Returns(3);
         A.CallTo(() => _population.AllGenders).Returns(new[] {_male, _male, _male});
         A.CallTo(() => _population.AllRaces).Returns(new[] {_american, _american, _american});
      }

      protected bool ResultsHasParameter(string columnName)
      {
         return _result.Columns.Cast<DataColumn>().Any(col => Equals(col.ColumnName, columnName));
      }
   }

   public class When_creating_population_data_with_units : population_data_for_given_population
   {
      protected override void Because()
      {
         _result = sut.CreatePopulationDataFor(_population, includeUnitsInHeader: true);
      }

      [Observation]
      public void should_create_a_table_containing_the_values_for_all_constant_vectorial_parameters()
      {
         ResultsHasParameter("P1").ShouldBeTrue();
      }

      [Observation]
      public void should_create_a_table_containing_the_values_for_all_advanced_parameters_that_are_also_formula()
      {
         ResultsHasParameter(Constants.NameWithUnitFor("P2", "m")).ShouldBeTrue();
      }
   }

   public class When_creating_the_population_data_for_a_given_population : population_data_for_given_population
   {
      protected override void Because()
      {
         _result = sut.CreatePopulationDataFor(_population);
      }

      [Observation]
      public void should_create_a_table_containing_the_values_for_all_constant_vectorial_parameters()
      {
         ResultsHasParameter("P1").ShouldBeTrue();
      }

      [Observation]
      public void should_create_a_table_containing_the_values_for_all_advanced_parameters_that_are_also_formula()
      {
         ResultsHasParameter("P2").ShouldBeTrue();
      }

      [Observation]
      public void should_create_a_table_containing_that_does_not_contain_formula_parameters_that_are_not_advanced_parameters()
      {
         ResultsHasParameter("P3").ShouldBeFalse();
      }
   }
}