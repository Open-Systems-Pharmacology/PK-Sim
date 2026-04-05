using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationToCommitSimulationParametersDTOMapper : ContextSpecification<SimulationToCommitSimulationParametersDTOMapper>
   {
      protected IContainerTask _containerTask;
      protected IPKSimProjectRetriever _projectRetriever;
      protected IndividualSimulation _simulation;
      protected Compound _templateCompound;
      protected Compound _simulationCompound;
      protected PKSimProject _project;
      protected PathCache<IParameter> _parameterCache;

      protected override void Context()
      {
         _containerTask = A.Fake<IContainerTask>();
         _projectRetriever = A.Fake<IPKSimProjectRetriever>();

         _templateCompound = new Compound { Name = "Aspirin", Id = "TemplateId" };
         _simulationCompound = new Compound { Name = "Aspirin", Id = "SimCompId" };

         _project = new PKSimProject();
         _project.AddBuildingBlock(_templateCompound);
         A.CallTo(() => _projectRetriever.Current).Returns(_project);

         var root = new Container { Name = "Sim" };
         var lipophilicity = DomainHelperForSpecs.ConstantParameterWithValue(3.5).WithName("Lipophilicity");
         var permeability = DomainHelperForSpecs.ConstantParameterWithValue(7.2).WithName("Permeability");
         root.Add(lipophilicity);
         root.Add(permeability);

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
         _parameterCache.Add("Organism|Aspirin|Lipophilicity", lipophilicity);
         _parameterCache.Add("Organism|Aspirin|Permeability", permeability);
         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(root)).Returns(_parameterCache);

         sut = new SimulationToCommitSimulationParametersDTOMapper(_containerTask, _projectRetriever);
      }
   }

   public class When_mapping_a_simulation_with_tracked_changes : concern_for_SimulationToCommitSimulationParametersDTOMapper
   {
      private CommitSimulationParametersDTO _result;

      protected override void Context()
      {
         base.Context();
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Permeability");
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_simulation);
      }

      [Observation]
      public void should_group_parameters_by_compound()
      {
         _result.Compounds.Count.ShouldBeEqualTo(1);
         _result.Compounds[0].CompoundName.ShouldBeEqualTo("Aspirin");
      }

      [Observation]
      public void should_resolve_the_template_compound()
      {
         _result.Compounds[0].TemplateCompound.ShouldBeEqualTo(_templateCompound);
      }

      [Observation]
      public void should_map_parameter_values()
      {
         _result.Compounds[0].Parameters.Count.ShouldBeEqualTo(2);
         _result.Compounds[0].Parameters.Any(p => p.Path == "Organism|Aspirin|Lipophilicity" && p.Value == 3.5).ShouldBeTrue();
         _result.Compounds[0].Parameters.Any(p => p.Path == "Organism|Aspirin|Permeability" && p.Value == 7.2).ShouldBeTrue();
      }

      [Observation]
      public void should_set_default_new_set_name_to_compound_name()
      {
         _result.Compounds[0].NewSetName.ShouldBeEqualTo("Aspirin");
      }

      [Observation]
      public void should_provide_available_existing_sets()
      {
         _result.Compounds[0].AvailableExistingSets.ShouldBeEqualTo(_templateCompound.OverwriteParameterSets);
      }
   }

   public class When_mapping_a_simulation_with_no_tracked_changes : concern_for_SimulationToCommitSimulationParametersDTOMapper
   {
      private CommitSimulationParametersDTO _result;

      protected override void Because()
      {
         _result = sut.MapFrom(_simulation);
      }

      [Observation]
      public void should_return_empty_dto()
      {
         _result.Compounds.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_mapping_a_simulation_with_non_compound_tracked_paths : concern_for_SimulationToCommitSimulationParametersDTOMapper
   {
      private CommitSimulationParametersDTO _result;

      protected override void Context()
      {
         base.Context();
         _simulation.ParameterChangeTracker.Track("Organism|Liver|Volume");
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_simulation);
      }

      [Observation]
      public void should_exclude_non_compound_paths()
      {
         _result.Compounds.Count.ShouldBeEqualTo(0);
      }
   }
}
