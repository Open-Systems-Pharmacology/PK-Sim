using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_IndividualCovariatesXmlSerializer : ContextForSerialization<IndividualCovariates>
   {
      protected IGenderRepository _genderRepository;
      protected IPopulationRepository _populationRepository;

      protected override void Context()
      {
         _genderRepository = IoC.Resolve<IGenderRepository>();
         _populationRepository = IoC.Resolve<IPopulationRepository>();
      }
   }

   public class When_serializing_an_individual_covariate : concern_for_IndividualCovariatesXmlSerializer
   {
      private IndividualCovariates _covariates;
      private IndividualCovariates _deserialized;

      protected override void Context()
      {
         base.Context();
         _covariates = new IndividualCovariates();
         _covariates.Gender = _genderRepository.Male;
         _covariates.Race = _populationRepository.All().First();
         _covariates.AddCovariate("Key1", "Value1");
         _covariates.AddCovariate("Key2", "Value2");
      }

      protected override void Because()
      {
         _deserialized = SerializeAndDeserialize(_covariates);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_individual_covariates()
      {
         _deserialized.Attributes.Count.ShouldBeEqualTo(2);
         _deserialized.Attributes["Key1"].ShouldBeEqualTo("Value1");
         _deserialized.Attributes["Key2"].ShouldBeEqualTo("Value2");
      }
   }
}