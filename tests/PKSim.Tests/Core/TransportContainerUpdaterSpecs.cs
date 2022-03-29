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
      protected IParameter _fractionExpressedApical;
      protected IParameter _fractionExpressedApicalBone;
      protected IParameter _fractionExpressedApicalMucosa;
      protected ISimulationSubject _individual;
      protected TransporterExpressionContainer _transporterWithTemplate;
      protected TransporterExpressionContainer _transporterWithoutTemplate;
      protected const string _species = "human";
      protected readonly List<TransporterContainerTemplate> _allTransporterTemplates = new List<TransporterContainerTemplate>();
      protected TransporterContainerTemplate _transporterContainerTemplate;
      protected IndividualTransporter _transporter;
      private TransporterExpressionContainer _transporterInMucosa;
      private TransporterExpressionContainer _transporterInBone;

      protected override void Context()
      {
         _repository = A.Fake<ITransporterContainerTemplateRepository>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _transportDirectionRepository = A.Fake<ITransportDirectionRepository>();
         sut = new TransportContainerUpdater(_repository, _eventPublisher);
         _fractionExpressedApical = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.FRACTION_EXPRESSED_APICAL);
         _fractionExpressedApicalMucosa = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.FRACTION_EXPRESSED_APICAL);
         _fractionExpressedApicalBone = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.FRACTION_EXPRESSED_APICAL);
         //Indicates that this parameter should not be visible to the user and therefore remains unchanged
         _fractionExpressedApicalBone.Visible = false;
         _individual = A.Fake<ISimulationSubject>();
         _individual.Species.Name = _species;
         _transporter = new IndividualTransporter {TransportType = TransportType.Efflux, Name = "toto"};
         var organism = new Organism();
         var mucosa = new Container().WithName(CoreConstants.Compartment.MUCOSA).WithParentContainer(organism);
         var liver = new Container().WithName(CoreConstants.Organ.LIVER).WithParentContainer(organism);
         var liverCell = new Container().WithName(CoreConstants.Compartment.INTRACELLULAR).WithParentContainer(liver);
         var kidney = new Container().WithName(CoreConstants.Organ.KIDNEY).WithParentContainer(organism);
         var kidneyCell = new Container().WithName(CoreConstants.Compartment.INTRACELLULAR).WithParentContainer(kidney);
         var bone = new Container().WithName(CoreConstants.Organ.BONE).WithParentContainer(organism);
         var boneInterstitial = new Container().WithName(CoreConstants.Compartment.INTERSTITIAL).WithParentContainer(bone);
         _transporterWithTemplate = new TransporterExpressionContainer {TransportDirection = TransportDirectionId.InfluxInterstitialToIntracellular}
            .WithParentContainer(liverCell);
         _transporterWithoutTemplate = new TransporterExpressionContainer
               {TransportDirection = TransportDirectionId.EffluxIntracellularToInterstitial}
            .WithParentContainer(kidneyCell);

         _transporterInMucosa = new TransporterExpressionContainer {TransportDirection = TransportDirectionId.InfluxInterstitialToIntracellular}
            .WithParentContainer(mucosa);

         _transporterInBone = new TransporterExpressionContainer { TransportDirection = TransportDirectionId.InfluxInterstitialToIntracellular }
            .WithParentContainer(boneInterstitial);
         
         _transporterInMucosa.Add(_fractionExpressedApicalMucosa);
         _transporterWithoutTemplate.Add(_fractionExpressedApical);
         _transporterInBone.Add(_fractionExpressedApicalBone);

         A.CallTo(() => _individual.AllMoleculeContainersFor<TransporterExpressionContainer>(_transporter))
            .Returns(new[] {_transporterWithTemplate, _transporterWithoutTemplate, _transporterInMucosa, _transporterInBone });

         _transporterContainerTemplate = new TransporterContainerTemplate {TransportType = TransportType.Influx};
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

   public class When_asked_to_set_the_default_settings_for_a_transporter_for_which_template_does_not_exist_in_the_db : concern_for_TransportContainerUpdater
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
         sut.SetDefaultSettingsForTransporter(_individual, _transporter, TransportType.Efflux);
      }

      [Observation]
      public void should_set_the_value_of_the_fraction_expressed_apical_to_one()
      {
         _fractionExpressedApical.Value.ShouldBeEqualTo(1);
      }
   }

   public class When_updating_the_transport_type_from_efflux_to_influx : concern_for_TransportContainerUpdater
   {
      protected override void Because()
      {
         sut.SetDefaultSettingsForTransporter(_individual, _transporter, TransportType.Influx);
      }

      [Observation]
      public void should_set_the_value_of_the_fraction_expressed_apical_to_zero_in_all_non_mucosa_compartment()
      {
         _fractionExpressedApical.Value.ShouldBeEqualTo(0);
      }


      [Observation]
      public void should_not_change_the_value_of_fraction_expressed_apical_for_organ_that_only_have_this_parameter_for_consistency_purpose_such_as_bone()
      {
         _fractionExpressedApicalBone.Value.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_set_the_value_of_the_fraction_expressed_apical_to_one_in_all_mucosa_compartment()
      {
         _fractionExpressedApicalMucosa.Value.ShouldBeEqualTo(1);
      }
   }

   public class When_updating_the_transport_type_from_efflux_to_influx_and_the_fraction_expressed_apical_was_changed_by_the_user : concern_for_TransportContainerUpdater
   {
      protected override void Context()
      {
         base.Context();
         _fractionExpressedApical.Value = 0.5;
      }

      protected override void Because()
      {
         sut.SetDefaultSettingsForTransporter(_individual, _transporter, TransportType.Influx);
      }

      [Observation]
      public void should_not_update_the_value()
      {
         _fractionExpressedApical.Value.ShouldBeEqualTo(0.5);
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
         _fractionExpressedApical.Value.ShouldBeEqualTo(1);
      }
   }
}