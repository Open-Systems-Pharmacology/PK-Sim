using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public class When_serializing_an_individual_with_expression_profile : ContextForSerialization<Individual>
   {
      private Individual _individual;
      private ExpressionProfile _expressionProfile;
      private Individual _deserializeIndividual;
      private IWithIdRepository _idRepository;

      protected override void Context()
      {
         base.Context();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _expressionProfile = DomainFactoryForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _individual.AddExpressionProfile(_expressionProfile);
         _idRepository = IoC.Resolve<IWithIdRepository>();
         //we need to add a reference to the expression profile to ensure it will be found
         _idRepository.Register(_expressionProfile);
      }

      protected override void Because()
      {
         _deserializeIndividual = SerializeAndDeserialize(_individual);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_individual_and_keep_the_reference_to_the_expression_profile()
      {
         _deserializeIndividual.Uses(_expressionProfile).ShouldBeTrue();
      }

      public override void Cleanup()
      {
         base.Cleanup();
         _idRepository.Unregister(_expressionProfile.Id);
      }
   }
}
