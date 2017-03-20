using System;
using PKSim.Assets;
using OSPSuite.Core.Domain;
using PKSim.Core.Events;


namespace PKSim.Core.Commands
{
   public abstract class RemoveEntityFromContainerCommand : BuildingBlockStructureChangeCommand
   {
      public string ParentContainerId { get; protected set; }
   }

   public abstract class RemoveEntityFromContainerCommand<TEntity, TContainer, TEvent> : RemoveEntityFromContainerCommand
      where TEntity : class, IEntity
      where TContainer : class, IContainer
      where TEvent : RemoveEntityEvent<TEntity, TContainer>, new()
   {
      private readonly Func<TContainer, Action<TEntity>> _removeFunction;
      protected TEntity _entityToRemove;
      protected TContainer _parentContainer;
      protected byte[] _serializationByte;

      protected RemoveEntityFromContainerCommand(TEntity entityToRemove, TContainer parentContainer, IExecutionContext context) :
         this(entityToRemove, parentContainer, context, x => x.RemoveChild)
      {
      }

      protected RemoveEntityFromContainerCommand(TEntity entityToRemove, TContainer parentContainer, IExecutionContext context, Func<TContainer, Action<TEntity>> removeFunction)
      {
         _entityToRemove = entityToRemove;
         _parentContainer = parentContainer;
         _removeFunction = removeFunction;
         ParentContainerId = parentContainer.Id;
         IncrementVersion = true;
         BuildingBlockId = context.BuildingBlockIdContaining(parentContainer);

         CommandType = PKSimConstants.Command.CommandTypeDelete;
         ObjectType = context.TypeFor(entityToRemove);
         Description = PKSimConstants.Command.RemoveEntityFromContainer(ObjectType, entityToRemove.Name, context.TypeFor(parentContainer), parentContainer.Name);
         context.UpdateBuildinBlockProperties(this, context.BuildingBlockContaining(parentContainer));
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         //current SBSuite.Core implemetation removes all entity children
         //in Container.RemoveChild(entity)
         //So deserialization must be done BEFORE RemoveChild
         _serializationByte = context.Serialize(_entityToRemove);
         _removeFunction(_parentContainer).Invoke(_entityToRemove);

         context.Unregister(_entityToRemove);
         context.PublishEvent(new TEvent {Entity = _entityToRemove, Container = _parentContainer});
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         _entityToRemove = context.Deserialize<TEntity>(_serializationByte);
         _parentContainer = context.Get<TContainer>(ParentContainerId);
      }

      protected override void ClearReferences()
      {
         _entityToRemove = null;
         _parentContainer = null;
      }
   }
}