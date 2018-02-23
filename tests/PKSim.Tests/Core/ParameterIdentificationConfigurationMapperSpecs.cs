using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.ParameterIdentifications;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using ParameterIdentificationConfiguration = OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentificationConfiguration;
using ParameterIdentificationRunMode = PKSim.Core.Snapshots.ParameterIdentificationRunMode;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterIdentificationConfigurationMapper : ContextSpecificationAsync<ParameterIdentificationConfigurationMapper>
   {
      protected ParameterIdentificationConfiguration _parameterIdentificationConfiguration;
      protected Snapshots.ParameterIdentificationConfiguration _snapshot;
      protected OptimizationAlgorithmProperties _algoProperties;
      private ParameterIdentificationRunModeMapper _parameterIdentificationRunModeMapper;
      protected ParameterIdentificationRunMode _snapshotRunMode;
      private ParameterIdentificationAlgorithmMapper _algorithmMapper;
      protected OptimizationAlgorithm _snapshotAlgorithmProperties;

      protected override Task Context()
      {
         _parameterIdentificationRunModeMapper = A.Fake<ParameterIdentificationRunModeMapper>();
         _algorithmMapper = A.Fake<ParameterIdentificationAlgorithmMapper>();
         sut = new ParameterIdentificationConfigurationMapper(_parameterIdentificationRunModeMapper, _algorithmMapper);

         _snapshotRunMode = new ParameterIdentificationRunMode();
         _snapshotAlgorithmProperties = new OptimizationAlgorithm();
         _parameterIdentificationConfiguration = new ParameterIdentificationConfiguration
         {
            CalculateJacobian = true,
            LLOQMode = LLOQModes.OnlyObservedData,
            RemoveLLOQMode = RemoveLLOQModes.NoTrailing,
            RunMode = new StandardParameterIdentificationRunMode(),
            AlgorithmProperties = new OptimizationAlgorithmProperties("HELLO")
         };

         A.CallTo(() => _parameterIdentificationRunModeMapper.MapToSnapshot(_parameterIdentificationConfiguration.RunMode)).Returns(_snapshotRunMode);
         A.CallTo(() => _algorithmMapper.MapToSnapshot(_parameterIdentificationConfiguration.AlgorithmProperties)).Returns(_snapshotAlgorithmProperties);
         return _completed;
      }
   }

   public class When_mapping_a_parameter_identification_configuration_to_snapshot : concern_for_ParameterIdentificationConfigurationMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_parameterIdentificationConfiguration);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_expected_properties()
      {
         _snapshot.LLOQMode.ShouldBeEqualTo(LLOQModes.OnlyObservedData.Name);
         _snapshot.RemoveLLOQMode.ShouldBeEqualTo(RemoveLLOQModes.NoTrailing.Name);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_mapped_algorithm_properties()
      {
         _snapshot.Algorithm.ShouldBeEqualTo(_snapshotAlgorithmProperties);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_mapped_run_mode()
      {
         _snapshot.RunMode.ShouldBeEqualTo(_snapshotRunMode);
      }
   }
}