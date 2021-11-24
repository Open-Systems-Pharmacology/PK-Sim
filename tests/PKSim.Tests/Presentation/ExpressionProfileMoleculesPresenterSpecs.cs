using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Exceptions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExpressionProfileMoleculesPresenter : ContextSpecification<IExpressionProfileMoleculesPresenter>
   {
      protected IExpressionProfileMoleculesView _view;
      protected IExpressionProfileFactory _expressionProfileFactory;
      protected IApplicationController _applicationController;
      protected IExpressionProfileToExpressionProfileDTOMapper _mapper;
      protected ExpressionProfile _expressionProfile;
      protected ExpressionProfileDTO _expressionProfileDTO;
      protected IndividualMolecule _enzyme;
      protected IIndividualEnzymeExpressionsPresenter<Individual> _enzymePresenter;
      protected IExpressionProfileUpdater _expressionProfileUpdater;
      protected IExpressionProfileProteinDatabaseTask _expressionProfileProteinDatabaseTask;
      protected IMoleculeParameterTask _moleculeParameterTask;

      protected override void Context()
      {
         _view = A.Fake<IExpressionProfileMoleculesView>();
         _expressionProfileFactory = A.Fake<IExpressionProfileFactory>();
         _applicationController = A.Fake<IApplicationController>();
         _mapper = A.Fake<IExpressionProfileToExpressionProfileDTOMapper>();
         _enzymePresenter = A.Fake<IIndividualEnzymeExpressionsPresenter<Individual>>();
         _expressionProfileUpdater = A.Fake<IExpressionProfileUpdater>();
         _expressionProfileProteinDatabaseTask = A.Fake<IExpressionProfileProteinDatabaseTask>();
         _moleculeParameterTask= A.Fake<IMoleculeParameterTask>();
         sut = new ExpressionProfileMoleculesPresenter(
            _view, 
            _expressionProfileFactory, 
            _applicationController, 
            _mapper, 
            _expressionProfileProteinDatabaseTask, 
            _expressionProfileUpdater,
            _moleculeParameterTask);

         sut.InitializeWith(new PKSimMacroCommand());
         _expressionProfile = A.Fake<ExpressionProfile>();
         _expressionProfileDTO = new ExpressionProfileDTO();
         _enzyme = new IndividualEnzyme();
         A.CallTo(() => _expressionProfile.Molecule).Returns(_enzyme);
         A.CallTo(() => _mapper.MapFrom(_expressionProfile)).Returns(_expressionProfileDTO);

         A.CallTo(() => _applicationController.Start<IIndividualEnzymeExpressionsPresenter<Individual>>()).Returns(_enzymePresenter);
      }
   }

   public class When_editing_an_expression_profile : concern_for_ExpressionProfileMoleculesPresenter
   {
      protected override void Because()
      {
         sut.Edit(_expressionProfile);
      }

      [Observation]
      public void should_map_the_profile_to_an_expression_profile_dto_that_will_be_bound_to_the_view()
      {
         A.CallTo(() => _view.BindTo(_expressionProfileDTO)).MustHaveHappened();
      }

      [Observation]
      public void should_active_the_presenter_corresponding_to_the_expression_profile_molecule()
      {
         A.CallTo(() => _view.AddExpressionView(_enzymePresenter.BaseView)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_command_collector_in_the_expression_molecule_presenter()
      {
         A.CallTo(() => _enzymePresenter.InitializeWith(sut.CommandCollector)).MustHaveHappened();
      }

      [Observation]
      public void should_refresh_the_expression_for_the_underlying_molecule()
      {
         _enzymePresenter.SimulationSubject.ShouldBeEqualTo(_expressionProfile.Individual);
         A.CallTo(() => _enzymePresenter.ActivateMolecule(_enzyme)).MustHaveHappened();
      }
   }

   public class When_the_user_changes_the_species_associated_with_an_expression_profile : concern_for_ExpressionProfileMoleculesPresenter
   {
      private Species _newSpecies;

      protected override void Context()
      {
         base.Context();
         _newSpecies = new Species();
         sut.Edit(_expressionProfile);
         _expressionProfileDTO.Species = _newSpecies;
      }

      protected override void Because()
      {
         sut.SpeciesChanged();
      }

      [Observation]
      public void should_update_the_expression_profile_to_match_the_selected_species()
      {
         A.CallTo(() => _expressionProfileFactory.UpdateSpecies(_expressionProfile, _newSpecies)).MustHaveHappened();
      }

      [Observation]
      public void should_refresh_the_expression_for_the_underlying_molecule_again()
      {
         //Once in edit and once after species changed
         A.CallTo(() => _enzymePresenter.ActivateMolecule(_enzyme)).MustHaveHappened(timesOption: Times.Exactly, numberOfTimes: 2);
      }
   }

   public class When_notified_to_save_the_expression_profile : concern_for_ExpressionProfileMoleculesPresenter
   {
      protected override void Context()
      {
         base.Context();
         _expressionProfileDTO.MoleculeName = "MOLECULE";
         _expressionProfileDTO.Category = "CATEGORY";
         sut.Edit(_expressionProfile);
      }

      protected override void Because()
      {
         sut.Save();
      }

      [Observation]
      public void should_molecule_name_as_defined_by_the_user()
      {
         A.CallTo(() => _expressionProfileUpdater.UpdateMoleculeName(_expressionProfile, _expressionProfileDTO.MoleculeName)).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_category_and_molecule_name_as_defined_by_the_user()
      {
         _expressionProfile.Category.ShouldBeEqualTo(_expressionProfileDTO.Category);
      }
   }

   public class When_loading_the_expression_profile_from_the_database_for_a_species_for_which_no_database_is_connected : concern_for_ExpressionProfileMoleculesPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Edit(_expressionProfile);
         A.CallTo(() => _expressionProfileProteinDatabaseTask.CanQueryProteinExpressionsFor(_expressionProfile)).Returns(false);
      }

      [Observation]
      public void should_throw_an_exception_warning_the_user_that_no_database_is_connected()
      {
         The.Action(() => sut.LoadExpressionFromDatabaseQuery()).ShouldThrowAn<OSPSuiteException>();
      }
   }

   public class When_loading_the_expression_profile_from_the_database_for_a_species_for_which_a_database_is_connected_in_create_mode : concern_for_ExpressionProfileMoleculesPresenter
   {
      private QueryExpressionResults _result;
      private ICommand _command;

      protected override void Context()
      {
         base.Context();
         sut.IsEditMode = false;
         _command = A.Fake<IPKSimCommand>();
         _result = new QueryExpressionResults(new ExpressionResult[] { }) {ProteinName = "NEW_NAME"};
         sut.Edit(_expressionProfile);
         _expressionProfileDTO.MoleculeName = "MOLECULE";
         A.CallTo(() => _expressionProfileProteinDatabaseTask.CanQueryProteinExpressionsFor(_expressionProfile)).Returns(true);
         A.CallTo(() => _expressionProfileProteinDatabaseTask.QueryDatabase(_expressionProfile, _expressionProfileDTO.MoleculeName))
            .Returns(_result);
         A.CallTo(() => _expressionProfileUpdater.UpdateExpressionFromQuery(_expressionProfile, _result)).Returns(_command);
      }

      protected override void Because()
      {
         sut.LoadExpressionFromDatabaseQuery();
      }

      [Observation]
      public void should_add_the_resulting_edit_as_command()
      {
         sut.CommandCollector.All().ShouldContain(_command);
      }

      [Observation]
      public void should_have_updated_the_name_based_on_the_query()
      {
         _expressionProfileDTO.MoleculeName.ShouldBeEqualTo("NEW_NAME");
      }

      [Observation]
      public void should_update_the_default_molecule_parameters()
      {
         A.CallTo(() => _moleculeParameterTask.SetDefaultFor(_expressionProfile, _expressionProfileDTO.MoleculeName)).MustHaveHappened();
      }
   }

   public class When_querying_the_expression_database_for_an_expression_profile_in_edit_mode : concern_for_ExpressionProfileMoleculesPresenter
   {
      private QueryExpressionResults _result;

      protected override void Context()
      {
         base.Context();
         sut.IsEditMode = true;
         _expressionProfileDTO.MoleculeName = "MOLECULE";
         _result = new QueryExpressionResults(new ExpressionResult[] { }) { ProteinName = "NEW_NAME" };

         A.CallTo(() => _expressionProfileProteinDatabaseTask.CanQueryProteinExpressionsFor(_expressionProfile)).Returns(true);
         A.CallTo(() => _expressionProfileProteinDatabaseTask.QueryDatabase(_expressionProfile, _expressionProfileDTO.MoleculeName))
            .Returns(_result);


      }

      [Observation]
      public void should_not_update_the_molecule_name()
      {
         _expressionProfileDTO.MoleculeName.ShouldBeEqualTo("MOLECULE");

      }

      [Observation]
      public void should_not_update_the_default_molecule_parameters()
      {
         A.CallTo(() => _moleculeParameterTask.SetDefaultFor(_expressionProfile, A<string>._)).MustNotHaveHappened();
      }
   }
}