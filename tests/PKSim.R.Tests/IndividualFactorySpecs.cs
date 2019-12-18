using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Populations;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.R.Domain;
using IIndividualFactory = PKSim.R.Services.IIndividualFactory;

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
      private IEnumerable<ParameterValue> _results;

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

      [Observation]
      public void should_return_all_individual_parameters_defined_by_the_create_individual_algorithm()
      {
         _results.Count().ShouldBeGreaterThan(0);
      }
   }

   public class When_creating_an_individual_based_on_a_valid_origin_data_with_ontogeny_information : concern_for_IndividualFactory
   {
      private IEnumerable<ParameterValue> _results;

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
            Gender = CoreConstants.Gender.Female
         };
      }

      protected override void Because()
      {
         _individualCharacteristics.AddMoleculeOntogeny(new MoleculeOntogeny { Molecule = "CYP3A4", Ontogeny = "CYP3A4" });
         _results = sut.CreateIndividual(_individualCharacteristics);
      }

      [Observation]
      public void should_return_an_entry_for_the_ontogeny_factor()
      {
         _results.Count().ShouldBeGreaterThan(0);
         _results.Last().ParameterPath.Contains("CYP3A4").ShouldBeTrue();
      }
   }

   public class When_retrieving_the_distributed_parameter_based_on_a_valid_origin_data : concern_for_IndividualFactory
   {
      private DistributedParameterValue[] _results;

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
   }
}