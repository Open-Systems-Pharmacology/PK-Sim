using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
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

      public override async Task GlobalCleanup()
      {
         await base.GlobalCleanup();
         FileHelper.DeleteFile(_fileName);
      }
   }

  
   public class When_serializing_a_parameter_with_a_value_set: concern_for_SnapshotSerializer
   {
      [TestCase(null)]
      [TestCase(1.123456789)]
      [TestCase(1e-12)]
      [TestCase(0.0000000142)]
      [TestCase(449.9999999996)]
      [TestCase(449.9999999999991, 450)]
      [TestCase(199.999998E-2)]
      [TestCase(199.999999998E-2, 2d)]
      public async Task should_serialized_the_number_using_the_expected_precision(double? originalValue, double? expectedValue=null)
      {
         _parameter.Value = originalValue;
         await sut.Serialize(_parameter, _fileName);
         _deserialiedParameter = (await sut.DeserializeAsArray(_fileName, typeof(Parameter))).Cast<Parameter>().First();

         var expectedDeserializedValue = expectedValue ?? originalValue;
         _deserialiedParameter.Value.ShouldBeEqualTo(expectedDeserializedValue);
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