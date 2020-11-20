using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Presentation.DTO.Individuals;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualTransporterExpressionsPresenter : ContextSpecification<IIndividualTransporterExpressionsPresenter>
   {
      private IIndividualTransporterExpressionsView _view;
      private IEditParameterPresenterTask _parameterTask;
      private IMoleculeExpressionTask<Individual> _moleculeTask;
      private IIndividualTransporterToTransporterExpressionDTOMapper _transporterMapper;
      private IIndividualMoleculePropertiesPresenter<Individual> _moleculePropertiesPresenter;
      private ITransporterExpressionParametersPresenter _transporterExpressionParametersPresenter;
      protected ICommandCollector _commandCollector;

      protected override void Context()
      {
         _view = A.Fake<IIndividualTransporterExpressionsView>();
         _parameterTask = A.Fake<IEditParameterPresenterTask>();
         _moleculeTask = A.Fake<IMoleculeExpressionTask<Individual>>();
         _transporterMapper = A.Fake<IIndividualTransporterToTransporterExpressionDTOMapper>();
         _moleculePropertiesPresenter = A.Fake<IIndividualMoleculePropertiesPresenter<Individual>>();
         _transporterExpressionParametersPresenter= A.Fake<ITransporterExpressionParametersPresenter>(); 
         sut = new IndividualTransporterExpressionsPresenter<Individual>(
            _view, _parameterTask, _moleculeTask, _transporterMapper, _moleculePropertiesPresenter,
            _transporterExpressionParametersPresenter);

         _commandCollector = new PKSimMacroCommand();
         sut.InitializeWith(_commandCollector);
      }
   }

   public class When_setting_the_transport_type_for_a_transporter_expression_container : concern_for_IndividualTransporterExpressionsPresenter
   {
      private TransporterExpressionParameterDTO _transporterExpressionDTO;

      protected override void Context()
      {
         base.Context();
         _transporterExpressionDTO = new TransporterExpressionParameterDTO
         {
            TransportDirection = TransportDirections.Efflux, 
            TransporterExpressionContainer = new TransporterExpressionContainer()
         };
      }

      protected override void Because()
      {
         sut.SetTransportDirection(_transporterExpressionDTO, TransportDirections.Influx);
      }

      [Observation]
      public void should_have_updated_the_transport_direction_of_the_transport_container()
      {
         _transporterExpressionDTO.TransportDirection.ShouldBeEqualTo(TransportDirections.Influx);
      }

      [Observation]
      public void should_have_triggered_a_command()
      {
         _commandCollector.All().Count().ShouldBeEqualTo(1);
      }
   }
}