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
      protected IParameter _fractionExpressedEpithelial;
      protected ISimulationSubject _individual;
      protected TransporterExpressionContainer _transporterWithTemplate;
      protected TransporterExpressionContainer _transporterWithoutTemplate;
      protected const string _species = "human";
      protected readonly List<TransporterContainerTemplate> _allTransporterTemplates = new List<TransporterContainerTemplate>();
      protected TransporterContainerTemplate _transporterContainerTemplate;
      protected IndividualTransporter _transporter;

      protected override void Context()
      {
         _repository = A.Fake<ITransporterContainerTemplateRepository>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _transportDirectionRepository = A.Fake<ITransportDirectionRepository>();
         sut = new TransportContainerUpdater(_repository, _eventPublisher);
         _fractionExpressedEpithelial = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.FRACTION_EXPRESSED_EPITHELIAL);
         _individual = A.Fake<ISimulationSubject>();
         _individual.Species.Name = _species;
         _transporter = new IndividualTransporter { TransportType = TransportType.Efflux, Name = "toto" };
         var organism = new Organism();
         var liver = new Container().WithName(CoreConstants.Organ.LIVER).WithParentContainer(organism);
         var liverCell = new Container().WithName(CoreConstants.Compartment.INTRACELLULAR).WithParentContainer(liver);
         var kidney = new Container().WithName(CoreConstants.Organ.KIDNEY).WithParentContainer(organism);
         var kidneyCell = new Container().WithName(CoreConstants.Compartment.INTRACELLULAR).WithParentContainer(kidney);
         _transporterWithTemplate = new TransporterExpressionContainer { TransportDirection = TransportDirectionId.InfluxInterstitialToIntracellular }
            .WithParentContainer(liverCell);
         _transporterWithoutTemplate = new TransporterExpressionContainer
         { TransportDirection = TransportDirectionId.EffluxIntracellularToInterstitial }
            .WithParentContainer(kidneyCell);

         _transporterWithoutTemplate.Add(_fractionExpressedEpithelial);
         A.CallTo(() => _individual.AllMoleculeContainersFor<TransporterExpressionContainer>(_transporter))
            .Returns(new[] { _transporterWithTemplate, _transporterWithoutTemplate, });

         _transporterContainerTemplate = new TransporterContainerTemplate { TransportType = TransportType.Influx };
         _allTransporterTemplates.Add(_transporterContainerTemplate);

         A.CallTo(() => _repository.HasTransporterTemplateFor(_species, _transporter.Name)).Returns(true);
         A.CallTo(() => _repository.TransportTypeFor(_species, _transporter.Name)).Returns(TransportType.Influx);
         A.CallTo(() => _repository.TransportersFor(_species, liver.Name, _transporter.Name)).Returns(_allTransporterTemplates);
         A.CallTo(() => _repository.TransportersFor(_species, kidney.Name, _transporter.Name)).Returns(new List<TransporterContainerTemplate>());

      }
   }

   public class When_asked_to_set_the_default_settings_for_a_transporter : concern_for_TransportContainerUpdater
   {
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
         _transporterWithTemplate.TransportDirection.ShouldBeEqualTo(TransportDirections.DefaultDirectionFor(TransportType.Influx, _transporterWithTemplate));
      }

      [Observation]
      public void should_use_the_default_settings_otherwise()
      {
         _transporterWithoutTemplate.TransportDirection.ShouldBeEqualTo(TransportDirections.DefaultDirectionFor(TransportType.Influx, _transporterWithoutTemplate));
      }
   }

   public class  When_asked_to_set_the_default_settings_for_a_transporter_for_which_template_does_not_exist_in_the_db : concern_for_TransportContainerUpdater
   {
      private const string _transporterName = "toto";

      protected override void Context()
      {
         base.Context();
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

   public class When_updating_the_transport_type_from_influx_to_efflux : concern_for_TransportContainerUpdater
   {
      protected override void Because()
      {
         sut.SetDefaultSettingsForTransporter(_individual, _transporter,TransportType.Efflux);
      }

      [Observation]
      public void should_set_the_value_of_the_fraction_expressed_epithelial_to_one ()
      {
         _fractionExpressedEpithelial.Value.ShouldBeEqualTo(1);
      }
   }

   public class When_updating_the_transport_type_from_efflux_to_influx : concern_for_TransportContainerUpdater
   {
      protected override void Because()
      {
         sut.SetDefaultSettingsForTransporter(_individual, _transporter, TransportType.Influx);
      }

      [Observation]
      public void should_set_the_value_of_the_fraction_expressed_epithelial_to_zero()
      {
         _fractionExpressedEpithelial.Value.ShouldBeEqualTo(0);
      }
   }

   public class When_updating_the_transport_type_from_efflux_to_influx_and_the_fraction_expressed_epithelial_was_changed_by_the_user: concern_for_TransportContainerUpdater
   {
      protected override void Context()
      {
         base.Context();
         _fractionExpressedEpithelial.Value = 0.5;
      }
      protected override void Because()
      {
         sut.SetDefaultSettingsForTransporter(_individual, _transporter, TransportType.Influx);
      }

      [Observation]
      public void should_not_update_the_value()
      {
         _fractionExpressedEpithelial.Value.ShouldBeEqualTo(0.5);
      }
   }

   public class When_updating_the_transport_type_from_a_type_that_is_not_influx_efflux_or_pgp : concern_for_TransportContainerUpdater
   {
      protected override void Because()
      {
         sut.SetDefaultSettingsForTransporter(_individual, _transporter, TransportType.BiDirectional);
      }

      [Observation]
      public void should_not_update_the_value()
      {
         _fractionExpressedEpithelial.Value.ShouldBeEqualTo(1);
      }
   }
}