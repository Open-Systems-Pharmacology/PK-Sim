using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Matlab
{
   [IntegrationTests]
   [Category("Matlab")]
   public abstract class concern_for_MatlabIndividualFactory : ContextSpecification<IMatlabIndividualFactory>
   {
      protected OriginData _matlabOriginData;

      public override void GlobalContext()
      {
         base.GlobalContext();
         ApplicationStartup.Initialize();
      }

      protected override void Context()
      {
         sut = new MatlabIndividualFactory();
      }
   }

   public class When_creating_an_individual_based_on_a_valid_origin_data : concern_for_MatlabIndividualFactory
   {
      private IEnumerable<ParameterValue> _results;

      protected override void Context()
      {
         base.Context();
         _matlabOriginData = new OriginData
         {
            Species = CoreConstants.Species.Human,
            Population = CoreConstants.Population.ICRP,
            Age = 30,
            Weight = 75,
            Height = 17.5,
            Gender = CoreConstants.Gender.Male
         };
      }

      protected override void Because()
      {
         _results = sut.CreateIndividual(_matlabOriginData, new MoleculeOntogeny[] { });
      }

      [Observation]
      public void should_return_all_individual_parameters_defined_by_the_create_individual_algorithm()
      {
         _results.Count().ShouldBeGreaterThan(0);
      }
   }

   public class When_creating_an_individual_based_on_a_valid_origin_data_with_ontogeny_information : concern_for_MatlabIndividualFactory
   {
      private IEnumerable<ParameterValue> _results;

      protected override void Context()
      {
         base.Context();
         _matlabOriginData = new OriginData
         {
            Species = CoreConstants.Species.Human,
            Population = CoreConstants.Population.ICRP,
            Age = 30,
            Weight = 75,
            Height = 17.5,
            Gender = CoreConstants.Gender.Male
         };
      }

      protected override void Because()
      {
         _results = sut.CreateIndividual(_matlabOriginData, new[] {new MoleculeOntogeny("CYP3A4", "CYP3A4")});
      }

      [Observation]
      public void should_return_an_entry_for_the_ontogeny_factor()
      {
         _results.Count().ShouldBeGreaterThan(0);
         _results.Last().ParameterPath.Contains("CYP3A4").ShouldBeTrue();
      }
   }

   public class When_retrieving_the_distributed_parmaeter_based_on_a_valid_origin_data : concern_for_MatlabIndividualFactory
   {
      private DistributedParameterValue[] _results;

      protected override void Context()
      {
         base.Context();
         _matlabOriginData = new OriginData
         {
            Species = CoreConstants.Species.Human,
            Population = CoreConstants.Population.ICRP,
            Age = 30,
            Weight = 75,
            Height = 17.5,
            Gender = CoreConstants.Gender.Male
         };
      }

      protected override void Because()
      {
         _results = sut.DistributionsFor(_matlabOriginData, null);
      }

      [Observation]
      public void should_return_all_distributed_parameters_defined_by_the_create_individual_algorithm()
      {
         _results.Count().ShouldBeGreaterThan(0);
      }
   }
}