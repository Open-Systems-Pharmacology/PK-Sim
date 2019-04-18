using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_ProtocolUpdater : ContextSpecification<IProtocolUpdater>
   {
      protected ISimpleProtocolToSchemaMapper _schemaMapper;

      protected override void Context()
      {
         _schemaMapper = A.Fake<ISimpleProtocolToSchemaMapper>();
         sut = new ProtocolUpdater(_schemaMapper);
      }
   }

   public class When_updating_a_simple_protocol_from_an_advanced_protocol : concern_for_ProtocolUpdater
   {
      private Protocol _sourceProtocol;
      private Protocol _targetProtocol;

      protected override void Context()
      {
         base.Context();
         _targetProtocol = new SimpleProtocol {DomainHelperForSpecs.ConstantParameterWithValue(1).WithName<IParameter>("SimpleParameter")};
         _sourceProtocol = new AdvancedProtocol {Id = "Id", Name = "Advanced Protocol", Description = "tralala"};
         _sourceProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName<IParameter>("AdvancedParameter"));
      }

      protected override void Because()
      {
         sut.UpdateProtocol(_sourceProtocol, _targetProtocol);
      }

      [Observation]
      public void should_only_update_the_standard_properties_of_the_protocol()
      {
         _targetProtocol.Name.ShouldBeEqualTo(_sourceProtocol.Name);
         _targetProtocol.Id.ShouldBeEqualTo(_sourceProtocol.Id);
         _targetProtocol.Description.ShouldBeEqualTo(_sourceProtocol.Description);
      }
   }

   public class When_updating_an_advanced_protocol_from_an_simple_protocol : concern_for_ProtocolUpdater
   {
      private SimpleProtocol _sourceProtocol;
      private AdvancedProtocol _targetProtocol;
      private Schema _schema1;
      private Schema _schema2;
      private Schema _oldSchema;

      protected override void Context()
      {
         base.Context();
         _sourceProtocol = new SimpleProtocol {Id = "Id", Name = "Simple Protocol", Description = "tralala"};
         _sourceProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("SimpleParameter"));
         _targetProtocol = new AdvancedProtocol {DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(name: "AdvancedParameter")};
         _schema1 = new Schema {Name = "Schema1"};
         _schema2 = new Schema {Name = "Schema2"};
         _oldSchema = new Schema {Name = "_oldSchema"};
         _targetProtocol.AddSchema(_oldSchema);
         A.CallTo(() => _schemaMapper.MapFrom(_sourceProtocol)).Returns(new[] {_schema1, _schema2});
      }

      protected override void Because()
      {
         sut.UpdateProtocol(_sourceProtocol, _targetProtocol);
      }

      [Observation]
      public void should_update_the_standard_properties_of_the_protocol()
      {
         _targetProtocol.Name.ShouldBeEqualTo(_sourceProtocol.Name);
         _targetProtocol.Id.ShouldBeEqualTo(_sourceProtocol.Id);
         _targetProtocol.Description.ShouldBeEqualTo(_sourceProtocol.Description);
      }

      [Observation]
      public void should_create_the_schema_items_matching_the_structure_of_the_simple_protocol_and_add_them_to_the_advanced_protocol()
      {
         _targetProtocol.AllSchemas.ShouldOnlyContain(_schema1, _schema2);
      }

      [Observation]
      public void should_remove_the_existing_schema_items_if_any_were_available()
      {
         _targetProtocol.AllSchemas.Contains(_oldSchema).ShouldBeFalse();
      }
   }
}