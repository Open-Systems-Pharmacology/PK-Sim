using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_SetDefaultOverwriteParameterSetCommand : ContextSpecification<SetDefaultOverwriteParameterSetCommand>
   {
      protected Compound _compound;
      protected OverwriteParameterSet _previousDefault;
      protected OverwriteParameterSet _newDefault;
      protected OverwriteParameterSet _otherSet;
      protected IExecutionContext _executionContext;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _compound = new Compound { Name = "Aspirin", Id = "CompId" };
         _previousDefault = new OverwriteParameterSet { Name = "PreviousDefault", Id = "PreviousId", IsDefault = true };
         _newDefault = new OverwriteParameterSet { Name = "NewDefault", Id = "NewId" };
         _otherSet = new OverwriteParameterSet { Name = "Other", Id = "OtherId" };
         _compound.AddOverwriteParameterSet(_previousDefault);
         _compound.AddOverwriteParameterSet(_newDefault);
         _compound.AddOverwriteParameterSet(_otherSet);

         A.CallTo(() => _executionContext.BuildingBlockContaining(_compound)).Returns(_compound);
         A.CallTo(() => _executionContext.Get<Compound>(_compound.Id)).Returns(_compound);
         A.CallTo(() => _executionContext.Get<OverwriteParameterSet>(_previousDefault.Id)).Returns(_previousDefault);
         A.CallTo(() => _executionContext.Get<OverwriteParameterSet>(_newDefault.Id)).Returns(_newDefault);
         A.CallTo(() => _executionContext.Get<OverwriteParameterSet>(_otherSet.Id)).Returns(_otherSet);

         sut = new SetDefaultOverwriteParameterSetCommand(_newDefault, _compound);
      }
   }

   public class When_executing_the_set_default_command_with_a_previous_default : concern_for_SetDefaultOverwriteParameterSetCommand
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_set_is_default_on_the_target_set()
      {
         _newDefault.IsDefault.ShouldBeTrue();
      }

      [Observation]
      public void should_clear_is_default_on_the_previously_default_set()
      {
         _previousDefault.IsDefault.ShouldBeFalse();
      }

      [Observation]
      public void should_leave_the_other_non_default_set_unchanged()
      {
         _otherSet.IsDefault.ShouldBeFalse();
      }

      [Observation]
      public void should_publish_an_overwrite_parameter_set_changed_event()
      {
         A.CallTo(() => _executionContext.PublishEvent(A<OverwriteParameterSetChangedEvent>.That.Matches(x => x.Compound == _compound && x.OverwriteParameterSet == _newDefault))).MustHaveHappened();
      }
   }

   public class When_undoing_the_set_default_command_with_a_previous_default : concern_for_SetDefaultOverwriteParameterSetCommand
   {
      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_restore_the_previously_default_set()
      {
         _previousDefault.IsDefault.ShouldBeTrue();
      }

      [Observation]
      public void should_clear_the_new_default()
      {
         _newDefault.IsDefault.ShouldBeFalse();
      }
   }

   public abstract class concern_for_SetDefaultOverwriteParameterSetCommand_with_no_previous_default : ContextSpecification<SetDefaultOverwriteParameterSetCommand>
   {
      protected Compound _compound;
      protected OverwriteParameterSet _newDefault;
      protected IExecutionContext _executionContext;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _compound = new Compound { Name = "Aspirin", Id = "CompId" };
         _newDefault = new OverwriteParameterSet { Name = "NewDefault", Id = "NewId" };
         _compound.AddOverwriteParameterSet(_newDefault);

         A.CallTo(() => _executionContext.BuildingBlockContaining(_compound)).Returns(_compound);
         A.CallTo(() => _executionContext.Get<Compound>(_compound.Id)).Returns(_compound);
         A.CallTo(() => _executionContext.Get<OverwriteParameterSet>(_newDefault.Id)).Returns(_newDefault);

         sut = new SetDefaultOverwriteParameterSetCommand(_newDefault, _compound);
      }
   }

   public class When_undoing_the_set_default_command_with_no_previous_default : concern_for_SetDefaultOverwriteParameterSetCommand_with_no_previous_default
   {
      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_clear_is_default_on_the_target_set()
      {
         _newDefault.IsDefault.ShouldBeFalse();
      }
   }

   public abstract class concern_for_ClearDefaultOverwriteParameterSetCommand : ContextSpecification<ClearDefaultOverwriteParameterSetCommand>
   {
      protected Compound _compound;
      protected OverwriteParameterSet _defaultSet;
      protected IExecutionContext _executionContext;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _compound = new Compound { Name = "Aspirin", Id = "CompId" };
         _defaultSet = new OverwriteParameterSet { Name = "DefaultSet", Id = "DefaultId", IsDefault = true };
         _compound.AddOverwriteParameterSet(_defaultSet);

         A.CallTo(() => _executionContext.BuildingBlockContaining(_compound)).Returns(_compound);
         A.CallTo(() => _executionContext.Get<Compound>(_compound.Id)).Returns(_compound);
         A.CallTo(() => _executionContext.Get<OverwriteParameterSet>(_defaultSet.Id)).Returns(_defaultSet);

         sut = new ClearDefaultOverwriteParameterSetCommand(_defaultSet, _compound);
      }
   }

   public class When_executing_the_clear_default_command : concern_for_ClearDefaultOverwriteParameterSetCommand
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_clear_is_default_on_the_target_set()
      {
         _defaultSet.IsDefault.ShouldBeFalse();
      }

      [Observation]
      public void should_publish_an_overwrite_parameter_set_changed_event()
      {
         A.CallTo(() => _executionContext.PublishEvent(A<OverwriteParameterSetChangedEvent>.That.Matches(x => x.Compound == _compound && x.OverwriteParameterSet == _defaultSet))).MustHaveHappened();
      }
   }

   public class When_undoing_the_clear_default_command : concern_for_ClearDefaultOverwriteParameterSetCommand
   {
      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_restore_is_default_on_the_target_set()
      {
         _defaultSet.IsDefault.ShouldBeTrue();
      }
   }
}
