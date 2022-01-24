using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Presentation
{
   public abstract class concern_for_BuildingBlockSelectionPresenter : ContextSpecification<IBuildingBlockSelectionPresenter>
   {
      protected IBuildingBlockRepository _individualRepository;
      protected IBuildingBlockSelectionView _view;
      protected IObjectTypeResolver _objectTypeResolver;
      protected IBuildingBlockTask<Individual> _individualBuildingBlockTask;
      protected List<Individual> _individualList;
      protected string _buildingBlockType;
      private IContainer _container;
      protected IToolTipPartCreator _toolTipCreator;
      protected IObjectBaseFactory _objectBaseFactory;
      protected Individual _emptySelection;
      private IBuildingBlockSelectionDisplayer _buildingBlockSelection;

      protected override void Context()
      {
         _individualList = new List<Individual>();
         _individualBuildingBlockTask = A.Fake<IBuildingBlockTask<Individual>>();
         _container = A.Fake<IContainer>();
         A.CallTo(() => _container.Resolve<IBuildingBlockTask<Individual>>()).Returns(_individualBuildingBlockTask);
         _view = A.Fake<IBuildingBlockSelectionView>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         _individualRepository = A.Fake<IBuildingBlockRepository>();
         A.CallTo(() => _individualRepository.All<Individual>()).Returns(_individualList);
         _buildingBlockType = "Tralal";
         A.CallTo(() => _objectTypeResolver.TypeFor<Individual>()).Returns(_buildingBlockType);
         _toolTipCreator = A.Fake<IToolTipPartCreator>();
         _buildingBlockSelection= A.Fake<IBuildingBlockSelectionDisplayer>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _emptySelection = new Individual().WithId("TOTO");
         A.CallTo(() => _objectBaseFactory.Create<Individual>()).Returns(_emptySelection);

         sut = new BuildingBlockSelectionPresenter<Individual>(_view, _objectTypeResolver, _individualRepository, _container, _objectBaseFactory,_buildingBlockSelection);
      }
   }

   public class When_the_building_block_selection_presenter_is_asked_to_create_a_new_building_block : concern_for_BuildingBlockSelectionPresenter
   {
      protected override void Because()
      {
         sut.CreateBuildingBlock();
      }

      [Observation]
      public void should_leverage_a_building_block_task_to_create_a_building_block_for_the_accurate_type()
      {
         A.CallTo(() => _individualBuildingBlockTask.AddToProject()).MustHaveHappened();
      }
   }

   public class When_the_selection_presenter_is_told_that_the_view_has_changed : concern_for_BuildingBlockSelectionPresenter
   {
      private bool _eventRaised;

      protected override void Context()
      {
         base.Context();
         sut.StatusChanged += (o, e) => { _eventRaised = true; };
      }

      protected override void Because()
      {
         sut.ViewChanged();
      }

      [Observation]
      public void should_notify_that_its_status_has_changed()
      {
         _eventRaised.ShouldBeTrue();
      }
   }

   public class When_the_building_block_selection_presenter_is_asked_for_the_building_block : concern_for_BuildingBlockSelectionPresenter
   {
      private IPKSimBuildingBlock _buildingBlock;

      protected override void Context()
      {
         base.Context();
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
      }

      protected override void Because()
      {
         sut.Edit(_buildingBlock);
      }

      [Observation]
      public void should_return_the_building_block_being_edited()
      {
         sut.BuildingBlock.ShouldBeEqualTo(_buildingBlock);
      }
   }

   public class When_retrieving_the_building_block_type_of_the_building_block_being_edited : concern_for_BuildingBlockSelectionPresenter
   {
      [Observation]
      public void should_return_the_type_of_the_building_block()
      {
         sut.BuildingBlockType.ShouldBeEqualTo(_buildingBlockType);
      }
   }

   public class When_retrieving_the_tool_tip_for_a_building_block_index : concern_for_BuildingBlockSelectionPresenter
   {
      private Individual _individual;
      private List<ToolTipPart> _report;

      protected override void Context()
      {
         base.Context();
         _individual = new Individual();
         _individualList.Add(_individual);
         _report = new List<ToolTipPart>();
         A.CallTo(() => _toolTipCreator.ToolTipFor(_individual)).Returns(_report);
      }

      [Observation]
      public void should_return_the_report_created_by_the_report_generator_for_a_valid_index()
      {
         sut.ToolTipPartsFor(0).ShouldOnlyContain(_report);
      }

      [Observation]
      public void should_return_an_empty_string_if_the_index_is_not_valid()
      {
         sut.ToolTipPartsFor(100).ShouldBeEmpty();
      }
   }

   public class When_empty_selection_is_enabled_in_the_building_block_selection_presenter : concern_for_BuildingBlockSelectionPresenter
   {
      private BuildingBlockSelectionDTO _dto;

      protected override void Context()
      {
         base.Context();
         sut.AllowEmptySelection = true;
         A.CallTo(() => _view.BindTo(A<BuildingBlockSelectionDTO>._))
            .Invokes(x => _dto = x.GetArgument<BuildingBlockSelectionDTO>(0).DowncastTo<BuildingBlockSelectionDTO>());

         sut.Edit(null);
      }

      [Observation]
      public void the_list_of_available_building_block_should_contain_the_empty_selection_as_first_item()
      {
         sut.AllAvailableBlocks.First().ShouldBeEqualTo(_emptySelection);
      }

      [Observation]
      public void the_returned_building_block_should_be_null_if_the_user_selects_the_empty_selection()
      {
         _dto.BuildingBlock = _emptySelection;
         sut.BuildingBlock.ShouldBeNull();
      }
   }
}