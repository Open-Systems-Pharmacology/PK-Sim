using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Events;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_TransportContainerUpdater : ContextSpecification<ITransportContainerUpdater>
   {
      protected ITransporterContainerTemplateRepository _repository;
      protected IEventPublisher _eventPublisher;
      protected ITransportDirectionRepository _transportDirectionRepository;

      protected override void Context()
      {
         _repository = A.Fake<ITransporterContainerTemplateRepository>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _transportDirectionRepository = A.Fake<ITransportDirectionRepository>();
         sut = new TransportContainerUpdater(_repository, _eventPublisher);
      }
   }

   public class When_asked_to_set_the_default_settings_for_a_transporter : concern_for_TransportContainerUpdater
   {
      private TransporterExpressionContainer _transporterWithTemplate;
      private TransporterExpressionContainer _transporterWithoutTemplate;
      private const string _species = "human";
      private readonly List<TransporterContainerTemplate> _allTransporterTemplates = new List<TransporterContainerTemplate>();
      private TransporterContainerTemplate _transporterContainerTemplate;
      private IndividualTransporter _transporter;
      private TransporterContainerTemplate _defaultTemplate;
      private ISimulationSubject _individual;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<ISimulationSubject>();
         _individual.Species.Name = _species;
         _transporter = new IndividualTransporter {TransportType = TransportType.Efflux, Name = "toto"};
         var organism = new Organism();
         var liver = new Container().WithName(CoreConstants.Organ.Liver).WithParentContainer(organism);
         var liverCell = new Container().WithName(CoreConstants.Compartment.INTRACELLULAR).WithParentContainer(liver);
         var kidney = new Container().WithName(CoreConstants.Organ.Kidney).WithParentContainer(organism);
         var kidneyCell = new Container().WithName(CoreConstants.Compartment.INTRACELLULAR).WithParentContainer(kidney);
         _transporterWithTemplate = new TransporterExpressionContainer {TransportDirection = TransportDirectionId.InfluxInterstitialToIntracellular}
            .WithParentContainer(liverCell);
         _transporterWithoutTemplate = new TransporterExpressionContainer
               {TransportDirection = TransportDirectionId.EffluxIntracellularToInterstitial}
            .WithParentContainer(kidneyCell);

         A.CallTo(() => _individual.AllMoleculeContainersFor<TransporterExpressionContainer>(_transporter))
            .Returns(new[] {_transporterWithTemplate, _transporterWithoutTemplate,});

         _transporterContainerTemplate = new TransporterContainerTemplate {TransportType = TransportType.Influx};
         _defaultTemplate = new TransporterContainerTemplate {TransportType = TransportType.Influx};
         _allTransporterTemplates.Add(_transporterContainerTemplate);

         A.CallTo(() => _repository.HasTransporterTemplateFor(_species, _transporter.Name)).Returns(true);
         A.CallTo(() => _repository.TransportTypeFor(_species, _transporter.Name)).Returns(TransportType.Influx);
         A.CallTo(() => _repository.TransportersFor(_species, liver.Name, _transporter.Name)).Returns(_allTransporterTemplates);
         A.CallTo(() => _repository.TransportersFor(_species, kidney.Name, _transporter.Name)).Returns(new List<TransporterContainerTemplate>());
         A.CallTo(() => _repository.TransportersFor(_species, kidney.Name)).Returns(new List<TransporterContainerTemplate> {_defaultTemplate});
      }

      protected override void Because()
      {
         sut.SetDefaultSettingsForTransporter(_individual, _transporter, _transporter.Name);
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
         _transporterWithTemplate.TransportDirection.ShouldBeEqualTo(_transporterContainerTemplate.TransportDirection);
      }

      [Observation]
      public void should_use_the_default_settings_otherwise()
      {
         _transporterWithoutTemplate.TransportDirection.ShouldBeEqualTo(_defaultTemplate.TransportDirection);
      }
   }

   public class
      When_asked_to_set_the_default_settings_for_a_transporter_for_which_template_does_not_exist_in_the_db : concern_for_TransportContainerUpdater
   {
      private const string _species = "human";
      private const string _transporterName = "toto";
      private IndividualTransporter _transporter;
      private ISimulationSubject _individual;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<ISimulationSubject>();
         _individual.Species.Name = _species;
         _transporter = new IndividualTransporter {TransportType = TransportType.Efflux, Name = "aa"};

         A.CallTo(() => _repository.HasTransporterTemplateFor(_species, _transporterName)).Returns(false);
         A.CallTo(() => _repository.TransportTypeFor(_species, _transporterName)).Returns(TransportType.Efflux);
      }

      protected override void Because()
      {
         sut.SetDefaultSettingsForTransporter(_individual, _transporter, _transporterName);
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