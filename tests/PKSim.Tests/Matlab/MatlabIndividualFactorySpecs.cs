using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using NUnit.Framework;
using PKSim.Core;
using PKSim.Core.Model;
using OriginData = PKSim.Core.Batch.OriginData;

namespace PKSim.Matlab
{
   [IntegrationTests]
   [Category("Matlab")]
   public abstract class concern_for_MatlabIndividualFactory : ContextSpecification<IMatlabIndividualFactory>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         ApplicationStartup.Initialize(DomainHelperForSpecs.DimensionFilePath, DomainHelperForSpecs.DbFilePath, DomainHelperForSpecs.PKParametersFilePath);
      }

      protected override void Context()
      {
         sut = new MatlabIndividualFactory();
      }
   }

   public class When_creating_an_individual_based_on_a_valid_origin_data : concern_for_MatlabIndividualFactory
   {
      private IEnumerable<ParameterValue> _results;
      private OriginData _batchOriginData;

      protected override void Context()
      {
         base.Context();
         _batchOriginData = new OriginData();
         _batchOriginData.Species = CoreConstants.Species.Human;
         _batchOriginData.Population = CoreConstants.Population.ICRP;
         _batchOriginData.Age = 30;
         _batchOriginData.Weight = 75;
         _batchOriginData.Height = 17.5;
         _batchOriginData.Gender = CoreConstants.Gender.Male;
      }

      protected override void Because()
      {
         _results = sut.CreateIndividual(_batchOriginData, new MoleculeOntogeny[]{});
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
      private OriginData _batchOriginData;

      protected override void Context()
      {
         base.Context();
         _batchOriginData = new OriginData();
         _batchOriginData.Species = CoreConstants.Species.Human;
         _batchOriginData.Population = CoreConstants.Population.ICRP;
         _batchOriginData.Age = 30;
         _batchOriginData.Weight = 75;
         _batchOriginData.Height = 17.5;
         _batchOriginData.Gender = CoreConstants.Gender.Male;
      }

      protected override void Because()
      {
         _results = sut.CreateIndividual(_batchOriginData, new[] {new MoleculeOntogeny("CYP3A4","CYP3A4")});
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
      private OriginData _batchOriginData;
      private DistributedParameterValue[] _results;

      protected override void Context()
      {
         base.Context();
         _batchOriginData = new OriginData();
         _batchOriginData.Species = CoreConstants.Species.Human;
         _batchOriginData.Population = CoreConstants.Population.ICRP;
         _batchOriginData.Age = 30;
         _batchOriginData.Weight = 75;
         _batchOriginData.Height = 17.5;
         _batchOriginData.Gender = CoreConstants.Gender.Male;
      }

      protected override void Because()
      {
         _results = sut.DistributionsFor(_batchOriginData,null);
      }

      [Observation]
      public void should_return_all_distributed_parameters_defined_by_the_create_individual_algorithm()
      {
         _results.Count().ShouldBeGreaterThan(0);
      }
   }
}