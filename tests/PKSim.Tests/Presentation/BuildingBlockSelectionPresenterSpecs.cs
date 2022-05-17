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
      protected IBuildingBlockRepository _buildingBlockRepository;
      protected IBuildingBlockSelectionView _view;
      protected IObjectTypeResolver _objectTypeResolver;
      protected IBuildingBlockTask<ISimulationSubject> _simulationBuildingBlockTask;
      protected List<ISimulationSubject> _simulationSubjectList;
      protected string _buildingBlockType;
      private IContainer _container;
      protected IToolTipPartCreator _toolTipCreator;
      protected IObjectBaseFactory _objectBaseFactory;
      protected ISimulationSubject _emptySelection;
      private IBuildingBlockSelectionDisplayer _buildingBlockSelection;

      protected override void Context()
      {
         _simulationSubjectList = new List<ISimulationSubject>();
         _simulationBuildingBlockTask = A.Fake<IBuildingBlockTask<ISimulationSubject>>();
         _container = A.Fake<IContainer>();
         A.CallTo(() => _container.Resolve<IBuildingBlockTask<ISimulationSubject>>()).Returns(_simulationBuildingBlockTask);
         _view = A.Fake<IBuildingBlockSelectionView>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         A.CallTo(() => _buildingBlockRepository.All<ISimulationSubject>()).Returns(_simulationSubjectList);
         _buildingBlockType = "Tralal";
         A.CallTo(() => _objectTypeResolver.TypeFor<ISimulationSubject>()).Returns(_buildingBlockType);
         _toolTipCreator = A.Fake<IToolTipPartCreator>();
         _buildingBlockSelection= A.Fake<IBuildingBlockSelectionDisplayer>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _emptySelection = new Individual().WithId("TOTO");
         A.CallTo(() => _objectBaseFactory.Create<ISimulationSubject>()).Returns(_emptySelection);

         sut = new BuildingBlockSelectionPresenter<ISimulationSubject>(_view, _objectTypeResolver, _buildingBlockRepository, _container, _objectBaseFactory,_buildingBlockSelection);
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
         A.CallTo(() => _simulationBuildingBlockTask.AddToProject()).MustHaveHappened();
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
         _simulationSubjectList.Add(_individual);
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
      private Population _pop1;
      private Population _pop2;
      private Individual _ind1;
      private Individual _ind2;

      protected override void Context()
      {
         base.Context();
         sut.AllowEmptySelection = true;
         _ind1 = new Individual().WithName("ZZ_IND");
         _ind2 = new Individual().WithName("BB_IND");
         _pop1 = new RandomPopulation().WithName("ZZ_POP");
         _pop2 = new RandomPopulation().WithName("BB_POP");
         _simulationSubjectList.Add(_pop1);
         _simulationSubjectList.Add(_ind1);
         _simulationSubjectList.Add(_pop2);
         _simulationSubjectList.Add(_ind2);
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

      [Observation]
      public void the_list_of_available_building_block_should_be_sorted_by_building_block_type_and_name()
      {
         sut.AllAvailableBlocks.ShouldOnlyContainInOrder(_emptySelection, _ind2, _ind1, _pop2,_pop1);
      }
   }
}