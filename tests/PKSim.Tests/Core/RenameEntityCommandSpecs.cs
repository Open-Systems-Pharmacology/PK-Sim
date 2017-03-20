using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_rename_entity_command : ContextSpecification<RenameEntityCommand>
   {
      protected IEntity _entity;
      protected string _oldName;
      protected string _newName;
      protected IExecutionContext _executionContext;

      protected override void Context()
      {
         _oldName = "_oldName";
         _newName = "_newName";
         _executionContext = A.Fake<IExecutionContext>();
         _entity = A.Fake<IEntity>().WithName(_oldName);
         _entity.Id="tralalal";
         sut = new RenameEntityCommand(_entity, _newName, _executionContext);
      }
   }

   
   public class When_executing_the_rename_entity_command : concern_for_rename_entity_command
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_change_the_name_of_the_given_entity()
      {
         _entity.Name.ShouldBeEqualTo(_newName);
      }
   }

   
   public class When_executing_the_rename_entity_inverse_command : concern_for_rename_entity_command
   {
      private IReversibleCommand<IExecutionContext> _inverseCommand;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _executionContext.Get<IEntity>(_entity.Id)).Returns(_entity);
         sut.Execute(_executionContext);
         sut.RestoreExecutionData(_executionContext);

         _inverseCommand = sut.InverseCommand(_executionContext);
      }

      protected override void Because()
      {
         _inverseCommand.Execute(_executionContext);
      }

      [Observation]
      public void should_reset_the_name_of_the_entity_to_its_orginal_value()
      {
         _entity.Name.ShouldBeEqualTo(_oldName);
      }
   }
}