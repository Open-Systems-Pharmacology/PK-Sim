using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_OverwriteParameterSetApplicationTask : ContextSpecification<OverwriteParameterSetApplicationTask>
   {
      protected IContainerTask _containerTask;
      protected IEntityPathResolver _entityPathResolver;
      protected IParameterValuesCreator _parameterValuesCreator;
      protected IndividualSimulation _simulation;
      protected Compound _compound;
      protected IParameter _lipophilicityParam;
      protected IParameter _permeabilityParam;
      protected PathCache<IParameter> _parameterCache;

      protected override void Context()
      {
         _containerTask = A.Fake<IContainerTask>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _parameterValuesCreator = A.Fake<IParameterValuesCreator>();

         _compound = new Compound { Name = "Aspirin", Id = "CompId" };

         _lipophilicityParam = DomainHelperForSpecs.ConstantParameterWithValue(3.5).WithName("Lipophilicity");
         _lipophilicityParam.BuildingBlockType = PKSimBuildingBlockType.Simulation;
         _permeabilityParam = DomainHelperForSpecs.ConstantParameterWithValue(7.2).WithName("Permeability");
         _permeabilityParam.BuildingBlockType = PKSimBuildingBlockType.Simulation;

         var root = new Container { Name = "Sim" };
         root.Add(_lipophilicityParam);
         root.Add(_permeabilityParam);

         _simulation = new IndividualSimulation
         {
            Id = "SimId",
            Model = new OSPSuite.Core.Domain.Model { Root = root }
         };
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("TemplateCompId", PKSimBuildingBlockType.Compound) { BuildingBlock = _compound });

         _parameterCache = new PathCacheForSpecs<IParameter>();
         _parameterCache.Add("Organism|Aspirin|Lipophilicity", _lipophilicityParam);
         _parameterCache.Add("Organism|Aspirin|Permeability", _permeabilityParam);
         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(root)).Returns(_parameterCache);

         A.CallTo(() => _entityPathResolver.ObjectPathFor(_lipophilicityParam, false)).Returns("Organism|Aspirin|Lipophilicity".ToObjectPath());
         A.CallTo(() => _entityPathResolver.ObjectPathFor(_permeabilityParam, false)).Returns("Organism|Aspirin|Permeability".ToObjectPath());
         A.CallTo(() => _parameterValuesCreator.CreateParameterValue(A<ObjectPath>._, A<IParameter>._))
            .ReturnsLazily((ObjectPath path, IParameter p) => new ParameterValue { Path = path, Value = p.Value });

         sut = new OverwriteParameterSetApplicationTask(_containerTask, _entityPathResolver, _parameterValuesCreator);
      }

      protected OverwriteParameterSet overwriteParameterSetWith(params (string path, double value)[] values)
      {
         var set = new OverwriteParameterSet { Name = "MySet" };
         foreach (var (path, value) in values)
            set.Add(new ParameterValue { Path = path.ToObjectPath(), Value = value });
         return set;
      }
   }

   public class When_applying_an_overwrite_parameter_set_with_matching_paths : concern_for_OverwriteParameterSetApplicationTask
   {
      protected override void Context()
      {
         base.Context();
         _lipophilicityParam.CanBeVariedInPopulation = true;
         var set = overwriteParameterSetWith(("Organism|Aspirin|Lipophilicity", 5.0));
         _simulation.AddOverwriteParameterSetSelection("Aspirin", set);
      }

      protected override void Because()
      {
         sut.ApplyOverwriteParameterSetsTo(_simulation);
      }

      [Observation]
      public void should_apply_the_value_from_the_set_to_the_matching_simulation_parameter()
      {
         _lipophilicityParam.Value.ShouldBeEqualTo(5.0);
      }

      [Observation]
      public void should_lock_the_applied_parameter_against_population_variation()
      {
         _lipophilicityParam.CanBeVariedInPopulation.ShouldBeFalse();
      }

      [Observation]
      public void should_mark_the_applied_parameter_as_a_compound_parameter()
      {
         _lipophilicityParam.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Compound);
      }

      [Observation]
      public void should_point_the_applied_parameter_origin_to_the_compound_building_block()
      {
         _lipophilicityParam.Origin.BuilingBlockId.ShouldBeEqualTo(_compound.Id);
      }

      [Observation]
      public void should_set_the_value_origin_of_the_applied_parameter_to_the_overwrite_parameter_set()
      {
         _lipophilicityParam.ValueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Other);
         _lipophilicityParam.ValueOrigin.Description.ShouldBeEqualTo($"{PKSimConstants.ObjectTypes.OverwriteParameterSet} 'MySet'");
      }

      [Observation]
      public void should_not_modify_parameters_that_are_not_part_of_the_set()
      {
         _permeabilityParam.Value.ShouldBeEqualTo(7.2);
         _permeabilityParam.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Simulation);
      }
   }

   public class When_no_overwrite_parameter_set_is_selected_for_the_compound : concern_for_OverwriteParameterSetApplicationTask
   {
      protected override void Context()
      {
         base.Context();
         //"None" selection => the selected set is null
         _simulation.AddOverwriteParameterSetSelection("Aspirin", null);
      }

      protected override void Because()
      {
         sut.ApplyOverwriteParameterSetsTo(_simulation);
      }

      [Observation]
      public void should_leave_all_simulation_parameters_unchanged()
      {
         _lipophilicityParam.Value.ShouldBeEqualTo(3.5);
         _lipophilicityParam.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Simulation);
      }
   }

   public class When_applying_an_overwrite_parameter_set_with_an_unresolved_path : concern_for_OverwriteParameterSetApplicationTask
   {
      protected override void Context()
      {
         base.Context();
         var set = overwriteParameterSetWith(
            ("Organism|Aspirin|Lipophilicity", 5.0),
            ("Organism|Aspirin|NonExistent", 9.0));
         _simulation.AddOverwriteParameterSetSelection("Aspirin", set);
      }

      [Observation]
      public void should_throw_a_cannot_apply_overwrite_parameter_set_exception()
      {
         The.Action(() => sut.ApplyOverwriteParameterSetsTo(_simulation)).ShouldThrowAn<CannotApplyOverwriteParameterSetException>();
      }

      [Observation]
      public void should_not_apply_any_value_when_a_path_cannot_be_resolved()
      {
         The.Action(() => sut.ApplyOverwriteParameterSetsTo(_simulation)).ShouldThrowAn<CannotApplyOverwriteParameterSetException>();
         _lipophilicityParam.Value.ShouldBeEqualTo(3.5);
         _lipophilicityParam.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Simulation);
      }

      [Observation]
      public void the_error_message_should_identify_the_unresolved_path()
      {
         var exception = new CannotApplyOverwriteParameterSetException(new[] { "Organism|Aspirin|NonExistent" });
         exception.Message.Contains("Organism|Aspirin|NonExistent").ShouldBeTrue();
      }
   }

   public class When_applying_overwrite_parameter_sets_for_multiple_compounds : concern_for_OverwriteParameterSetApplicationTask
   {
      private Compound _secondCompound;
      private IParameter _secondCompoundParam;

      protected override void Context()
      {
         base.Context();
         _secondCompound = new Compound { Name = "Midazolam", Id = "CompId2" };
         _secondCompoundParam = DomainHelperForSpecs.ConstantParameterWithValue(1.0).WithName("Solubility");
         _secondCompoundParam.BuildingBlockType = PKSimBuildingBlockType.Simulation;

         _simulation.Model.Root.Add(_secondCompoundParam);
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("TemplateCompId2", PKSimBuildingBlockType.Compound) { BuildingBlock = _secondCompound });
         _parameterCache.Add("Organism|Midazolam|Solubility", _secondCompoundParam);

         _simulation.AddOverwriteParameterSetSelection("Aspirin", overwriteParameterSetWith(("Organism|Aspirin|Lipophilicity", 5.0)));
         _simulation.AddOverwriteParameterSetSelection("Midazolam", overwriteParameterSetWith(("Organism|Midazolam|Solubility", 2.0)));
      }

      protected override void Because()
      {
         sut.ApplyOverwriteParameterSetsTo(_simulation);
      }

      [Observation]
      public void should_apply_each_compound_set_to_its_own_parameter()
      {
         _lipophilicityParam.Value.ShouldBeEqualTo(5.0);
         _secondCompoundParam.Value.ShouldBeEqualTo(2.0);
      }

      [Observation]
      public void should_point_each_applied_parameter_origin_to_the_matching_compound()
      {
         _lipophilicityParam.Origin.BuilingBlockId.ShouldBeEqualTo(_compound.Id);
         _secondCompoundParam.Origin.BuilingBlockId.ShouldBeEqualTo(_secondCompound.Id);
      }
   }

   public class When_applying_an_overwrite_parameter_set_to_a_population_simulation_that_varies_the_overwritten_parameter : concern_for_OverwriteParameterSetApplicationTask
   {
      private PopulationSimulation _populationSimulation;
      private IParameter _variedParameter;
      private AdvancedParameter _advancedParameter;

      protected override void Context()
      {
         base.Context();

         _variedParameter = DomainHelperForSpecs.ConstantParameterWithValue(3.5).WithName("Lipophilicity");
         _variedParameter.BuildingBlockType = PKSimBuildingBlockType.Simulation;
         _variedParameter.CanBeVariedInPopulation = true;

         var root = new Container { Name = "PopSim" };
         root.Add(_variedParameter);

         _populationSimulation = new PopulationSimulation
         {
            Id = "PopId",
            Model = new OSPSuite.Core.Domain.Model { Root = root }
         };
         _populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("TemplateCompId", PKSimBuildingBlockType.Compound) { BuildingBlock = _compound });

         var advancedParameters = new AdvancedParameterCollection();
         _advancedParameter = new AdvancedParameter { ParameterPath = "Organism|Aspirin|Lipophilicity" };
         advancedParameters.AddAdvancedParameter(_advancedParameter);
         _populationSimulation.SetAdvancedParameters(advancedParameters);

         var populationCache = new PathCacheForSpecs<IParameter>();
         populationCache.Add("Organism|Aspirin|Lipophilicity", _variedParameter);
         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(root)).Returns(populationCache);
         A.CallTo(() => _entityPathResolver.PathFor(_variedParameter)).Returns("Organism|Aspirin|Lipophilicity");

         _populationSimulation.AddOverwriteParameterSetSelection("Aspirin", overwriteParameterSetWith(("Organism|Aspirin|Lipophilicity", 5.0)));
      }

      protected override void Because()
      {
         sut.ApplyOverwriteParameterSetsTo(_populationSimulation);
      }

      [Observation]
      public void should_apply_the_overwrite_value_to_the_matching_simulation_parameter()
      {
         _variedParameter.Value.ShouldBeEqualTo(5.0);
      }

      [Observation]
      public void should_lock_the_applied_parameter_against_population_variation()
      {
         _variedParameter.CanBeVariedInPopulation.ShouldBeFalse();
      }

      [Observation]
      public void should_remove_the_advanced_parameter_previously_defined_for_the_overwritten_path()
      {
         _populationSimulation.AdvancedParameters.Contains(_advancedParameter).ShouldBeFalse();
      }
   }

   public class When_writing_an_overwrite_parameter_set_into_a_parameter_values_building_block : concern_for_OverwriteParameterSetApplicationTask
   {
      private ParameterValuesBuildingBlock _parameterValues;

      protected override void Context()
      {
         base.Context();
         _parameterValues = new ParameterValuesBuildingBlock();
         _simulation.AddOverwriteParameterSetSelection("Aspirin", overwriteParameterSetWith(("Organism|Aspirin|Lipophilicity", 5.0)));
      }

      protected override void Because()
      {
         sut.ApplyOverwriteParameterSetsTo(_parameterValues, _simulation);
      }

      [Observation]
      public void should_add_an_entry_for_the_overwritten_parameter_with_the_overwrite_value()
      {
         var entry = _parameterValues["Organism|Aspirin|Lipophilicity".ToObjectPath()];
         entry.ShouldNotBeNull();
         entry.Value.ShouldBeEqualTo(5.0);
      }

      [Observation]
      public void should_write_the_entry_as_value_only_without_a_formula()
      {
         _parameterValues["Organism|Aspirin|Lipophilicity".ToObjectPath()].Formula.ShouldBeNull();
      }

      [Observation]
      public void should_not_add_entries_for_parameters_that_are_not_part_of_the_set()
      {
         _parameterValues["Organism|Aspirin|Permeability".ToObjectPath()].ShouldBeNull();
      }
   }

   public class When_writing_an_overwrite_parameter_set_over_an_existing_formula_based_entry : concern_for_OverwriteParameterSetApplicationTask
   {
      private ParameterValuesBuildingBlock _parameterValues;

      protected override void Context()
      {
         base.Context();
         _parameterValues = new ParameterValuesBuildingBlock();
         _parameterValues.Add(new ParameterValue { Path = "Organism|Aspirin|Lipophilicity".ToObjectPath(), Formula = _lipophilicityParam.Formula });
         _simulation.AddOverwriteParameterSetSelection("Aspirin", overwriteParameterSetWith(("Organism|Aspirin|Lipophilicity", 5.0)));
      }

      protected override void Because()
      {
         sut.ApplyOverwriteParameterSetsTo(_parameterValues, _simulation);
      }

      [Observation]
      public void should_set_the_overwrite_value_on_the_existing_entry()
      {
         _parameterValues["Organism|Aspirin|Lipophilicity".ToObjectPath()].Value.ShouldBeEqualTo(5.0);
      }

      [Observation]
      public void should_clear_the_existing_formula_so_the_entry_is_value_only()
      {
         _parameterValues["Organism|Aspirin|Lipophilicity".ToObjectPath()].Formula.ShouldBeNull();
      }
   }

   public class When_writing_an_overwrite_parameter_set_into_a_building_block_that_already_contains_the_path : concern_for_OverwriteParameterSetApplicationTask
   {
      private ParameterValuesBuildingBlock _parameterValues;

      protected override void Context()
      {
         base.Context();
         _parameterValues = new ParameterValuesBuildingBlock();
         _parameterValues.Add(new ParameterValue { Path = "Organism|Aspirin|Lipophilicity".ToObjectPath(), Value = 1.0 });
         _simulation.AddOverwriteParameterSetSelection("Aspirin", overwriteParameterSetWith(("Organism|Aspirin|Lipophilicity", 5.0)));
      }

      protected override void Because()
      {
         sut.ApplyOverwriteParameterSetsTo(_parameterValues, _simulation);
      }

      [Observation]
      public void should_update_the_existing_entry_rather_than_add_a_duplicate()
      {
         _parameterValues.Count().ShouldBeEqualTo(1);
         _parameterValues["Organism|Aspirin|Lipophilicity".ToObjectPath()].Value.ShouldBeEqualTo(5.0);
      }
   }

   public class When_writing_into_a_building_block_and_no_overwrite_parameter_set_is_selected : concern_for_OverwriteParameterSetApplicationTask
   {
      private ParameterValuesBuildingBlock _parameterValues;

      protected override void Context()
      {
         base.Context();
         _parameterValues = new ParameterValuesBuildingBlock();
         _simulation.AddOverwriteParameterSetSelection("Aspirin", null);
      }

      protected override void Because()
      {
         sut.ApplyOverwriteParameterSetsTo(_parameterValues, _simulation);
      }

      [Observation]
      public void should_leave_the_parameter_values_building_block_empty()
      {
         _parameterValues.Count().ShouldBeEqualTo(0);
      }
   }

   public class When_writing_overwrite_parameter_sets_for_multiple_compounds_into_a_building_block : concern_for_OverwriteParameterSetApplicationTask
   {
      private ParameterValuesBuildingBlock _parameterValues;
      private Compound _secondCompound;
      private IParameter _secondCompoundParam;

      protected override void Context()
      {
         base.Context();
         _parameterValues = new ParameterValuesBuildingBlock();
         _secondCompound = new Compound { Name = "Midazolam", Id = "CompId2" };
         _secondCompoundParam = DomainHelperForSpecs.ConstantParameterWithValue(1.0).WithName("Solubility");
         _secondCompoundParam.BuildingBlockType = PKSimBuildingBlockType.Simulation;

         _simulation.Model.Root.Add(_secondCompoundParam);
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("TemplateCompId2", PKSimBuildingBlockType.Compound) { BuildingBlock = _secondCompound });
         _parameterCache.Add("Organism|Midazolam|Solubility", _secondCompoundParam);
         A.CallTo(() => _entityPathResolver.ObjectPathFor(_secondCompoundParam, false)).Returns("Organism|Midazolam|Solubility".ToObjectPath());

         _simulation.AddOverwriteParameterSetSelection("Aspirin", overwriteParameterSetWith(("Organism|Aspirin|Lipophilicity", 5.0)));
         _simulation.AddOverwriteParameterSetSelection("Midazolam", overwriteParameterSetWith(("Organism|Midazolam|Solubility", 2.0)));
      }

      protected override void Because()
      {
         sut.ApplyOverwriteParameterSetsTo(_parameterValues, _simulation);
      }

      [Observation]
      public void should_add_an_entry_for_each_compound_parameter_with_its_overwrite_value()
      {
         _parameterValues["Organism|Aspirin|Lipophilicity".ToObjectPath()].Value.ShouldBeEqualTo(5.0);
         _parameterValues["Organism|Midazolam|Solubility".ToObjectPath()].Value.ShouldBeEqualTo(2.0);
      }
   }
}
