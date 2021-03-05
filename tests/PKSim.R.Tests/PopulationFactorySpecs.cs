using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Utility.Exceptions;
using PKSim.Core;
using PKSim.Core.Snapshots;
using PKSim.R.Domain;
using PKSim.R.Services;

namespace PKSim.R
{
   public abstract class concern_for_PopulationFactory : ContextForIntegration<IPopulationFactory>
   {
      protected PopulationCharacteristics _populationCharacteristics;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = Api.GetPopulationFactory();
      }
   }

   public class When_creating_a_population_with_invalid_settings_from_R : concern_for_PopulationFactory
   {
      protected override void Context()
      {
         base.Context();

         _populationCharacteristics = new PopulationCharacteristics
         {
            Species = CoreConstants.Species.HUMAN,
            Population = CoreConstants.Population.ICRP,
            Age = new ParameterRange
            {
               Min = 20,
               Max = 15,
               Unit = "year(s)",
            },
            NumberOfIndividuals = 10,
            ProportionOfFemales = 70
         };
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.CreatePopulation(_populationCharacteristics)).ShouldThrowAn<OSPSuiteException>();
      }
   }

   public class When_creating_a_non_preterm_population_from_r : concern_for_PopulationFactory
   {
      private IndividualValuesCache _result;

      protected override void Context()
      {
         base.Context();

         _populationCharacteristics = new PopulationCharacteristics
         {
            Species = CoreConstants.Species.HUMAN,
            Population = CoreConstants.Population.ICRP,
            Age = new ParameterRange
            {
               Min = 0,
               Max = 80,
               Unit = "year(s)",
            },
            Weight = new ParameterRange
            {
               Min = 70,
               Unit = "kg",
            },
            NumberOfIndividuals = 10,
            ProportionOfFemales = 70,
            Seed = 2
         };

         _populationCharacteristics.AddMoleculeOntogeny(new MoleculeOntogeny {Molecule = "CYP3A4", Ontogeny = "CYP3A4"});
         _populationCharacteristics.AddMoleculeOntogeny(new MoleculeOntogeny {Molecule = "CYP2D6", Ontogeny = "CYP2D6"});
      }

      protected override void Because()
      {
         _result = sut.CreatePopulation(_populationCharacteristics);
      }

      [Observation]
      public void should_be_able_to_generate_a_basic_population()
      {
         _result.AllParameterValues.First().Values.Count.ShouldBeEqualTo(_populationCharacteristics.NumberOfIndividuals);
      }

      [Observation]
      public void should_return_enzyme_ontogenies()
      {
         var parameterPaths = _result.AllParameterPaths().ToArray();
         foreach (var moleculeOntogeny in _populationCharacteristics.MoleculeOntogenies)
         {
            parameterPaths.ShouldContain($"{moleculeOntogeny.Molecule}|{CoreConstants.Parameters.ONTOGENY_FACTOR}");
            parameterPaths.ShouldContain($"{moleculeOntogeny.Molecule}|{CoreConstants.Parameters.ONTOGENY_FACTOR_GI}");
         }
      }
   }

   public class When_creating_a_preterm_population_from_r : concern_for_PopulationFactory
   {
      private IndividualValuesCache _result;

      protected override void Context()
      {
         base.Context();

         _populationCharacteristics = new PopulationCharacteristics
         {
            Species = CoreConstants.Species.HUMAN,
            Population = CoreConstants.Population.PRETERM,
            Age = new ParameterRange
            {
               Min = 0,
               Max = 80,
               Unit = "year(s)",
            },
            Weight = new ParameterRange
            {
               Min = 70,
               Unit = "kg",
            },
            GestationalAge = new ParameterRange
            {
               Min = 30,
               Max = 35,
               Unit = "week(s)",
            },

            NumberOfIndividuals = 10,
            ProportionOfFemales = 70
         };
      }

      protected override void Because()
      {
         _result = sut.CreatePopulation(_populationCharacteristics);
      }

      [Observation]
      public void should_be_able_to_generate_a_basic_population()
      {
         _result.AllParameterValues.First().Values.Count.ShouldBeEqualTo(_populationCharacteristics.NumberOfIndividuals);
      }
   }
}