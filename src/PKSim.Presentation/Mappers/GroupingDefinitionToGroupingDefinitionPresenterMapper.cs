using System;
using OSPSuite.Utility;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Mappers
{
   public interface IGroupingDefinitionToGroupingDefinitionPresenterMapper :
      IMapper<GroupingDefinitionItem, IGroupingDefinitionPresenter>,
      IMapper<IGroupingDefinition, IGroupingDefinitionPresenter>
   {
   }

   public class GroupingDefinitionToGroupingDefinitionPresenterMapper : IGroupingDefinitionToGroupingDefinitionPresenterMapper
   {
      private readonly IApplicationController _applicationController;

      public GroupingDefinitionToGroupingDefinitionPresenterMapper(IApplicationController applicationController)
      {
         _applicationController = applicationController;
      }

      public IGroupingDefinitionPresenter MapFrom(GroupingDefinitionItem groupingDefinitionItem)
      {
         if (groupingDefinitionItem == GroupingDefinitions.FixedLimits)
            return _applicationController.Start<IFixedLimitsGroupingPresenter>();

         if (groupingDefinitionItem == GroupingDefinitions.NumberOfBins)
            return _applicationController.Start<INumberOfBinsGroupingPresenter>();

         if(groupingDefinitionItem==GroupingDefinitions.ValueMapping)
            return _applicationController.Start<IValueMappingGroupingPresenter>();

         throw new Exception("Not found for " + groupingDefinitionItem.DisplayName);
      }

      public IGroupingDefinitionPresenter MapFrom(IGroupingDefinition groupingDefinition)
      {
         return MapFrom(GroupingDefinitions.For(groupingDefinition));
      }
   }
}