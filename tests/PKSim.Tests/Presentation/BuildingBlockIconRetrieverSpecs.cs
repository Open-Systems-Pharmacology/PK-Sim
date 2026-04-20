using FakeItEasy;
using OSPSuite.Assets;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_BuildingBlockIconRetriever : ContextSpecification<BuildingBlockIconRetriever>
   {
      protected IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      protected IBuildingBlockRepository _buildingBlockRepository;

      protected override void Context()
      {
         _buildingBlockInProjectManager = A.Fake<IBuildingBlockInProjectManager>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         sut = new BuildingBlockIconRetriever(_buildingBlockInProjectManager, _buildingBlockRepository);
      }
   }

   public class When_retrieving_icon_for_compound_with_red_status_and_uncommitted_changes : concern_for_BuildingBlockIconRetriever
   {
      private ApplicationIcon _result;
      private UsedBuildingBlock _usedBuildingBlock;
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         var templateCompound = new Compound().WithName("Aspirin").WithId("templateId");
         var simulationCompound = new Compound().WithName("Aspirin").WithId("CompId");
         _usedBuildingBlock = new UsedBuildingBlock("templateId", PKSimBuildingBlockType.Compound);
         _usedBuildingBlock.BuildingBlock = simulationCompound;

         _simulation = new IndividualSimulation { Properties = new SimulationProperties(), ModelProperties = new ModelProperties() };
         _simulation.ModelConfiguration = A.Fake<ModelConfiguration>();
         _simulation.AddUsedBuildingBlock(_usedBuildingBlock);
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");

         A.CallTo(() => _buildingBlockInProjectManager.StatusFor(_usedBuildingBlock)).Returns(BuildingBlockStatus.Red);
         A.CallTo(() => _buildingBlockRepository.ById("templateId")).Returns(templateCompound);
      }

      protected override void Because()
      {
         _result = sut.IconFor(_simulation, _usedBuildingBlock);
      }

      [Observation]
      public void should_return_compound_red_orange_icon()
      {
         _result.ShouldBeEqualTo(ApplicationIcons.CompoundRedOrange);
      }
   }

   public class When_retrieving_icon_for_compound_with_red_status_and_no_uncommitted_changes : concern_for_BuildingBlockIconRetriever
   {
      private ApplicationIcon _result;
      private UsedBuildingBlock _usedBuildingBlock;
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         var templateCompound = new Compound().WithName("Aspirin").WithId("templateId");
         var simulationCompound = new Compound().WithName("Aspirin").WithId("CompId");
         _usedBuildingBlock = new UsedBuildingBlock("templateId", PKSimBuildingBlockType.Compound);
         _usedBuildingBlock.BuildingBlock = simulationCompound;

         _simulation = new IndividualSimulation { Properties = new SimulationProperties(), ModelProperties = new ModelProperties() };
         _simulation.ModelConfiguration = A.Fake<ModelConfiguration>();
         _simulation.AddUsedBuildingBlock(_usedBuildingBlock);

         A.CallTo(() => _buildingBlockInProjectManager.StatusFor(_usedBuildingBlock)).Returns(BuildingBlockStatus.Red);
         A.CallTo(() => _buildingBlockRepository.ById("templateId")).Returns(templateCompound);
      }

      protected override void Because()
      {
         _result = sut.IconFor(_simulation, _usedBuildingBlock);
      }

      [Observation]
      public void should_return_compound_red_icon()
      {
         _result.ShouldBeEqualTo(ApplicationIcons.CompoundRed);
      }
   }

   public class When_retrieving_icon_for_compound_with_green_status_and_uncommitted_changes : concern_for_BuildingBlockIconRetriever
   {
      private ApplicationIcon _result;
      private UsedBuildingBlock _usedBuildingBlock;
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         var templateCompound = new Compound().WithName("Aspirin").WithId("templateId");
         var simulationCompound = new Compound().WithName("Aspirin").WithId("CompId");
         _usedBuildingBlock = new UsedBuildingBlock("templateId", PKSimBuildingBlockType.Compound);
         _usedBuildingBlock.BuildingBlock = simulationCompound;

         _simulation = new IndividualSimulation { Properties = new SimulationProperties(), ModelProperties = new ModelProperties() };
         _simulation.ModelConfiguration = A.Fake<ModelConfiguration>();
         _simulation.AddUsedBuildingBlock(_usedBuildingBlock);
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");

         A.CallTo(() => _buildingBlockInProjectManager.StatusFor(_usedBuildingBlock)).Returns(BuildingBlockStatus.Green);
         A.CallTo(() => _buildingBlockRepository.ById("templateId")).Returns(templateCompound);
      }

      protected override void Because()
      {
         _result = sut.IconFor(_simulation, _usedBuildingBlock);
      }

      [Observation]
      public void should_return_compound_green_orange_icon()
      {
         _result.ShouldBeEqualTo(ApplicationIcons.CompoundGreenOrange);
      }
   }

   public class When_retrieving_icon_for_compound_with_green_status_and_no_uncommitted_changes : concern_for_BuildingBlockIconRetriever
   {
      private ApplicationIcon _result;
      private UsedBuildingBlock _usedBuildingBlock;
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         var simulationCompound = new Compound().WithName("Aspirin").WithId("CompId");
         _usedBuildingBlock = new UsedBuildingBlock("templateId", PKSimBuildingBlockType.Compound);
         _usedBuildingBlock.BuildingBlock = simulationCompound;

         _simulation = new IndividualSimulation { Properties = new SimulationProperties(), ModelProperties = new ModelProperties() };
         _simulation.ModelConfiguration = A.Fake<ModelConfiguration>();
         _simulation.AddUsedBuildingBlock(_usedBuildingBlock);

         A.CallTo(() => _buildingBlockInProjectManager.StatusFor(_usedBuildingBlock)).Returns(BuildingBlockStatus.Green);
      }

      protected override void Because()
      {
         _result = sut.IconFor(_simulation, _usedBuildingBlock);
      }

      [Observation]
      public void should_return_compound_green_icon()
      {
         _result.ShouldBeEqualTo(ApplicationIcons.CompoundGreen);
      }
   }

   public class When_retrieving_icon_for_non_compound_building_block_with_simulation : concern_for_BuildingBlockIconRetriever
   {
      private ApplicationIcon _result;
      private UsedBuildingBlock _usedBuildingBlock;
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _usedBuildingBlock = new UsedBuildingBlock("templateId", PKSimBuildingBlockType.Protocol);

         _simulation = new IndividualSimulation { Properties = new SimulationProperties(), ModelProperties = new ModelProperties() };
         _simulation.ModelConfiguration = A.Fake<ModelConfiguration>();

         A.CallTo(() => _buildingBlockInProjectManager.StatusFor(_usedBuildingBlock)).Returns(BuildingBlockStatus.Red);
      }

      protected override void Because()
      {
         _result = sut.IconFor(_simulation, _usedBuildingBlock);
      }

      [Observation]
      public void should_return_standard_red_icon_for_that_type()
      {
         _result.ShouldBeEqualTo(ApplicationIcons.ProtocolRed);
      }
   }
}