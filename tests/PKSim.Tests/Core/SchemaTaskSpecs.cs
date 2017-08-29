using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SchemaTask : ContextSpecification<ISchemaTask>
   {
      private IExecutionContext _executionContext;
      private ISchemaItemFactory _schemaItemFactory;
      private ISchemaFactory _schemaFactory;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _schemaItemFactory = A.Fake<ISchemaItemFactory>();
         _schemaFactory =A.Fake<ISchemaFactory>();
         sut = new SchemaTask(_executionContext,_schemaFactory, _schemaItemFactory);
      }
   }


   
   public class When_the_schema_task_is_removing_a_schema_item_from_a_schema_containing_only_one_schema_item : concern_for_SchemaTask
   {
      private Schema _schema;
      private SchemaItem _schemaItem;

      protected override void Context()
      {
         base.Context();
         _schemaItem = A.Fake<SchemaItem>();
         _schema =A.Fake<Schema>();
         A.CallTo(() => _schema.SchemaItems).Returns(new[] { _schemaItem });
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.RemoveSchemaItemFrom(_schemaItem,_schema)).ShouldThrowAn<CannotDeleteSchemaItemException>();
      }
   }

   
   public class When_the_schema_task_is_removing_a_schema_from_protocol_containing_only_one_schema : concern_for_SchemaTask
   {
      private Schema _schema;
      private  PKSim.Core.Model.AdvancedProtocol _advancedProtocol;

      protected override void Context()
      {
         base.Context();
         _schema = A.Fake<Schema>();
         _advancedProtocol =A.Fake< PKSim.Core.Model.AdvancedProtocol>();
         A.CallTo(() => _advancedProtocol.AllSchemas).Returns(new[] {_schema});
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.RemoveSchemaFrom(_schema,_advancedProtocol)).ShouldThrowAn<CannotDeleteSchemaException>();
      }
   }

}