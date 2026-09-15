using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationToCompoundCommitDTOMapper : ContextSpecification<SimulationToCompoundCommitDTOMapper>
   {
      protected IContainerTask _containerTask;
      protected IParameterToParameterCommitDTOMapper _parameterCommitDTOMapper;
      protected IndividualSimulation _simulation;
      protected Compound _templateCompound;
      protected Compound _simulationCompound;
      protected PathCache<IParameter> _parameterCache;
      protected IParameter _lipophilicity;
      protected IParameter _permeability;

      protected override void Context()
      {
         _containerTask = A.Fake<IContainerTask>();
         _parameterCommitDTOMapper = A.Fake<IParameterToParameterCommitDTOMapper>();

         _templateCompound = new Compound { Name = "Aspirin", Id = "TemplateId" };
         _simulationCompound = new Compound { Name = "Aspirin", Id = "SimCompId" };

         var root = new Container { Name = "Sim" };
         _lipophilicity = DomainHelperForSpecs.ConstantParameterWithValue(3.5).WithName("Lipophilicity");
         _permeability = DomainHelperForSpecs.ConstantParameterWithValue(7.2).WithName("Permeability");
         root.Add(_lipophilicity);
         root.Add(_permeability);

         _simulation = new IndividualSimulation
         {
            Id = "SimId",
            Model = new OSPSuite.Core.Domain.Model { Root = root }
         };

         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("TemplateId", PKSimBuildingBlockType.Compound)
         {
            BuildingBlock = _simulationCompound
         });

         _parameterCache = new PathCacheForSpecs<IParameter>();
         _parameterCache.Add("Organism|Aspirin|Lipophilicity", _lipophilicity);
         _parameterCache.Add("Organism|Aspirin|Permeability", _permeability);
         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(root)).Returns(_parameterCache);

         A.CallTo(() => _parameterCommitDTOMapper.MapFrom("Organism|Aspirin|Lipophilicity", _lipophilicity, A<bool>._))
            .ReturnsLazily(x => new ParameterCommitDTO { Path = "Organism|Aspirin|Lipophilicity", Value = 3.5, IsRemoval = x.GetArgument<bool>(2) });
         A.CallTo(() => _parameterCommitDTOMapper.MapFrom("Organism|Aspirin|Permeability", _permeability, A<bool>._))
            .ReturnsLazily(x => new ParameterCommitDTO { Path = "Organism|Aspirin|Permeability", Value = 7.2, IsRemoval = x.GetArgument<bool>(2) });

         sut = new SimulationToCompoundCommitDTOMapper(_containerTask, _parameterCommitDTOMapper);
      }
   }

   public class When_mapping_a_simulation_with_tracked_changes_for_compound : concern_for_SimulationToCompoundCommitDTOMapper
   {
      private CompoundCommitDTO _result;

      protected override void Context()
      {
         base.Context();
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Permeability");
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_simulation, _templateCompound);
      }

      [Observation]
      public void should_return_dto_for_compound()
      {
         _result.ShouldNotBeNull();
         _result.CompoundName.ShouldBeEqualTo("Aspirin");
      }

      [Observation]
      public void should_resolve_the_template_compound()
      {
         _result.Compound.ShouldBeEqualTo(_templateCompound);
      }

      [Observation]
      public void should_map_parameter_values()
      {
         _result.Parameters.Count.ShouldBeEqualTo(2);
         _result.Parameters.Any(p => p.Path == "Organism|Aspirin|Lipophilicity" && p.Value == 3.5).ShouldBeTrue();
         _result.Parameters.Any(p => p.Path == "Organism|Aspirin|Permeability" && p.Value == 7.2).ShouldBeTrue();
      }

      [Observation]
      public void should_set_default_new_set_name_to_compound_name()
      {
         _result.NewSetName.ShouldBeEqualTo("Aspirin");
      }

      [Observation]
      public void should_provide_available_existing_sets()
      {
         _result.AvailableExistingSets.ShouldBeEqualTo(_templateCompound.OverwriteParameterSets);
      }

      [Observation]
      public void should_default_to_create_new()
      {
         _result.CreateNew.ShouldBeTrue();
      }
   }

   public class When_mapping_a_simulation_with_no_tracked_changes_for_compound : concern_for_SimulationToCompoundCommitDTOMapper
   {
      private CompoundCommitDTO _result;

      protected override void Because()
      {
         _result = sut.MapFrom(_simulation, _templateCompound);
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }
   }

   public class When_mapping_a_simulation_whose_selection_holds_the_set_of_the_compound_in_the_simulation : concern_for_SimulationToCompoundCommitDTOMapper
   {
      private CompoundCommitDTO _result;
      private OverwriteParameterSet _setInTemplateCompound;

      protected override void Context()
      {
         base.Context();
         //a selection made in the simulation configuration holds the set of the compound in the simulation, which is never
         //the same instance as the set of the template compound
         _setInTemplateCompound = new OverwriteParameterSet { Name = "ExistingSet", Id = "TemplateSetId" };
         _templateCompound.AddOverwriteParameterSet(_setInTemplateCompound);

         var setInSimulationCompound = new OverwriteParameterSet { Name = "ExistingSet", Id = "SimulationSetId" };
         _simulationCompound.AddOverwriteParameterSet(setInSimulationCompound);
         _simulation.OverwriteParameterSetSelections.SetSelectionForCompound("Aspirin", setInSimulationCompound);

         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_simulation, _templateCompound);
      }

      [Observation]
      public void should_default_to_update_existing()
      {
         _result.CreateNew.ShouldBeFalse();
      }

      [Observation]
      public void should_select_the_set_of_the_template_compound()
      {
         _result.SelectedExistingSet.ShouldBeEqualTo(_setInTemplateCompound);
      }
   }

   public class When_mapping_a_simulation_whose_selected_set_is_not_defined_in_the_template_compound : concern_for_SimulationToCompoundCommitDTOMapper
   {
      private CompoundCommitDTO _result;

      protected override void Context()
      {
         base.Context();
         _simulation.OverwriteParameterSetSelections.SetSelectionForCompound("Aspirin", new OverwriteParameterSet { Name = "RemovedSet" });

         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_simulation, _templateCompound);
      }

      [Observation]
      public void should_default_to_create_new()
      {
         _result.CreateNew.ShouldBeTrue();
      }

      [Observation]
      public void should_not_select_an_existing_set()
      {
         _result.SelectedExistingSet.ShouldBeNull();
      }
   }

   public class When_mapping_a_simulation_with_existing_overwrite_set_selection : concern_for_SimulationToCompoundCommitDTOMapper
   {
      private CompoundCommitDTO _result;
      private OverwriteParameterSet _existingSet;

      protected override void Context()
      {
         base.Context();
         //a selection restored from a snapshot holds the set of the template compound
         _existingSet = new OverwriteParameterSet { Name = "ExistingSet" };
         _templateCompound.AddOverwriteParameterSet(_existingSet);
         _simulation.OverwriteParameterSetSelections.SetSelectionForCompound("Aspirin", _existingSet);

         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_simulation, _templateCompound);
      }

      [Observation]
      public void should_default_to_update_existing()
      {
         _result.CreateNew.ShouldBeFalse();
      }

      [Observation]
      public void should_select_the_existing_set()
      {
         _result.SelectedExistingSet.ShouldBeEqualTo(_existingSet);
      }

      [Observation]
      public void should_remember_the_set_selected_in_the_simulation_as_the_target_of_removals()
      {
         _result.SetSelectedInSimulation.ShouldBeEqualTo(_existingSet);
      }
   }

   public class When_mapping_a_simulation_where_the_user_reset_a_parameter_applied_from_the_selected_set : concern_for_SimulationToCompoundCommitDTOMapper
   {
      private CompoundCommitDTO _result;

      protected override void Context()
      {
         base.Context();
         var existingSet = new OverwriteParameterSet { Name = "ExistingSet" };
         existingSet.Add(new ParameterValue { Path = "Organism|Aspirin|Lipophilicity".ToObjectPath(), Value = 5.0 });
         existingSet.Add(new ParameterValue { Path = "Organism|Aspirin|Permeability".ToObjectPath(), Value = 9.0 });
         _templateCompound.AddOverwriteParameterSet(existingSet);
         _simulation.OverwriteParameterSetSelections.SetSelectionForCompound("Aspirin", existingSet);

         //Lipophilicity was reset to its calculated value, Permeability was changed by the user
         _lipophilicity.IsDefault = true;
         _permeability.IsDefault = false;
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Permeability");
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_simulation, _templateCompound);
      }

      [Observation]
      public void should_mark_the_reset_parameter_as_a_removal_from_the_set()
      {
         _result.Parameters.Single(p => p.Path == "Organism|Aspirin|Lipophilicity").IsRemoval.ShouldBeTrue();
      }

      [Observation]
      public void should_mark_the_changed_parameter_as_an_update_of_the_set()
      {
         _result.Parameters.Single(p => p.Path == "Organism|Aspirin|Permeability").IsRemoval.ShouldBeFalse();
      }
   }

   public class When_mapping_a_simulation_where_the_user_reset_a_parameter_that_is_not_part_of_the_selected_set : concern_for_SimulationToCompoundCommitDTOMapper
   {
      private CompoundCommitDTO _result;

      protected override void Context()
      {
         base.Context();
         var existingSet = new OverwriteParameterSet { Name = "ExistingSet" };
         existingSet.Add(new ParameterValue { Path = "Organism|Aspirin|Permeability".ToObjectPath(), Value = 9.0 });
         _templateCompound.AddOverwriteParameterSet(existingSet);
         _simulation.OverwriteParameterSetSelections.SetSelectionForCompound("Aspirin", existingSet);

         _lipophilicity.IsDefault = true;
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_simulation, _templateCompound);
      }

      [Observation]
      public void should_map_the_parameter_as_an_update_of_the_set()
      {
         _result.Parameters.Single().IsRemoval.ShouldBeFalse();
      }
   }

   public class When_mapping_a_simulation_whose_compound_is_outdated_compared_to_the_project_compound : concern_for_SimulationToCompoundCommitDTOMapper
   {
      private CompoundCommitDTO _result;
      private OverwriteParameterSet _setInProjectCompound;

      protected override void Context()
      {
         base.Context();
         //a set committed from another simulation exists in the project compound but not in the outdated compound of this simulation
         _setInProjectCompound = new OverwriteParameterSet { Name = "ExistingSet" };
         _templateCompound.AddOverwriteParameterSet(_setInProjectCompound);

         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_simulation, _templateCompound);
      }

      [Observation]
      public void should_use_the_project_compound()
      {
         _result.Compound.ShouldBeEqualTo(_templateCompound);
      }

      [Observation]
      public void should_offer_the_sets_defined_in_the_project_compound()
      {
         _result.AvailableExistingSets.ShouldOnlyContain(_setInProjectCompound);
      }
   }
}
