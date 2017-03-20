using System.Linq;

using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Assets;
using OSPSuite.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_ProtocolUpdater : ContextSpecification<IProtocolUpdater>
   {
      protected ISimpleProtocolToSchemaMapper _schemaMapper;
      protected IDialogCreator _dialogCreator;

      protected override void Context()
      {
         _schemaMapper=A.Fake<ISimpleProtocolToSchemaMapper>();
         _dialogCreator = A.Fake<IDialogCreator>();
         sut = new ProtocolUpdater(_schemaMapper,_dialogCreator);
      }
   }

   
   public class When_updating_a_simple_protocol_from_an_advanced_protocol : concern_for_ProtocolUpdater
   {
      private  PKSim.Core.Model.Protocol _sourceProtocol;
      private  PKSim.Core.Model.Protocol _targetProtocol;

      protected override void Context()
      {
         base.Context();
         _targetProtocol = new SimpleProtocol();
         _targetProtocol.Add(WithExtensions.WithName<IParameter>(DomainHelperForSpecs.ConstantParameterWithValue(1), "SimpleParameter"));
         _sourceProtocol = new AdvancedProtocol { Id = "Id", Name = "Advanced Protocol", Description = "tralala" };
         _sourceProtocol.Add(WithExtensions.WithName<IParameter>(DomainHelperForSpecs.ConstantParameterWithValue(1), "AdvancedParameter"));

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
      private  PKSim.Core.Model.SimpleProtocol _sourceProtocol;
      private  PKSim.Core.Model.AdvancedProtocol _targetProtocol;
      private Schema _schema1;
      private Schema _schema2;
      private Schema _oldSchema;

      protected override void Context()
      {
         base.Context();
         _sourceProtocol = new SimpleProtocol { Id = "Id", Name = "Simple Protocol", Description = "tralala" };
         _sourceProtocol.Add(WithExtensions.WithName<IParameter>(DomainHelperForSpecs.ConstantParameterWithValue(1), "SimpleParameter"));
         _targetProtocol = new AdvancedProtocol();
         _targetProtocol.Add(WithExtensions.WithName<IParameter>(DomainHelperForSpecs.ConstantParameterWithValue(1), "AdvancedParameter"));
         _schema1 =new Schema{Name = "Schema1"};
         _schema2 = new Schema { Name = "Schema2" };
         _oldSchema = new Schema { Name = "_oldSchema" };
         _targetProtocol.AddSchema(_oldSchema);
         A.CallTo(() => _schemaMapper.MapFrom(_sourceProtocol)).Returns(new[] { _schema1, _schema2 });

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

   
   public class When_the_protocol_updater_is_asked_if_a_switch_is_possible_from_an_advanced_protocol : concern_for_ProtocolUpdater
   {
      private  PKSim.Core.Model.Protocol _sourceProtocol;
      private bool _result;

      protected override void Context()
      {
         base.Context();
         _sourceProtocol = new AdvancedProtocol();
      }

      protected override void Because()
      {
         _result= sut.ValidateSwitchFrom(_sourceProtocol);
      }

      [Observation]
      public void should_return_that_a_switch_is_possible()
      {
         _result.ShouldBeTrue();
      }

      [Observation]
      public void should_not_display_a_message_to_the_user()
      {
         A.CallTo(() => _dialogCreator.MessageBoxError(A<string>.Ignored)).MustNotHaveHappened();
      }
   }

   
   public class When_the_protocol_updater_is_asked_if_a_switch_is_possible_from_a_simple_protocol_using_a_user_defined_application : concern_for_ProtocolUpdater
   {
      private  PKSim.Core.Model.SimpleProtocol _sourceProtocol;
      private bool _result;

      protected override void Context()
      {
         base.Context();
         _sourceProtocol = new SimpleProtocol();
         _sourceProtocol.ApplicationType = ApplicationTypes.UserDefined;
      }

      protected override void Because()
      {
         _result = sut.ValidateSwitchFrom(_sourceProtocol);
      }

      [Observation]
      public void should_return_that_a_switch_is_impossible()
      {
         _result.ShouldBeFalse();
      }

      [Observation]
      public void should_display_an_error_explaining_why_the_switch_is_impossible()
      {
         A.CallTo(() => _dialogCreator.MessageBoxError(PKSimConstants.Error.CannotSwitchToAdvancedProtocolWhenUsingUserDefinedAppplication)).MustHaveHappened();
      }
   }
}	