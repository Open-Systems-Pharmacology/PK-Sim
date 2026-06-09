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

   public class When_mapping_an_individual_with_an_overwritten_distributed_parameter_to_building_block : concern_for_IndividualToIndividualBuildingBlockMapper
   {
      private IDistributedParameter _individualLiverVolume;
      private double _overwrittenValue;
      private IndividualParameter _mappedLiverVolume;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _individualLiverVolume = _individual.Organism.Organ(CoreConstants.Organ.LIVER).Parameter(Constants.Parameters.VOLUME) as IDistributedParameter;
         _individualLiverVolume.ShouldNotBeNull();

         _overwrittenValue = _individualLiverVolume.Value * 1.5;
         _individualLiverVolume.Value = _overwrittenValue;
      }

      protected override void Because()
      {
         base.Because();
         _mappedLiverVolume = _individualBuildingBlock.Single(x => x.Name == Constants.Parameters.VOLUME && x.ContainerPath.Contains(CoreConstants.Organ.LIVER));
      }

      [Observation]
      public void should_have_exported_the_overwritten_value()
      {
         _mappedLiverVolume.Value.ShouldBeEqualTo(_overwrittenValue);
      }

      [Observation]
      public void should_have_preserved_the_distribution_type()
      {
         _mappedLiverVolume.DistributionType.ShouldBeEqualTo(_individualLiverVolume.Formula.DistributionType);
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

   public class When_mapping_a_building_block_from_an_individual_with_gestational_age_in_weeks : concern_for_IndividualToIndividualBuildingBlockMapper
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         //value stored in base unit of "Age in weeks" dimension i.e. weeks
         _individual.OriginData.GestationalAge = new OriginDataParameter(40, "week(s)");
      }

      [Observation]
      public void should_format_gestational_age_using_the_age_in_weeks_dimension()
      {
         _individualBuildingBlock.OriginData["Gestational age"].ValueAsObject.ShouldBeEqualTo("40.00 week(s)");
      }
   }
}