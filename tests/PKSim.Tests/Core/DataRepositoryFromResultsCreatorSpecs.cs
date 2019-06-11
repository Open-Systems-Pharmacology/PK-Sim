using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_DataRepositoryFromResultsCreator : ContextSpecification<IDataRepositoryFromResultsCreator>
   {
      private IDataRepositoryTask _dataFactory;
      private IDimensionRepository _dimensionRepository;
      protected IndividualSimulation _simulation;
      protected DataRepository _dataRepository;
      protected SimulationResults _simulationResults;
      protected IndividualResults _individualResults;
      protected QuantityValues _timeValues;
      private OutputSelections _outputSelection;

      protected override void Context()
      {
         _dataFactory = A.Fake<IDataRepositoryTask>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         sut = new DataRepositoryFromResultsCreator(
            _dimensionRepository, new ObjectPathFactoryForSpecs(), _dataFactory);

         _simulation = new IndividualSimulation().WithName("S");
         _simulation.Model = new OSPSuite.Core.Domain.Model();
         var root = new Container().WithContainerType(ContainerType.Simulation).WithName(_simulation.Name);
         _simulation.Model.Root = root;
         var liver = new Container().WithName("LIVER");
         liver.Add(new Observer().WithName("C"));
         var kidney = new Container().WithName("KIDNEY");
         kidney.Add(new Observer().WithName("C"));
         root.Add(liver);
         root.Add(kidney);

         _simulationResults = new SimulationResults();
         _simulation.Results = _simulationResults;
         _timeValues = new QuantityValues { QuantityPath = "Time", Values = new[] { 1f, 2f, 3f } };
         _simulationResults.Time = _timeValues;

         _individualResults = new IndividualResults();
         _individualResults.Add(new QuantityValues { QuantityPath = "LIVER|C", Time = _timeValues, Values = new[] { 10f, 20f, 30f } });
         _individualResults.Add(new QuantityValues {QuantityPath = "KIDNEY|C", Time = _timeValues, Values = new[] {11f, 22f, 33f}});

         _outputSelection = new OutputSelections();
         _outputSelection.AddOutput(new QuantitySelection(new[] { "LIVER", "C" }.ToPathString(), QuantityType.Molecule));

         _simulation.SimulationSettings = new SimulationSettings();
         _simulation.SimulationSettings.OutputSelections = _outputSelection;
      }

      protected override void Because()
      {
         _dataRepository = sut.CreateResultsFor(_simulation);
      }
   }

   public class When_creating_a_data_repository_from_empty_results : concern_for_DataRepositoryFromResultsCreator
   {
      [Observation]
      public void should_return_an_empty_data_repository()
      {
         _dataRepository.Any().ShouldBeFalse();
      }
   }

   public class When_creating_a_data_repository_for_results_that_can_be_resolved : concern_for_DataRepositoryFromResultsCreator
   {
      protected override void Context()
      {
         base.Context();
         _simulationResults.Add(_individualResults);
      }

      [Observation]
      public void should_return_a_data_repository_containing_one_column_for_each_available_results()
      {
         _dataRepository.Any().ShouldBeTrue();
         var liverConc = _dataRepository.Find(x => x.QuantityInfo.PathAsString == "S|LIVER|C");
         liverConc.ShouldNotBeNull();
         liverConc.Values.ShouldOnlyContainInOrder(10f, 20f, 30f);

         var kidneyConc = _dataRepository.Find(x => x.QuantityInfo.PathAsString == "S|KIDNEY|C");
         kidneyConc.ShouldNotBeNull();
         kidneyConc.Values.ShouldOnlyContainInOrder(11f, 22f, 33f);
      }

      [Observation]
      public void should_have_updated_the_internal_column_states()
      {
         var liverConc = _dataRepository.Find(x => x.QuantityInfo.PathAsString == "S|LIVER|C");
         liverConc.IsInternal.ShouldBeFalse();

         var kidneyConc = _dataRepository.Find(x => x.QuantityInfo.PathAsString == "S|KIDNEY|C");
         kidneyConc.IsInternal.ShouldBeTrue();
      }
   }

   public class When_creating_a_data_repository_for_results_that_cannot_be_resolved : concern_for_DataRepositoryFromResultsCreator
   {
      protected override void Context()
      {
         base.Context();
         _simulationResults.Add(_individualResults);
         _individualResults.Add(new QuantityValues {QuantityPath = "DOES NOT EXIST|C", Time = _timeValues, Values = new[] {10f, 20f, 30f}});
      }

      [Observation]
      public void should_not_crash()
      {
         _dataRepository.Any().ShouldBeTrue();
      }
   }

   public class When_updating_the_internal_status_of_a_data_repository_for_a_given_simulation : concern_for_DataRepositoryFromResultsCreator
   {
      [Observation]
      public void Observation()
      {
         
      }
   }
}