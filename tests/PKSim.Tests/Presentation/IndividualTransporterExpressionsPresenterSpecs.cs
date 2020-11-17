using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;

using OSPSuite.Core.Domain;
using OSPSuite.Assets;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualTransporterExpressionsPresenter : ContextSpecification<IIndividualTransporterExpressionsPresenter>
   {
      private IIndividualTransporterExpressionsView _view;
      private IEditParameterPresenterTask _parameterTask;
      private IMoleculeExpressionTask<Individual> _moleculeTask;
      private IIndividualTransporterToTransporterExpressionDTOMapper _transporterMapper;
      private ITransporterContainerTemplateRepository _transportRepository;
      private IIndividualMoleculePropertiesPresenter<Individual> _moleculePropertiesPresenter;
      private ITransporterExpressionParametersPresenter _transporterExpressionParametersPresenter;

      protected override void Context()
      {
         _view = A.Fake<IIndividualTransporterExpressionsView>();
         _parameterTask = A.Fake<IEditParameterPresenterTask>();
         _moleculeTask = A.Fake<IMoleculeExpressionTask<Individual>>();
         _transporterMapper = A.Fake<IIndividualTransporterToTransporterExpressionDTOMapper>();
         _transportRepository = A.Fake<ITransporterContainerTemplateRepository>();
         _moleculePropertiesPresenter = A.Fake<IIndividualMoleculePropertiesPresenter<Individual>>();
         _transporterExpressionParametersPresenter= A.Fake<ITransporterExpressionParametersPresenter>(); 
         sut = new IndividualTransporterExpressionsPresenter<Individual>(
            _view, _parameterTask, _moleculeTask, _transporterMapper, _transportRepository, _moleculePropertiesPresenter,
            _transporterExpressionParametersPresenter);
      }
   }

   public class When_returning_the_application_icon_defined_for_a_given_transporter_type : concern_for_IndividualTransporterExpressionsPresenter
   {
      [Observation]
      public void should_return_the_expected_icon()
      {
         sut.IconFor(TransportType.Efflux).ShouldBeEqualTo(ApplicationIcons.Efflux);
      }
   }

   public class When_returning_the_display_name_defined_for_a_given_transporter_type : concern_for_IndividualTransporterExpressionsPresenter
   {
      [Observation]
      public void should_return_the_expected_caption()
      {
         sut.TransportTypeCaptionFor(TransportType.Efflux).ShouldBeEqualTo(PKSimConstants.UI.Efflux);
      }
   }
}