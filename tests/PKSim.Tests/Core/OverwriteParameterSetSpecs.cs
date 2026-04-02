using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_OverwriteParameterSet : ContextSpecification<OverwriteParameterSet>
   {
      protected override void Context()
      {
         sut = new OverwriteParameterSet { Name = "TestSet", IsDefault = false };
      }
   }

   public class When_adding_a_parameter_value : concern_for_OverwriteParameterSet
   {
      protected override void Because()
      {
         sut.Add(new ParameterValue { Path = "Path|To|Param".ToObjectPath(), Value = 1.0 });
      }

      [Observation]
      public void should_contain_the_parameter_value()
      {
         sut.ParameterValues.Count.ShouldBeEqualTo(1);
         sut.ParameterValues[0].Path.PathAsString.ShouldBeEqualTo("Path|To|Param");
      }
   }

   public class When_retrieving_a_parameter_value_by_path : concern_for_OverwriteParameterSet
   {
      private ParameterValue _result;

      protected override void Context()
      {
         base.Context();
         sut.Add(new ParameterValue { Path = "Path|To|Param".ToObjectPath(), Value = 1.0 });
      }

      protected override void Because()
      {
         _result = sut.ParameterValueByPath("Path|To|Param");
      }

      [Observation]
      public void should_return_the_matching_parameter_value()
      {
         _result.ShouldNotBeNull();
         _result.Value.ShouldBeEqualTo(1.0);
      }
   }

   public class When_retrieving_a_parameter_value_by_nonexistent_path : concern_for_OverwriteParameterSet
   {
      private ParameterValue _result;

      protected override void Because()
      {
         _result = sut.ParameterValueByPath("NonExistent");
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }
   }

   public class When_adding_a_parameter_value_with_an_existing_path : concern_for_OverwriteParameterSet
   {
      protected override void Context()
      {
         base.Context();
         sut.Add(new ParameterValue { Path = "Path|To|Param".ToObjectPath(), Value = 1.0 });
      }

      protected override void Because()
      {
         sut.Add(new ParameterValue { Path = "Path|To|Param".ToObjectPath(), Value = 2.0 });
      }

      [Observation]
      public void should_replace_the_existing_parameter_value()
      {
         sut.ParameterValues.Count.ShouldBeEqualTo(1);
         sut.ParameterValues[0].Value.ShouldBeEqualTo(2.0);
      }
   }

   public class When_removing_a_parameter_value : concern_for_OverwriteParameterSet
   {
      private ParameterValue _parameterValue;

      protected override void Context()
      {
         base.Context();
         _parameterValue = new ParameterValue { Path = "Path|To|Param".ToObjectPath(), Value = 1.0 };
         sut.Add(_parameterValue);
      }

      protected override void Because()
      {
         sut.Remove(_parameterValue);
      }

      [Observation]
      public void should_no_longer_contain_the_parameter_value()
      {
         sut.ParameterValues.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_updating_properties_from_another_overwrite_parameter_set : concern_for_OverwriteParameterSet
   {
      private OverwriteParameterSet _source;
      private ICloneManager _cloneManager;
      private ParameterValue _sourceParameterValue;
      private ParameterValue _clonedParameterValue;

      protected override void Context()
      {
         base.Context();
         _cloneManager = A.Fake<ICloneManager>();
         _source = new OverwriteParameterSet { Name = "SourceSet", IsDefault = true };
         _sourceParameterValue = new ParameterValue { Path = "Path|To|Param".ToObjectPath(), Value = 1.0 };
         _source.Add(_sourceParameterValue);
         _source.ExtendedProperties.Add(new ExtendedProperty<string> { Name = "Species", Value = "Human" });
         _clonedParameterValue = new ParameterValue { Path = "Path|To|Param".ToObjectPath(), Value = 1.0 };
         A.CallTo(() => _cloneManager.Clone(_sourceParameterValue)).Returns(_clonedParameterValue);
      }

      protected override void Because()
      {
         sut.UpdatePropertiesFrom(_source, _cloneManager);
      }

      [Observation]
      public void should_update_basic_properties()
      {
         sut.Name.ShouldBeEqualTo("SourceSet");
         sut.IsDefault.ShouldBeTrue();
      }

      [Observation]
      public void should_clone_parameter_values_using_clone_manager()
      {
         sut.ParameterValues.Count.ShouldBeEqualTo(1);
         sut.ParameterValues[0].ShouldBeEqualTo(_clonedParameterValue);
      }

      [Observation]
      public void should_update_extended_properties()
      {
         sut.ExtendedProperties["Species"].ValueAsObject.ShouldBeEqualTo("Human");
      }
   }
}
