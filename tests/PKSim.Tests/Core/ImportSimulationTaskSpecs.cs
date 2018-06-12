using System;
using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Exchange;

namespace PKSim.Core
{
   public abstract class concern_for_ImportSimulationTask : ContextSpecification<IImportSimulationTask>
   {
      protected ISimulationTransferLoader _simulationTransferLoader;
      protected ISimulationFactory _simulationFactory;
      protected IEntitiesInContainerRetriever _parameterRetriever;
      protected ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      protected IIndividualPropertiesCacheImporter _individualPropertiesCacheImporter;
      protected IObjectBaseFactory _objectBaseFactory;
      protected IExecutionContext _executionContext;
      protected ISimulationUpdaterAfterDeserialization _simulationUpdaterAfterDeserialization;
      protected IAdvancedParameterFactory _advancedParameterFactory;

      protected const string _pkmlFile = "File";

      protected override void Context()
      {
         _simulationTransferLoader = A.Fake<ISimulationTransferLoader>();
         _simulationFactory = A.Fake<ISimulationFactory>();
         _parameterRetriever = A.Fake<IEntitiesInContainerRetriever>();
         _simulationBuildingBlockUpdater = A.Fake<ISimulationBuildingBlockUpdater>();
         _executionContext= A.Fake<IExecutionContext>();
         _individualPropertiesCacheImporter = A.Fake<IIndividualPropertiesCacheImporter>();
         _objectBaseFactory= A.Fake<IObjectBaseFactory>();
         _simulationUpdaterAfterDeserialization= A.Fake<ISimulationUpdaterAfterDeserialization>();
         _advancedParameterFactory= A.Fake<IAdvancedParameterFactory>();
         sut = new ImportSimulationTask(_simulationTransferLoader, _simulationFactory, _parameterRetriever,
            _simulationBuildingBlockUpdater, _individualPropertiesCacheImporter,_executionContext, _objectBaseFactory,
            _simulationUpdaterAfterDeserialization,_advancedParameterFactory);
      }
   }

