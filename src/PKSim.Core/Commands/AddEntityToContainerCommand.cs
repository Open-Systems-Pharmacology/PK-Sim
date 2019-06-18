using System;
using PKSim.Assets;
using PKSim.Core.Events;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public abstract class AddEntityToContainerCommand<TEntity, TContainer, TEvent> : BuildingBlockStructureChangeCommand
      where TEntity : class, IEntity
      where TContainer : class, IContainer
      where TEvent : AddEntityEvent<TEntity, TContainer>, new()

   {
      private readonly Func<TContainer, Action<TEntity>> _addFunction;
      private readonly string _entityToAddId;
      private readonly string _parentContainerId;
      protected TEntity _entityToAdd;
      protected TContainer _parentContainer;

      protected AddEntityToContainerCommand(TEntity entityToAdd, TContainer parentContainer, IExecutionContext context)
         : this(entityToAdd, parentContainer, context, x => x.Add)
      {
      }

      protected AddEntityToContainerCommand(TEntity entityToAdd, TContainer parentContainer, IExecutionContext context, Func<TContainer, Action<TEntity>> addFunction)
      {
         _entityToAdd = entityToAdd;
         _parentContainer = parentContainer;
         _addFunction = addFunction;
         _entityToAddId = _entityToAdd.Id;
         _parentContainerId = _parentContainer.Id;

         IncrementVersion = true;
         BuildingBlockId = context.BuildingBlockIdContaining(parentContainer);

         ObjectType = context.TypeFor(entityToAdd);
         CommandType = PKSimConstants.Command.CommandTypeAdd;
         var buildingBlock = context.BuildingBlockContaining(parentContainer);
         context.UpdateBuildingBlockPropertiesInCommand(this, buildingBlock);
         string containerName = string.IsNullOrEmpty(parentContainer.Name) ? CoreConstants.ContainerName.NameTemplate : parentContainer.Name;
         Description = PKSimConstants.Command.AddEntityToContainer(ObjectType, entityToAdd.Name, context.TypeFor(parentContainer), containerName);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _addFunction(_parentContainer).Invoke(_entityToAdd);
         context.Register(_entityToAdd);
         context.PublishEvent(new TEvent {Entity = _entityToAdd, Container = _parentContainer});
      }

      protected override void ClearReferences()
      {
         _entityToAdd = null;
         _parentContainer = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _entityToAdd = context.Get<TEntity>(_entityToAddId);
         _parentContainer = context.Get<TContainer>(_parentContainerId);
      }
   }
}