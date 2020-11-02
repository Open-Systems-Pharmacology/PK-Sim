using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;


namespace PKSim.Core
{
   public abstract class concern_for_set_schema_item_formulation_key_command : ContextSpecification<SetSchemaItemFormulationKeyCommand>
   {
      protected IExecutionContext _context;
      protected ISchemaItem _schemaItem;
      protected string _formulationKey;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         _schemaItem = A.Fake<ISchemaItem>();
         _formulationKey = "tralala";
         sut = new SetSchemaItemFormulationKeyCommand(_schemaItem,_formulationKey,_context);
      }
   }

   
   public class When_executing_the_set_schema_item_formulation_key_command : concern_for_set_schema_item_formulation_key_command
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_change_the_formulation_key_in_the_schema_item  ()
      {
         _schemaItem.FormulationKey.ShouldBeEqualTo(_formulationKey);     
      }
   }

   
   public class The_inverse_of_set_schema_item_formulation_key_command : concern_for_set_schema_item_formulation_key_command
   {
      private ICommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_a_set_schema_item_formulation_key_command()
      {
         _result.ShouldBeAnInstanceOf<SetSchemaItemFormulationKeyCommand>();
      }

      [Observation]
      public void should_have_beeen_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}	