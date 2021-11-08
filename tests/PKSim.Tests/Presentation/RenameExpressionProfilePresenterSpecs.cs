using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation
{
   public abstract class concern_for_RenameExpressionProfilePresenter : ContextSpecification<IRenameExpressionProfilePresenter>
   {
      protected IRenameExpressionProfileView _view;
      protected IExpressionProfileToExpressionProfileDTOMapper _mapper;

      protected override void Context()
      {
         _view = A.Fake<IRenameExpressionProfileView>();
         _mapper = A.Fake<IExpressionProfileToExpressionProfileDTOMapper>();
         sut = new RenameExpressionProfilePresenter(_view,_mapper);
      }
   }

   public class When_renaming_an_expression_profile : concern_for_RenameExpressionProfilePresenter
   {
      private ExpressionProfile _expressionProfile;
      private IEnumerable<string> _forbiddenNames;
      private string _entityType;
      private string _newName;
      private ExpressionProfileDTO _expressionProfileDTO;

      protected override void Context()
      {
         base.Context();
         _expressionProfile = new ExpressionProfile();
         _forbiddenNames = new List<string>();
         _entityType = "Type";
         _expressionProfileDTO= new ExpressionProfileDTO();
         A.CallTo(() => _mapper.MapFrom(_expressionProfile)).Returns(_expressionProfileDTO);
         _expressionProfileDTO.MoleculeName = "MOLECULE";
         _expressionProfileDTO.Category = "NEW_CATEGORY";
      }

      protected override void Because()
      {
         _newName = sut.NewNameFrom(_expressionProfile, _forbiddenNames, _entityType);
      }

      [Observation]
      public void should_map_the_expression_profile_to_a_corresponding_dto_and_bind_to_the_view()
      {
         A.CallTo(() => _view.BindTo(_expressionProfileDTO)).MustHaveHappened();
         A.CallTo(() => _view.Display()).MustHaveHappened();
      }

      [Observation]
      public void should_return_the_new_name()
      {
         _newName.ShouldBeEqualTo(_expressionProfileDTO.Name);
      }
   }
}