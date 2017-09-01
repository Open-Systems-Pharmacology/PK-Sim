using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_SimpleProtocolMapper : ContextSpecification<ProtocolMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected SimpleProtocol _simpleProtocol;
      protected Snapshots.Protocol _snapshot;

      protected override void Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         sut = new ProtocolMapper(_parameterMapper);

         sut = new ProtocolMapper(_parameterMapper);

         _simpleProtocol = new SimpleProtocol
         {
            ApplicationType = ApplicationTypes.Intravenous,
            DosingInterval = DosingIntervals.DI_6_6_12,
            Name = "Prot",
            Description = "Prot description",
         };
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(3).WithName(Constants.Parameters.START_TIME));
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(4).WithName(CoreConstants.Parameter.INPUT_DOSE));
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(Constants.Parameters.END_TIME));

         A.CallTo(() => _parameterMapper.MapToSnapshot(_simpleProtocol.StartTime)).Returns(new Parameter().WithName(_simpleProtocol.StartTime.Name));
         A.CallTo(() => _parameterMapper.MapToSnapshot(_simpleProtocol.Dose)).Returns(new Parameter().WithName(_simpleProtocol.Dose.Name));
         A.CallTo(() => _parameterMapper.MapToSnapshot(_simpleProtocol.EndTimeParameter)).Returns(new Parameter().WithName(_simpleProtocol.EndTimeParameter.Name));
      }
   }

   public class When_mapping_a_simple_protocol_to_a_snapshot_simple_protcol : concern_for_SimpleProtocolMapper
   {
      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_simpleProtocol);
      }

      [Observation]
      public void should_save_the_simple_protocol_properties()
      {
         _snapshot.ApplicationType.ShouldBeEqualTo(_simpleProtocol.ApplicationType.Name);
         _snapshot.Name.ShouldBeEqualTo(_simpleProtocol.Name);
         _snapshot.Description.ShouldBeEqualTo(_simpleProtocol.Description);
      }

      [Observation]
      public void should_save_all_protocol_parameters()
      {
         _snapshot.Parameters.ExistsByName(Constants.Parameters.START_TIME).ShouldBeTrue();
         _snapshot.Parameters.ExistsByName(CoreConstants.Parameter.INPUT_DOSE).ShouldBeTrue();
         _snapshot.Parameters.ExistsByName(Constants.Parameters.END_TIME).ShouldBeTrue();
      }
   }
}