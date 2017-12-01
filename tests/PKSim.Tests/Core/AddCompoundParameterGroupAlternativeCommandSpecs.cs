using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_AddParameterAlternativeCommand : ContextSpecification<AddParameterAlternativeCommand>
   {
      protected ParameterAlternativeGroup _compoundParameterGroup;
      protected IExecutionContext _executionContext;
      protected ParameterAlternative _alternativeToAdd;
      protected ParameterAlternative _alternative2;
      protected PKSimProject _project;

      protected override void Context()
      {
         _project = new PKSimProject();

         _executionContext = A.Fake<IExecutionContext>();
         A.CallTo(() => _executionContext.CurrentProject).Returns(_project);
         _compoundParameterGroup = new ParameterAlternativeGroup().WithId("abcd");
         A.CallTo(() => _executionContext.Get<ParameterAlternativeGroup>(_compoundParameterGroup.Id)).Returns(_compoundParameterGroup);

         _alternativeToAdd = new ParameterAlternative().WithName("P1").WithId("99");

         A.CallTo(() => _executionContext.Get<ParameterAlternative>(_alternativeToAdd.Id)).Returns(_alternativeToAdd);
         _alternative2 = new ParameterAlternative().WithName("P2").WithId("100");

         _compoundParameterGroup.AddAlternative(_alternative2);

         //setup serialization manager for serialize/deserialize of _groupAlternativeToAdd
         var serializedStream = new byte[1];
         A.CallTo(() => _executionContext.Serialize(_alternativeToAdd)).Returns(serializedStream);
         A.CallTo(() => _executionContext.Deserialize<ParameterAlternative>(serializedStream)).Returns(_alternativeToAdd);

         sut = new AddParameterAlternativeCommand(_alternativeToAdd, _compoundParameterGroup, _executionContext);
      }
   }

   public class When_executing_add_parameter_alternative_to_command : concern_for_AddParameterAlternativeCommand
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void project_should_contain_only_given_compound()
      {
         _compoundParameterGroup.AllAlternatives.ShouldOnlyContain(_alternative2, _alternativeToAdd);
      }
   }

   public class When_executing_add_parameter_alternative_inverse_command : concern_for_AddParameterAlternativeCommand
   {
      protected IReversibleCommand<IExecutionContext> _inverseCommand;

      protected override void Context()
      {
         base.Context();
         sut.Execute(_executionContext);
         sut.RestoreExecutionData(_executionContext);

         _inverseCommand = sut.InverseCommand(_executionContext);
      }

      protected override void Because()
      {
         _inverseCommand.Execute(_executionContext);
         _inverseCommand.RestoreExecutionData(_executionContext);
      }

      [Observation]
      public void inverse_command_should_be_inverse_for_add_command()
      {
         _inverseCommand.IsInverseFor(sut).ShouldBeTrue();
      }

      [Observation]
      public void inverse_command_should_remove_only_added_compound()
      {
         _compoundParameterGroup.AllAlternatives.ShouldOnlyContain(_alternative2);
      }
   }

   public class When_undoing_add_parameter_alternative_inverse_command : concern_for_AddParameterAlternativeCommand
   {
      protected IReversibleCommand<IExecutionContext> _inverseCommand;
      protected IReversibleCommand<IExecutionContext> _redoCommand;

      protected override void Context()
      {
         base.Context();
         sut.Execute(_executionContext);
         sut.RestoreExecutionData(_executionContext);

         _inverseCommand = sut.InverseCommand(_executionContext);
         _inverseCommand.Execute(_executionContext);

         _inverseCommand.RestoreExecutionData(_executionContext);
         _redoCommand = _inverseCommand.InverseCommand(_executionContext);
      }

      protected override void Because()
      {
         _redoCommand.Execute(_executionContext);
      }

      [Observation]
      public void redo_command_should_restore_state_before_undo()
      {
         _compoundParameterGroup.AllAlternatives.ShouldOnlyContain(_alternative2, _alternativeToAdd);
      }
   }
}