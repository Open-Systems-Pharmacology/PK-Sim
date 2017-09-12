using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_AdvancedParameterMapper : ContextSpecification<AdvancedParameterMapper>
   {
      private ParameterMapper _parameterMapper;
      protected AdvancedParameter _advancedParameter;

      protected override void Context()
      {
         _parameterMapper= A.Fake<ParameterMapper>();
         _advancedParameter = new AdvancedParameter();
         _advancedParameter.DistributedParameter = DomainHelperForSpecs.NormalDistributedParameter();
         _advancedParameter.ParameterPath = "ParameterPath";
         _advancedParameter.Name = "ParameterName";

         sut = new AdvancedParameterMapper(_parameterMapper);
      }
   }

   public class When_mapping_an_advanced_parameter_to_snapshot : concern_for_AdvancedParameterMapper
   {
      private Snapshots.AdvancedParameter _snapshot;

      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_advancedParameter);
      }

      [Observation]
      public void should_return_a_snapshot_containing_the_expected_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_advancedParameter.ParameterPath);
         _snapshot.DistributionType.ShouldBeEqualTo(_advancedParameter.DistributionType.Id); 
      }
   }
}	