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
}