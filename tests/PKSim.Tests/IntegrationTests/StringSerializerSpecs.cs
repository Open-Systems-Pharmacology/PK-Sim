using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_StringSerializer : ContextForIntegration<IStringSerializer>
   {
      protected override void Context()
      {
         sut = IoC.Resolve<IStringSerializer>();
      }
   }

   public class When_serializing_a_parameter_alternative_with_species : concern_for_StringSerializer
   {
      private string _serializationString;
      private ParameterAlternative _deserializedObject;

      protected override void Context()
      {
         base.Context();
         var compound = DomainFactoryForSpecs.CreateStandardCompound();
         var compoundAlternativeTask = IoC.Resolve<IParameterAlternativeFactory>();
         var alternative = compoundAlternativeTask.CreateAlternativeFor(compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND));
         _serializationString = sut.Serialize(alternative);
      }

      protected override void Because()
      {
         _deserializedObject = sut.Deserialize<ParameterAlternative>(_serializationString);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_xml_and_return_an_object_of_the_same_type()
      {
         _deserializedObject.IsAnImplementationOf<ParameterAlternativeWithSpecies>().ShouldBeTrue();
      }
   }
}