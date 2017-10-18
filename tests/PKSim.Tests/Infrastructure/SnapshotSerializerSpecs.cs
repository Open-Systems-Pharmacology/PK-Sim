using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Services;
using PKSim.Infrastructure.Serialization.Json;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SnapshotSerializer : ContextSpecificationAsync<ISnapshotSerializer>
   {
      protected Parameter _parameter;
      protected string _fileName;
      protected Parameter _deserialiedParameter;

      public override async Task GlobalContext()
      {
         await base.GlobalContext();
         _fileName = FileHelper.GenerateTemporaryFileName();
      }

      protected override Task Context()
      {
         _parameter = new Parameter();
         sut = new SnapshotSerializer();
         return _completed;
      }

      protected override async Task Because()
      {
         await sut.Serialize(_parameter, _fileName);
         _deserialiedParameter = (await sut.DeserializeAsArray(_fileName, typeof(Parameter))).Cast<Parameter>().First();
      }

      public override async Task GlobalCleanup()
      {
         await base.GlobalCleanup();
         FileHelper.DeleteFile(_fileName);
      }
   }

   public class When_serializing_a_parameter_with_a_value_not_set : concern_for_SnapshotSerializer
   {
      protected override async Task Context()
      {
         await base.Context();
         _parameter.Value = null;
      }

      [Observation]
      public void should_deserialize_the_parmaeter_with_a_value_also_not_set()
      {
         _deserialiedParameter.Value.ShouldBeNull();
      }
   }

   public class When_serialziing_a_parameter_with_a_value_set_with_a_mantis_of_length_less_than_the_precision : concern_for_SnapshotSerializer
   {
      protected override async Task Context()
      {
         await base.Context();
         _parameter.Value = 1.123456789;
      }

      [Observation]
      public void should_not_round_the_number()
      {
         _deserialiedParameter.Value.ShouldBeEqualTo(_parameter.Value);
      }
   }

   public class When_serialziing_a_parameter_with_a_value_set_with_a_mantis_of_length_less_than_the_precision_using_exponent_notation : concern_for_SnapshotSerializer
   {
      protected override async Task Context()
      {
         await base.Context();
         _parameter.Value = 1e-12;
      }

      [Observation]
      public void should_not_round_the_number()
      {
         _deserialiedParameter.Value.ShouldBeEqualTo(_parameter.Value);
      }
   }

   public class When_serialziing_a_parameter_with_a_very_small_value_and_with_a_mantis_of_length_less_than_the_precision : concern_for_SnapshotSerializer
   {
      protected override async Task Context()
      {
         await base.Context();
         _parameter.Value = 0.0000000142;
      }

      [Observation]
      public void should_not_round_the_number()
      {
         _deserialiedParameter.Value.ShouldBeEqualTo(_parameter.Value);
      }
   }

   public class When_serializing_a_parameter_whose_value_was_tainted_with_numerical_noise : concern_for_SnapshotSerializer
   {
      protected override async Task Context()
      {
         await base.Context();
         _parameter.Value = 449.9999999999991;
      }

      [Observation]
      public void should_round_the_value()
      {
         _deserialiedParameter.Value.ShouldBeEqualTo(450);
      }
   }
   
   public class When_serializing_a_parameter_whose_value_should_not_be_converted : concern_for_SnapshotSerializer
   {
      protected override async Task Context()
      {
         await base.Context();
         _parameter.Value = 449.9999999996;
      }

      [Observation]
      public void should_not_round_the_value()
      {
         _deserialiedParameter.Value.ShouldBeEqualTo(_parameter.Value);
      }
   }

   public class When_serializing_a_parameter_whose_value_with_sientific_notation_and_long_mantis : concern_for_SnapshotSerializer
   {
      protected override async Task Context()
      {
         await base.Context();
         _parameter.Value = 199.999999998E-2;
      }

      [Observation]
      public void should_round_the_value()
      {
         _deserialiedParameter.Value.ShouldBeEqualTo(2d);
      }
   }
   
   public class When_serializing_an_array_of_objects_to_json : concern_for_SnapshotSerializer
   {
      private IEnumerable<Parameter> _deserialiedParameters;

      protected override async Task Because()
      {
         await sut.Serialize(new[] {_parameter, _parameter}, _fileName);
         _deserialiedParameters = (await sut.DeserializeAsArray(_fileName, typeof(Parameter))).Cast<Parameter>();
      }

      [Observation]
      public void should_be_able_to_retrievee_an_array_of_deserialized_objects()
      {
         _deserialiedParameters.Count().ShouldBeEqualTo(2);
      }
   }
}