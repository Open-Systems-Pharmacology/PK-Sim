using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Infrastructure.ProjectConverter;
using IContainer = OSPSuite.Core.Domain.IContainer;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_Individual : ContextForIntegration<Individual>
   {
      protected override void Context()
      {
      }
   }

   public class When_creating_a_default_individual : concern_for_Individual
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = DomainFactoryForSpecs.CreateStandardIndividual();
      }

      [Observation]
      public void the_resulting_GFR_spec_value_should_be_comparable_with_the_value_defined_in_the_literature()
      {
         sut.Organism.Organ(CoreConstants.Organ.KIDNEY)
            .Parameter(ConverterConstants.Parameters.GFRspec).Value.ShouldBeEqualTo(0.266, 1e-2);
      }

      [Observation]
      public void should_have_defined_a_BSA_parameter_as_formula()
      {
         var bsa = sut.Organism.Parameter(CoreConstants.Parameters.BSA);
         bsa.ShouldNotBeNull();

         bsa.Formula.IsExplicit().ShouldBeTrue();
         bsa.DisplayUnit = bsa.Dimension.Unit("m²");
         sut.Organism.Parameter(CoreConstants.Parameters.WEIGHT).Value = 73;
         sut.Organism.Parameter(CoreConstants.Parameters.HEIGHT).Value = 17.6;
         bsa.ValueInDisplayUnit.ShouldBeEqualTo(1.89, 1e-2);
      }

      [Observation]
      public void the_default_calculation_method_for_bsa_calculation_should_be_mosteller()
      {
         sut.OriginData.CalculationMethodFor(ConverterConstants.Category.BSA).Name.ShouldBeEqualTo(ConverterConstants.CalculationMethod.BSA_Mosteller);
      }

   }

   public class When_creating_an_individual_for_each_population_defined_in_the_database :   concern_for_Individual
   {
      private IPopulationRepository _populationRepository;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _populationRepository = IoC.Resolve<IPopulationRepository>();
      }

      [Observation]
      public void should_create_a_valid_individual()
      {
         _populationRepository.All().Each(x =>
         {
            var individual = DomainFactoryForSpecs.CreateStandardIndividual(population: x.Name);
            individual.ShouldNotBeNull();

            //check that the sum of the organ weights is equal to the input weight
            individual.InputWeight.ShouldBeEqualTo(individual.WeightParameter.Value, 1e-5, x.Name);
         });
      }

   }

   public class When_creating_rabbit : concern_for_Individual
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = DomainFactoryForSpecs.CreateStandardIndividual(CoreConstants.Species.RABBIT);
      }

      [Observation]
      public void individual_species_should_be_rabbit()
      {
         sut.Population.Species.ShouldBeEqualTo(CoreConstants.Species.RABBIT);
      }

      [Observation]
      public void individual_should_not_have_BSA_parameter()
      {
         sut.Organism.Parameter(CoreConstants.Parameters.BSA).ShouldBeNull();
      }
   }

   public class When_creating_cat : concern_for_Individual
   {
      private Individual _beagle;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = DomainFactoryForSpecs.CreateStandardIndividual(CoreConstants.Species.CAT);
         _beagle = DomainFactoryForSpecs.CreateStandardIndividual(CoreConstants.Species.BEAGLE);
      }

      [Observation]
      public void individual_species_should_be_cat()
      {
         sut.Population.Species.ShouldBeEqualTo(CoreConstants.Species.CAT);
      }

      [Observation]
      public void individual_should_not_have_BSA_parameter()
      {
         sut.Organism.Parameter(CoreConstants.Parameters.BSA).ShouldBeNull();
      }

      [Observation]
      public void all_parameters_with_value_origin_copied_from_beagle_should_have_the_same_value_as_in_beagle()
      {
         var allConstCatParameter = sut.Organism.GetAllChildren<Parameter>().Where(p => p.IsConstantParameter()).OrderBy(p=>p.EntityPath()).ToArray();
         var allConstBeagleParameter = _beagle.Organism.GetAllChildren<Parameter>().Where(p => p.IsConstantParameter()).OrderBy(p => p.EntityPath()).ToArray();

         allConstCatParameter.Length.ShouldBeGreaterThan(0);
         allConstCatParameter.Length.ShouldBeEqualTo(allConstBeagleParameter.Length);

         for (var i = 0; i < allConstCatParameter.Length; i++)
         {
            var catParameter = allConstCatParameter[i];
            if (catParameter.ValueOrigin.Description != "Copied from Beagle")
               continue;

            catParameter.Value.ShouldBeEqualTo(allConstBeagleParameter[i].Value, 1e-5);
         }
      }
   }

   public class When_creating_cattle : concern_for_Individual
   {
      private Individual _minipig;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = DomainFactoryForSpecs.CreateStandardIndividual(CoreConstants.Species.CATTLE);
         _minipig = DomainFactoryForSpecs.CreateStandardIndividual(CoreConstants.Species.MINIPIG);
      }

      [Observation]
      public void individual_species_should_be_cattle()
      {
         sut.Population.Species.ShouldBeEqualTo(CoreConstants.Species.CATTLE);
      }

      [Observation]
      public void individual_should_not_have_BSA_parameter()
      {
         sut.Organism.Parameter(CoreConstants.Parameters.BSA).ShouldBeNull();
      }

      [Observation]
      public void all_parameters_with_value_origin_copied_from_minipig_should_have_the_same_value_as_in_minipig()
      {
         var allConstCattleParameter = sut.Organism.GetAllChildren<Parameter>().Where(p => p.IsConstantParameter()).OrderBy(p => p.EntityPath()).ToArray();
         var allConstMinipigParameter = _minipig.Organism.GetAllChildren<Parameter>().Where(p => p.IsConstantParameter()).OrderBy(p => p.EntityPath()).ToArray();

         allConstCattleParameter.Length.ShouldBeGreaterThan(0);
         allConstCattleParameter.Length.ShouldBeEqualTo(allConstMinipigParameter.Length);

         for (var i = 0; i < allConstCattleParameter.Length; i++)
         {
            var catParameter = allConstCattleParameter[i];
            if (catParameter.ValueOrigin.Description != "Copied from Minipig")
               continue;

            catParameter.Value.ShouldBeEqualTo(allConstMinipigParameter[i].Value, 1e-5, catParameter.Name);
         }
      }
   }

   public class When_returning_all_physical_containers_involved_with_an_enzyme_expression : concern_for_Individual
   {
      private IndividualMolecule _enzyme;
      private IReadOnlyList<IContainer> _allPhysicalContainers;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var moleculeFactory = IoC.Resolve<IIndividualEnzymeFactory>();
         sut = DomainFactoryForSpecs.CreateStandardIndividual();
         _enzyme = moleculeFactory.AddMoleculeTo(sut, "CYP");

      }

      protected override void Because()
      {
         _allPhysicalContainers = sut.AllPhysicalContainersWithMoleculeFor(_enzyme);
      }

      [Observation]
      public void should_also_return_all_instances_of_plasma_and_blood_cells_in_blood_container()
      {
         _allPhysicalContainers.Find(x=>x.IsPlasma() && x.ParentContainer.IsNamed(CoreConstants.Organ.ARTERIAL_BLOOD)).Any().ShouldBeTrue();
      }
   }


   public class When_removing_a_molecule_from_an_individual : concern_for_Individual
   {
      private IndividualMolecule _enzyme;
      private IReadOnlyList<IContainer> _allRemovedContainers;
      private IReadOnlyList<IContainer> _allMoleculeContainers;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var moleculeFactory = IoC.Resolve<IIndividualEnzymeFactory>();
         sut = DomainFactoryForSpecs.CreateStandardIndividual();
         _enzyme = moleculeFactory.AddMoleculeTo(sut, "CYP");
         _allMoleculeContainers = sut.AllMoleculeContainersFor(_enzyme);

      }

      protected override void Because()
      {
         _allRemovedContainers = sut.RemoveMolecule(_enzyme);
      }

      [Observation]
      public void should_return_the_list_of_all_containers_associated_with_the_molecule_that_were_removed_from_the_individual_structure()
      {
         _allRemovedContainers.Count.ShouldBeGreaterThan(1);
         //we add the molecule as it is not a molecule container returned by the individual
         _allRemovedContainers.ShouldOnlyContain(new List<IContainer>(_allMoleculeContainers){_enzyme}); 
         _allMoleculeContainers = sut.AllMoleculeContainersFor(_enzyme);
         _allMoleculeContainers.Count.ShouldBeEqualTo(0);
      }
   }
}