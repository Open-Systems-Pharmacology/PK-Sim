using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Extensions;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_IndividualToIndividualBuildingBlockMapper : ContextForIntegration<IIndividualToIndividualBuildingBlockMapper>
   {
      protected Individual _individual;

      public override void GlobalContext()
      {
         base.GlobalContext();
         //specify a population here as we need a human specific formula
         _individual = DomainFactoryForSpecs.CreateStandardIndividual(population: CoreConstants.Population.ICRP);
      }
   }

   public class When_mapping_building_block_from_individual : concern_for_IndividualToIndividualBuildingBlockMapper
   {
      private IndividualBuildingBlock _buildingBlock;

      protected override void Because()
      {
         _buildingBlock = sut.MapFrom(_individual);
      }

      [Observation]
      public void the_properties_of_the_building_block_should_match()
      {
         var allDataItems = _buildingBlock.OriginData.All;
         allDataItems.ExistsByName("Species").ShouldBeTrue();
         allDataItems.ExistsByName("Population").ShouldBeTrue();
         allDataItems.ExistsByName("Gender").ShouldBeTrue();
         allDataItems.ExistsByName("Weight").ShouldBeTrue();
         allDataItems.ExistsByName("Age").ShouldBeTrue();
         allDataItems.ExistsByName("Height").ShouldBeTrue();

         //the following are not mapped because they are not defined in the default individual
         allDataItems.ExistsByName("Gestational Age").ShouldBeFalse();
         allDataItems.ExistsByName("Disease State").ShouldBeFalse();
      }

      [Observation]
      public void should_have_replaced_the_ROOT_key_element_path_in_all_formula()
      {
         //this formula is defined as it is human specific
         var formula = _buildingBlock.FormulaCache.FindByName("PARAM_eGFR");
         formula.ObjectPaths.Count.ShouldBeEqualTo(2);
         formula.ObjectPaths[0][0].ShouldBeEqualTo(Constants.ORGANISM);
      }
   }
}