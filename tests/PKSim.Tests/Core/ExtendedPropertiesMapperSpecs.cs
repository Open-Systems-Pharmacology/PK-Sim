using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using ExtendedProperties = OSPSuite.Core.Domain.ExtendedProperties;

namespace PKSim.Core
{
   public abstract class concern_for_ExtendedPropertiesMapper : ContextSpecification<ExtendedPropertiesMapper>
   {
      protected ExtendedPropertyMapper _extendedPropertyMapper;

      protected override void Context()
      {
         _extendedPropertyMapper = A.Fake<ExtendedPropertyMapper>();
         sut = new ExtendedPropertiesMapper(_extendedPropertyMapper);
      }
   }

   public class When_mapping_an_extended_properties_to_snapshot : concern_for_ExtendedPropertiesMapper
   {
      private ExtendedProperties _extendedProperties;
      private ExtendedProperty<string> _extendedProperty;
      private ExtendedProperty<string> _secondExtendedProperty;

      protected override void Context()
      {
         base.Context();
         _extendedProperty = new ExtendedProperty<string> { Description = "Description", FullName = "FullName", Name = "FirstName", ReadOnly = true, Value = "Value"};
         _secondExtendedProperty = new ExtendedProperty<string> { Description = "Description", FullName = "FullName", Name = "SecondName", ReadOnly = true, Value = "Value"};
         _extendedProperties = new ExtendedProperties { _extendedProperty, _secondExtendedProperty };
      }

      protected override void Because()
      {
         sut.MapToSnapshot(_extendedProperties);
      }

      [Observation]
      public void the_property_mapper_must_be_used_to_map_all_the_properties()
      {
         A.CallTo(() => _extendedPropertyMapper.MapToSnapshot(_extendedProperty)).MustHaveHappened();
         A.CallTo(() => _extendedPropertyMapper.MapToSnapshot(_secondExtendedProperty)).MustHaveHappened();
      }
   }

   public class When_mapping_snapshot_to_extended_properties : concern_for_ExtendedPropertiesMapper
   {
      private Snapshots.ExtendedProperties _snapshot;
      private ExtendedProperty _firstExtendedProperty;
      private ExtendedProperty _secondExtendedProperty;

      protected override void Context()
      {
         base.Context();
         _firstExtendedProperty = new ExtendedProperty {Description = "Property Description", DisplayName = "Display Name", FullName = "First Full Name", Name = "First Name", ReadOnly = true, Type = typeof(string), Value = "Value"};
         _secondExtendedProperty = new ExtendedProperty {Description = "Property Description", DisplayName = "Display Name", FullName = "Second Full Name", Name = "Second Name", ReadOnly = true, Type = typeof(string), Value = "Value"};
         _snapshot = new Snapshots.ExtendedProperties {Description = "A Description", Name = "A Name", ListOfExtendedProperties = new List<ExtendedProperty>
         {
            _firstExtendedProperty,
            _secondExtendedProperty
         }};

         A.CallTo(() => _extendedPropertyMapper.MapToModel(_firstExtendedProperty)).Returns(A.Fake<IExtendedProperty>().WithName(_firstExtendedProperty.FullName));
         A.CallTo(() => _extendedPropertyMapper.MapToModel(_secondExtendedProperty)).Returns(A.Fake<IExtendedProperty>().WithName(_secondExtendedProperty.FullName));
      }

      protected override void Because()
      {
         sut.MapToModel(_snapshot);
      }

      [Observation]
      public void the_property_mapper_should_be_used_to_map_all_the_properties()
      {
         A.CallTo(() => _extendedPropertyMapper.MapToModel(_firstExtendedProperty)).MustHaveHappened();
         A.CallTo(() => _extendedPropertyMapper.MapToModel(_secondExtendedProperty)).MustHaveHappened();
      }
   }
}
