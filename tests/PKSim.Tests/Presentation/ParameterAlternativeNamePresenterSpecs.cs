using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Views;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Presenters.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_ParameterAlternativeNamePresenter : ContextSpecification<IParameterAlternativeNamePresenter>
   {
      protected IObjectBaseView _view;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected ParameterAlternativeGroup _parameterAlternativeGroup;

      protected override void Context()
      {
         _view= A.Fake<IObjectBaseView>();
         _representationInfoRepository= A.Fake<IRepresentationInfoRepository>();
         sut = new ParameterAlternativeNamePresenter(_view,_representationInfoRepository);

         _parameterAlternativeGroup = new ParameterAlternativeGroup().WithName("Group");
      }
   }

   public class When_retrieving_the_name_for_a_new_alternative_in_a_given_parameter_alternative_group : concern_for_ParameterAlternativeNamePresenter
   {
      private string _displayGroupName;

      protected override void Context()
      {
         base.Context();
         _displayGroupName = "DISPLAY for groyp";
         A.CallTo(() => _representationInfoRepository.DisplayNameFor(RepresentationObjectType.GROUP, _parameterAlternativeGroup.Name)).Returns(_displayGroupName);
      }
      protected override void Because()
      {
         sut.Edit(_parameterAlternativeGroup);
      }

      [Observation]
      public void should_update_the_caption_to_use_the_display_name_of_the_alternative_group()
      {
         _view.Caption.ShouldBeEqualTo(PKSimConstants.UI.CreateGroupParameterAlternativeCaption(_displayGroupName));
      }

      [Observation]
      public void should_hide_the_description_field()
      {
         _view.DescriptionVisible.ShouldBeFalse();
      }

      [Observation]
      public void should_rename_the_name_description_to_use_the_name_constant()
      {
         _view.NameDescription.ShouldBeEqualTo(PKSimConstants.UI.Name);
      }
   }
}	