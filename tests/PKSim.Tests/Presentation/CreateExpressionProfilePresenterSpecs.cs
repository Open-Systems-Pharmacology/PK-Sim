using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.DiseaseStates;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.DiseaseStates;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation
{
   public abstract class concern_for_CreateExpressionProfilePresenter : ContextSpecification<ICreateExpressionProfilePresenter>
   {
      protected ICreateExpressionProfileView _view;
      protected IExpressionProfileFactory _expressionProfileFactory;
      protected ExpressionProfile _expressionProfile;
      protected IExpressionProfileToExpressionProfileDTOMapper _expressionProfileDTOMapper;
      protected IMoleculeParameterTask _moleculeParameterTask;
      protected ExpressionProfileDTO _expressionProfileDTO;
      protected ExpressionProfile _updatedExpressionProfile;
      protected IDialogCreator _dialogCreator;
      private IDiseaseStateSelectionPresenter _diseaseStateSelectionPresenter;
      private IDiseaseStateRepository _diseaseStateRepository;
      private IDiseaseStateUpdater _diseaseStateUpdater;

      protected override void Context()
      {
         _view = A.Fake<ICreateExpressionProfileView>();
         _expressionProfileFactory = A.Fake<IExpressionProfileFactory>();
         _expressionProfileDTOMapper = A.Fake<IExpressionProfileToExpressionProfileDTOMapper>();
         _moleculeParameterTask = A.Fake<IMoleculeParameterTask>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _diseaseStateSelectionPresenter = A.Fake<IDiseaseStateSelectionPresenter>();
         _diseaseStateRepository = A.Fake<IDiseaseStateRepository>();
         _diseaseStateUpdater = A.Fake<IDiseaseStateUpdater>();

         sut = new CreateExpressionProfilePresenter(
            _view, _expressionProfileFactory, _expressionProfileDTOMapper, _moleculeParameterTask, 
            _dialogCreator, _diseaseStateSelectionPresenter,_diseaseStateRepository, _diseaseStateUpdater);

         _expressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _updatedExpressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _expressionProfileDTO = new ExpressionProfileDTO
         {
            Species = new Species(),
            MoleculeName = "TOTO"
         };

         A.CallTo(() => _expressionProfileFactory.Create<IndividualEnzyme>()).Returns(_expressionProfile);

         A.CallTo(() => _expressionProfileDTOMapper.MapFrom(_expressionProfile)).Returns(_expressionProfileDTO);

         A.CallTo(() => _expressionProfileFactory.Create<IndividualEnzyme>(_expressionProfileDTO.Species, _expressionProfileDTO.MoleculeName))
            .Returns(_updatedExpressionProfile);
      }
   }

   public class When_creating_a_new_expression_profile : concern_for_CreateExpressionProfilePresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         sut.Create<IndividualEnzyme>();
      }

      [Observation]
      public void should_update_the_view_with_the_expected_caption()
      {
         _view.Caption.ShouldBeEqualTo(PKSimConstants.UI.CreateExpressionProfile);
      }

      [Observation]
      public void should_bind_the_expression_dto_to_edit_to_the_view()
      {
         A.CallTo(() => _view.BindTo(_expressionProfileDTO)).MustHaveHappened();
      }

      [Observation]
      public void the_returned_building_block_should_be_the_expression_profile_created()
      {
         sut.BuildingBlock.ShouldBeEqualTo(_updatedExpressionProfile);
      }
   }

   public class When_the_create_expression_profile_presenter_is_being_notified_that_the_view_has_changed : concern_for_CreateExpressionProfilePresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Create<IndividualEnzyme>();
         A.CallTo(() => _view.HasError).Returns(true);
      }

      protected override void Because()
      {
         sut.ViewChanged();
      }

      [Observation]
      public void should_update_the_ok_button_state()
      {
         _view.OkEnabled.ShouldBeFalse();
      }
   }

   public class When_creating_a_new_expression_profile_and_the_user_cancels_the_action : concern_for_CreateExpressionProfilePresenter
   {
      private IPKSimCommand _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(true);
      }

      protected override void Because()
      {
         _result = sut.Create<IndividualEnzyme>();
      }

      [Observation]
      public void should_return_an_empty_command()
      {
         _result.IsEmpty().ShouldBeTrue();
      }
   }

   public class When_saving_a_new_expression_profile_to_the_project_that_is_in_valid_state : concern_for_CreateExpressionProfilePresenter
   {
      protected override void Context()
      {
         base.Context();
         _expressionProfileDTO.MoleculeName = "A";
         _expressionProfileDTO.Species = new Species().WithName("B");
         _expressionProfileDTO.Category = "C";

         A.CallTo(() => _expressionProfileFactory.Create<IndividualEnzyme>(_expressionProfileDTO.Species, _expressionProfileDTO.MoleculeName))
            .Returns(_updatedExpressionProfile);
         sut.Create<IndividualEnzyme>();
      }

      protected override void Because()
      {
         sut.Save();
      }

      [Observation]
      public void should_close_the_view()
      {
         A.CallTo(() => _view.CloseView()).MustHaveHappened();
      }
   }

   public class When_saving_a_new_expression_profile_to_the_project_that_is_not_in_a_valid_state : concern_for_CreateExpressionProfilePresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Create<IndividualEnzyme>();
         _expressionProfileDTO.Category = "A";
         _expressionProfileDTO.Species = new Species().WithName("B");
         _expressionProfileDTO.Category = "C";

         _expressionProfileDTO.AddExistingExpressionProfileNames(new[] {_expressionProfileDTO.Name});
      }

      protected override void Because()
      {
         sut.Save();
      }

      [Observation]
      public void should_not_close_the_view()
      {
         A.CallTo(() => _view.CloseView()).MustNotHaveHappened();
      }

      [Observation]
      public void should_notify_the_user_that_something_is_wrong()
      {
         A.CallTo(() => _dialogCreator.MessageBoxInfo(_expressionProfileDTO.Validate().Message)).MustHaveHappened();
      }
   }
}