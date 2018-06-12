using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_TransportContainerUpdater : ContextSpecification<ITransportContainerUpdater>
   {
      protected ITransporterContainerTemplateRepository _repository;
      protected IEventPublisher _eventPublisher;

      protected override void Context()
      {
         _repository = A.Fake<ITransporterContainerTemplateRepository>();
         _eventPublisher = A.Fake<IEventPublisher>();
         sut = new TransportContainerUpdater(_repository, _eventPublisher);
      }
   }

   public class When_asked_to_update_a_transport_container_for_a_given_species_membrane_and_transport_type_that_do_exit_in_the_databse : concern_for_TransportContainerUpdater
   {
      private TransporterExpressionContainer _transporterContainer;
      private string _species;
      private MembraneLocation _membrane;
      private TransportType _transportType;
      private const string _liver = "Liver";
      private readonly List<TransporterContainerTemplate> _allTransporters = new List<TransporterContainerTemplate>();
      private TransporterContainerTemplate _transporterContainerTemplate;

      protected override void Context()
      {
         base.Context();
         _transporterContainer = new TransporterExpressionContainer().WithName(_liver);
         _species = "human";
         A.CallTo(() => _repository.TransportersFor(_species, _liver)).Returns(_allTransporters);
         _membrane = MembraneLocation.Apical;
         _transportType = TransportType.Efflux;
         _transporterContainerTemplate = new TransporterContainerTemplate {MembraneLocation = _membrane, TransportType = _transportType};
         _allTransporters.Add(_transporterContainerTemplate);
      }

      protected override void Because()
      {
         sut.UpdateTransporterFromTemplate(_transporterContainer, _species, _membrane, _transportType);
      }

      [Observation]
      public void should_update_the_transporter_if_the_template_exists()
      {
         _transporterContainer.MembraneLocation.ShouldBeEqualTo(_membrane);
      }
   }

   public class When_asked_to_update_a_transport_container_for_a_given_species_membrane_and_transport_type_that_do_not_exit_in_the_databse : concern_for_TransportContainerUpdater
   {
      private TransporterExpressionContainer _transporterContainer;
      private string _species;
      private MembraneLocation _membrane;
      private TransportType _transportType;
      private string _liver = "Liver";
      private readonly List<TransporterContainerTemplate> _allTransporters = new List<TransporterContainerTemplate>();
      private TransporterContainerTemplate _transporterContainerTemplate;

      protected override void Context()
      {
         base.Context();
         _transporterContainer = new TransporterExpressionContainer().WithName(_liver);
         _species = "human";
         A.CallTo(() => _repository.TransportersFor(_species, _liver)).Returns(_allTransporters);
         _membrane = MembraneLocation.Apical;
         _transportType = TransportType.Efflux;
         _transporterContainerTemplate = new TransporterContainerTemplate {MembraneLocation = _membrane, TransportType = _transportType};
         _allTransporters.Add(_transporterContainerTemplate);
      }

      [Observation]
      public void should_throw_an_exception_if_the_template_does_not_exist_for_the_given_membrane()
      {
         The.Action(() => sut.UpdateTransporterFromTemplate(_transporterContainer, _species, MembraneLocation.Basolateral, _transportType))
            .ShouldThrowAn<PKSimException>();
      }

      [Observation]
      public void should_throw_an_exception_if_the_template_does_not_exist_for_the_given_transport_type()
      {
         The.Action(() => sut.UpdateTransporterFromTemplate(_transporterContainer, _species, _membrane, TransportType.Secretion))
            .ShouldThrowAn<PKSimException>();
      }
   }

   public class When_asked_to_set_the_default_settings_for_a_transporter : concern_for_TransportContainerUpdater
   {
      private TransporterExpressionContainer _transporterWithTemplate;
      private TransporterExpressionContainer _transporterWithoutTemplate;
      private const string _species = "human";
      private const string _liver = "Liver";
      private const string _kidney = "Kidney";
      private readonly List<TransporterContainerTemplate> _allTransporterTemplates = new List<TransporterContainerTemplate>();
      private TransporterContainerTemplate _transporterContainerTemplate;
      private IndividualTransporter _transporter;
      private TransporterContainerTemplate _defaultTemplate;

      protected override void Context()
      {
         base.Context();
         _transporter = new IndividualTransporter {TransportType = TransportType.Efflux, Name = "toto"};
         _transporterWithTemplate = new TransporterExpressionContainer {MembraneLocation = MembraneLocation.Apical}.WithName(_liver);
         _transporterWithoutTemplate = new TransporterExpressionContainer {MembraneLocation = MembraneLocation.Basolateral}.WithName("Kidney");
         _transporter.Add(_transporterWithTemplate);
         _transporter.Add(_transporterWithoutTemplate);

         _transporterContainerTemplate = new TransporterContainerTemplate {MembraneLocation = MembraneLocation.Apical, TransportType = TransportType.Influx};
         _defaultTemplate = new TransporterContainerTemplate {MembraneLocation = MembraneLocation.Basolateral, TransportType = TransportType.Influx};
         _allTransporterTemplates.Add(_transporterContainerTemplate);

         A.CallTo(() => _repository.HasTransporterTemplateFor(_species, _transporter.Name)).Returns(true);
         A.CallTo(() => _repository.TransportTypeFor(_species, _transporter.Name)).Returns(TransportType.Influx);
         A.CallTo(() => _repository.TransportersFor(_species, _liver, _transporter.Name)).Returns(_allTransporterTemplates);
         A.CallTo(() => _repository.TransportersFor(_species, _kidney, _transporter.Name)).Returns(new List<TransporterContainerTemplate>());
         A.CallTo(() => _repository.TransportersFor(_species, _kidney)).Returns(new List<TransporterContainerTemplate> {_defaultTemplate});
      }

      protected override void Because()
      {
         sut.SetDefaultSettingsForTransporter(_transporter, _species, _transporter.Name);
      }

      [Observation]
      public void should_have_set_the_transporter_type()
      {
         _transporter.TransportType.ShouldBeEqualTo(TransportType.Influx);
      }

      [Observation]
      public void should_not_notify_that_a_template_was_not_found()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<NoTranporterTemplateAvailableEvent>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_use_the_template_defined_in_the_database_if_available()
      {
         _transporterWithTemplate.MembraneLocation.ShouldBeEqualTo(_transporterContainerTemplate.MembraneLocation);
      }

      [Observation]
      public void should_use_the_default_settings_otherwise()
      {
         _transporterWithoutTemplate.MembraneLocation.ShouldBeEqualTo(_defaultTemplate.MembraneLocation);
      }
   }

   public class When_asked_to_set_the_default_settings_for_a_transporter_for_which_template_does_not_exist_in_the_db : concern_for_TransportContainerUpdater
   {
      private const string _species = "human";
      private const string _transporterName = "toto";
      private IndividualTransporter _transporter;

      protected override void Context()
      {
         base.Context();
         _transporter = new IndividualTransporter {TransportType = TransportType.Efflux, Name = "aa"};

         A.CallTo(() => _repository.HasTransporterTemplateFor(_species, _transporterName)).Returns(false);
         A.CallTo(() => _repository.TransportTypeFor(_species, _transporterName)).Returns(TransportType.Efflux);
      }

      protected override void Because()
      {
         sut.SetDefaultSettingsForTransporter(_transporter, _species, _transporterName);
      }

      [Observation]
      public void should_have_set_the_transporter_type()
      {
         _transporter.TransportType.ShouldBeEqualTo(TransportType.Efflux);
      }

      [Observation]
      public void should_not_notify_that_a_template_was_not_found()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<NoTranporterTemplateAvailableEvent>._)).MustHaveHappened();
      }
   }
}