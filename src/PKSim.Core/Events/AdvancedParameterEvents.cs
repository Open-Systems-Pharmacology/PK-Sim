using PKSim.Core.Model;

namespace PKSim.Core.Events
{
   public interface IAdvancedParameterEvent
   {
      IAdvancedParameter AdvancedParameter { get; }
      IAdvancedParameterContainer AdvancedParameterContainer { get; }
   }

   public class RemoveAdvancedParameterFromContainerEvent : RemoveEntityEvent<IAdvancedParameter, IAdvancedParameterContainer>, IAdvancedParameterEvent
   {
      public IAdvancedParameterContainer AdvancedParameterContainer
      {
         get { return Container; }
      }

      public IAdvancedParameter AdvancedParameter
      {
         get { return Entity; }
      }
   }

   public class AddAdvancedParameterToContainerEvent : AddEntityEvent<IAdvancedParameter, IAdvancedParameterContainer>, IAdvancedParameterEvent
   {
      public IAdvancedParameterContainer AdvancedParameterContainer
      {
         get { return Container; }
      }

      public IAdvancedParameter AdvancedParameter
      {
         get { return Entity; }
      }
   }

   public class AdvancedParameteSelectedEvent : IAdvancedParameterEvent
   {
      public IAdvancedParameterContainer AdvancedParameterContainer { get; private set; }
      public IAdvancedParameter AdvancedParameter { get; private set; }

      public AdvancedParameteSelectedEvent(IAdvancedParameterContainer advancedParameterContainer, IAdvancedParameter advancedParameter)
      {
         AdvancedParameterContainer = advancedParameterContainer;
         AdvancedParameter = advancedParameter;
      }
   }

   public class AdvancedParameterDistributionChangedEvent : IAdvancedParameterEvent
   {
      public IAdvancedParameterContainer AdvancedParameterContainer { get; private set; }
      public IAdvancedParameter AdvancedParameter { get; private set; }

      public AdvancedParameterDistributionChangedEvent(IAdvancedParameterContainer advancedParameterContainer, IAdvancedParameter advancedParameter)
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