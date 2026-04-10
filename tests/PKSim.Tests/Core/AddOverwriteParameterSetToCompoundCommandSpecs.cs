using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_AddOverwriteParameterSetToCompoundCommand : ContextSpecification<AddOverwriteParameterSetToCompoundCommand>
   {
      protected Compound _compound;
      protected OverwriteParameterSet _overwriteParameterSet;
      protected IExecutionContext _executionContext;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _compound = new Compound { Name = "Aspirin", Id = "CompId" };
         _overwriteParameterSet = new OverwriteParameterSet { Name = "MySet", Id = "SetId" };

         A.CallTo(() => _executionContext.BuildingBlockContaining(_compound)).Returns(_compound);
         A.CallTo(() => _executionContext.Get<Compound>(_compound.Id)).Returns(_compound);
         A.CallTo(() => _executionContext.Get<OverwriteParameterSet>(_overwriteParameterSet.Id)).Returns(_overwriteParameterSet);

         sut = new AddOverwriteParameterSetToCompoundCommand(_overwriteParameterSet, _compound);
      }
   }

   public class When_executing_the_add_overwrite_parameter_set_command : concern_for_AddOverwriteParameterSetToCompoundCommand
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_add_the_set_to_the_compound()
      {
         _compound.OverwriteParameterSets.ShouldContain(_overwriteParameterSet);
      }
   }

   public class When_undoing_the_add_overwrite_parameter_set_command : concern_for_AddOverwriteParameterSetToCompoundCommand
   {
      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_remove_the_set_from_the_compound()
      {
         _compound.OverwriteParameterSets.ShouldNotContain(_overwriteParameterSet);
      }
   }
}
