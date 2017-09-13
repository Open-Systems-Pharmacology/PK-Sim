using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_ExtendedPropertyMapper : ContextSpecification<ExtendedPropertyMapper>
   {

      protected override void Context()
      {
         sut = new ExtendedPropertyMapper();
      }
   }

   public abstract class When_mapping_extended_property_to_snapshot<T> : concern_for_ExtendedPropertyMapper
   {
      protected ExtendedProperty<T> _extendedProperty;
      protected ExtendedProperty _snapshot;

      protected abstract void CreateExtendedProperty();

      protected override void Context()
      {
         base.Context();
         CreateExtendedProperty();
      }

      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_extendedProperty);
      }

      [Observation]
      public void the_snapshot_should_have_properties_set_as_expected()
      {
         _snapshot.ReadOnly.ShouldBeEqualTo(_extendedProperty.ReadOnly);
         _snapshot.Value.ShouldBeEqualTo(_extendedProperty.ValueAsObject);
         _snapshot.Name.ShouldBeEqualTo(_extendedProperty.Name);
         _snapshot.FullName.ShouldBeEqualTo(_extendedProperty.FullName);
         _snapshot.Description.ShouldBeEqualTo(_extendedProperty.Description);
         _extendedProperty.ListOfValues.Each(option => _snapshot.ListOfValues.ShouldContain(option));
      }
   }

   public class When_mapping_extended_property_boolean_to_snapshot : When_mapping_extended_property_to_snapshot<bool>
   {
      protected override void CreateExtendedProperty()
      {
         _extendedProperty = new ExtendedProperty<bool> { Description = "Description", FullName = "FullName", Name = "FirstName", ReadOnly = true, Value = true };
         _extendedProperty.AddToListOfValues(true);
         _extendedProperty.AddToListOfValues(false);
      }
   }

   public class When_mapping_extended_property_double_to_snapshot : When_mapping_extended_property_to_snapshot<double>
   {
      protected override void CreateExtendedProperty()
      {
         _extendedProperty = new ExtendedProperty<double> { Description = "Description", FullName = "FullName", Name = "FirstName", ReadOnly = true, Value = 5.5 };
         _extendedProperty.AddToListOfValues(6.5);
         _extendedProperty.AddToListOfValues(7.5);
      }
   }

   public class When_mapping_extended_property_string_to_snapshot : When_mapping_extended_property_to_snapshot<string>
   {
      protected override void CreateExtendedProperty()
      {
         _extendedProperty = new ExtendedProperty<string> {Description = "Description", FullName = "FullName", Name = "FirstName", ReadOnly = true, Value = "Value"};
         _extendedProperty.AddToListOfValues("Option 1");
         _extendedProperty.AddToListOfValues("Option 2");
      }
   }

   public abstract class When_mapping_snapshot_to_extended_property : concern_for_ExtendedPropertyMapper
   {
      protected IExtendedProperty _extendedProperty;
      protected ExtendedProperty _snapshot;
      protected abstract void CreateSnapshot();

      protected override void Context()
      {
         base.Context();
         CreateSnapshot();
      }

      protected override void Because()
      {
         _extendedProperty = sut.MapToModel(_snapshot);
      }

      [Observation]
      public void the_model_should_have_properties_set_as_expected()
      {
         _extendedProperty.ReadOnly.ShouldBeEqualTo(_snapshot.ReadOnly);
         _extendedProperty.Description.ShouldBeEqualTo(_snapshot.Description);
         _extendedProperty.FullName.ShouldBeEqualTo(_snapshot.FullName);
         _extendedProperty.Name.ShouldBeEqualTo(_snapshot.Name);
         _extendedProperty.ValueAsObject.ShouldBeEqualTo(_snapshot.Value);

         _snapshot.ListOfValues.Each(snapshotOption => _extendedProperty.ListOfValuesAsObjects.ShouldContain(snapshotOption));
      }
   }

   public class When_mapping_snapshot_to_ExtendedProperty_string : When_mapping_snapshot_to_extended_property
   {
      protected override void CreateSnapshot()
      {
         _snapshot = new ExtendedProperty {Description = "Description", ReadOnly = true, FullName = "Full Name", ListOfValues = new List<object> {"option 1", "option 2"}, Name = "Name", Type = typeof(string), Value = "Value"};
      }
   }

   public class When_mapping_snapshot_to_extended_property_bool : When_mapping_snapshot_to_extended_property
   {
      protected override void CreateSnapshot()
      {
         _snapshot = new ExtendedProperty { Description = "Description", ReadOnly = true, FullName = "Full Name", ListOfValues = new List<object> { true, false }, Name = "Name", Type = typeof(bool), Value = false };
      }
   }

   public class When_mapping_snapshot_to_extended_property_object : When_mapping_snapshot_to_extended_property
   {
      protected override void CreateSnapshot()
      {
         _snapshot = new ExtendedProperty { Description = "Description", ReadOnly = true, FullName = "Full Name", ListOfValues = new List<object> { true, 6.5 }, Name = "Name", Type = typeof(object), Value = "string" };
      }
   }

   public class When_mapping_snapshot_to_extended_property_double : When_mapping_snapshot_to_extended_property
   {
      protected override void CreateSnapshot()
      {
         _snapshot = new ExtendedProperty { Description = "Description", ReadOnly = true, FullName = "Full Name", ListOfValues = new List<object> { 4.5, 6.5 }, Name = "Name", Type = typeof(double), Value = 5.5 };
      }
   }
}
