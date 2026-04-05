using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_UpdateOverwriteParameterSetCommand : ContextSpecification<UpdateOverwriteParameterSetCommand>
   {
      protected Compound _compound;
      protected OverwriteParameterSet _overwriteParameterSet;
      protected IExecutionContext _executionContext;
      protected ParameterValue _existingPV;
      protected ParameterValue _newPV;
      protected ParameterValue _replacementPV;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _compound = new Compound { Name = "Aspirin", Id = "CompId" };
         _overwriteParameterSet = new OverwriteParameterSet { Name = "MySet", Id = "SetId" };

         _existingPV = new ParameterValue { Path = "Organism|Aspirin|Lipophilicity".ToObjectPath(), Value = 1.0 };
         _overwriteParameterSet.Add(_existingPV);

         _compound.AddOverwriteParameterSet(_overwriteParameterSet);

         A.CallTo(() => _executionContext.BuildingBlockContaining(_compound)).Returns(_compound);
         A.CallTo(() => _executionContext.Get<Compound>(_compound.Id)).Returns(_compound);
      }
   }

   public class When_updating_an_overwrite_parameter_set_with_new_values : concern_for_UpdateOverwriteParameterSetCommand
   {
      protected override void Context()
      {
         base.Context();
         _newPV = new ParameterValue { Path = "Organism|Aspirin|Permeability".ToObjectPath(), Value = 5.0 };
         sut = new UpdateOverwriteParameterSetCommand(_overwriteParameterSet, _compound, new[] { _newPV }, new List<string>());
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_add_the_new_parameter_value()
      {
         _overwriteParameterSet.ParameterValueByPath("Organism|Aspirin|Permeability").ShouldNotBeNull();
         _overwriteParameterSet.ParameterValueByPath("Organism|Aspirin|Permeability").Value.ShouldBeEqualTo(5.0);
      }

      [Observation]
      public void should_keep_existing_values()
      {
         _overwriteParameterSet.ParameterValueByPath("Organism|Aspirin|Lipophilicity").ShouldNotBeNull();
      }
   }

   public class When_updating_an_overwrite_parameter_set_with_replacement_values : concern_for_UpdateOverwriteParameterSetCommand
   {
      protected override void Context()
      {
         base.Context();
         _replacementPV = new ParameterValue { Path = "Organism|Aspirin|Lipophilicity".ToObjectPath(), Value = 9.0 };
         sut = new UpdateOverwriteParameterSetCommand(_overwriteParameterSet, _compound, new[] { _replacementPV }, new List<string>());
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_replace_the_existing_value()
      {
         _overwriteParameterSet.ParameterValues.Count.ShouldBeEqualTo(1);
         _overwriteParameterSet.ParameterValueByPath("Organism|Aspirin|Lipophilicity").Value.ShouldBeEqualTo(9.0);
      }
   }

   public class When_updating_an_overwrite_parameter_set_with_paths_to_remove : concern_for_UpdateOverwriteParameterSetCommand
   {
      protected override void Context()
      {
         base.Context();
         sut = new UpdateOverwriteParameterSetCommand(_overwriteParameterSet, _compound, new List<ParameterValue>(), new[] { "Organism|Aspirin|Lipophilicity" });
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_remove_the_parameter()
      {
         _overwriteParameterSet.ParameterValues.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_undoing_an_update_overwrite_parameter_set_command : concern_for_UpdateOverwriteParameterSetCommand
   {
      protected override void Context()
      {
         base.Context();
         _newPV = new ParameterValue { Path = "Organism|Aspirin|Permeability".ToObjectPath(), Value = 5.0 };
         sut = new UpdateOverwriteParameterSetCommand(_overwriteParameterSet, _compound, new[] { _newPV }, new[] { "Organism|Aspirin|Lipophilicity" });
      }

      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_restore_the_original_parameter_values()
      {
         _overwriteParameterSet.ParameterValues.Count.ShouldBeEqualTo(1);
         _overwriteParameterSet.ParameterValueByPath("Organism|Aspirin|Lipophilicity").ShouldNotBeNull();
         _overwriteParameterSet.ParameterValueByPath("Organism|Aspirin|Lipophilicity").Value.ShouldBeEqualTo(1.0);
      }

      [Observation]
      public void should_not_contain_the_new_parameter()
      {
         _overwriteParameterSet.ParameterValueByPath("Organism|Aspirin|Permeability").ShouldBeNull();
      }
   }
}
