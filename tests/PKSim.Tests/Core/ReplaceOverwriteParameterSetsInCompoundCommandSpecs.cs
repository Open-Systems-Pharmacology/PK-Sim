using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_ReplaceOverwriteParameterSetsInCompoundCommand : ContextSpecification<ReplaceOverwriteParameterSetsInCompoundCommand>
   {
      protected IExecutionContext _executionContext;
      protected ICloneManager _cloneManager;
      protected Compound _compound;
      protected OverwriteParameterSet _setUpdatedInPlace;
      protected OverwriteParameterSet _setRemoved;
      protected OverwriteParameterSet _sourceOfUpdatedSet;
      protected OverwriteParameterSet _sourceOfNewSet;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _cloneManager = A.Fake<ICloneManager>();
         A.CallTo(() => _executionContext.CloneManager).Returns(_cloneManager);
         A.CallTo(() => _cloneManager.Clone(A<OverwriteParameterSet>._)).ReturnsLazily(x => cloneOf(x.GetArgument<OverwriteParameterSet>(0)));
         A.CallTo(() => _cloneManager.Clone(A<ParameterValue>._)).ReturnsLazily(x => cloneOf(x.GetArgument<ParameterValue>(0)));

         _compound = new Compound { Name = "Aspirin", Id = "CompId" };
         _setUpdatedInPlace = setNamed("Human", "HumanId", 1.0);
         _setRemoved = setNamed("Rat", "RatId", 2.0);
         _compound.AddOverwriteParameterSet(_setUpdatedInPlace);
         _compound.AddOverwriteParameterSet(_setRemoved);

         _sourceOfUpdatedSet = setNamed("Human", "TemplateHumanId", 10.0);
         _sourceOfNewSet = setNamed("Dog", "TemplateDogId", 3.0);

         A.CallTo(() => _executionContext.Get<Compound>(_compound.Id)).Returns(_compound);

         sut = new ReplaceOverwriteParameterSetsInCompoundCommand(_compound, new List<OverwriteParameterSet> { _sourceOfUpdatedSet, _sourceOfNewSet });
      }

      private static OverwriteParameterSet setNamed(string name, string id, double value)
      {
         var set = new OverwriteParameterSet { Name = name, Id = id };
         set.Add(new ParameterValue { Path = "Organism|Aspirin|Lipophilicity".ToObjectPath(), Value = value });
         return set;
      }

      private OverwriteParameterSet cloneOf(OverwriteParameterSet overwriteParameterSet)
      {
         var clone = new OverwriteParameterSet { Id = $"CloneOf{overwriteParameterSet.Id}" };
         clone.UpdatePropertiesFrom(overwriteParameterSet, _cloneManager);
         return clone;
      }

      private static ParameterValue cloneOf(ParameterValue parameterValue) => new() { Path = parameterValue.Path, Value = parameterValue.Value };

      protected double valueIn(string setName) => _compound.OverwriteParameterSets.FindByName(setName).ParameterValueByPath("Organism|Aspirin|Lipophilicity").Value.Value;
   }

   public class When_replacing_the_overwrite_parameter_sets_of_a_compound : concern_for_ReplaceOverwriteParameterSetsInCompoundCommand
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_update_a_set_defined_in_both_compounds_in_place()
      {
         _compound.OverwriteParameterSets.FindByName("Human").ShouldBeEqualTo(_setUpdatedInPlace);
         valueIn("Human").ShouldBeEqualTo(10.0);
      }

      [Observation]
      public void should_add_a_copy_of_a_set_missing_in_the_compound()
      {
         var newSet = _compound.OverwriteParameterSets.FindByName("Dog");
         newSet.ShouldNotBeEqualTo(_sourceOfNewSet);
         valueIn("Dog").ShouldBeEqualTo(3.0);
         A.CallTo(() => _executionContext.Register(newSet)).MustHaveHappened();
      }

      [Observation]
      public void should_remove_a_set_missing_in_the_source()
      {
         _compound.OverwriteParameterSets.FindByName("Rat").ShouldBeNull();
      }

      [Observation]
      public void should_not_be_shown_in_the_history()
      {
         sut.Visible.ShouldBeFalse();
      }
   }

   public class When_undoing_the_replacement_of_the_overwrite_parameter_sets_of_a_compound : concern_for_ReplaceOverwriteParameterSetsInCompoundCommand
   {
      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_restore_the_previous_content_of_the_updated_set()
      {
         _compound.OverwriteParameterSets.FindByName("Human").ShouldBeEqualTo(_setUpdatedInPlace);
         valueIn("Human").ShouldBeEqualTo(1.0);
      }

      [Observation]
      public void should_restore_the_removed_set()
      {
         valueIn("Rat").ShouldBeEqualTo(2.0);
      }

      [Observation]
      public void should_remove_the_added_set()
      {
         _compound.OverwriteParameterSets.FindByName("Dog").ShouldBeNull();
      }
   }
}
