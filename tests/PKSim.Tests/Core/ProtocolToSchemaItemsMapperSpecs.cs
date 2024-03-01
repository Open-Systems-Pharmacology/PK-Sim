using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ProtocolToSchemaItemsMapper : ContextSpecification<IProtocolToSchemaItemsMapper>
   {
      private ISimpleProtocolToSchemaMapper _simpleMapper;
      private ICloner _cloneManager;

      protected override void Context()
      {
         _simpleMapper = A.Fake<ISimpleProtocolToSchemaMapper>();
         _cloneManager = A.Fake<ICloner>();
         sut = new ProtocolToSchemaItemsMapper(_simpleMapper, _cloneManager);
      }
   }

   public class When_mapping_a_null_protocol_to_schema_item : concern_for_ProtocolToSchemaItemsMapper
   {
      [Observation]
      public void should_return_an_empty_list_of_schema_items()
      {
         sut.MapFrom(null).ShouldBeEmpty();
      }
   }
}