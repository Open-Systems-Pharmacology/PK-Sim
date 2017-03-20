using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;
using OSPSuite.Assets;

namespace PKSim.Presentation
{
   public abstract class concern_for_CloneBuildingBlockPresenter : ContextSpecification<ICloneBuildingBlockPresenter>
   {
      protected IObjectBaseView _view;
      protected ICloner _cloner;
      protected IPKSimBuildingBlock _buildingBlock;
      protected IObjectTypeResolver _objectTypeResolver;
      protected string _entityType;
      protected string _entityName;
      protected RenameObjectDTO _buildingBlockDTO;
      protected IPKSimBuildingBlock _cloneObject;
      private IRenameObjectDTOFactory _renameObjectBaseDTOFactory;

      protected override void Context()
      {
         _entityType = "tralala";
         _entityName = "tutu";
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
         A.CallTo(() => _buildingBlock.Rules).Returns(A.Fake<IBusinessRuleSet>());
         _buildingBlock.Name = _entityName;
         _view = A.Fake<IObjectBaseView>();
         A.CallTo(() => _objectTypeResolver.TypeFor(_buildingBlock)).Returns(_entityType);
         _buildingBlockDTO = new RenameObjectDTO(_entityName);
         _cloneObject = A.Fake<IPKSimBuildingBlock>();
         _cloner = A.Fake<ICloner>();
         A.CallTo(() => _cloner.Clone(_buildingBlock)).Returns(_cloneObject);
         _renameObjectBaseDTOFactory = A.Fake<IRenameObjectDTOFactory>();
         A.CallTo(() => _renameObjectBaseDTOFactory.CreateFor(_buildingBlock)).Returns(_buildingBlockDTO);
         sut = new CloneBuildingBlockPresenter(_view, _objectTypeResolver, _renameObjectBaseDTOFactory, _cloner);
      }
   }

   public class When_the_clone_presenter_is_told_to_create_a_clone_for_a_given_entity : concern_for_CloneBuildingBlockPresenter
   {
      private IPKSimBuildingBlock _result;

      protected override void Because()
      {
         _result = sut.CreateCloneFor(_buildingBlock);
      }

      [Observation]
      public void should_set_the_description_for_the_view_according_to_the_entity_type_and_name()
      {
         _view.NameDescription.ShouldBeEqualTo(Captions.CloneObjectBase(_entityType, _entityName));
      }

      [Observation]
      public void should_intitialize_the_view_with_the_entity_info_representing_the_object_to_clone()
      {
         A.CallTo(() => _view.BindTo(_buildingBlockDTO)).MustHaveHappened();
      }

      [Observation]
      public void should_display_the_view_to_allow_the_user_to_give_a_new_name_for_the_clone()
      {
         A.CallTo(() => _view.Display()).MustHaveHappened();
      }
   }

   public class When_the_user_decides_to_cancel_the_cloning_process : concern_for_CloneBuildingBlockPresenter
   {
      private IPKSimBuildingBlock _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(true);
      }

      protected override void Because()
      {
         _result = sut.CreateCloneFor(_buildingBlock);
      }

      [Observation]
      public void the_clone_presenter_should_return_null()
      {
         _result.ShouldBeNull();
      }
   }

   public class When_the_user_confirms_the_clone_process : concern_for_CloneBuildingBlockPresenter
   {
      private IPKSimBuildingBlock _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(false);
         _buildingBlockDTO.Name = "new name";
      }

      protected override void Because()
      {
         _result = sut.CreateCloneFor(_buildingBlock);
      }

      [Observation]
      public void the_clone_presenter_should_return_a_clone_from_the_given_building_block_with_the_name_set_by_the_user()
      {
         _result.ShouldBeEqualTo(_cloneObject);
         _result.Name.ShouldBeEqualTo(_buildingBlockDTO.Name);
      }
   }
}