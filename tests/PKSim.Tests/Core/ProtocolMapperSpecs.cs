using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;
using Parameter = PKSim.Core.Snapshots.Parameter;
using Protocol = PKSim.Core.Snapshots.Protocol;

namespace PKSim.Core
{
   public abstract class concern_for_ProtocolMapper : ContextSpecificationAsync<ProtocolMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected SimpleProtocol _simpleProtocol;
      protected Protocol _snapshot;
      protected IProtocolFactory _protocolFactory;
      protected SchemaMapper _schemaMapper;
      protected AdvancedProtocol _advancedProtocol;
      protected Schema _schema;
      protected IParameter _advancedProtocolParameter;
      protected IDimensionRepository _dimensionRepository;
      protected Snapshots.Schema _snapshotSchema;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _schemaMapper = A.Fake<SchemaMapper>();
         _protocolFactory = A.Fake<IProtocolFactory>();
         _dimensionRepository= A.Fake<IDimensionRepository>(); 

         sut = new ProtocolMapper(_parameterMapper, _protocolFactory, _schemaMapper,_dimensionRepository);

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
         _advancedProtocolParameter = DomainHelperForSpecs.ConstantParameterWithValue(3).WithName("AdvancedProtocolParameter");
         _advancedProtocol.Add(_advancedProtocolParameter);
         A.CallTo(() => _parameterMapper.MapToSnapshot(_advancedProtocolParameter)).Returns(new Parameter().WithName(_advancedProtocolParameter.Name));

         _snapshotSchema = new Snapshots.Schema().WithName(_schema.Name);
         A.CallTo(() => _schemaMapper.MapToSnapshot(_schema)).Returns(_snapshotSchema);

         A.CallTo(() => _dimensionRepository.Time).Returns(DomainHelperForSpecs.TimeDimensionForSpecs());

         return Task.FromResult(true);
      }
   }

   public class When_mapping_a_simple_protocol_to_a_snapshot_protcol : concern_for_ProtocolMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_simpleProtocol);
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
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_advancedProtocol);
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
         _snapshot.Parameters.ExistsByName(_advancedProtocolParameter.Name).ShouldBeTrue();
      }

      [Observation]
      public void should_save_all_protocol_schemas()
      {
         _snapshot.Schemas.ExistsByName(_schema.Name).ShouldBeTrue();
      }
   }

   public class When_mapping_a_valid_protocol_snapshot_to_a_simple_protocol : concern_for_ProtocolMapper
   {
      private SimpleProtocol _newProtocol;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_simpleProtocol);
         A.CallTo(() => _protocolFactory.Create(ProtocolMode.Simple, _simpleProtocol.ApplicationType)).Returns(_simpleProtocol);

         _snapshot.Name = "New Protocol";
         _snapshot.Description = "The description that will be deserialized";
         _snapshot.TargetOrgan = "Liver";
         _snapshot.TargetCompartment = "Cells";
      }

      protected override async Task Because()
      {
         _newProtocol = (await sut.MapToModel(_snapshot)).DowncastTo<SimpleProtocol>();
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
         A.CallTo(() => _parameterMapper.MapParameters(_snapshot.Parameters, _newProtocol, _newProtocol.Name)).MustHaveHappened();
     }
   }

   public class When_mapping_a_valid_protocol_snapshot_to_an_advanced_protocol : concern_for_ProtocolMapper
   {
      private AdvancedProtocol _newProtocol;
      private Schema _newSchema;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_advancedProtocol);
         A.CallTo(() => _protocolFactory.Create(ProtocolMode.Advanced)).Returns(_advancedProtocol);

         _snapshot.Name = "New Advanced Protocol";
         _snapshot.Description = "The description that will be deserialized";

         _newSchema = new Schema().WithName("I am a new schema");
         A.CallTo(() => _schemaMapper.MapToModel(_snapshotSchema)).Returns(_newSchema);
      }

      protected override async Task Because()
      {
         _newProtocol = (await sut.MapToModel(_snapshot)).DowncastTo<AdvancedProtocol>();
      }

      [Observation]
      public void should_have_created_a_protocol_with_the_expected_properties()
      {
         _newProtocol.Name.ShouldBeEqualTo(_snapshot.Name);
         _newProtocol.Description.ShouldBeEqualTo(_snapshot.Description);
      }

      [Observation]
      public void should_have_added_a_schema_based_on_snapshot_schema()
      {
         _advancedProtocol.Contains(_newSchema).ShouldBeTrue();
      }

      [Observation]
      public void should_have_updated_all_visible_parameters()
      {
         A.CallTo(() => _parameterMapper.MapParameters(_snapshot.Parameters, _newProtocol, _newProtocol.Name)).MustHaveHappened();
      }
   }

}