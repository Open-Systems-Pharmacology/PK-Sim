using PKSim.Core.Model;

namespace PKSim.Core.Events
{
   public interface IAdvancedParameterEvent
   {
      AdvancedParameter AdvancedParameter { get; }
      IAdvancedParameterContainer AdvancedParameterContainer { get; }
   }

   public class RemoveAdvancedParameterFromContainerEvent : RemoveEntityEvent<AdvancedParameter, IAdvancedParameterContainer>, IAdvancedParameterEvent
   {
      public IAdvancedParameterContainer AdvancedParameterContainer => Container;

      public AdvancedParameter AdvancedParameter => Entity;
   }

   public class AddAdvancedParameterToContainerEvent : AddEntityEvent<AdvancedParameter, IAdvancedParameterContainer>, IAdvancedParameterEvent
   {
      public IAdvancedParameterContainer AdvancedParameterContainer => Container;

      public AdvancedParameter AdvancedParameter => Entity;
   }

   public class AdvancedParameteSelectedEvent : IAdvancedParameterEvent
   {
      public IAdvancedParameterContainer AdvancedParameterContainer { get; }
      public AdvancedParameter AdvancedParameter { get; }

      public AdvancedParameteSelectedEvent(IAdvancedParameterContainer advancedParameterContainer, AdvancedParameter advancedParameter)
      {
         AdvancedParameterContainer = advancedParameterContainer;
         AdvancedParameter = advancedParameter;
      }
   }

   public class AdvancedParameterDistributionChangedEvent : IAdvancedParameterEvent
   {
      public IAdvancedParameterContainer AdvancedParameterContainer { get; }
      public AdvancedParameter AdvancedParameter { get; }

      public AdvancedParameterDistributionChangedEvent(IAdvancedParameterContainer advancedParameterContainer, AdvancedParameter advancedParameter)
      {
         AdvancedParameterContainer = advancedParameterContainer;
         AdvancedParameter = advancedParameter;
      }
   }

   public class RemoveAdvancedParameterContainerFromPopulationEvent : EntityEvent<Population>
   {
      public RemoveAdvancedParameterContainerFromPopulationEvent(Population population)
      {
         Entity = population;
      }
   }

   public class AddAdvancedParameterContainerToPopulationEvent : EntityEvent<Population>
   {
      public AddAdvancedParameterContainerToPopulationEvent(Population population)
      {
         Entity = population;
      }
   }
}