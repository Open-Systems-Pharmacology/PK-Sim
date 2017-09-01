using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using Parameter = PKSim.Core.Snapshots.Parameter;
using Protocol = PKSim.Core.Snapshots.Protocol;

namespace PKSim.Core
{
   public abstract class concern_for_ProtocolMapper : ContextSpecification<ProtocolMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected SimpleProtocol _simpleProtocol;
      protected Protocol _snapshot;
      protected IProtocolFactory _protocolFactory;
      private SchemaMapper _schemaMapper;
      protected AdvancedProtocol _advancedProtocol;
      protected Schema _schema;
      protected IParameter advancedProtocolParameter;

      protected override void Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _schemaMapper = A.Fake<SchemaMapper>();
         _protocolFactory = A.Fake<IProtocolFactory>();

         sut = new ProtocolMapper(_parameterMapper, _protocolFactory, _schemaMapper);

         _simpleProtocol = new SimpleProtocol
         {
            ApplicationType = ApplicationTypes.Intravenous,
            DosingInterval = DosingIntervals.DI_6_6_12,
            Name = "Simple Protocol",
            Description = "Simple Protocol description",
         };
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(3).WithName(Constants.Parameters.START_TIME));
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(4).WithName(CoreConstants.Parameter.INPUT_DOSE));
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(Constants.Parameters.END_TIME));

         A.CallTo(() => _parameterMapper.MapToSnapshot(_simpleProtocol.StartTime)).Returns(new Parameter().WithName(_simpleProtocol.StartTime.Name));
         A.CallTo(() => _parameterMapper.MapToSnapshot(_simpleProtocol.Dose)).Returns(new Parameter().WithName(_simpleProtocol.Dose.Name));
         A.CallTo(() => _parameterMapper.MapToSnapshot(_simpleProtocol.EndTimeParameter)).Returns(new Parameter().WithName(_simpleProtocol.EndTimeParameter.Name));

         _advancedProtocol = new AdvancedProtocol
         {
            Name = "Advanced Protocol",
            Description = "Advanced Protocol description",
            TimeUnit = DomainHelperForSpecs.TimeDimensionForSpecs().DefaultUnit,
         };
         _schema = new Schema().WithName("Schema1");
         _advancedProtocol.AddSchema(_schema);
         advancedProtocolParameter = DomainHelperForSpecs.ConstantParameterWithValue(3).WithName("AdvancedProtocolParameter");
         _advancedProtocol.Add(advancedProtocolParameter);
         A.CallTo(() => _parameterMapper.MapToSnapshot(advancedProtocolParameter)).Returns(new Parameter().WithName(advancedProtocolParameter.Name));

         A.CallTo(() => _schemaMapper.MapToSnapshot(_schema)).Returns(new Snapshots.Schema().WithName(_schema.Name));
      }
   }

   public class When_mapping_a_simple_protocol_to_a_snapshot_protcol : concern_for_ProtocolMapper
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

   public class When_mapping_an_advanced_protocol_to_a_snapshot_protcol : concern_for_ProtocolMapper
   {
      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_advancedProtocol);
      }

      [Observation]
      public void should_save_the_advanced_protocol_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_advancedProtocol.Name);
         _snapshot.Description.ShouldBeEqualTo(_advancedProtocol.Description);
         _snapshot.TimeUnit.ShouldBeEqualTo(_advancedProtocol.TimeUnit.Name);
      }

      [Observation]
      public void should_save_all_protocol_parameters()
      {
         _snapshot.Parameters.ExistsByName(advancedProtocolParameter.Name).ShouldBeTrue();
      }

      [Observation]
      public void should_save_all_protocol_schemas()
      {
         _snapshot.Schemas.ExistsByName(_schema.Name).ShouldBeTrue();
      }
   }

   public class When_mapping_a_valid_protocol_snapshot_to_a_protocol : concern_for_ProtocolMapper
   {
      private SimpleProtocol _newProtocol;

      protected override void Context()
      {
         base.Context();
         _snapshot = sut.MapToSnapshot(_simpleProtocol);
         A.CallTo(() => _protocolFactory.Create(ProtocolMode.Simple, _simpleProtocol.ApplicationType)).Returns(_simpleProtocol);

         _snapshot.Name = "New Protocol";
         _snapshot.Description = "The description that will be deserialized";
         _snapshot.TargetOrgan = "Liver";
         _snapshot.TargetCompartment = "Cells";
      }

      protected override void Because()
      {
         _newProtocol = sut.MapToModel(_snapshot).DowncastTo<SimpleProtocol>();
      }

      [Observation]
      public void should_have_created_a_protocol_with_the_expected_properties()
      {
         _newProtocol.Name.ShouldBeEqualTo(_snapshot.Name);
         _newProtocol.Description.ShouldBeEqualTo(_snapshot.Description);
         _newProtocol.TargetOrgan.ShouldBeEqualTo(_snapshot.TargetOrgan);
         _newProtocol.TargetCompartment.ShouldBeEqualTo(_snapshot.TargetCompartment);
      }

      [Observation]
      public void should_have_updated_all_visible_parameters()
      {
         A.CallTo(() => _parameterMapper.UpdateParameterFromSnapshot(_newProtocol.Parameter(Constants.Parameters.START_TIME), _snapshot.Parameters.FindByName(Constants.Parameters.START_TIME))).MustHaveHappened();
         A.CallTo(() => _parameterMapper.UpdateParameterFromSnapshot(_newProtocol.Parameter(Constants.Parameters.END_TIME), _snapshot.Parameters.FindByName(Constants.Parameters.END_TIME))).MustHaveHappened();
         A.CallTo(() => _parameterMapper.UpdateParameterFromSnapshot(_newProtocol.Parameter(CoreConstants.Parameter.INPUT_DOSE), _snapshot.Parameters.FindByName(CoreConstants.Parameter.INPUT_DOSE))).MustHaveHappened();
      }
   }
}