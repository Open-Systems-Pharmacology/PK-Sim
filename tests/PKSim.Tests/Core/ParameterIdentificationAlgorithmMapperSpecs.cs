using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterIdentificationAlgorithmMapper : ContextSpecificationAsync<ParameterIdentificationAlgorithmMapper>
   {
      protected ExtendedPropertyMapper _extendedPropertyMapper;
      private IExtendedProperty _property1;
      protected ExtendedProperty _snapshotExtendedProperty;
      protected OptimizationAlgorithmProperties _algoProperties;
      protected OptimizationAlgorithm _snapshot;

      protected override Task Context()
      {
         _extendedPropertyMapper = A.Fake<ExtendedPropertyMapper>();
         _property1 = new ExtendedProperty<string> {Name = "Hello", Value = "Val", FullName = "This is full", Description = "Description"};
         _algoProperties = new OptimizationAlgorithmProperties("PROP") {_property1};

         sut = new ParameterIdentificationAlgorithmMapper(_extendedPropertyMapper);

         _snapshotExtendedProperty = new ExtendedProperty
         {
            Description = "HELLO",
            FullName = "This is the full name"
         };

         A.CallTo(() => _extendedPropertyMapper.MapToSnapshot(_property1)).Returns(_snapshotExtendedProperty);

         return _completed;
      }
   }

   public class When_mapping_a_parameter_identification_algorithm_to_snapshot : concern_for_ParameterIdentificationAlgorithmMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_algoProperties);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_expected_alogorithm_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_algoProperties.Name);
         _snapshot.Properties.ShouldContain(_snapshotExtendedProperty);
      }

      [Observation]
      public void should_have_removed_the_properties_coming_from_database_that_should_not_be_exported()
      {
         _snapshotExtendedProperty.Description.ShouldBeNull();
         _snapshotExtendedProperty.FullName.ShouldBeNull();
      }
   }

   public class When_mapping_a_parameter_identification_algorithm_snapshot_to_algoritm_properties : concern_for_ParameterIdentificationAlgorithmMapper
   {
      private OptimizationAlgorithmProperties _newAlgorithmProperties;
      private IExtendedProperty _newExtendedProperties;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_algoProperties);
         _newExtendedProperties = new ExtendedProperty<string> { Name = "Hello", Value = "Val", FullName = "This is full", Description = "Description" };

         A.CallTo(() => _extendedPropertyMapper.MapToModel(_snapshotExtendedProperty, A<SnapshotContext>._)).Returns(_newExtendedProperties);
      }

      protected override async Task Because()
      {
         _newAlgorithmProperties = await sut.MapToModel(_snapshot, new SnapshotContext());
      }

      [Observation]
      public void should_return_an_algorithm_properties_object_with_the_expected_properties()
      {
         _newAlgorithmProperties.Name.ShouldBeEqualTo(_algoProperties.Name);
         _newAlgorithmProperties.ShouldContain(_newExtendedProperties);
      }
   }
}