using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using ParameterIdentificationConfiguration = OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentificationConfiguration;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterIdentificationConfigurationMapper : ContextSpecificationAsync<ParameterIdentificationConfigurationMapper>
   {
      protected ParameterIdentificationConfiguration _parameterIdentificationConfiguration;
      protected Snapshots.ParameterIdentificationConfiguration _snapshot;
      protected OptimizationAlgorithmProperties _algoProperties;
      protected ExtendedPropertyMapper _extendedPropertyMapper;
      private IExtendedProperty _property1;
      protected Snapshots.ExtendedProperty _snapshotExtendedProperty;

      protected override Task Context()
      {
         _extendedPropertyMapper= A.Fake<ExtendedPropertyMapper>();
         sut = new ParameterIdentificationConfigurationMapper(_extendedPropertyMapper);

         _property1 = new ExtendedProperty<string> {Name = "Hello", Value = "Val"};
         _algoProperties = new OptimizationAlgorithmProperties("PROP") {_property1};

         _parameterIdentificationConfiguration = new ParameterIdentificationConfiguration
         {
            CalculateJacobian = true,
            LLOQMode = LLOQModes.OnlyObservedData,
            RemoveLLOQMode = RemoveLLOQModes.NoTrailing,
            RunMode = new StandardParameterIdentificationRunMode(),
            AlgorithmProperties = _algoProperties
         };

         _snapshotExtendedProperty = new ExtendedProperty();
         A.CallTo(() => _extendedPropertyMapper.MapToSnapshot(_property1)).Returns(_snapshotExtendedProperty);
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
      public void should_return_a_snapshot_having_the_expected_alogorithm_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_algoProperties.Name);
         _snapshot.Properties.ShouldContain(_snapshotExtendedProperty);
      }
   }
}	