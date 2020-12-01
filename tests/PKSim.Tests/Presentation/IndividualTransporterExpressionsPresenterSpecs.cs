using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
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
      private IIndividualTransporterExpressionsView _view;
      private IEditParameterPresenterTask _parameterTask;
      protected IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      private IIndividualTransporterToTransporterExpressionDTOMapper _transporterMapper;
      private IIndividualMoleculePropertiesPresenter<Individual> _moleculePropertiesPresenter;
      private ITransporterExpressionParametersPresenter _transporterExpressionParametersPresenter;
      protected ICommandCollector _commandCollector;

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
}