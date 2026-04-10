using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationToCompoundCommitDTOMapper : ContextSpecification<SimulationToCompoundCommitDTOMapper>
   {
      protected IContainerTask _containerTask;
      protected IBuildingBlockInProjectManager _buildingBlockInProjectManager;
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
         _buildingBlockInProjectManager = A.Fake<IBuildingBlockInProjectManager>();
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

         A.CallTo(() => _buildingBlockInProjectManager.TemplateBuildingBlockUsedBy<Compound>(_simulation, _simulationCompound)).Returns(_templateCompound);

         _parameterCache = new PathCacheForSpecs<IParameter>();
         _parameterCache.Add("Organism|Aspirin|Lipophilicity", _lipophilicity);
         _parameterCache.Add("Organism|Aspirin|Permeability", _permeability);
         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(root)).Returns(_parameterCache);

         A.CallTo(() => _parameterCommitDTOMapper.MapFrom("Organism|Aspirin|Lipophilicity", _lipophilicity))
            .Returns(new ParameterCommitDTO { Path = "Organism|Aspirin|Lipophilicity", Value = 3.5 });
         A.CallTo(() => _parameterCommitDTOMapper.MapFrom("Organism|Aspirin|Permeability", _permeability))
            .Returns(new ParameterCommitDTO { Path = "Organism|Aspirin|Permeability", Value = 7.2 });

         sut = new SimulationToCompoundCommitDTOMapper(_containerTask, _buildingBlockInProjectManager, _parameterCommitDTOMapper);
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
         _result = sut.MapFrom(_simulation, _simulationCompound);
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
         _result.TemplateCompound.ShouldBeEqualTo(_templateCompound);
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
         _result = sut.MapFrom(_simulation, _simulationCompound);
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }
   }

   public class When_mapping_a_simulation_with_existing_overwrite_set_selection : concern_for_SimulationToCompoundCommitDTOMapper
   {
      private CompoundCommitDTO _result;
      private OverwriteParameterSet _existingSet;

      protected override void Context()
      {
         base.Context();
         _existingSet = new OverwriteParameterSet { Name = "ExistingSet" };
         _templateCompound.AddOverwriteParameterSet(_existingSet);
         _simulation.OverwriteParameterSetSelections.SetSelectionForCompound("Aspirin", _existingSet);

         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_simulation, _simulationCompound);
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
   }
}
