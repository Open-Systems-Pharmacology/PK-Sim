using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Nodes;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_BuildingBlockSelectionDisplayer : ContextSpecification<IBuildingBlockSelectionDisplayer>
   {
      protected IToolTipPartCreator _toolTipPartCreator;
      private IBuildingBlockRepository _buildingBlockRepository;
      protected IPKSimBuildingBlock _templateCompound;
      protected IPKSimBuildingBlock _nonTemplateCompound;

      protected override void Context()
      {
         _toolTipPartCreator = A.Fake<IToolTipPartCreator>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         sut = new BuildingBlockSelectionDisplayer(_buildingBlockRepository, _toolTipPartCreator);

         _templateCompound = new Compound().WithName("A").WithId("A");
         _nonTemplateCompound = new Compound().WithName("B").WithId("B");

         A.CallTo(() => _buildingBlockRepository.ById(_templateCompound.Id)).Returns(_templateCompound);
         A.CallTo(() => _buildingBlockRepository.ById(_nonTemplateCompound.Id)).Returns(null);
      }
   }

   public class When_retrieving_the_display_name_for_a_given_building_block : concern_for_BuildingBlockSelectionDisplayer
   {
      [Observation]
      public void should_return_the_name_of_the_building_block_for_a_template_building_block()
      {
         sut.DisplayNameFor(_templateCompound).ShouldBeEqualTo(_templateCompound.Name);
      }

      [Observation]
      public void should_return_a_name_with_a_note_to_the_user_if_the_building_block_is_not_a_template_building_block()
      {
         sut.DisplayNameFor(_nonTemplateCompound).ShouldNotBeEqualTo(_nonTemplateCompound.Name);
      }

      [Observation]
      public void should_return_the_name_of_the_building_block_if_the_building_block_represents_the_empty_selection()
      {
         sut.DisplayNameFor(_nonTemplateCompound, _nonTemplateCompound).ShouldBeEqualTo(_nonTemplateCompound.Name);
      }
   }

   public class When_retrieving_the_display_name_for_a_null_building_block : concern_for_BuildingBlockSelectionDisplayer
   {
      [Observation]
      public void should_return_an_empty_string()
      {
         string.IsNullOrEmpty(sut.DisplayNameFor(null)).ShouldBeTrue();
      }
   }

   public class When_retrieving_the_tool_tip_for_a_given_building_block : concern_for_BuildingBlockSelectionDisplayer
   {
      private IList<ToolTipPart> _toolTip;

      protected override void Context()
      {
         base.Context();
         _toolTip = new List<ToolTipPart>();
         A.CallTo(() => _toolTipPartCreator.ToolTipFor(_templateCompound)).Returns(_toolTip);
      }

      [Observation]
      public void shoud_return_am_empty_enumerable_if_the_building_block_is_null()
      {
         sut.ToolTipFor(null).ShouldBeEmpty();
      }

      [Observation]
      public void should_return_the_defined_building_block_otherwise()
      {
         sut.ToolTipFor(_templateCompound).ShouldBeEqualTo(_toolTip);
      }
   }
}