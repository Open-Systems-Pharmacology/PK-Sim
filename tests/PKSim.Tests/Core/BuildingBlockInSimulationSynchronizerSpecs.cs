using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_BuildingBlockInSimulationSynchronizer : ContextSpecification<IBuildingBlockInSimulationSynchronizer>
   {
      protected ICloner _cloner;
      protected IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      protected ICompoundPropertiesUpdater _compoundPropertiesUpdater;

      protected override void Context()
      {
         _cloner= A.Fake<ICloner>();
         _buildingBlockInProjectManager= A.Fake<IBuildingBlockInProjectManager>();
         _compoundPropertiesUpdater= A.Fake<ICompoundPropertiesUpdater>();

         sut = new BuildingBlockInSimulationSynchronizer(_buildingBlockInProjectManager,_cloner,_compoundPropertiesUpdater);
      }
   }

   public class When_syncrhonizing_the_building_blocks_defined_in_a_simulation_with_the_template_building_blocks_define_in_project : concern_for_BuildingBlockInSimulationSynchronizer
   {
      private Simulation _simulation;
      private UsedBuildingBlock _usedCompound1;
      private UsedBuildingBlock _usedCompound2;
      private UsedBuildingBlock _usedIndividual;
      private UsedBuildingBlock _usedProtocol;
      private Compound _compound1;
      private Compound _compound2;
      private Individual _individual;
      private Protocol _protocol;
      private Compound _templateCompound2;
      private Compound _cloneOfTemplateCompound2;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation();
         _compound1 = A.Fake<Compound>().WithName("C1");
         _compound2 = A.Fake<Compound>().WithName("C2");
         _templateCompound2 = A.Fake<Compound>().WithName("C2_TEMP");
         _cloneOfTemplateCompound2 = A.Fake<Compound>().WithName("C2_TEMP_CLONE");
         _individual = A.Fake<Individual>();  
         _protocol = A.Fake<Protocol>();  


         _usedCompound1 = new UsedBuildingBlock("Comp1", PKSimBuildingBlockType.Compound){BuildingBlock = _compound1};
         _usedCompound2 = new UsedBuildingBlock("Comp2", PKSimBuildingBlockType.Compound) { BuildingBlock = _compound2 };
         _usedIndividual = new UsedBuildingBlock("Ind", PKSimBuildingBlockType.Individual) { BuildingBlock = _individual };
         _usedProtocol = new UsedBuildingBlock("Protocol", PKSimBuildingBlockType.Protocol) { BuildingBlock = _protocol };

         _simulation.AddUsedBuildingBlock(_usedCompound1);
         _simulation.AddUsedBuildingBlock(_usedCompound2);
         _simulation.AddUsedBuildingBlock(_usedIndividual);
         _simulation.AddUsedBuildingBlock(_usedProtocol);


         A.CallTo(() => _buildingBlockInProjectManager.TemplateBuildingBlockUsedBy(_usedCompound1)).Returns(_compound1);
         A.CallTo(() => _buildingBlockInProjectManager.TemplateBuildingBlockUsedBy(_usedCompound2)).Returns(_templateCompound2);

         A.CallTo(() => _cloner.Clone<IPKSimBuildingBlock>(_templateCompound2)).Returns(_cloneOfTemplateCompound2);
      }

      protected override void Because()
      {
         sut.UpdateUsedBuildingBlockBasedOnTemplateIn(_simulation);
      }

      [Observation]
      public void should_not_update_any_building_block_that_is_not_a_compound_building_block()
      {
         _usedIndividual.BuildingBlock.ShouldBeEqualTo(_individual);
         _usedProtocol.BuildingBlock.ShouldBeEqualTo(_protocol);
      }

      [Observation]
      public void should_not_update_any_compound_building_block_that_was_changed_by_the_user()
      {
         _usedCompound1.BuildingBlock.ShouldBeEqualTo(_compound1);
      }

      [Observation]
      public void should_update_any_compound_building_block_that_was_not_changed_by_the_user_to_point_to_actual_template   ()
      {
         _usedCompound2.BuildingBlock.ShouldBeEqualTo(_cloneOfTemplateCompound2);
      }

      [Observation]
      public void should_ensure_that_the_compound_properties_where_updated_as_well()
      {
         A.CallTo(() => _compoundPropertiesUpdater.UpdateCompoundPropertiesIn(_simulation)).MustHaveHappened();
      }
   }
}	