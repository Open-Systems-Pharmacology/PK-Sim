using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SchemaItemFactory : ContextSpecification<ISchemaItemFactory>
   {
      protected IObjectBaseFactory _objectBaseFactory;
      protected ISchemaItemParameterRetriever _schemaItemParameterRetriever;
      protected IContainerTask _containerTask;
      protected ICloner _cloner;

      protected override void Context()
      {
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _schemaItemParameterRetriever = A.Fake<ISchemaItemParameterRetriever>();
         _containerTask = A.Fake<IContainerTask>();
         _cloner = A.Fake<ICloner>();
         sut = new SchemaItemFactory(_objectBaseFactory, _schemaItemParameterRetriever, _containerTask, _cloner);
         A.CallTo(() => _objectBaseFactory.Create<SchemaItem>()).Returns(new SchemaItem());
      }
   }

   public class When_creating_a_schema_item_for_a_given_application_type : concern_for_SchemaItemFactory
   {
      private IContainer _container;
      private ISchemaItem _schemaItem;
      private IParameter _p1;
      private IParameter _p2;

      protected override void Context()
      {
         base.Context();
         _container = new Container();
         _p1 = A.Fake<IParameter>().WithName("P1");
         _p2 = A.Fake<IParameter>().WithName("P2");
         A.CallTo(() => _schemaItemParameterRetriever.AllParametersFor(ApplicationTypes.Oral)).Returns(new[] {_p1, _p2});
      }

      protected override void Because()
      {
         _schemaItem = sut.Create(ApplicationTypes.Oral, _container);
      }

      [Observation]
      public void should_return_a_schema_item_with_the_expected_application_type()
      {
         _schemaItem.ApplicationType.ShouldBeEqualTo(ApplicationTypes.Oral);
      }

      [Observation]
      public void should_clear_the_default_formulation_key()
      {
         _schemaItem.FormulationKey.IsNullOrEmpty().ShouldBeTrue();
      }

      [Observation]
      public void should_have_added_the_default_parameter_for_the_given_application_type()
      {
         _schemaItem.Parameter("P1").ShouldBeEqualTo(_p1);
         _schemaItem.Parameter("P2").ShouldBeEqualTo(_p2);
      }
   }

   public class When_creating_a_schema_item_based_on_a_given_schema_item : concern_for_SchemaItemFactory
   {
      private IContainer _container;
      private SchemaItem _schemaItemToClone;
      private SchemaItem _schemaItem;
      private SchemaItem _clone;

      protected override void Context()
      {
         base.Context();
         _container = new Container();
         _schemaItemToClone = A.Fake<SchemaItem>();
         _clone = A.Fake<SchemaItem>();
         A.CallTo(() => _cloner.Clone(_schemaItemToClone)).Returns(_clone);
         A.CallTo(_containerTask).WithReturnType<string>().Returns("NEW NAME");
      }

      protected override void Because()
      {
         _schemaItem = sut.CreateBasedOn(_schemaItemToClone, _container);
      }

      [Observation]
      public void should_create_a_clone_of_the_given_schema_iten()
      {
         _schemaItem.ShouldBeEqualTo(_clone);
      }


      [Observation]
      public void should_use_the_container_task_to_retrieve_a_unique_name_for_the_schema_item()
      {
         _schemaItem.Name.ShouldBeEqualTo("NEW NAME");
      }
   }
}