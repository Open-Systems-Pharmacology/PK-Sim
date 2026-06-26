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
      protected IndividualSimulation _simulation;
      protected Compound _compound;
      protected IParameter _lipophilicityParam;
      protected IParameter _permeabilityParam;
      protected PathCache<IParameter> _parameterCache;

      protected override void Context()
      {
         _containerTask = A.Fake<IContainerTask>();

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

         sut = new OverwriteParameterSetApplicationTask(_containerTask);
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
}
