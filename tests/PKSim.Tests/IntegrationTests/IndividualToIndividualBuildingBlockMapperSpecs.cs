using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Extensions;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Infrastructure;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_IndividualToIndividualBuildingBlockMapper : ContextForIntegration<IIndividualToIndividualBuildingBlockMapper>
   {
      protected Individual _individual;
      protected IndividualBuildingBlock _individualBuildingBlock;


      public override void GlobalContext()
      {
         base.GlobalContext();
         //specify a population here as we need a human specific formula
         _individual = DomainFactoryForSpecs.CreateStandardIndividual(population: CoreConstants.Population.ICRP);
      }


      protected override void Because()
      {
         _individualBuildingBlock = sut.MapFrom(_individual);
      }

   }

   public class When_mapping_building_block_from_individual : concern_for_IndividualToIndividualBuildingBlockMapper
   {
    
      [Observation]
      public void the_properties_of_the_building_block_should_match()
      {
         var allDataItems = _individualBuildingBlock.OriginData.All;
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
         var formula = _individualBuildingBlock.FormulaCache.FindByName("PARAM_eGFR");
         formula.ObjectPaths.Count.ShouldBeEqualTo(2);
         formula.ObjectPaths[0][0].ShouldBeEqualTo(Constants.ORGANISM);
      }
   }

   public class When_mapping_an_individual_with_expression_to_individual : concern_for_IndividualToIndividualBuildingBlockMapper
   {
      public override void GlobalContext()
      {
         base.GlobalContext();

         DomainFactoryForSpecs.CreateExpressionProfileAndAddToIndividual<IndividualEnzyme>(_individual, "CYP");
      }

      [Observation]
      public void should_not_export_the_molecule_specific_ontogeny_factors()
      {
         var allOntogenyFactors = _individualBuildingBlock.Where(x => x.Name.IsOneOf(OntogenyFactors.ToArray()));
         allOntogenyFactors.Count().ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_have_exported_the_plasma_protein_molecules()
      {
         var allOntogenyFactors = _individualBuildingBlock.Where(x => x.Name.IsOneOf(AllPlasmaProteinOntogenyFactors.ToArray()));
         allOntogenyFactors.Count().ShouldBeEqualTo(2);
      }

   }
}