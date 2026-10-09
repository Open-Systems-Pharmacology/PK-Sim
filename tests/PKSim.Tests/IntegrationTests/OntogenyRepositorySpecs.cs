using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Maths.Interpolations;
using OSPSuite.Core.Maths.Random;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_OntogenyRepository : ContextForIntegration<IOntogenyRepository>
   {
   }

   public class When_retrieving_all_ontogenies : concern_for_OntogenyRepository
   {
      private IEnumerable<Ontogeny> _result;
      private OriginData _originData;

      protected override void Context()
      {
         base.Context();
         _originData = new OriginData
         {
            Species = new Species {Name = CoreConstants.Species.HUMAN},
            Age = new OriginDataParameter(0.8)
         };
      }

      protected override void Because()
      {
         _result = sut.All();
      }

      [Observation]
      public void should_return_at_least_one_element()
      {
         _result.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void all_ontogenies_values_in_liver_should_be_greater_than_0()
      {
         foreach (var ontogeny in _result)
         {
            var ontogenyFactor = sut.OntogenyFactorFor(ontogeny, CoreConstants.Organ.LIVER, _originData);
            ontogenyFactor.ShouldBeGreaterThan(0);
         }
      }
   }

   public class When_retrieving_the_ontogeny_for_a_given_origin_data : concern_for_OntogenyRepository
   {
      private OriginData _originData;
      private Ontogeny CYP3A4;

      protected override void Context()
      {
         base.Context();
         CYP3A4 = sut.All().FindByName("CYP3A4");
         _originData = new OriginData {Species = new Species {Name = CoreConstants.Species.HUMAN}};
      }

      [Observation]
      public void should_retrieve_the_value_for_the_given_age_and_gestational_age()
      {
         _originData.Age = new OriginDataParameter(5 / 12.0);     //5month
         _originData.GestationalAge = new OriginDataParameter(30); //30 weeks
         var v1 = sut.OntogenyFactorFor(CYP3A4, CoreConstants.Organ.LIVER, _originData);

         _originData.Age = new OriginDataParameter(6 / 12.0);     //6month
         _originData.GestationalAge = new OriginDataParameter(26); //26 weeks
         var v2 = sut.OntogenyFactorFor(CYP3A4, CoreConstants.Organ.LIVER, _originData);

         _originData.Age = new OriginDataParameter(0.5 - 14.0 / 52.0);
         _originData.GestationalAge = null;
         var v3 = sut.OntogenyFactorFor(CYP3A4, CoreConstants.Organ.LIVER, _originData);

         v1.ShouldBeEqualTo(v2, 1e-1);
         v3.ShouldBeEqualTo(v2, 1e-1);
      }
   }

   public class When_retrieving_the_ontogeny_for_a_given_origin_data_with_random_values : concern_for_OntogenyRepository
   {
      private OriginData _originData;
      private Ontogeny CYP3A4;
      private RandomGenerator _randomGenerator;

      protected override void Context()
      {
         base.Context();
         CYP3A4 = sut.All().FindByName("CYP3A4");
         _randomGenerator = new RandomGenerator();
         _originData = new OriginData {Species = new Species {Name = CoreConstants.Species.HUMAN}};
      }

      [Observation]
      public void should_always_create_different_values_based_on_the_distribution()
      {
         _originData.Age = new OriginDataParameter(5 / 12.0);     //5month
         _originData.GestationalAge = new OriginDataParameter(30); //30 weeks
         var v1 = sut.OntogenyFactorFor(CYP3A4, CoreConstants.Organ.LIVER, _originData, _randomGenerator);
         var v2 = sut.OntogenyFactorFor(CYP3A4, CoreConstants.Organ.LIVER, _originData, _randomGenerator);

         v1.ShouldNotBeEqualTo(v2);
      }
   }

   public class When_retrieving_the_ontogeny_for_a_non_existing_ontogeny : concern_for_OntogenyRepository
   {
      private OriginData _originData;
      private RandomGenerator _randomGenerator;

      protected override void Context()
      {
         base.Context();
         _randomGenerator = new RandomGenerator();
         _originData = new OriginData {Species = new Species {Name = CoreConstants.Species.HUMAN}};
      }

      [Observation]
      public void should_always_create_different_values_based_on_the_distribution()
      {
         sut.OntogenyFactorFor(new NullOntogeny(), CoreConstants.Organ.LIVER, _originData, _randomGenerator).ShouldBeEqualTo(1);
      }
   }

   public class When_retrieving_the_ontogeny_for_an_imported_ontogeny_table_containing_a_first_value_resulting_in_an_undefined_percentile : concern_for_OntogenyRepository
   {
      private OriginData _originData;
      private RandomGenerator _randomGenerator;
      private UserDefinedOntogeny _userDefinedOntogeny;

      protected override void Context()
      {
         base.Context();
         _randomGenerator = new RandomGenerator();
         _userDefinedOntogeny= A.Fake<UserDefinedOntogeny>();
         A.CallTo(() => _userDefinedOntogeny.PostmenstrualAges()).Returns(new [] {1, 2, 3f});
         A.CallTo(() => _userDefinedOntogeny.OntogenyFactors()).Returns(new [] {0, 1, 31f});
         A.CallTo(() => _userDefinedOntogeny.Deviations()).Returns(new [] {1, 1, 1f});
         _originData = new OriginData { Species = new Species { Name = CoreConstants.Species.HUMAN }, Age = new OriginDataParameter(1), GestationalAge = new OriginDataParameter(24)};
      }

      [Observation]
      public void should_always_create_different_values_based_on_the_distribution()
      {
         sut.OntogenyFactorFor(_userDefinedOntogeny, CoreConstants.Organ.LIVER, _originData, _randomGenerator).ShouldBeSmallerThan(1);
      }
   }

   public class When_retrieving_the_ontogeny_factor_for_a_human_adult : concern_for_OntogenyRepository
   {
      private OriginData _adultOriginData;

      protected override void Context()
      {
         base.Context();
         _adultOriginData = new OriginData
         {
            Species = new Species {Name = CoreConstants.Species.HUMAN}, 
            Age = new OriginDataParameter(30)
         };
      }

      [Observation]
      public void should_always_return_1()
      {
         foreach (var ontogeny in sut.AllFor(CoreConstants.Species.HUMAN))
         {
            if (ontogeny.Name.IsOneOf("UGT1A1","CYP3A7")) continue;
            sut.OntogenyFactorFor(ontogeny, "Liver", _adultOriginData).ShouldBeEqualTo(1, ontogeny.Name);
         }
      }
   }

   //https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3392
   public class When_retrieving_the_ontogeny_factor_for_the_newly_added_ontogenies : concern_for_OntogenyRepository
   {
      private static OriginData originDataFor(double ageInYears) => new OriginData
      {
         Species = new Species {Name = CoreConstants.Species.HUMAN},
         Age = new OriginDataParameter(ageInYears)
      };

      private Ontogeny ontogenyFor(string ontogenyName)
      {
         var ontogeny = sut.AllFor(CoreConstants.Species.HUMAN).FindByName(ontogenyName);
         (ontogeny != null).ShouldBeTrue($"Ontogeny '{ontogenyName}' is not defined in the database");
         return ontogeny;
      }

      [Observation]
      public void should_use_the_same_ontogeny_in_the_liver_and_in_the_intestine()
      {
         //both ontogenies are defined for all tissues, so the GI value must not differ from the liver value
         foreach (var ontogenyName in new[] {CoreConstantsForSpecs.Molecule.CES1, CoreConstantsForSpecs.Molecule.P_GP})
         {
            var ontogeny = ontogenyFor(ontogenyName);
            var originData = originDataFor(1);

            var liverFactor = sut.OntogenyFactorFor(ontogeny, CoreConstants.Groups.ONTOGENY_LIVER, originData);
            var duodenumFactor = sut.OntogenyFactorFor(ontogeny, CoreConstants.Groups.ONTOGENY_DUODENUM, originData);

            //a pre-school child is not fully matured yet, so the factor must be below the adult value of 1
            liverFactor.ShouldBeSmallerThan(1);
            duodenumFactor.ShouldBeEqualTo(liverFactor, 1e-10, ontogenyName);
         }
      }

      [Observation]
      public void should_increase_monotonously_with_age()
      {
         foreach (var ontogenyName in new[] {CoreConstantsForSpecs.Molecule.CES1, CoreConstantsForSpecs.Molecule.P_GP})
         {
            var allValues = sut.AllValuesFor(ontogenyFor(ontogenyName)).OrderBy(x => x.PostmenstrualAge).ToList();
            allValues.Count.ShouldBeGreaterThan(0);

            for (var i = 1; i < allValues.Count; i++)
            {
               allValues[i].OntogenyFactor.ShouldBeGreaterThanOrEqualTo(allValues[i - 1].OntogenyFactor);
            }
         }
      }

      [Observation]
      public void should_return_a_defined_value_for_a_newborn_even_though_the_p_gp_ontogeny_starts_at_zero()
      {
         var pgp = ontogenyFor(CoreConstantsForSpecs.Molecule.P_GP);

         //the first supporting point of the P-gp ontogeny has an ontogeny factor of 0, which has no defined percentile
         var ontogenyFactor = sut.OntogenyFactorFor(pgp, CoreConstants.Organ.LIVER, originDataFor(0), new RandomGenerator());

         double.IsNaN(ontogenyFactor).ShouldBeFalse();
         ontogenyFactor.ShouldBeGreaterThanOrEqualTo(0);
      }
   }

   public class When_retrieving_all_ontogeny_factors_for_a_protein_strict_bigger_than_a_given_PMA_using_a_random_factor : concern_for_OntogenyRepository
   {
      private readonly RandomGenerator _randomGenerator = new RandomGenerator();
      private readonly OriginData _originData = new OriginData {Species = new Species {Name = CoreConstants.Species.HUMAN}, GestationalAge = new OriginDataParameter(25), Age = new OriginDataParameter(0)};
      private IReadOnlyList<Sample> _resultRandom;
      private IReadOnlyList<Sample> _resultMean;

      protected override void Because()
      {
         _resultRandom = sut.AllPlasmaProteinOntogenyFactorForStrictBiggerThanPMA(CoreConstants.Parameters.ONTOGENY_FACTOR_ALBUMIN, _originData, _randomGenerator);
         _resultMean = sut.AllPlasmaProteinOntogenyFactorForStrictBiggerThanPMA(CoreConstants.Parameters.ONTOGENY_FACTOR_ALBUMIN, _originData);
      }

      [Observation]
      public void should_return_the_distributed_value_using_the_same_percentile()
      {
         _resultRandom.Count.ShouldBeEqualTo(_resultMean.Count);
         var firstRandom = _resultRandom[0].Y;
         var firstMean = _resultMean[0].Y;
         for (int i = 1; i < _resultMean.Count; i++)
         {
            if (firstMean >= firstRandom)
               _resultMean[i].Y.ShouldBeGreaterThanOrEqualTo(_resultRandom[i].Y);
            else
               _resultMean[i].Y.ShouldBeSmallerThan(_resultRandom[i].Y);
         }
      }
   }
}