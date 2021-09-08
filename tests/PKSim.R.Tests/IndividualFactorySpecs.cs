using System.Linq;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.R.Domain;
using PKSim.R.Services;
using IIndividualFactory = PKSim.R.Services.IIndividualFactory;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.R
{
   public abstract class concern_for_IndividualFactory : ContextForIntegration<IIndividualFactory>
   {
      protected IndividualCharacteristics _individualCharacteristics;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = Api.GetIndividualFactory();
      }
   }

   public class When_creating_an_individual_based_on_a_valid_origin_data : concern_for_IndividualFactory
   {
      private CreateIndividualResults _results;

      protected override void Context()
      {
         base.Context();
         _individualCharacteristics = new IndividualCharacteristics
         {
            Species = CoreConstants.Species.HUMAN,
            Population = CoreConstants.Population.ICRP,
            Age = new Parameter
            {
               Value = 30,
               Unit = "year(s)",
            },
            Weight = new Parameter
            {
               Value = 75,
               Unit = "kg",
            },
            Height = new Parameter
            {
               Value = 175,
               Unit = "cm",
            },
            Gender = CoreConstants.Gender.Male
         };
      }

      protected override void Because()
      {
         _results = sut.CreateIndividual(_individualCharacteristics);
      }

      [Test]
      public void should_return_all_individual_parameters_defined_by_the_create_individual_algorithm()
      {
         _results.DistributedParameters.Length.ShouldBeGreaterThan(0);
         _results.DerivedParameters.Length.ShouldBeGreaterThan(0);
      }
   }

   public class When_creating_an_individual_for_the_human_species_with_a_missing_age_parameter : concern_for_IndividualFactory
   {
      private CreateIndividualResults _result;

      protected override void Context()
      {
         base.Context();
         _individualCharacteristics = new IndividualCharacteristics
         {
            Species = CoreConstants.Species.HUMAN,
            Population = CoreConstants.Population.ICRP,
         };
      }

      protected override void Because()
      {
         _result = sut.CreateIndividual(_individualCharacteristics);
      }

      [Observation]
      public void should_use_the_default_age_parameter()
      {
         _result.ShouldNotBeNull();
      }
   }

   public class When_creating_an_individual_based_on_a_valid_origin_data_with_ontogeny_information : concern_for_IndividualFactory
   {
      private CreateIndividualResults _results;

      protected override void Context()
      {
         base.Context();
         _individualCharacteristics = new IndividualCharacteristics
         {
            Species = CoreConstants.Species.HUMAN,
            Population = CoreConstants.Population.ICRP,
            Age = new Parameter
            {
               Value = 30,
               Unit = "year(s)",
            },
            Weight = new Parameter
            {
               Value = 75,
               Unit = "kg",
            },
            Height = new Parameter
            {
               Value = 17.5,
               Unit = "dm",
            },
            Gender = CoreConstants.Gender.Female,
            Seed =  1234
         };
      }

      protected override void Because()
      {
         _individualCharacteristics.AddMoleculeOntogeny(new MoleculeOntogeny {Molecule = "CYP3A4", Ontogeny = "CYP3A4"});
         _results = sut.CreateIndividual(_individualCharacteristics);
      }

      [Observation]
      public void should_return_an_entry_for_the_ontogeny_factor()
      {
         _results.DistributedParameters.Length.ShouldBeGreaterThan(0);
         _results.DistributedParameters.Last().ParameterPath.Contains("CYP3A4").ShouldBeTrue();
      }

      [Observation]
      public void should_return_the_seed_used()
      {
         _results.Seed.ShouldBeEqualTo(_individualCharacteristics.Seed.GetValueOrDefault(0));
      }

      [Observation]
      public void should_have_added_height_as_a_distributed_parameter()
      {
         var height = _results.DistributedParameters.FirstOrDefault(x =>
            x.ParameterPath == new[] {Constants.ORGANISM, CoreConstants.Parameters.HEIGHT}.ToPathString());

         height.ShouldNotBeNull();
         height.Value.ShouldBeEqualTo(17.5);
         height.Unit.ShouldBeEqualTo("dm");
      }

      [Observation]
      public void should_have_added_the_age_parameter_as_a_distributed_parameter()
      {
         var age = _results.DistributedParameters.FirstOrDefault(x =>
            x.ParameterPath == new[] { Constants.ORGANISM, CoreConstants.Parameters.AGE }.ToPathString());

         age.ShouldNotBeNull();
         age.Value.ShouldBeEqualTo(30);
         age.Unit.ShouldBeEqualTo("year(s)");
      }

      [Observation]
      public void should_have_added_the_gestational_age_parameter_as_a_distributed_parameter()
      {
         var ga = _results.DistributedParameters.FirstOrDefault(x =>
            x.ParameterPath == new[] { Constants.ORGANISM, Constants.Parameters.GESTATIONAL_AGE }.ToPathString());

         ga.ShouldNotBeNull();
         ga.Value.ShouldBeEqualTo(CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS);
         ga.Unit.ShouldBeEqualTo("week(s)");
      }
   }

   public class When_retrieving_the_distributed_parameter_based_on_a_valid_origin_data : concern_for_IndividualFactory
   {
      private DistributedParameterValueWithUnit[] _results;

      protected override void Context()
      {
         base.Context();
         _individualCharacteristics = new IndividualCharacteristics

         {
            Species = CoreConstants.Species.HUMAN,
            Population = CoreConstants.Population.ICRP,
            Age = new Parameter
            {
               Value = 30,
               Unit = "year(s)",
            },
            Weight = new Parameter
            {
               Value = 75,
               Unit = "kg",
            },
            Height = new Parameter
            {
               Value = 175,
               Unit = "cm",
            },
            Gender = CoreConstants.Gender.Male
         };
      }

      protected override void Because()
      {
         _results = sut.DistributionsFor(_individualCharacteristics);
      }

      [Observation]
      public void should_return_all_distributed_parameters_defined_by_the_create_individual_algorithm()
      {
         _results.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void should_have_added_the_unit_to_the_parameter()
      {
         _results.Count(x => !string.IsNullOrEmpty(x.Unit)).ShouldBeGreaterThan(0);
      }

     
   }

   public class When_retrieving_the_distributed_parameter_based_on_a_valid_origin_data_with_ontogeny : concern_for_IndividualFactory
   {
      private DistributedParameterValueWithUnit[] _results;

      protected override void Context()
      {
         base.Context();
         _individualCharacteristics = new IndividualCharacteristics

         {
            Species = CoreConstants.Species.HUMAN,
            Population = CoreConstants.Population.ICRP,
            Age = new Parameter
            {
               Value = 0.5,
               Unit = "year(s)",
            },
            Weight = new Parameter
            {
               Value = 8,
               Unit = "kg",
            },
            Height = new Parameter
            {
               Value = 60,
               Unit = "cm",
            },
            Gender = CoreConstants.Gender.Male
         };
      }

      protected override void Because()
      {
         _individualCharacteristics.AddMoleculeOntogeny(new MoleculeOntogeny {Molecule = "AAA", Ontogeny = "CYP3A4"});
         _individualCharacteristics.AddMoleculeOntogeny(new MoleculeOntogeny {Molecule = "BBB", Ontogeny = "CYP2E1"});
         _results = sut.DistributionsFor(_individualCharacteristics);
      }

      [Observation]
      public void should_return_all_distributed_parameters_defined_by_the_create_individual_algorithm()
      {
         _results.Count().ShouldBeGreaterThan(0);
         // 2 parameters created 
         var allCYP3A4Parameters = _results.Where(x => x.ParameterPath.Contains("AAA")).ToList();
         allCYP3A4Parameters.Count.ShouldBeEqualTo(2);
         allCYP3A4Parameters[0].ParameterPath.ShouldBeEqualTo(new[] {"AAA", CoreConstants.Parameters.ONTOGENY_FACTOR}.ToPathString());
         allCYP3A4Parameters[1].ParameterPath.ShouldBeEqualTo(new[] {"AAA", CoreConstants.Parameters.ONTOGENY_FACTOR_GI}.ToPathString());


         var allCYP2E1Parameters = _results.Where(x => x.ParameterPath.Contains("BBB")).ToList();
         allCYP2E1Parameters.Count.ShouldBeEqualTo(2);
         allCYP2E1Parameters[0].ParameterPath.ShouldBeEqualTo(new[] {"BBB", CoreConstants.Parameters.ONTOGENY_FACTOR}.ToPathString());
         allCYP2E1Parameters[1].ParameterPath.ShouldBeEqualTo(new[] {"BBB", CoreConstants.Parameters.ONTOGENY_FACTOR_GI}.ToPathString());
      }

      [Observation]
      public void should_have_interpolated_the_ontogeny_using_the_expected_values_coming_from_the_database()
      {
         // CYP3A4 GI  PMA   Mean Std
         //             0.77  0.32  1.52
         //             1.08  0.41  1.45
         //             1.44  0.5   1.45


         // CYP3A4 LIV  PMA   Mean Std
         //             0.77  0.12  2.22
         //             0.92  0.2   1.92
         //             1.39  0.49  1.45

         // CYP2E1  PMA   Mean Std
         //             0.9   0.26  2.51
         //             1.05  0.6   1.63
         //             1.12  0.73  1.47
         //             1.21  0.85  1.39
         //             1.3   0.92  1.33


         //0.5 years and 40 weeks GA => PMA is 1.26
         var allCYP3A4Parameters = _results.Where(x => x.ParameterPath.Contains("AAA")).ToList();
         var allCYP2E1Parameters = _results.Where(x => x.ParameterPath.Contains("BBB")).ToList();

         allCYP3A4Parameters[0].Mean.ShouldBeGreaterThan(0.2);
         allCYP3A4Parameters[0].Mean.ShouldBeSmallerThan(0.49);
         allCYP3A4Parameters[0].Std.ShouldBeSmallerThan(1.92);
         allCYP3A4Parameters[0].Std.ShouldBeGreaterThan(1.45);

         allCYP3A4Parameters[1].DistributionType.ShouldBeEqualTo(DistributionTypes.LogNormal);
         allCYP3A4Parameters[1].Mean.ShouldBeGreaterThan(0.41);
         allCYP3A4Parameters[1].Mean.ShouldBeSmallerThan(0.5);
         allCYP3A4Parameters[1].Std.ShouldBeEqualTo(1.45);

         allCYP2E1Parameters[0].DistributionType.ShouldBeEqualTo(DistributionTypes.LogNormal);
         allCYP2E1Parameters[0].Mean.ShouldBeGreaterThan(0.85);
         allCYP2E1Parameters[0].Mean.ShouldBeSmallerThan(0.92);
         allCYP2E1Parameters[0].Std.ShouldBeSmallerThan(1.39);
         allCYP2E1Parameters[0].Std.ShouldBeGreaterThan(1.33);
      }
   }
}