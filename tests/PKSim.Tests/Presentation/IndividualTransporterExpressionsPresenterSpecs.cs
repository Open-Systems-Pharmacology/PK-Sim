using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualTransporterExpressionsPresenter : ContextSpecification<IIndividualTransporterExpressionsPresenter>
   {
      protected IIndividualTransporterExpressionsView _view;
      private IEditParameterPresenterTask _parameterTask;
      protected IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      private IIndividualTransporterToTransporterExpressionDTOMapper _transporterMapper;
      protected IIndividualMoleculePropertiesPresenter<Individual> _moleculePropertiesPresenter;
      protected ITransporterExpressionParametersPresenter _transporterExpressionParametersPresenter;
      protected ICommandCollector _commandCollector;
      protected IndividualTransporter _transporter;
      protected ISimulationSubject _simulationSubject;
      protected IndividualTransporterDTO _transporterDTO;

      protected override void Context()
      {
         _view = A.Fake<IIndividualTransporterExpressionsView>();
         _parameterTask = A.Fake<IEditParameterPresenterTask>();
         _moleculeExpressionTask = A.Fake<IMoleculeExpressionTask<Individual>>();
         _transporterMapper = A.Fake<IIndividualTransporterToTransporterExpressionDTOMapper>();
         _moleculePropertiesPresenter = A.Fake<IIndividualMoleculePropertiesPresenter<Individual>>();
         _transporterExpressionParametersPresenter = A.Fake<ITransporterExpressionParametersPresenter>();
         sut = new IndividualTransporterExpressionsPresenter<Individual>(
            _view, _parameterTask, _moleculeExpressionTask, _transporterMapper, _moleculePropertiesPresenter,
            _transporterExpressionParametersPresenter);

         _commandCollector = new PKSimMacroCommand();
         sut.InitializeWith(_commandCollector);


         _transporter = new IndividualTransporter();
         _simulationSubject = new Individual();
         _transporterDTO = new IndividualTransporterDTO(_transporter);
         sut.SimulationSubject = _simulationSubject;

         A.CallTo(() => _transporterMapper.MapFrom(_transporter, _simulationSubject)).Returns(_transporterDTO);
      }
   }

   public class When_setting_the_transport_direction_for_a_transporter_expression_container : concern_for_IndividualTransporterExpressionsPresenter
   {
      private TransporterExpressionParameterDTO _transporterExpressionDTO;
      private TransportDirection _effluxDirection;
      private TransportDirection _influxDirection;
      private ICommand _command;

      protected override void Context()
      {
         base.Context();
         _effluxDirection = new TransportDirection {Id = TransportDirectionId.EffluxIntracellularToInterstitial};
         _influxDirection = new TransportDirection {Id = TransportDirectionId.InfluxInterstitialToIntracellular};
         _transporterExpressionDTO = new TransporterExpressionParameterDTO
         {
            TransportDirection = _effluxDirection,
            TransporterExpressionContainer = new TransporterExpressionContainer()
         };

         _command = A.Fake<IPKSimCommand>();
         A.CallTo(() => _moleculeExpressionTask.SetTransportDirection(_transporterExpressionDTO.TransporterExpressionContainer, _influxDirection.Id))
            .Returns(_command);
      }

      protected override void Because()
      {
         sut.SetTransportDirection(_transporterExpressionDTO, _influxDirection);
      }

      [Observation]
      public void should_have_updated_the_transport_direction_of_the_transport_container_by_triggering_the_set_transport_direction_command()
      {
         _commandCollector.All().Count().ShouldBeEqualTo(1);
         _commandCollector.All().ShouldOnlyContain(_command);
      }
   }

   public class When_editing_a_transporter_that_was_loaded_from_database : concern_for_IndividualTransporterExpressionsPresenter
   {
      protected override void Context()
      {
         base.Context();
         _transporterDTO.DefaultAvailableInDatabase = true;
      }

      protected override void Because()
      {
         sut.ActivateMolecule(_transporter);
      }

      [Observation]
      public void should_not_show_the_warning()
      {
         A.CallTo(() => _view.ShowWarning(A<string>._)).MustNotHaveHappened();
      }
   }

   public class When_editing_a_transporter_that_was_not_loaded_from_database_and_the_transporter_type_is_not_set_to_the_default_transporter_type : concern_for_IndividualTransporterExpressionsPresenter
   {
      protected override void Context()
      {
         base.Context();
         _transporter.TransportType = TransportType.Influx;
         _transporterDTO.DefaultAvailableInDatabase = false;
      }

      protected override void Because()
      {
         sut.ActivateMolecule(_transporter);
      }

      [Observation]
      public void should_not_show_the_warning()
      {
         A.CallTo(() => _view.ShowWarning(A<string>._)).MustNotHaveHappened();
      }
   }

   public class When_disabling_edit_for_an_individual_transporter : concern_for_IndividualTransporterExpressionsPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.ActivateMolecule(_transporter);
      }

      protected override void Because()
      {
         sut.DisableEdit();
      }

      [Observation]
      public void should_disable_edits_for_sub_presenter_and_male_the_view_readonly()
      {
         A.CallTo(() => _moleculePropertiesPresenter.DisableEdit()).MustHaveHappened(); ;
         A.CallTo(() => _transporterExpressionParametersPresenter.DisableEdit()).MustHaveHappened();
         _view.ReadOnly.ShouldBeTrue();
      }
   }

   public class When_editing_a_transporter_that_was_not_loaded_from_database_and_the_transporter_type_is_set_to_the_default_transporter_type : concern_for_IndividualTransporterExpressionsPresenter
   {
      protected override void Context()
      {
         base.Context();
         _transporter.TransportType = CoreConstants.DEFAULT_TRANSPORTER_TYPE;
         _transporterDTO.DefaultAvailableInDatabase = false;
      }

      protected override void Because()
      {
         sut.ActivateMolecule(_transporter);
      }

      [Observation]
      public void should_show_the_warning()
      {
         A.CallTo(() => _view.ShowWarning(A<string>._)).MustHaveHappened();
      }
   }
}