   public class When_loading_a_simulation_from_an_invalid_pkml_file : concern_for_ImportSimulationTask
   {

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _simulationTransferLoader.Load(_pkmlFile)).Throws<Exception>();
      }

      [Observation]
      public void should_return_a_pksim_exception_stipulating_the_file_could_not_be_loaded()
      {
         The.Action(()=>sut.ImportIndividualSimulation(_pkmlFile)).ShouldThrowAn<PKSimException>();
      }
   }

   public class When_loading_a_simulation_from_valid_pkml_file : concern_for_ImportSimulationTask
   {
      private IndividualSimulation _individualSimulation;
      private IndividualSimulation _result;

      protected override void Context()
      {
         base.Context();
         var simTransfer=new SimulationTransfer();
         _individualSimulation= A.Fake<IndividualSimulation>();
         A.CallTo(() => _simulationTransferLoader.Load(_pkmlFile)).Returns(simTransfer);
         A.CallTo(() => _simulationFactory.CreateBasedOn<IndividualSimulation>(simTransfer.Simulation)).Returns(_individualSimulation);
      }

      protected override void Because()
      {
         _result = sut.ImportIndividualSimulation(_pkmlFile);
      }

      [Observation]
      public void should_return_a_simulation_created_based_on_the_model_defined_in_the_pkml_file()
      {
         _result.ShouldBeEqualTo(_individualSimulation);
      }

      [Observation]
      public void should_update_the_simulation_after_deserialization()
      {
         A.CallTo(() => _simulationUpdaterAfterDeserialization.UpdateSimulation(_individualSimulation)).MustHaveHappened();
      }
   }

   public class When_loading_a_simulation_from_valid_pkml_file_and_the_import_is_canceled : concern_for_ImportSimulationTask
   {
      private IndividualSimulation _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _simulationTransferLoader.Load(_pkmlFile)).Returns(null);
      }

      protected override void Because()
      {
         _result = sut.ImportIndividualSimulation(_pkmlFile);
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }
   }

   public class Whhen_importing_a_population_simulation_from_a_valid_pkml_file_using_an_existing_population_building_block : concern_for_ImportSimulationTask
   {
      private Population _population;
      private PopulationSimulationImport _simulationImport;
      private PopulationSimulation _populationSimulation;

      protected override void Context()
      {
         base.Context();
         _population= A.Fake<Population>();
         _populationSimulation= A.Fake<PopulationSimulation>();
         A.CallTo(_simulationFactory).WithReturnType<PopulationSimulation>().Returns(_populationSimulation);
      }

      protected override void Because()
      {
         _simulationImport = sut.ImportFromBuidlingBlock(_pkmlFile, _population);
      }

      [Observation]
      public void should_load_the_population()
      {
         A.CallTo(() => _executionContext.Load(_population)).MustHaveHappened();
      }

      [Observation]
      public void should_return_a_population_simulation()
      {
         _simulationImport.PopulationSimulation.ShouldBeEqualTo(_populationSimulation);
      }

      [Observation]
      public void should_have_set_the_population_in_the_simulation_from_the_given_template()
      {
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateUsedBuildingBlockInSimulationFromTemplate(_populationSimulation, _population, PKSimBuildingBlockType.SimulationSubject)).MustHaveHappened();
      }
   }

   public class Whhen_importing_a_population_simulation_from_a_valid_pkml_file_using_a_population_file: concern_for_ImportSimulationTask
   {
      private PopulationSimulationImport _simulationImport;
      private PopulationSimulation _populationSimulation;
      private IndividualPropertiesCache _individualPropertiesCache;
      private MoBiPopulation _mobiPopulation;
      private ParameterValuesCache _parameterValueCache;
      private readonly ParameterValues _value1 = new ParameterValues("Path1");
      private readonly ParameterValues _value2 = new ParameterValues("Path2");
      private readonly ParameterValues _value3 = new ParameterValues("PathAdvanced");
      private PathCache<IParameter> _patchCache;
      private AdvancedParameter _advancedParameterContainer;
      private const string _populationFile = "PopulationFile";

      protected override void Context()
      {
         base.Context();
         _individualPropertiesCache= A.Fake<IndividualPropertiesCache>();
         _patchCache=new PathCacheForSpecs<IParameter>();
         var individualParameter = A.Fake<IParameter>();
         A.CallTo(() => individualParameter.IsChangedByCreateIndividual).Returns(true);
         _patchCache.Add("Path1", individualParameter);
         
         var advancedParameter = A.Fake<IParameter>();
         A.CallTo(() => advancedParameter.IsChangedByCreateIndividual).Returns(false);
         advancedParameter.CanBeVariedInPopulation = true;
         _patchCache.Add("PathAdvanced", advancedParameter);

         A.CallTo(() => _individualPropertiesCache.AllParameterPaths()).Returns(new[] { "Path1", "Path2", "PathAdvanced" });
         A.CallTo(() => _individualPropertiesCache.ParameterValuesFor("Path1")).Returns(_value1);
         A.CallTo(() => _individualPropertiesCache.ParameterValuesFor("Path2")).Returns(_value2);
         A.CallTo(() => _individualPropertiesCache.ParameterValuesFor("PathAdvanced")).Returns(_value3);
         _populationSimulation = A.Fake<PopulationSimulation>();
         _parameterValueCache= A.Fake<ParameterValuesCache>();
         A.CallTo(() => _populationSimulation.ParameterValuesCache).Returns(_parameterValueCache);
         _mobiPopulation= A.Fake<MoBiPopulation>();
         A.CallTo(_simulationFactory).WithReturnType<PopulationSimulation>().Returns(_populationSimulation);
         A.CallTo(() => _objectBaseFactory.Create<MoBiPopulation>()).Returns(_mobiPopulation);
         A.CallTo(() => _individualPropertiesCacheImporter.ImportFrom(_populationFile, A<IImportLogger>._)).Returns(_individualPropertiesCache);
         A.CallTo(() => _parameterRetriever.ParametersFrom(_populationSimulation)).Returns(_patchCache);

         _advancedParameterContainer= new AdvancedParameter();
         A.CallTo(() => _advancedParameterFactory.Create(advancedParameter, DistributionTypes.Unknown)).Returns(_advancedParameterContainer);
      }

      protected override void Because()
      {
         _simulationImport = sut.ImportFromPopulationFile(_pkmlFile,_populationFile);
      }

      [Observation]
      public void should_return_a_population_simulation()
      {
         _simulationImport.PopulationSimulation.ShouldBeEqualTo(_populationSimulation);
      }

      [Observation]
      public void should_have_added_the_existing_parameter_by_path_as_population_parameters()
      {
         A.CallTo(() => _parameterValueCache.Add(_value1)).MustHaveHappened();
         A.CallTo(() => _parameterValueCache.Add(_value2)).MustNotHaveHappened();
         A.CallTo(() => _parameterValueCache.Add(_value3)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_possible_advanced_parameters_as_advanced_parameters_in_the_simulation_without_generating_new_values()
      {
         A.CallTo(() => _populationSimulation.AddAdvancedParameter(_advancedParameterContainer,false)).MustHaveHappened();
      }

      [Observation]
      public void should_have_logged_the_missing_parameter_by_path()
      {
         _simulationImport.Status.Is(NotificationType.Warning).ShouldBeTrue();
      }


      [Observation]
      public void should_have_set_the_population_in_the_simulation_from_the_given_template()
      {
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateUsedBuildingBlockInSimulationFromTemplate(_populationSimulation, _mobiPopulation, PKSimBuildingBlockType.SimulationSubject)).MustHaveHappened();
      }
   }

   public class Whhen_importing_a_population_simulation_from_a_valid_pkml_file_using_a_population_size : concern_for_ImportSimulationTask
   {
      private PopulationSimulationImport _simulationImport;
      private PopulationSimulation _populationSimulation;
      private MoBiPopulation _mobiPopulation;
      private PathCache<IParameter> _distributedParameters;
      private IDistributedParameter _parameter;
      private AdvancedParameter _advancedParameter;
      private const int _size = 10;
      private ParameterValuesCache _parameterValueCache;

      protected override void Context()
      {
         base.Context();
         _parameter= A.Fake<IDistributedParameter>();
         _distributedParameters=new PathCacheForSpecs<IParameter> {{"P1", _parameter}};
         _populationSimulation = A.Fake<PopulationSimulation>();
         _mobiPopulation = A.Fake<MoBiPopulation>();
         _advancedParameter= new AdvancedParameter();
         _parameterValueCache = A.Fake<ParameterValuesCache>();
         A.CallTo(() => _populationSimulation.ParameterValuesCache).Returns(_parameterValueCache);
         A.CallTo(_simulationFactory).WithReturnType<PopulationSimulation>().Returns(_populationSimulation);
         A.CallTo(() => _objectBaseFactory.Create<MoBiPopulation>()).Returns(_mobiPopulation);
         A.CallTo(() => _objectBaseFactory.Create<AdvancedParameter>()).Returns(_advancedParameter);
         A.CallTo(_parameterRetriever).WithReturnType<PathCache<IParameter>>().Returns(_distributedParameters);
      }

      protected override void Because()
      {
         _simulationImport = sut.ImportFromPopulationSize(_pkmlFile,_size);
      }

      [Observation]
      public void should_return_a_population_simulation()
      {
         _simulationImport.PopulationSimulation.ShouldBeEqualTo(_populationSimulation);
      }

      [Observation]
      public void should_have_set_the_loaded_as_population_parameters()
      {
         A.CallTo(() => _mobiPopulation.SetNumberOfItems(_size)).MustHaveHappened();
      }

      [Observation]
      public void should_have_set_the_population_in_the_simulation_from_the_given_template()
      {
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateUsedBuildingBlockInSimulationFromTemplate(_populationSimulation, _mobiPopulation, PKSimBuildingBlockType.SimulationSubject)).MustHaveHappened();
      }

      [Observation]
      public void should_have_created_some_random_values_for_each_distributed_parameters_defined_in_the_simulation()
      {
         A.CallTo(() => _parameterValueCache.SetValues("P1",A<IEnumerable<RandomValue>>._)).MustHaveHappened();
      }
   }
}	