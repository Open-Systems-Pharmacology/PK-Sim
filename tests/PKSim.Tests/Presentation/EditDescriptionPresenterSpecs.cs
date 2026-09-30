using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Validation;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditDescriptionPresenter : ContextSpecification<IEditDescriptionPresenter>
   {
      protected IObjectBaseView _view;
      protected IObjectTypeResolver _objectTypeResolver;
      protected IObjectBase _objectBase;
      protected ObjectBaseDTO _dto;

      protected override void Context()
      {
         _view = A.Fake<IObjectBaseView>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         _objectBase = A.Fake<IObjectBase>();
         _objectBase.Name = "CYP2C8|Human|Healthy";
         _objectBase.Description = "Represents one gene copy (one allele).";
         A.CallTo(() => _view.BindTo(A<ObjectBaseDTO>._)).Invokes(x => _dto = x.GetArgument<ObjectBaseDTO>(0));
         sut = new EditDescriptionPresenter(_view, _objectTypeResolver);
      }

      protected override void Because()
      {
         sut.Edit(_objectBase);
      }
   }

   public class When_editing_the_description_of_an_object_whose_name_contains_illegal_characters : concern_for_EditDescriptionPresenter
   {
      [Observation]
      public void should_not_report_a_validation_error()
      {
         _dto.Validate().IsEmpty.ShouldBeTrue();
      }
   }

   public class When_the_description_of_an_object_is_cleared : concern_for_EditDescriptionPresenter
   {
      protected override void Because()
      {
         base.Because();
         _dto.Description = string.Empty;
      }

      [Observation]
      public void should_not_report_a_validation_error()
      {
         _dto.Validate().IsEmpty.ShouldBeTrue();
      }
   }
}
