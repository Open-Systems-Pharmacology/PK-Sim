using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using ExtendedProperties = OSPSuite.Core.Domain.ExtendedProperties;

namespace PKSim.Core
{
   public abstract class concern_for_ExtendedPropertiesMapper : ContextSpecificationAsync<ExtendedPropertiesMapper>
   {
      protected ExtendedPropertyMapper _extendedPropertyMapper;
      protected ExtendedProperty<string> _extendedProperty;
      protected ExtendedProperty<string> _secondExtendedProperty;
      protected ExtendedProperty _extendedPropertySnapshot;
      protected ExtendedProperty _secondExtendedPropertySnapshot;
      protected ExtendedProperties _extendedProperties;

      protected override Task Context()
      {
         _extendedPropertyMapper = A.Fake<ExtendedPropertyMapper>();
         sut = new ExtendedPropertiesMapper(_extendedPropertyMapper);
         _extendedProperty = new ExtendedProperty<string> {Description = "Description", FullName = "FirstFullName", Name = "FirstName", ReadOnly = true, Value = "Value"};
         _secondExtendedProperty = new ExtendedProperty<string> {Description = "Description", FullName = "SecondFullName", Name = "SecondName", ReadOnly = true, Value = "Value"};
         _extendedProperties = new ExtendedProperties {_extendedProperty, _secondExtendedProperty};

         _extendedPropertySnapshot = new ExtendedProperty {Name = "FirstName"};
         _secondExtendedPropertySnapshot = new ExtendedProperty {Name = "SecondName"};

         A.CallTo(() => _extendedPropertyMapper.MapToSnapshot(_extendedProperty)).Returns(_extendedPropertySnapshot);
         A.CallTo(() => _extendedPropertyMapper.MapToSnapshot(_secondExtendedProperty)).Returns(_secondExtendedPropertySnapshot);

         return _completed;
      }
   }

   public class When_mapping_a_snapshot_to_extended_properties : concern_for_ExtendedPropertiesMapper
   {
      private Snapshots.ExtendedProperties _snapshot;
      private ExtendedProperties _result;
      private ExtendedProperty<string> _modelExtendedProperty;
      private ExtendedProperty<string> _secondModelExtendedProperty;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_extendedProperties);
         _modelExtendedProperty = new ExtendedProperty<string> {Name = "FirstName"};
         _secondModelExtendedProperty = new ExtendedProperty<string> {Name = "SecondName"};
         A.CallTo(() => _extendedPropertyMapper.MapToModel(_extendedPropertySnapshot)).Returns(_modelExtendedProperty);
         A.CallTo(() => _extendedPropertyMapper.MapToModel(_secondExtendedPropertySnapshot)).Returns(_secondModelExtendedProperty);
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void the_extended_properties_should_contain_matching_properties_from_original()
      {
         _result["FirstName"].ShouldBeEqualTo(_modelExtendedProperty);
         _result["SecondName"].ShouldBeEqualTo(_secondModelExtendedProperty);
      }
   }

   public class When_mapping_an_extended_properties_to_snapshot : concern_for_ExtendedPropertiesMapper
   {
      private Snapshots.ExtendedProperties _snapshot;

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_extendedProperties);
      }

      [Observation]
      public void the_property_mapper_must_be_used_to_map_all_the_properties()
      {
         _snapshot.ShouldOnlyContain(_extendedPropertySnapshot, _secondExtendedPropertySnapshot);
      }
   }
}