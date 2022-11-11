using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_OutputMappingMapper : ContextSpecificationAsync<OutputMappingMapper>
   {
      protected OutputMapping _outputMapping;
      protected Snapshots.OutputMapping _snapshot;
      protected Simulation _simulation;
      protected IObserver _output;
      protected DataRepository _dataRepository;
      protected IOSPSuiteLogger _logger;
      protected PKSimProject _project;

      protected override Task Context()
      {
         sut = new OutputMappingMapper(_logger);

         _project = new PKSimProject();
         _simulation = A.Fake<Simulation>().WithName("S");
         _output = new Observer().WithName("OBS");
         _simulation.Model.Root = new Container {_output};
         _logger= A.Fake<IOSPSuiteLogger>();
         _dataRepository = DomainHelperForSpecs.ObservedData("OBS_DATA");
         _project.AddObservedData(_dataRepository);   
         _outputMapping = new OutputMapping
         {
            Scaling = Scalings.Log,
            Weight = 5,
            OutputSelection = new SimulationQuantitySelection(_simulation, new QuantitySelection(_output.Name, QuantityType.Observer)),
            WeightedObservedData = new WeightedObservedData(_dataRepository)
            {
               Weights =
               {
                  [1] = 2f
               }
            }
         };

         return _completed;
      }
   }

   public class When_mapping_output_mapping_to_snapshot : concern_for_OutputMappingMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_outputMapping);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.Scaling.ShouldBeEqualTo(_outputMapping.Scaling);
         _snapshot.Path.ShouldBeEqualTo(_outputMapping.FullOutputPath);
      }

      [Observation]
      public void should_have_mapped_the_global_weight_and_local_weight_and_observed_data()
      {
         _snapshot.Weight.ShouldBeEqualTo(_outputMapping.Weight);
         _snapshot.Weights.ShouldBeEqualTo(_outputMapping.WeightedObservedData.Weights);
         _snapshot.ObservedData.ShouldBeEqualTo(_outputMapping.WeightedObservedData.Name);
      }
   }

   public class When_mapping_output_mapping_to_snapshot_and_all_weights_are_the_default_weight : concern_for_OutputMappingMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _outputMapping.Weight = Constants.DEFAULT_WEIGHT;
         _outputMapping.WeightedObservedData.Weights[1] = Constants.DEFAULT_WEIGHT;
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_outputMapping);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.Scaling.ShouldBeEqualTo(_outputMapping.Scaling);
         _snapshot.Path.ShouldBeEqualTo(_outputMapping.FullOutputPath);
         _snapshot.ObservedData.ShouldBeEqualTo(_outputMapping.WeightedObservedData.Name);
      }

      [Observation]
      public void should_not_save_weights()
      {
         _snapshot.Weight.ShouldBeNull();
         _snapshot.Weights.ShouldBeNull();
      }
   }

   public class When_mapping_output_mapping_without_observed_data_to_snapshot : concern_for_OutputMappingMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _outputMapping.WeightedObservedData = null;
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_outputMapping);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.Scaling.ShouldBeEqualTo(_outputMapping.Scaling);
         _snapshot.Path.ShouldBeEqualTo(_outputMapping.FullOutputPath);
      }

      [Observation]
      public void should_not_have_any_information_regarding_obs_data_weight()
      {
         _snapshot.Weights.ShouldBeNull();
         _snapshot.ObservedData.ShouldBeNull();
      }
   }


   public class When_mapping_an_output_mapping_snapshot_to_model_in_a_simulation_context : concern_for_OutputMappingMapper
   {
      private SnapshotContext _simulationContext;
      private OutputMapping _newOutputMapping;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_outputMapping);
         _simulationContext = new SnapshotContextWithSimulation(_simulation, new SnapshotContext(_project, 1));
      }

      protected override async Task Because()
      {
         _newOutputMapping = await sut.MapToModel(_snapshot, _simulationContext);
      }

      [Observation]
      public void should_be_able_to_return_the_output_mapping_initialized()
      {
         _newOutputMapping.Simulation.ShouldBeEqualTo(_simulation);
         _newOutputMapping.FullOutputPath.ShouldBeEqualTo(_outputMapping.FullOutputPath);
      }
   }
}