using System;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IAdvancedParameterInPopulationUpdater
   {
      /// <summary>
      ///    this function should retrieve the advanced parameter containing the given distribution parameter.
      ///    Then update the advanced parameter container containing the retrieved advanced parameter and notify the change
      /// </summary>
      /// <param name="parameter">
      ///    Parameter of the underlying distribution of an advanced parmaeter that was changed and requires
      ///    the containing population to update
      /// </param>
      void UpdatePopulationContaining(IParameter parameter);
   }

   public class AdvancedParameterInPopulationUpdater : IAdvancedParameterInPopulationUpdater
   {
      private readonly IBuildingBlockRetriever _buildingBlockRetriever;
      private readonly IEventPublisher _eventPublisher;

      public AdvancedParameterInPopulationUpdater(IBuildingBlockRetriever buildingBlockRetriever, IEventPublisher eventPublisher)
      {
         _buildingBlockRetriever = buildingBlockRetriever;
         _eventPublisher = eventPublisher;
      }

      public void UpdatePopulationContaining(IParameter parameter)
      {
         var advancedParameterContainer = _buildingBlockRetriever.BuildingBlockContaining(parameter) as IAdvancedParameterContainer;

         if (advancedParameterContainer == null)
            throw new ArgumentException(PKSimConstants.Error.CouldNotFindAdvancedParameterContainerForParameter(parameter.Name));

         var advancedParameter = parameter.ParentContainer.ParentContainer as AdvancedParameter;
         if (advancedParameter == null)
            throw new ArgumentException(PKSimConstants.Error.CouldNotFindAdvancedParameterInContainerForParameter(advancedParameterContainer.Name, parameter.Name));

         advancedParameterContainer.GenerateRandomValuesFor(advancedParameter);
         _eventPublisher.PublishEvent(new AdvancedParameterDistributionChangedEvent(advancedParameterContainer, advancedParameter));
      }
   }
}