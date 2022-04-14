using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation
{
   public abstract class concern_for_CloneExpressionProfilePresenter : ContextSpecification<ICloneExpressionProfilePresenter>
   {
      protected ICreateExpressionProfileView _view;
      protected IExpressionProfileToExpressionProfileDTOMapper _mapper;
      protected IExpressionProfileUpdater _expressionProfileUpdater;
      protected IExpressionProfileFactory _expressionProfileFactory;
      protected IDialogCreator _dialogCreator;

      protected override void Context()
      {
         _view = A.Fake<ICreateExpressionProfileView>();
         _mapper = A.Fake<IExpressionProfileToExpressionProfileDTOMapper>();
         _expressionProfileUpdater = A.Fake<IExpressionProfileUpdater>();
         _expressionProfileFactory = A.Fake<IExpressionProfileFactory>();
         _dialogCreator= A.Fake<IDialogCreator>();
         sut = new CloneExpressionProfilePresenter(_view, _mapper, _expressionProfileUpdater, _expressionProfileFactory, _dialogCreator);
      }
   }

   public class When_cloning_an_expression_profile : concern_for_CloneExpressionProfilePresenter
   {
      private ExpressionProfile _expressionProfile;
      private ExpressionProfileDTO _dto;
      private ExpressionProfile _clone;
      private ExpressionProfile _newExpressionProfile;

      protected override void Context()
      {
         base.Context();
         _dto = new ExpressionProfileDTO
         {
            MoleculeName = "NEW_MOL",
            Species = new Species {Name = "Species"},
            Category = "NEW_CATEGORY"
         };

         _expressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _newExpressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>(_dto.Species.Name, _dto.MoleculeName);

         A.CallTo(() => _mapper.MapFrom(_expressionProfile)).Returns(_dto);

         //Create a new one using the original molecule name to ensure that values will be updated by path as expected
         A.CallTo(() => _expressionProfileFactory.Create(_expressionProfile.Molecule.MoleculeType, _dto.Species.Name, _expressionProfile.MoleculeName))
            .Returns(_newExpressionProfile);
      }

      protected override void Because()
      {
         _clone = sut.CreateCloneFor(_expressionProfile) as ExpressionProfile;
      }

      [Observation]
      public void should_return_an_expression_profile_that_was_created_based_on_the_expression_profile_to_clone()
      {
         _clone.ShouldNotBeNull();
         _clone.ShouldBeEqualTo(_newExpressionProfile);

         A.CallTo(() => _expressionProfileUpdater.SynchronizeExpressionProfileWithExpressionProfile(_expressionProfile, _newExpressionProfile)).MustHaveHappened();
         _clone.Category.ShouldBeEqualTo(_dto.Category);
      }

      [Observation]
      public void should_rename_the_molecule_in_the_expression_profile()
      {
         A.CallTo(() => _expressionProfileUpdater.UpdateMoleculeName(_newExpressionProfile, _dto.MoleculeName)).MustHaveHappened();
      }
   }
}